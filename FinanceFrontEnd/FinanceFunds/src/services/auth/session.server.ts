import { createCookieSessionStorage, redirect } from 'react-router';
import redis from './redis.server';
import { verifyIdToken } from './auth.server';
import type { SessionUser } from './types/SessionUser';
import SafeLogger from '@/utils/SafeLogger';
import { refreshSessionTokens, isTokenExpired } from './tokenRefresh.server';

const userSessionStorage = createCookieSessionStorage({
  cookie: {
    name: '__user_session',
    httpOnly: true,
    maxAge: 60 * 60 * 24 * 7, // 7 days
    path: '/',
    sameSite: 'lax',
    secrets: (() => {
      const raw = process.env.USER_SESSION_SECRET || '';
      const list = raw
        .split(',')
        .map(s => s.trim())
        .filter(Boolean);
      if (process.env.NODE_ENV === 'production' && list.length === 0) {
        throw new Error('Missing session secret: set USER_SESSION_SECRET in the environment');
      }
      return list.length ? list : ['dev-session-secret'];
    })(),
    secure: process.env.NODE_ENV === 'production',
  },
});

export async function getUserFromSession(request: Request): Promise<SessionUser | null> {
  const session = await userSessionStorage.getSession(request.headers.get('Cookie'));

  const serverSessionId = session.get('serverSessionId');
  if (!serverSessionId) return null;

  try {
    const tokenData = await redis.get(`serverSession:${serverSessionId}`);
    if (!tokenData) {
      SafeLogger.warn('Session not found in Redis, clearing cookie:', serverSessionId);
      return null;
    }

    const tokens = JSON.parse(tokenData);
    const idToken = tokens.idToken;
    if (!idToken) {
      SafeLogger.warn('No ID token found in session:', serverSessionId);
      await redis.del(`serverSession:${serverSessionId}`);
      return null;
    }

    try {
      const payload = await verifyIdToken(idToken);

      if (!payload.sub || !payload.email) {
        SafeLogger.error('Invalid token payload - missing required fields');
        await redis.del(`serverSession:${serverSessionId}`);
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
      SafeLogger.warn('Token verification failed:', error);
      await redis.del(`serverSession:${serverSessionId}`);
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
    try {
      await redis.del(`serverSession:${serverSessionId}`);
    } catch (error) {
      SafeLogger.error('Failed to delete Redis session on destroy:', error);
    }
  }

  return redirect('/auth/login', {
    headers: {
      'Set-Cookie': await userSessionStorage.destroySession(session),
    },
  });
}

export async function getUserAndTokens(request: Request) {
  const user = await getUserFromSession(request);
  if (!user || !user.serverSessionId) return null;

  const tokenData = await redis.get(`serverSession:${user.serverSessionId}`);
  if (!tokenData) return null;

  const tokens = JSON.parse(tokenData) as Record<string, unknown>;
  return { user, tokens };
}

export async function requireAuth(request: Request) {
  const result = await getUserAndTokens(request);
  if (!result) {
    const session = await userSessionStorage.getSession(request.headers.get('Cookie'));
    throw redirect('/auth/login', {
      headers: {
        'Set-Cookie': await userSessionStorage.destroySession(session),
      },
    });
  }

  type Tokens = {
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    [key: string]: unknown;
  };

  const { user, tokens } = result as { user: SessionUser; tokens: Tokens };

  let accessToken = tokens.accessToken as string;
  if (!accessToken) {
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

  if (isTokenExpired(accessToken)) {
    if (!user.serverSessionId) {
      await destroyUserSession(request);
      throw redirect('/auth/login');
    }
    const refreshResult = await refreshSessionTokens(user.serverSessionId);
    if (refreshResult) {
      accessToken = refreshResult.accessToken;
    } else {
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
