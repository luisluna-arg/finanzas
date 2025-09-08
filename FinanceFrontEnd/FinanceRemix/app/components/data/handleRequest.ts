import { fetchData } from "./fetchData";
import SafeLogger from "@/utils/SafeLogger";

export const handleRequest = async (url: string, method: string, record: any, reload: boolean = false) => {
    try {
        const headers: HeadersInit = {
            "Content-Type": "application/json",
        };
        if (method === "DELETE") {
            headers["accept"] = "application/octet-stream";
        }

        const response = await fetch(url ?? '', {
            method: method,
            headers: headers,
            body: JSON.stringify(record),
        });

        if (response.ok) {
            // Handle successful response
        } else {
            // Handle error response
        }
    } catch (error) {
        SafeLogger.error("Error:", error);
    } finally {
        if (url && reload) return fetchData(url);
    }
};