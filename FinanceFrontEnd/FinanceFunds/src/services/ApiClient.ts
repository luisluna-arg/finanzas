// Base API client for making HTTP requests
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

// Remove trailing slashes to avoid double slash issues in URL construction
const BASE_URL = API_BASE_URL.endsWith('/') ? API_BASE_URL.slice(0, -1) : API_BASE_URL;

// Only log in development
if (import.meta.env.DEV) {
  console.log('API URL:', BASE_URL);
}

// Common headers
const COMMON_HEADERS = {
  'Content-Type': 'application/json',
};

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
        headers: COMMON_HEADERS,
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
        console.error(`API request error for ${url}:`, error);
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
  async post<T>(endpoint: string, data: any): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      if (import.meta.env.DEV) {
        console.log(`Sending POST request to ${url} with data:`, data);
      }

      const response = await fetch(url, {
        method: 'POST',
        headers: COMMON_HEADERS,
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      const responseData = await response.json();

      if (import.meta.env.DEV) {
        console.log(`Received response from ${url}:`, responseData);
      }

      return responseData;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.error(`API request error for ${url}:`, error);
      }
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
  async put<T>(endpoint: string, data: any): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      if (import.meta.env.DEV) {
        console.log(`Sending PUT request to ${url} with data:`, data);
      }

      const response = await fetch(url, {
        method: 'PUT',
        headers: COMMON_HEADERS,
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      const responseData = await response.json();

      if (import.meta.env.DEV) {
        console.log(`Received response from ${url}:`, responseData);
      }

      return responseData;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.error(`API request error for ${url}:`, error);
      }
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
  async delete<T = void>(endpoint: string, data?: any): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      if (import.meta.env.DEV) {
        console.log(`Sending DELETE request to ${url}`, data ? `with data: ${JSON.stringify(data)}` : '');
      }

      const response = await fetch(url, {
        method: 'DELETE',
        headers: COMMON_HEADERS,
        ...(data && { body: JSON.stringify(data) }),
      });

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
        if (import.meta.env.DEV) {
          console.log(`Received response from ${url}:`, responseData);
        }
        return responseData;
      }

      return undefined as T;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.error(`API request error for ${url}:`, error);
      }
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
  async patch<T = void>(endpoint: string, data?: any): Promise<T> {
    // Ensure endpoint doesn't start with a slash to avoid double slashes
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
    const url = `${BASE_URL}/${normalizedEndpoint}`;

    try {
      if (import.meta.env.DEV) {
        console.log(`Sending PATCH request to ${url}`, data ? `with data: ${JSON.stringify(data)}` : '');
      }

      const response = await fetch(url, {
        method: 'PATCH',
        headers: COMMON_HEADERS,
        ...(data && { body: JSON.stringify(data) }),
      });

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
        if (import.meta.env.DEV) {
          console.log(`Received response from ${url}:`, responseData);
        }
        return responseData;
      }

      return undefined as T;
    } catch (error) {
      if (import.meta.env.DEV) {
        console.error(`API request error for ${url}:`, error);
      }
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
