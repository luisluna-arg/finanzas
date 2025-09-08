import { AuthConstants } from "@/services/auth/auth.constants";
import { OAuth2Tokens } from "arctic";
import { Authenticator } from "remix-auth";
import { Auth0Strategy } from "remix-auth-auth0";
import { createRemoteJWKSet, jwtVerify, type JWTPayload } from "jose";

interface Auth0IdTokenPayload extends JWTPayload {
    sub?: string;
    name?: string;
    email?: string;
    picture?: string;
}

export const authenticator = new Authenticator<any>();

const JWKS = createRemoteJWKSet(
    new URL(`https://${AuthConstants.DOMAIN}/.well-known/jwks.json`)
);

async function verifyIdToken(idToken: string): Promise<Auth0IdTokenPayload> {
    if (!idToken) throw new Error("No idToken found in tokens");

    const { payload } = await jwtVerify(idToken, JWKS, {
        issuer: `https://${AuthConstants.DOMAIN}/`,
        audience: AuthConstants.CLIENT_ID,
    });

    return payload as Auth0IdTokenPayload;
}

async function getUser(tokens: OAuth2Tokens) {
    const idToken = tokens.idToken();
    const payload = await verifyIdToken(idToken);

    return {
        id: payload.sub ?? "",
        name: (payload.name as string) ?? "",
        email: (payload.email as string) ?? "",
        picture: (payload.picture as string) ?? "",
    };
}

import serverLogger from "@/utils/logger.server";

serverLogger.info("AuthConstants loaded for Auth0Strategy");

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
            return {
                ...user,
                accessToken: tokens.accessToken(),
                refreshToken: tokens.hasRefreshToken()
                    ? tokens.refreshToken()
                    : null,
            };
        }
    ),
    AuthConstants.PROVIDER
);
