import { Authenticator } from 'remix-auth';
import { Auth0Strategy } from 'remix-auth-auth0';
import { randomUUID } from 'crypto';
import { createRemoteJWKSet, jwtVerify } from 'jose';
import { AuthConstants } from './auth.constants';
import redis from './redis.server';

interface User {
  id: string;
  name: string;
  email: string;
  picture: string;
  serverSessionId: string;
}

export const authenticator = new Authenticator<User>();

const JWKS = createRemoteJWKSet(new URL(`https://${AuthConstants.DOMAIN}/.well-known/jwks.json`));

export async function verifyIdToken(idToken: string) {
  const verifyPromise = jwtVerify(idToken, JWKS, {
    issuer: `https://${AuthConstants.DOMAIN}/`,
    audience: AuthConstants.CLIENT_ID,
  });
  const timeoutPromise = new Promise<never>((_, reject) =>
    setTimeout(() => reject(new Error('JWKS verification timed out')), 5000)
  );
  const { payload } = await Promise.race([verifyPromise, timeoutPromise]);
  return payload;
}

type TokenSet = {
  idToken?: () => string | undefined;
  accessToken?: () => string | undefined;
  refreshToken?: () => string | undefined;
  hasRefreshToken?: () => boolean;
};

async function getUser(tokens: TokenSet) {
  const idToken = tokens.idToken?.();
  if (idToken) {
    const payload = (await verifyIdToken(idToken)) as Record<string, unknown>;
    return {
      id: (payload.sub as string) ?? '',
      name: (payload.name as string) ?? '',
      email: (payload.email as string) ?? '',
      picture: (payload.picture as string) ?? '',
    };
  }

  const accessToken = tokens.accessToken?.();
  const response = await fetch(`https://${AuthConstants.DOMAIN}/userinfo`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch user info');
  }

  const data = (await response.json()) as Record<string, unknown>;
  return {
    id: (data.sub as string) ?? '',
    name: (data.name as string) ?? '',
    email: (data.email as string) ?? '',
    picture: (data.picture as string) ?? '',
  };
}

authenticator.use(
  new Auth0Strategy(
    {
      domain: AuthConstants.DOMAIN,
      clientId: AuthConstants.CLIENT_ID,
      clientSecret: AuthConstants.CLIENT_SECRET,
      redirectURI: AuthConstants.REDIRECT_URI,
      scopes: AuthConstants.SCOPES,
      audience: AuthConstants.AUDIENCE,
    },
    async ({ tokens }) => {
      const user = await getUser(tokens);
      const serverSessionId = randomUUID();

      const tokenPayload = {
        accessToken: tokens.accessToken(),
        refreshToken: tokens.hasRefreshToken() ? tokens.refreshToken() : null,
        idToken: tokens.idToken(),
      };

      await redis.set(
        `serverSession:${serverSessionId}`,
        JSON.stringify(tokenPayload),
        'EX',
        60 * 60 * 24 * 7 // 7 days
      );

      return {
        id: user.id,
        name: user.name,
        email: user.email,
        picture: user.picture,
        serverSessionId,
      };
    }
  ),
  AuthConstants.PROVIDER
);
