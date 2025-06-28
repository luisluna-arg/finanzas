// Base API client for making HTTP requests
const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000";

// Add console log to ensure the API URL is read correctly
console.log("API URL:", API_BASE_URL);

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
   */  async get<T>(
    endpoint: string,
    queryParams?: Record<string, string | number | boolean | null | undefined>,
  ): Promise<T> {
    const url = this.buildUrl(endpoint, queryParams);

    try {
      const response = await fetch(url, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
        // Don't include credentials for now to avoid CORS issues
        // credentials: 'include',
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorText}`,
        );
      }

      return await response.json();
    } catch (error) {
      console.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  /**
   * Build a URL with query parameters
   *
   * @param endpoint - The API endpoint
   * @param params - Query parameters to add to the URL
   * @returns The formatted URL
   */  private buildUrl(
    endpoint: string, 
    params?: Record<string, string | number | boolean | null | undefined>
  ): string {
    const url = new URL(`${API_BASE_URL}/${endpoint}`);

    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          url.searchParams.append(key, value.toString());
        }
      });
    }

    return url.toString();
  }
}

export default new ApiClient();
