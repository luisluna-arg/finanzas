import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "./auth.constants";

// Extract user information from JWT Id token
export function decodeIdToken(idToken: string): any {
    if (!idToken) {
        throw new Error("No Id Token found in tokens");
    }

    // Decode the JWT (without verifying for simplicity, use a library for production)
    const base64Url = idToken.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split("")
            .map(function (c) {
                return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
            })
            .join("")
    );
    return JSON.parse(jsonPayload);
}

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
