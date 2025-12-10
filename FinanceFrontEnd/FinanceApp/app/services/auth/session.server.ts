import { createCookieSessionStorage, redirect } from 'react-router';
import redis from './redis.server';
import { verifyIdToken } from './auth.server';
import type { SessionUser } from './types/SessionUser';
import SafeLogger from '@/utils/SafeLogger';
import { sessionLogger } from '@/middleware/logger.server';

// User session storage (different from auth flow)
const userSessionStorage = createCookieSessionStorage({
  cookie: {
    name: '__user_session',
    httpOnly: true,
    maxAge: 60 * 60 * 24 * 7, // 7 days
    path: '/',
    sameSite: 'lax',
    // Read session secrets from environment for production safety. Support
    // comma-separated secrets to allow rotation (old, new).
    // Prefer runtime secret injection (USER_SESSION_SECRET or SESSION_SECRET).
    // In production a secret must be provided; in development we provide a
    // non-sensitive fallback to simplify local setup.
    // Example: USER_SESSION_SECRET="s1,s2"
    secrets: (() => {
      const raw = process.env.USER_SESSION_SECRET || process.env.SESSION_SECRET || '';
      const list = raw
        .split(',')
        .map((s) => s.trim())
        .filter(Boolean);
      if (process.env.NODE_ENV === 'production' && list.length === 0) {
        throw new Error(
          'Missing session secret: set USER_SESSION_SECRET or SESSION_SECRET in the environment for production'
        );
      }
      return list.length ? list : ['dev-session-secret'];
    })(),
    secure: process.env.NODE_ENV === 'production',
  },
});

export async function getUserFromSession(request: Request): Promise<SessionUser | null> {
  const session = await userSessionStorage.getSession(request.headers.get('Cookie'));

  const serverSessionId = session.get('serverSessionId');
  sessionLogger.sessionCheck(serverSessionId, false);
  if (!serverSessionId) return null;

  // Get tokens from Redis with error handling
  try {
    const tokenData = await redis.get(`serverSession:${serverSessionId}`);
    if (!tokenData) {
      // Session not found in Redis, clear the invalid cookie
      sessionLogger.sessionCheck(serverSessionId, false);
      SafeLogger.warn('Session not found in Redis, clearing cookie:', serverSessionId);
      return null;
    }
    sessionLogger.sessionCheck(serverSessionId, true);

    const tokens = JSON.parse(tokenData);

    // Verify ID token and return user info
    const idToken = tokens.idToken;
    if (!idToken) {
      SafeLogger.warn('No ID token found in session:', serverSessionId);
      // Clean up invalid session
      try {
        await redis.del(`serverSession:${serverSessionId}`);
      } catch (redisError) {
        SafeLogger.error('Failed to delete Redis session:', redisError);
      }
      return null;
    }

    try {
      const payload = await verifyIdToken(idToken);
      
      // Validate required fields from token payload
      if (!payload.sub || !payload.email) {
        SafeLogger.error('Invalid token payload - missing required fields:', {
          hasSub: !!payload.sub,
          hasEmail: !!payload.email,
        });
        // Clean up invalid session
        try {
          await redis.del(`serverSession:${serverSessionId}`);
        } catch (redisError) {
          SafeLogger.error('Failed to delete Redis session:', redisError);
        }
        return null;
      }

      return {
        id: payload.sub as string,
        name: (payload.name as string) || '',
        email: payload.email as string,
        picture: (payload.picture as string) || undefined,
        serverSessionId,
      };
    } catch (error) {
      // Token expired or invalid, remove session
      SafeLogger.warn('Token verification failed:', error);
      try {
        await redis.del(`serverSession:${serverSessionId}`);
      } catch (redisError) {
        SafeLogger.error('Failed to delete Redis session:', redisError);
      }
      return null;
    }
  } catch (redisError) {
    SafeLogger.error('Redis connection error in getUserFromSession:', redisError);
    return null;
  }
}

