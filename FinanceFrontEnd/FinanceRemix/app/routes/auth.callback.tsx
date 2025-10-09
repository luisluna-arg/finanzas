import type { LoaderFunctionArgs } from "@remix-run/node";
import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "@/services/auth/auth.constants";
import { createUserSession } from "@/services/auth/session.server";
import SafeLogger from "@/utils/SafeLogger";

export async function loader({ request }: LoaderFunctionArgs) {
    SafeLogger.info("[auth.callback] Callback loader called");
    SafeLogger.info("[auth.callback] Request URL:", request.url);

    try {
        const user = await authenticator.authenticate(
            AuthConstants.PROVIDER,
            request
        );
        SafeLogger.info(
            "[auth.callback] Authentication successful, user:",
            user
        );

        // Create user session with Redis session ID
        return createUserSession(user, "/dashboard");
    } catch (error) {
        SafeLogger.error("[auth.callback] Authentication failed:", error);

        // If it's a redirect Response, throw it
        if (error instanceof Response) {
            throw error;
        }

        // For other errors, redirect to login
        const { redirect } = await import("@remix-run/node");
        return redirect("/auth/login?error=callback_failed");
    }
}
