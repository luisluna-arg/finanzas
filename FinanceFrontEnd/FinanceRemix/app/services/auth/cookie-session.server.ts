import { createCookieSessionStorage } from "@remix-run/node";

// This is ONLY for remix-auth OAuth flow, not for user sessions
export const cookieSessionStorage = createCookieSessionStorage({
    cookie: {
        name: "__auth_session", // Different from user session cookie
        httpOnly: true,
        maxAge: 60 * 10, // 10 minutes (just for OAuth flow)
        path: "/",
        sameSite: "lax",
        secrets: ["your-secret-key"], // Use a proper secret
        secure: process.env.NODE_ENV === "production",
    },
});
