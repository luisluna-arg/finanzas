import { createCookieSessionStorage } from "@remix-run/node";

// Create a session storage for remix-auth to use
export const sessionStorage = createCookieSessionStorage({
    cookie: {
        name: "_auth",
        sameSite: "lax",
        path: "/",
        httpOnly: true,
        secrets: ["s3cr3t"], // In production, use a secure secret
        secure: process.env.NODE_ENV === "production",
    },
});

export const { getSession, commitSession, destroySession } = sessionStorage;
