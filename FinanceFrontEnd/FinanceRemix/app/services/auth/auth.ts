import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "./auth.constants";

export async function authenticate(request: Request) {
    try {
        return await authenticator.authenticate(
            AuthConstants.PROVIDER,
            request
        );
    } catch (error) {
        console.error("Authentication failed:", error);
        throw new Error("Authentication failed. Please log in again.");
    }
}
