import type { LoaderFunctionArgs } from "@remix-run/node";
import { sessionStorage } from "@/services/auth/session.server";
import {
    SessionContants,
    HttpStatusConstants,
} from "@/services/auth/auth.constants";
import { JsonErrorResponse } from "@/utils/JsonResponse";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const url = new URL(request.url);
    const apiPath = url.searchParams.get("path");
    if (!apiPath) {
        throw JsonErrorResponse(
            "Missing 'path' parameter",
            HttpStatusConstants.BAD_REQUEST
        );
    }

    // Remove 'path' from search params for backend API
    url.searchParams.delete("path");
    const queryString = url.searchParams.toString();

    // Build the external API URL
    const apiBaseUrl =
        process.env.VITE_API_URL || process.env.VITE_API_ENDPOINT;
    const endpoint = `${apiBaseUrl}${apiPath}${
        queryString ? `?${queryString}` : ""
    }`;

    // Get access token from session
    const cookie = request.headers.get(SessionContants.COOKIE_HEADER);
    const session = await sessionStorage.getSession(cookie);
    const user = session.get(SessionContants.USER_KEY);
    const accessToken = user?.accessToken;

    if (!accessToken) {
        throw JsonErrorResponse(
            "Not authenticated",
            HttpStatusConstants.UNAUTHORIZED
        );
    }

    try {
        // Fetch from external API
        const apiResponse = await fetch(endpoint, {
            headers: {
                Authorization: `Bearer ${accessToken}`,
                "Content-Type": "application/json",
            },
        });

        if (!apiResponse.ok) {
            const errorText = await apiResponse.text();
            throw JsonErrorResponse(
                `Backend error: ${apiResponse.status} ${apiResponse.statusText}`,
                apiResponse.status,
                errorText
            );
        }

        const data = await apiResponse.json();
        return data;
    } catch (error) {
        if (error instanceof Response) {
            throw error;
        }
        throw JsonErrorResponse(
            "Failed to fetch from backend",
            HttpStatusConstants.SERVICE_UNAVAILABLE,
            String(error)
        );
    }
};

export const shouldRevalidate = () => false;
