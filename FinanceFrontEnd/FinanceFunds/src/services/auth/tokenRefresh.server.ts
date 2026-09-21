import { AuthConstants } from './auth.constants';
import redis from './redis.server';
import SafeLogger from '@/utils/SafeLogger';

interface Auth0TokenResponse {
  access_token: string;
  id_token?: string;
  refresh_token?: string;
  token_type: string;
  expires_in: number;
}

/**
 * Check whether a JWT access token is expired (or will expire within `bufferSeconds`).
 */
export function isTokenExpired(accessToken: string, bufferSeconds = 60): boolean {
  try {
    const parts = accessToken.split('.');
    if (parts.length !== 3) return true;
    const payload = JSON.parse(atob(parts[1]));
    const now = Math.floor(Date.now() / 1000);
    return !payload.exp || payload.exp < now + bufferSeconds;
  } catch {
    return true;
  }
}

async function requestNewTokens(refreshToken: string): Promise<Auth0TokenResponse> {
  const response = await fetch(`https://${AuthConstants.DOMAIN}/oauth/token`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      grant_type: 'refresh_token',
      client_id: AuthConstants.CLIENT_ID,
      client_secret: AuthConstants.CLIENT_SECRET,
      refresh_token: refreshToken,
    }),
  });

  if (!response.ok) {
    const errorBody = await response.text();
    SafeLogger.error('[tokenRefresh] Auth0 token refresh failed:', response.status, errorBody);
    throw new Error(`Token refresh failed: ${response.status}`);
  }

  return (await response.json()) as Auth0TokenResponse;
}

/**
 * Attempt to refresh the tokens stored in Redis for the given server session.
 */
export async function refreshSessionTokens(
  serverSessionId: string
): Promise<{ refreshed: boolean; accessToken: string } | null> {
  const raw = await redis.get(`serverSession:${serverSessionId}`);
  if (!raw) return null;

  const tokens = JSON.parse(raw) as {
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
  };

  const { accessToken, refreshToken } = tokens;

  if (accessToken && !isTokenExpired(accessToken)) {
    return { refreshed: false, accessToken };
  }

  if (!refreshToken) {
    SafeLogger.warn('[tokenRefresh] No refresh token available for session:', serverSessionId);
    return null;
  }

  try {
    const newTokens = await requestNewTokens(refreshToken);

    const updatedPayload = {
      accessToken: newTokens.access_token,
      refreshToken: newTokens.refresh_token ?? refreshToken,
      idToken: newTokens.id_token ?? tokens.idToken,
    };

    await redis.set(
      `serverSession:${serverSessionId}`,
      JSON.stringify(updatedPayload),
      'EX',
      60 * 60 * 24 * 7
    );

    return { refreshed: true, accessToken: newTokens.access_token };
  } catch (error) {
    SafeLogger.error('[tokenRefresh] Failed to refresh tokens:', error);
    return null;
  }
}
