// Base API client for making HTTP requests.
//
// Dual-mode: in the browser it always calls same-origin `/api/proxy?path=...`
// (no token attached — the proxy route resolves it server-side from the session
// cookie), so the default singleton export below is safe to share across a tab.
// On the server it calls the backend directly with an explicit per-request
// access token, since a shared module-level token would leak between
// concurrent requests from different users.
import SafeLogger from '@/utils/SafeLogger';

const isServer = typeof window === 'undefined';

const COMMON_HEADERS = {
  'Content-Type': 'application/json',
};

function resolveServerBaseUrl(): string {
  const rawApiUrl = process.env.API_URL;
  const allowInsecure = process.env.ALLOW_INSECURE_API === 'true';
  let apiBaseUrl = rawApiUrl;

  if (!apiBaseUrl) {
    if (process.env.NODE_ENV !== 'production') {
      apiBaseUrl = 'http://localhost:5000';
    } else {
      throw new Error(
        'Missing required environment variable API_URL. Set the API base URL and ensure it uses HTTPS in production.'
      );
    }
  }

  const isLocalhost = apiBaseUrl.includes('localhost') || apiBaseUrl.includes('127.0.0.1');
  const shouldEnforceHttps =
    process.env.NODE_ENV === 'production' && !allowInsecure && !isLocalhost;

  if (shouldEnforceHttps) {
    const parsed = new URL(apiBaseUrl);
    if (parsed.protocol !== 'https:') {
      throw new Error(
        `Insecure API base URL protocol '${parsed.protocol}'. Production requires HTTPS or set ALLOW_INSECURE_API=true to override.`
      );
    }
  }

  return apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl;
}

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

class ApiClient {
  constructor(private readonly accessToken?: string) {}

  async get<T>(
    endpoint: string,
    queryParams?: Record<string, string | number | boolean | null | undefined>
  ): Promise<T> {
    const url = this.buildUrl(endpoint, queryParams);
    return this.request<T>(url, { method: 'GET', headers: this.getHeaders() });
  }

  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    const url = this.buildUrl(endpoint);
    return this.request<T>(url, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(data),
    });
  }

  async put<T>(endpoint: string, data?: unknown): Promise<T> {
    const url = this.buildUrl(endpoint);
    return this.request<T>(url, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify(data),
    });
  }

  async delete<T = void>(endpoint: string, data?: unknown): Promise<T> {
    const url = this.buildUrl(endpoint);
    const options: RequestInit = { method: 'DELETE', headers: this.getHeaders() };
    if (data !== undefined) options.body = JSON.stringify(data);
    return this.request<T>(url, options, true);
  }

  async patch<T = void>(endpoint: string, data?: unknown): Promise<T> {
    const url = this.buildUrl(endpoint);
    const options: RequestInit = { method: 'PATCH', headers: this.getHeaders() };
    if (data !== undefined) options.body = JSON.stringify(data);
    return this.request<T>(url, options, true);
  }

  private getHeaders(): HeadersInit {
    if (isServer) {
      if (!this.accessToken) {
        throw new Error('ApiClient used server-side without an access token.');
      }
      return { ...COMMON_HEADERS, Authorization: `Bearer ${this.accessToken}` };
    }
    // Browser mode: no Authorization header — /api/proxy attaches it from the session cookie.
    return COMMON_HEADERS;
  }

  private async request<T>(
    url: string,
    options: RequestInit,
    allowEmptyResponse = false
  ): Promise<T> {
    try {
      const response = await fetch(url, options);

      if (!response.ok) {
        const errorMessage = await processErrorResponse(response);
        throw new Error(
          `API request failed: ${response.status} ${response.statusText} - ${errorMessage}`
        );
      }

      if (allowEmptyResponse) {
        const text = await response.text();
        return (text ? JSON.parse(text) : undefined) as T;
      }

      return (await response.json()) as T;
    } catch (error) {
      SafeLogger.error(`API request error for ${url}:`, error);
      throw error;
    }
  }

  private buildUrl(
    endpoint: string,
    params?: Record<string, string | number | boolean | null | undefined>
  ): string {
    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`;

    if (isServer) {
      const url = new URL(`${resolveServerBaseUrl()}${normalizedEndpoint}`);
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) url.searchParams.append(key, value.toString());
        });
      }
      return url.toString();
    }

    // Browser mode: route through the same-origin proxy, which forwards `path`
    // (and any other query params) to the real backend with the session's token.
    const proxyParams = new URLSearchParams({ path: normalizedEndpoint });
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) proxyParams.append(key, value.toString());
      });
    }
    return `/api/proxy?${proxyParams.toString()}`;
  }
}

export { ApiClient };
export default new ApiClient();
