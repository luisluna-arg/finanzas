import type { ActionFunctionArgs, LoaderFunctionArgs } from "react-router";
import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "@/services/auth/auth.constants";
import serverLogger from "@/utils/logger.server";

export async function action({ request }: ActionFunctionArgs) {
    return authenticator.authenticate(AuthConstants.PROVIDER, request);
}

export async function loader({ request }: LoaderFunctionArgs) {
    serverLogger.info("Login loader called");

    // Check if user is already authenticated
    const { getUserFromSession } = await import(
        "@/services/auth/session.server"
    );
    const user = await getUserFromSession(request);

    if (user) {
        // User is already logged in, redirect to dashboard
        const { redirect } = await import("@remix-run/node");
        return redirect("/dashboard");
    }

    // Just return empty object, don't auto-authenticate
    return {};
}

export default function Login() {
    return (
        <div style={{ padding: "2rem", textAlign: "center" }}>
            <h1>Login to Finance App</h1>
            <form action="/auth/auth0" method="post">
                <button type="submit">Login with Auth0</button>
            </form>
            <p style={{ marginTop: "1rem", color: "#666" }}>
                Click the button above to authenticate with Auth0
            </p>
        </div>
    );
}
