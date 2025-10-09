import { createCookieSessionStorage, redirect } from "@remix-run/node";
import redis from "./redis.server";
import { verifyIdToken } from "./auth.server";

// User session storage (different from auth flow)
const userSessionStorage = createCookieSessionStorage({
    cookie: {
        name: "__user_session",
        httpOnly: true,
        maxAge: 60 * 60 * 24 * 7, // 7 days
        path: "/",
        sameSite: "lax",
        // Read session secrets from environment for production safety. Support
        // comma-separated secrets to allow rotation (old, new).
        // Prefer runtime secret injection (USER_SESSION_SECRET or SESSION_SECRET).
        // In production a secret must be provided; in development we provide a
        // non-sensitive fallback to simplify local setup.
        // Example: USER_SESSION_SECRET="s1,s2"
        secrets: (() => {
            const raw =
                process.env.USER_SESSION_SECRET ||
                process.env.SESSION_SECRET ||
                "";
            const list = raw
                .split(",")
                .map((s) => s.trim())
                .filter(Boolean);
            if (process.env.NODE_ENV === "production" && list.length === 0) {
                throw new Error(
                    "Missing session secret: set USER_SESSION_SECRET or SESSION_SECRET in the environment for production"
                );
            }
            return list.length ? list : ["dev-session-secret"];
        })(),
        secure: process.env.NODE_ENV === "production",
    },
});

export async function getUserFromSession(request: Request) {
    const session = await userSessionStorage.getSession(
        request.headers.get("Cookie")
    );

    const serverSessionId = session.get("serverSessionId");
    if (!serverSessionId) return null;

    // Get tokens from Redis
    const tokenData = await redis.get(`serverSession:${serverSessionId}`);
    if (!tokenData) return null;

    const tokens = JSON.parse(tokenData);

    // Verify ID token and return user info
    try {
        const idToken = tokens.idToken;
        if (idToken) {
            const payload = await verifyIdToken(idToken);
            return {
                id: payload.sub,
                name: payload.name,
                email: payload.email,
                picture: payload.picture,
                serverSessionId,
            };
        }
    } catch (error) {
        // Token expired or invalid, remove session
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
        // Remove tokens from Redis
        await redis.del(`serverSession:${serverSessionId}`);
    }

    return redirect("/", {
        headers: {
            "Set-Cookie": await userSessionStorage.destroySession(session),
        },
    });
}

export async function requireAuth(request: Request) {
    const result = await getUserAndTokens(request);
    if (!result) {
        throw redirect("/auth/login");
    }

    const { user, tokens } = result;

    // Avoid printing full tokens to logs. Use a safe logger and only print a short preview.
    try {
        const { default: SafeLogger } = await import("@/utils/SafeLogger");
        SafeLogger.info(
            "[requireAuth] Access token preview:",
            tokens.accessToken
                ? `${tokens.accessToken.substring(0, 10)}...`
                : null
        );
        SafeLogger.info(
            "[requireAuth] Access token length:",
            tokens.accessToken?.length
        );
    } catch (e) {
        // If logger import fails for any reason, don't block authentication flow.
    }

    return {
        ...user,
        accessToken: tokens.accessToken,
        refreshToken: tokens.refreshToken,
    };
}

export async function getUserAndTokens(request: Request) {
    const user = await getUserFromSession(request);
    if (!user || !user.serverSessionId) return null;

    const tokenData = await redis.get(`serverSession:${user.serverSessionId}`);
    if (!tokenData) return null;

    const tokens = JSON.parse(tokenData);
    return { user, tokens };
}
