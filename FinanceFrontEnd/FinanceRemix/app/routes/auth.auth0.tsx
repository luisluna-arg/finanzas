import { ActionFunctionArgs, LoaderFunctionArgs } from "@remix-run/node";
import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "@/services/auth/auth.constants";
import { createUserSession } from "@/services/auth/session.server";
import SafeLogger from "@/utils/SafeLogger";

export async function loader({ request }: LoaderFunctionArgs) {
    const user = await authenticator.authenticate(
        AuthConstants.PROVIDER,
        request
    );

    // Create user session with Redis session ID
    return createUserSession(user, "/dashboard");
}

export const action = ({ request }: ActionFunctionArgs) => {
    try {
        return authenticator.authenticate(AuthConstants.PROVIDER, request);
    } catch (error) {
        SafeLogger.error("[auth.auth0] Action error:", error);
        throw error;
    }
};
