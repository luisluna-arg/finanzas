# Auth0 + Redis Session Implementation Guide

## Current Library Versions

-   **remix-auth**: 4.2.0 ✅ (latest)
-   **remix-auth-auth0**: 2.2.0 ✅ (latest)
-   **ioredis**: 5.7.0 ✅ (latest)
-   **jose**: 6.0.12 ✅ (for JWT verification)

## Architecture Overview

# Auth0 + Redis Session Implementation Guide

## Current library versions

-   **remix-auth**: 4.2.0 ✅ (latest)
-   **remix-auth-auth0**: 2.2.0 ✅ (latest)
-   **ioredis**: 5.7.0 ✅ (latest)
-   **jose**: 6.0.12 ✅ (for JWT verification)

## Architecture overview

The authentication flow should work like this:

1. remix-auth handles the OAuth flow using the normal cookie-backed session used by the strategy (this is internal to the library).
2. After authentication completes, store the token set (access token, id token, refresh token) in Redis under a generated server-side session ID.
3. Create a separate, minimal user session cookie that stores only the server-side session ID. Use that cookie to locate tokens in Redis when needed.
4. remix-auth keeps handling the redirect / login / logout flows — the Redis-backed token storage is a post-login store for server-side token usage.

## Step-by-step implementation

### 1. Update `auth.server.ts`

The example below shows a practical implementation pattern. The exact shape of the `tokens` object depends on the strategy and the underlying library version; the snippets use defensive accessors so they work across minor differences (e.g. `tokens.id_token`, `tokens.idToken`, or `tokens.idToken()`).

