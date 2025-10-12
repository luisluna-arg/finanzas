// Base API client for making HTTP requests
import SafeLogger from '@/utils/SafeLogger';

// Acquire configured API URL. In development, allow a localhost fallback for convenience.
const rawApiUrl = import.meta.env.VITE_API_URL;
const allowInsecure = Boolean(import.meta.env.VITE_ALLOW_INSECURE_API === 'true');
let API_BASE_URL: string | undefined = rawApiUrl;

// Debug logging
SafeLogger.log('Environment Debug:', {
  VITE_API_URL: import.meta.env.VITE_API_URL,
  VITE_ALLOW_INSECURE_API: import.meta.env.VITE_ALLOW_INSECURE_API,
  DEV: import.meta.env.DEV,
  allowInsecure: allowInsecure,
});

if (!API_BASE_URL) {
  if (import.meta.env.DEV) {
    // In dev only, allow explicit localhost so dev servers work without env vars
    API_BASE_URL = 'http://localhost:5000';
  } else {
    // In non-dev (staging/production) do not allow an implicit fallback
    throw new Error(
      'Missing required environment variable VITE_API_URL. Set the API base URL and ensure it uses HTTPS in production.'
    );
  }
}

// Ensure HTTPS is used in non-development environments unless explicitly allowed
// Allow localhost URLs in any environment for local development
const isLocalhost = API_BASE_URL.includes('localhost') || API_BASE_URL.includes('127.0.0.1');
const shouldEnforceHttps = !import.meta.env.DEV && !allowInsecure && !isLocalhost;

if (shouldEnforceHttps) {
  try {
    const parsed = new URL(API_BASE_URL);
    if (parsed.protocol !== 'https:') {
      throw new Error(
        `Insecure API base URL protocol '${parsed.protocol}'. Production requires HTTPS or set VITE_ALLOW_INSECURE_API=true to override.`
      );
    }
  } catch (err) {
    // Re-throw with clearer message
    throw new Error(
      `Invalid VITE_API_URL '${API_BASE_URL}'. Ensure it is a valid HTTPS URL. ${err instanceof Error ? err.message : ''}`
    );
  }
}

// Remove trailing slashes to avoid double slash issues in URL construction
const BASE_URL = API_BASE_URL.endsWith('/') ? API_BASE_URL.slice(0, -1) : API_BASE_URL;

// Only log the resolved API URL in development; avoid logging production endpoints
if (import.meta.env.DEV) {
  SafeLogger.info('API URL:', BASE_URL);
}

// Common headers
const COMMON_HEADERS = {
  'Content-Type': 'application/json',
};

// Token provider function type
type TokenProvider = () => Promise<string>;

let tokenProvider: TokenProvider | null = null;

/**
 * Set the function that provides the Auth0 access token
 */
export function setTokenProvider(provider: TokenProvider) {
  tokenProvider = provider;
}

// Error processing
async function processErrorResponse(response: Response): Promise<string> {
  try {
    const errorResponse = await response.json();
    return errorResponse.message || errorResponse.error || `Status: ${response.status}`;
  } catch {
    try {
      const errorText = await response.text();
      return errorText || `Status: ${response.status}`;
    } catch {
      return `Status: ${response.status} ${response.statusText}`;
    }
  }
}

// Helper to get headers with Authorization
async function getAuthHeaders() {
  if (!tokenProvider) {
    throw new Error('Token provider not set. Please call setTokenProvider in your app startup.');
  }
  const token = await tokenProvider();
  return {
    ...COMMON_HEADERS,
    Authorization: `Bearer ${token}`,
  };
}

/**
 * Base API client for making HTTP requests
 */
