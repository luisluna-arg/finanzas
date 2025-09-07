/**
 * Creates a JSON response with the specified data and status code
 * @param data The data to serialize as JSON
 * @param options Response options including status code
 * @returns A Response object with JSON content
 */
export function JsonResponse<T>(
    data: T,
    options?: { status?: number; headers?: HeadersInit }
): Response {
    const { status = 200, headers = {} } = options || {};

    return new Response(JSON.stringify(data), {
        status,
        headers: {
            "Content-Type": "application/json",
            ...headers,
        },
    });
}

/**
 * Type for JSON response data
 */
export type JsonResponseData<T = unknown> = {
    data?: T;
    error?: string;
    message?: string;
    details?: unknown;
};

/**
 * Creates an error JSON response
 * @param error Error message
 * @param status HTTP status code (default: 400)
 * @param details Additional error details
 * @returns A Response object with error JSON content
 */
export function JsonErrorResponse(
    error: string,
    status: number = 400,
    details?: unknown
): Response {
    return JsonResponse<JsonResponseData>(
        {
            error,
            details,
        },
        { status }
    );
}

/**
 * Creates a success JSON response
 * @param data Response data
 * @param status HTTP status code (default: 200)
 * @returns A Response object with success JSON content
 */
export function JsonSuccessResponse<T>(
    data: T,
    status: number = 200
): Response {
    return JsonResponse<JsonResponseData<T>>(
        {
            data,
        },
        { status }
    );
}