```typescript
import { Authenticator } from "remix-auth";
import { Auth0Strategy } from "remix-auth-auth0";
import { AuthConstants } from "./auth.constants";
import redis from "./redis.server"; // your redis wrapper
import { randomUUID } from "crypto";
import { createRemoteJWKSet, jwtVerify } from "jose";

interface User {
    id: string;
    name: string;
    email: string;
    picture?: string;
    serverSessionId: string;
}

export const authenticator = new Authenticator<User>();

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

// Defensive helpers for token access (the token object shape can vary)
function getIdTokenFromSet(tokens: any) {
    return (
        tokens?.id_token ??
        tokens?.idToken ??
        (typeof tokens?.idToken === "function" ? tokens.idToken() : undefined)
    );
}

function getAccessTokenFromSet(tokens: any) {
    return (
        tokens?.access_token ??
        tokens?.accessToken ??
        (typeof tokens?.accessToken === "function"
            ? tokens.accessToken()
            : undefined)
    );
}

function getRefreshTokenFromSet(tokens: any) {
    return (
        tokens?.refresh_token ??
        tokens?.refreshToken ??
        (typeof tokens?.refreshToken === "function"
            ? tokens.refreshToken()
            : undefined)
    );
}

async function getUser(tokens: any) {
    const idToken = getIdTokenFromSet(tokens);
    if (idToken) {
        const payload = await verifyIdToken(idToken as string);
        return {
            id: (payload.sub as string) ?? "",
            name: (payload.name as string) ?? "",
            email: (payload.email as string) ?? "",
            picture: (payload.picture as string) ?? "",
        };
    }

    // Fallback to userinfo endpoint if no id_token available
    const accessToken = getAccessTokenFromSet(tokens);
    if (!accessToken)
        throw new Error("No access token available to fetch userinfo");

    const response = await fetch(`https://${AuthConstants.DOMAIN}/userinfo`, {
        headers: { Authorization: `Bearer ${accessToken}` },
    });
    if (!response.ok) throw new Error("Failed to fetch user info");
    const data = await response.json();

    return {
        id: data.sub ?? "",
        name: data.name ?? "",
        email: data.email ?? "",
        picture: data.picture ?? "",
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
                accessToken: getAccessTokenFromSet(tokens) ?? null,
                refreshToken: getRefreshTokenFromSet(tokens) ?? null,
                idToken: getIdTokenFromSet(tokens) ?? null,
            };

            // Persist token set in Redis (encrypt in production if needed)
            await redis.set(
                `serverSession:${serverSessionId}`,
                JSON.stringify(tokenPayload),
                "EX",
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
```

### 2. Update `session.server.ts` for user session management

Keep a minimal cookie-backed session for the user. That cookie should contain only the server-side session id (no tokens). Use `USER_SESSION_SECRET` (from env) rather than a hard-coded secret.

```typescript
import { createCookieSessionStorage, redirect } from "@remix-run/node";
import redis from "./redis.server";
import { verifyIdToken } from "./auth.server";

const userSessionStorage = createCookieSessionStorage({
    cookie: {
        name: "__user_session",
        httpOnly: true,
        maxAge: 60 * 60 * 24 * 7, // 7 days
        path: "/",
        sameSite: "lax",
        secrets: [process.env.USER_SESSION_SECRET ?? "dev-secret"],
        secure: process.env.NODE_ENV === "production",
    },
});

export async function getUserFromSession(request: Request) {
    const session = await userSessionStorage.getSession(
        request.headers.get("Cookie")
    );
    const serverSessionId = session.get("serverSessionId");
    if (!serverSessionId) return null;

    const tokenData = await redis.get(`serverSession:${serverSessionId}`);
    if (!tokenData) return null;

    const tokens = JSON.parse(tokenData);

    try {
        const idToken = tokens?.idToken ?? tokens?.id_token ?? null;
        if (idToken) {
            const payload = await verifyIdToken(idToken as string);
            return {
                id: payload.sub,
                name: payload.name,
                email: payload.email,
                picture: payload.picture,
                serverSessionId,
            };
        }
    } catch (error) {
        // Token invalid / expired — clean up server session
        await redis.del(`serverSession:${serverSessionId}`);
        return null;
    }

    return null;
}

export async function createUserSession(user: any, redirectTo: string) {
    const session = await userSessionStorage.getSession();
    session.set("serverSessionId", user.serverSessionId);

    return redirect(redirectTo, {
        headers: {
            "Set-Cookie": await userSessionStorage.commitSession(session),
        },
    });
}

export async function destroyUserSession(request: Request) {
    const session = await userSessionStorage.getSession(
        request.headers.get("Cookie")
    );
    const serverSessionId = session.get("serverSessionId");
    if (serverSessionId) {
        await redis.del(`serverSession:${serverSessionId}`);
    }

    return redirect("/", {
        headers: {
            "Set-Cookie": await userSessionStorage.destroySession(session),
        },
    });
}
```

### 3. Route handlers (examples)

app/routes/auth.login.tsx (action)

```typescript
export async function action({ request }: ActionFunctionArgs) {
    return authenticator.authenticate(AuthConstants.PROVIDER, request);
}
```

app/routes/auth.callback.tsx (callback loader)

```typescript
import type { LoaderFunctionArgs } from "@remix-run/node";
import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "@/services/auth/auth.constants";
import { createUserSession } from "@/services/auth/session.server";

export async function loader({ request }: LoaderFunctionArgs) {
    try {
        const user = await authenticator.authenticate(
            AuthConstants.PROVIDER,
            request
        );
        // On success, create the minimal user session that stores serverSessionId
        return createUserSession(user, "/dashboard");
    } catch (error) {
        if (error instanceof Response) throw error;
        const { redirect } = await import("@remix-run/node");
        return redirect("/auth/login?error=callback_failed");
    }
}
```

app/routes/auth.logout.tsx

```typescript
export async function action({ request }: ActionFunctionArgs) {
    return destroyUserSession(request);
}

export async function loader({ request }: LoaderFunctionArgs) {
    return destroyUserSession(request);
}
```

### 4. Key points

-   Two session systems:

    -   remix-auth uses its own session for the OAuth redirect flow.
    -   Your application uses a minimal cookie session that contains only the serverSessionId. Tokens live in Redis.

-   OAuth flow summary:

    -   POST/GET to `/auth/login` triggers the Auth0 redirect.
    -   Auth0 calls your `/auth/callback` route on success.
    -   Callback code stores tokens in Redis and sets the minimal user cookie with `serverSessionId`.

-   Token management:

    -   Token set is stored in Redis and TTL'd appropriately.
    -   Only the serverSessionId is stored in the cookie.
    -   Fetch tokens from Redis when you need to call your API or refresh tokens.

-   Security best practices:
    -   Never store tokens in client-side cookies or localStorage.
    -   Use HTTPS and set the cookie `secure` flag in production.
    -   Use a strong `USER_SESSION_SECRET` from env (rotate as needed).
    -   Consider encrypting token blobs at rest in Redis if required by your security policy.

### 5. Environment variables required

```bash
# Auth0 configuration
AUTH0_DOMAIN=dev-kw5mnh5gkr7n30zu.us.auth0.com
AUTH0_CLIENT_ID=7vZ7qBNw6SiVmylAx1fP4nSRDYa8ikZM
AUTH0_CLIENT_SECRET=your-actual-secret
AUTH0_AUDIENCE=urn:finances:api

# Session secret for the minimal user cookie
USER_SESSION_SECRET=your-user-session-secret

# App URL
BASE_URL=http://localhost:5300

# Redis (example)
REDIS_URL=redis://localhost:6379
```

### 6. Auth0 application configuration

In the Auth0 dashboard configure the application settings:

-   Allowed Callback URLs: `http://localhost:5300/auth/callback`
-   Allowed Logout URLs: `http://localhost:5300`
-   Allowed Web Origins: `http://localhost:5300`

## Testing the flow

1. Navigate to `/auth/login` in the browser.
2. Perform the login flow via Auth0.
3. Auth0 should redirect back to `/auth/callback` and then to your configured redirect (e.g. `/dashboard`).
4. Confirm a `serverSession:<uuid>` key exists in Redis and contains the token set.
5. Confirm the cookie `__user_session` is present and contains the `serverSessionId` value (the cookie stores a session id token, not raw tokens).

## Troubleshooting

-   Check Auth0 domain and credentials.
-   Ensure the callback URL exactly matches the one configured in Auth0 (including protocol and port).
-   Verify the Redis connection and `REDIS_URL` are correct.
-   Ensure `USER_SESSION_SECRET` is set in your environment.
-   If the user info step fails, inspect the token set saved in Redis to ensure an `idToken` or `accessToken` exists.

If you need a small Redis client wrapper, here's a recommended `redis.server.ts` for use with `ioredis`:

```typescript
// redis.server.ts
import Redis from "ioredis";

const redis = new Redis(process.env.REDIS_URL);

export default redis;
```

Optional: in production use a managed Redis instance with TLS and set connection options accordingly.

---

This guide updates the original implementation with defensive token handling, clearer session separation, and a small Redis snippet you can drop into `services`.
