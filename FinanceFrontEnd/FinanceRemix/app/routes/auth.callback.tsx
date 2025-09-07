import { redirect, type LoaderFunction } from "@remix-run/node";
import { authenticator } from "@/services/auth/auth.server";
import {
    AuthConstants,
    SessionContants,
    HttpStatusConstants,
} from "@/services/auth/auth.constants";
import { sessionStorage, commitSession } from "@/services/auth/session.server";

export const loader: LoaderFunction = async ({ request }) => {
    try {
        // Authenticate and get user
        const user = await authenticator.authenticate(
            AuthConstants.PROVIDER,
            request
        );

        // Get session
        const session = await sessionStorage.getSession(
            request.headers.get(SessionContants.COOKIE_HEADER)
        );

        // Store user in session (server-side only)
        session.set(SessionContants.USER_KEY, user);

        // Commit session and redirect
        return redirect("/dashboard", {
            headers: {
                [SessionContants.SET_COOKIE_HEADER]: await commitSession(
                    session
                ),
            },
        });
    } catch (error: any) {
        // If authentication fails with 403, redirect to forbidden page
        if (error?.status === HttpStatusConstants.FORBIDDEN) {
            return redirect("/auth/forbidden");
        }
        // Otherwise, redirect to login
        return redirect("/auth/login");
    }
};
