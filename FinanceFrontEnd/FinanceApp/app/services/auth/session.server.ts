import { createCookieSessionStorage, redirect } from 'react-router';
import redis from './redis.server';
import { verifyIdToken } from './auth.server';
import type { SessionUser } from './types/SessionUser';
import SafeLogger from '@/utils/SafeLogger';
import { sessionLogger } from '@/middleware/logger.server';
import { refreshSessionTokens, isTokenExpired } from './tokenRefresh.server';

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

  // Validate that access token exists
  let accessToken = tokens.accessToken as string;
  if (!accessToken) {
    // No access token — try to refresh if we have a refresh token
    if (user.serverSessionId) {
      const refreshResult = await refreshSessionTokens(user.serverSessionId);
      if (refreshResult) {
        accessToken = refreshResult.accessToken;
      } else {
        await destroyUserSession(request);
        throw redirect('/auth/login');
      }
    } else {
      await destroyUserSession(request);
      throw redirect('/auth/login');
    }
  }

  // Check if token is expired (or about to expire within 60s) and try to refresh
  if (isTokenExpired(accessToken)) {
    SafeLogger.info('[requireAuth] Access token expired or about to expire, attempting refresh...');
    if (!user.serverSessionId) {
      await destroyUserSession(request);
      throw redirect('/auth/login');
    }
    const refreshResult = await refreshSessionTokens(user.serverSessionId);
    if (refreshResult) {
      accessToken = refreshResult.accessToken;
      if (refreshResult.refreshed) {
        SafeLogger.info('[requireAuth] Token refreshed successfully');
      }
    } else {
      SafeLogger.info('[requireAuth] Token refresh failed, redirecting to login');
      await destroyUserSession(request);
      throw redirect('/auth/login');
    }
  }

  return {
    ...user,
    accessToken,
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
