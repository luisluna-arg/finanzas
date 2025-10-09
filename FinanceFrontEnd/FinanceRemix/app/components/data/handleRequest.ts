import { fetchData } from "./fetchData";
import SafeLogger from "@/utils/SafeLogger";

export const handleRequest = async <T = unknown>(
    url: string,
    method: string,
    record?: T,
    reload: boolean = false
): Promise<T | undefined> => {
    let responseData: T | undefined;

    try {
        const headers: HeadersInit = {
            "Content-Type": "application/json",
        };
        if (method === "DELETE") {
            headers["accept"] = "application/octet-stream";
        }

        const response = await fetch(url ?? "", {
            method: method,
            headers: headers,
            body: record === undefined ? undefined : JSON.stringify(record),
        });

        if (response.ok) {
            try {
                // attempt to parse JSON, but guard in case there's no body
                responseData = (await response.json()) as T;
            } catch (parseError) {
                // no JSON body or parse failed
                responseData = undefined;
            }
        } else {
            const errorText = await response.text().catch(() => "");
            SafeLogger.error("Request failed", {
                status: response.status,
                body: errorText,
            });
        }
    } catch (error) {
        SafeLogger.error("Error:", error);
    }

    // Avoid returning from inside finally. If reload is requested, return the refreshed data.
    if (url && reload) {
        return fetchData<T>(url);
    }

    return responseData;
};
