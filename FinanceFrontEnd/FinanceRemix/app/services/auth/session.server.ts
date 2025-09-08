import { createCookieSessionStorage } from "@remix-run/node";
import { User } from "./types/User";
import { SessionContants, HttpStatusConstants } from "./auth.constants";

export const sessionStorage = createCookieSessionStorage({
    cookie: {
        name: "__session",
        httpOnly: true,
        path: "/",
        sameSite: "lax",
        secrets: [process.env.SESSION_SECRET!],
        secure: process.env.NODE_ENV === "production",
    },
});

export const { getSession, commitSession, destroySession } = sessionStorage;

// Helper function to get user from session
export async function getUserFromSession(request: Request): Promise<User | null> {
    const cookieHeader = request.headers.get(SessionContants.COOKIE_HEADER);
    const session = await getSession(cookieHeader);
    return session.get(SessionContants.USER_KEY) || null;
}

// Helper function to require authentication
export async function requireAuth(request: Request): Promise<User> {
    const user = await getUserFromSession(request);
    if (!user) {
        throw new Response("Unauthorized", { status: HttpStatusConstants.UNAUTHORIZED });
    }
    return user;
}
