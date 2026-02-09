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
 * Returns `true` when the token should be refreshed.
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

/**
 * Exchange a refresh token for a new access token (and optionally a new refresh token)
 * using Auth0's `/oauth/token` endpoint.
 */
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
 *
 * - If the access token is still valid, returns `{ refreshed: false }`.
 * - If no refresh token is available, returns `null` (caller should redirect to login).
 * - On success, updates Redis and returns the new access token.
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

  // If the access token is still valid, nothing to do
  if (accessToken && !isTokenExpired(accessToken)) {
    return { refreshed: false, accessToken };
  }

  // No refresh token stored — can't refresh silently
  if (!refreshToken) {
    SafeLogger.warn('[tokenRefresh] No refresh token available for session:', serverSessionId);
    return null;
  }

  try {
    SafeLogger.info('[tokenRefresh] Refreshing tokens for session:', serverSessionId);
    const newTokens = await requestNewTokens(refreshToken);

    // Build updated payload — Auth0 may or may not return a rotated refresh token
    const updatedPayload = {
      accessToken: newTokens.access_token,
      refreshToken: newTokens.refresh_token ?? refreshToken, // keep old if not rotated
      idToken: newTokens.id_token ?? tokens.idToken, // keep old if not returned
    };

    await redis.set(
      `serverSession:${serverSessionId}`,
      JSON.stringify(updatedPayload),
      'EX',
      60 * 60 * 24 * 7 // 7 days
    );

    SafeLogger.info('[tokenRefresh] Tokens refreshed successfully for session:', serverSessionId);
    return { refreshed: true, accessToken: newTokens.access_token };
  } catch (error) {
    SafeLogger.error('[tokenRefresh] Failed to refresh tokens:', error);
    return null;
  }
}
