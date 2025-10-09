import { Authenticator } from "remix-auth";
import { Auth0Strategy } from "remix-auth-auth0";
import { AuthConstants } from "./auth.constants";
import redis from "./redis.server";
import { randomUUID } from "crypto";
import { createRemoteJWKSet, jwtVerify } from "jose";

interface User {
    id: string;
    name: string;
    email: string;
    picture: string;
    serverSessionId: string;
}

// Create authenticator (no session storage parameter needed)
export const authenticator = new Authenticator<User>();

// JWT verification setup
const JWKS = createRemoteJWKSet(
    new URL(`https://${AuthConstants.DOMAIN}/.well-known/jwks.json`)
);

export async function verifyIdToken(idToken: string) {
    const { payload } = await jwtVerify(idToken, JWKS, {
        issuer: `https://${AuthConstants.DOMAIN}/`,
        audience: AuthConstants.CLIENT_ID,
    });
    return payload;
}

async function getUser(tokens: any) {
    const idToken = tokens.idToken();
    if (idToken) {
        const payload = await verifyIdToken(idToken);
        return {
            id: payload.sub ?? "",
            name: (payload.name as string) ?? "",
            email: (payload.email as string) ?? "",
            picture: (payload.picture as string) ?? "",
        };
    }

    // Fallback to userinfo endpoint if no id_token
    const accessToken = tokens.accessToken();
    const response = await fetch(`https://${AuthConstants.DOMAIN}/userinfo`, {
        headers: { Authorization: `Bearer ${accessToken}` },
    });

    if (!response.ok) {
        throw new Error("Failed to fetch user info");
    }

    const data = await response.json();
    return {
        id: data.sub ?? "",
        name: data.name ?? "",
        email: data.email ?? "",
        picture: data.picture ?? "",
    };
}

// Configure Auth0 Strategy
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
            // Get user info from tokens
            const user = await getUser(tokens);

            // Create a session ID for Redis storage
            const serverSessionId = randomUUID();

            // Store tokens in Redis
            const tokenPayload = {
                accessToken: tokens.accessToken(),
                refreshToken: tokens.hasRefreshToken()
                    ? tokens.refreshToken()
                    : null,
                idToken: tokens.idToken(),
            };

            await redis.set(
                `serverSession:${serverSessionId}`,
                JSON.stringify(tokenPayload),
                "EX",
                60 * 60 * 24 * 7 // 7 days
            );

            // Return user with serverSessionId
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
