import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "./auth.constants";
import serverLogger from "@/utils/logger.server";

export async function authenticate(request: Request) {
    try {
        return await authenticator.authenticate(
            AuthConstants.PROVIDER,
            request
        );
    } catch (error) {
        serverLogger.error("Authentication failed:", error);
        throw new Error("Authentication failed. Please log in again.");
    }
}