export async function createUserSession(user: { serverSessionId: string }, redirectTo: string) {
  const session = await userSessionStorage.getSession();
  session.set('serverSessionId', user.serverSessionId);
  sessionLogger.sessionCreated(user.serverSessionId);

  return redirect(redirectTo, {
    headers: {
      'Set-Cookie': await userSessionStorage.commitSession(session),
    },
  });
}

export async function destroyUserSession(request: Request) {
  const session = await userSessionStorage.getSession(request.headers.get('Cookie'));

  const serverSessionId = session.get('serverSessionId');
  if (serverSessionId) {
    sessionLogger.sessionDestroyed(serverSessionId);
    // Remove tokens from Redis with error handling
    try {
      await redis.del(`serverSession:${serverSessionId}`);
    } catch (error) {
      SafeLogger.error('Failed to delete Redis session on destroy:', error);
    }
  }

  return redirect('/', {
    headers: {
      'Set-Cookie': await userSessionStorage.destroySession(session),
    },
  });
}

export async function requireAuth(request: Request) {
  const url = new URL(request.url);
  const result = await getUserAndTokens(request);
  if (!result) {
    sessionLogger.authCheck(url.pathname, false);
    // Clear invalid session cookie before redirecting
    const session = await userSessionStorage.getSession(request.headers.get('Cookie'));
    const sessionId = session.get('serverSessionId');
    if (sessionId) sessionLogger.sessionDestroyed(sessionId);
    throw redirect('/auth/login', {
      headers: {
        'Set-Cookie': await userSessionStorage.destroySession(session),
      },
    });
  }
  sessionLogger.authCheck(url.pathname, true, result.user.id);
  type Tokens = {
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    [key: string]: unknown;
  };

  const { user, tokens } = result as {
    user: SessionUser;
    tokens: Tokens;
  };

  // Validate that access token exists and is not expired
  const accessToken = tokens.accessToken as string;
  if (!accessToken) {
    // No access token, clear session and redirect to login
    await destroyUserSession(request);
    throw redirect('/auth/login');
  }

  // Check if token is expired by decoding the JWT payload
  try {
    const tokenParts = accessToken.split('.');
    if (tokenParts.length !== 3) {
      throw new Error('Invalid JWT format');
    }

    const payload = JSON.parse(atob(tokenParts[1]));
    const now = Math.floor(Date.now() / 1000); // Current time in seconds

    if (payload.exp && payload.exp < now) {
      // Token is expired, clear session and redirect to login
      SafeLogger.info(
        `[requireAuth] Token expired at ${new Date(
          payload.exp * 1000
        )}, current time: ${new Date()}`
      );
      await destroyUserSession(request);
      throw redirect('/auth/login');
    }
  } catch (tokenError) {
    SafeLogger.error('[requireAuth] Token validation failed:', tokenError);
    await destroyUserSession(request);
    throw redirect('/auth/login');
  }

  // Avoid printing full tokens to logs. Use a safe logger and only print a short preview.
  try {
    const { default: SafeLogger } = await import('@/utils/SafeLogger');
    const tokenInfo = tokens as { accessToken?: string };
    SafeLogger.info(
      '[requireAuth] Access token preview:',
      tokenInfo.accessToken ? `${tokenInfo.accessToken.substring(0, 10)}...` : null
    );
    SafeLogger.info('[requireAuth] Access token length:', tokenInfo.accessToken?.length);
  } catch (e) {
    // If logger import fails for any reason, don't block authentication flow.
  }

  return {
    ...user,
    accessToken: tokens.accessToken as unknown as string | undefined,
    refreshToken: tokens.refreshToken as unknown as string | undefined,
  };
}

export async function getUserAndTokens(request: Request) {
  const user = await getUserFromSession(request);
  if (!user || !user.serverSessionId) return null;

  const tokenData = await redis.get(`serverSession:${user.serverSessionId}`);
  if (!tokenData) return null;

  const tokens = JSON.parse(tokenData) as Record<string, unknown>;
  return { user, tokens };
}