class ApiClient {
  /**
   * Make a GET request to the API
   *
   * @param endpoint - The API endpoint to request
   * @param queryParams - Optional query parameters
   * @returns Promise with the response data
   */
  async get<T>(
    endpoint: string,
    queryParams?: Record<string, string | number | boolean | null | undefined>
  ): Promise<T> {
    const url = this.buildUrl(endpoint, queryParams);

    try {
      const response = await fetch(url, {
        method: 'GET',
        headers: await getAuthHeaders(),
        // Add cache control for better performance
        cache: 'default',
      });

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      return await response.json();
    } catch (error) {
      if (import.meta.env.DEV) {
        // Use SafeLogger to redact sensitive values in dev and avoid raw console in code
        const { default: SafeLogger } = await import('@/utils/SafeLogger');
        SafeLogger.error(`API request error for ${url}:`, error);
      }
      throw error;
    }
  }

  /**
   * Make a POST request to the API
   *
   * @param endpoint - The API endpoint to request
   * @param data - The data to send in the request body
   * @returns Promise with the response data
   */
  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      SafeLogger.info(`Sending POST request to ${url} with data:`, data);

      const response = await fetch(url, {
        method: 'POST',
        headers: await getAuthHeaders(),
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      const responseData = await response.json();

      SafeLogger.info(`Received response from ${url}:`, responseData);

      return responseData;
    } catch (error) {
      SafeLogger.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  /**
   * Make a PUT request to the API
   *
   * @param endpoint - The API endpoint to request
   * @param data - The data to send in the request body
   * @returns Promise with the response data
   */
  async put<T>(endpoint: string, data?: unknown): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      SafeLogger.info(`Sending PUT request to ${url} with data:`, data);

      const response = await fetch(url, {
        method: 'PUT',
        headers: await getAuthHeaders(),
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      const responseData = await response.json();

      SafeLogger.info(`Received response from ${url}:`, responseData);

      return responseData;
    } catch (error) {
      SafeLogger.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  /**
   * Make a DELETE request to the API
   *
   * @param endpoint - The API endpoint to request
   * @param data - Optional data to send in the request body
   * @returns Promise with the response data
   */
  async delete<T = void>(endpoint: string, data?: unknown): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      SafeLogger.info(
        `Sending DELETE request to ${url}`,
        data ? `with data: ${JSON.stringify(data)}` : ''
      );

      const deleteOptions: RequestInit = {
        method: 'DELETE',
        headers: await getAuthHeaders(),
      };
      if (data !== undefined) deleteOptions.body = JSON.stringify(data as unknown);

      const response = await fetch(url, deleteOptions);

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      // Handle empty responses
      const text = await response.text();
      if (text) {
        const responseData = JSON.parse(text);
        SafeLogger.info(`Received response from ${url}:`, responseData);
        return responseData;
      }

      return undefined as T;
    } catch (error) {
      SafeLogger.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  /**
   * Make a PATCH request to the API
   *
   * @param endpoint - The API endpoint to request
   * @param data - Optional data to send in the request body
   * @returns Promise with the response data
   */
  async patch<T = void>(endpoint: string, data?: unknown): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      SafeLogger.info(
        `Sending PATCH request to ${url}`,
        data ? `with data: ${JSON.stringify(data)}` : ''
      );

      const patchOptions: RequestInit = {
        method: 'PATCH',
        headers: await getAuthHeaders(),
      };
      if (data !== undefined) patchOptions.body = JSON.stringify(data as unknown);

      const response = await fetch(url, patchOptions);

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      // Handle empty responses
      const text = await response.text();
      if (text) {
        const responseData = JSON.parse(text);
        SafeLogger.info(`Received response from ${url}:`, responseData);
        return responseData;
      }

      return undefined as T;
    } catch (error) {
      SafeLogger.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  /**
   * Build a URL with query parameters
   *
   * @param endpoint - The API endpoint
   * @param params - Query parameters to add to the URL
   * @returns The formatted URL
   */
  private buildUrl(
    endpoint: string,
    params?: Record<string, string | number | boolean | null | undefined>
  ): string {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;

    // Avoid creating a URL object for simple cases
    if (!params || Object.keys(params).length === 0) {
      return `${BASE_URL}/${normalizedEndpoint}`;
    }

    const url = new URL(`${BASE_URL}/${normalizedEndpoint}`);

    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        url.searchParams.append(key, value.toString());
      }
    });

    return url.toString();
  }
}

export default new ApiClient();
