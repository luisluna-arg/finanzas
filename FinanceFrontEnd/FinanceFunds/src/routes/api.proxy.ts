import type { ActionFunctionArgs, LoaderFunctionArgs } from 'react-router';
import { getUserAndTokens } from '@/services/auth/session.server';
import { HttpStatusConstants } from '@/services/auth/auth.constants';
import { refreshSessionTokens, isTokenExpired } from '@/services/auth/tokenRefresh.server';

function jsonError(message: string, status: number, details?: string) {
  return new Response(JSON.stringify({ message, details }), {
    status,
    headers: { 'Content-Type': 'application/json' },
  });
}

function resolveApiBaseUrl(): { apiBaseUrl: string } | Response {
  const apiBaseUrl = process.env.API_URL;
  const allowInsecure = process.env.ALLOW_INSECURE_API === 'true';

  if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
    if (!apiBaseUrl) {
      return jsonError(
        'Server misconfiguration: missing API_URL. Contact administrators.',
        HttpStatusConstants.SERVICE_UNAVAILABLE
      );
    }
    try {
      const parsed = new URL(apiBaseUrl);
      if (parsed.protocol !== 'https:' && !allowInsecure) {
        return jsonError(
          'Insecure backend configuration: API base URL must use HTTPS in production unless ALLOW_INSECURE_API is enabled.',
          HttpStatusConstants.SERVICE_UNAVAILABLE
        );
      }
    } catch (err) {
      return jsonError(`Invalid API_URL: ${String(err)}`, HttpStatusConstants.SERVICE_UNAVAILABLE);
    }
  }

  return { apiBaseUrl: apiBaseUrl ?? 'http://localhost:5000' };
}

async function resolveAccessToken(request: Request): Promise<string | Response> {
  const result = await getUserAndTokens(request);
  if (!result) {
    return jsonError('Not authenticated', HttpStatusConstants.UNAUTHORIZED);
  }

  const { user, tokens } = result;
  let accessToken = tokens.accessToken as string | undefined;

  if (!accessToken || isTokenExpired(accessToken)) {
    const refreshResult = user.serverSessionId
      ? await refreshSessionTokens(user.serverSessionId)
      : null;
    if (refreshResult) {
      accessToken = refreshResult.accessToken;
    } else {
      return jsonError('Not authenticated', HttpStatusConstants.UNAUTHORIZED);
    }
  }

  return accessToken;
}

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const url = new URL(request.url);
  const apiPath = url.searchParams.get('path');
  if (!apiPath) {
    return jsonError("Missing 'path' parameter", HttpStatusConstants.BAD_REQUEST);
  }
  url.searchParams.delete('path');
  const queryString = url.searchParams.toString();

  const baseUrlResult = resolveApiBaseUrl();
  if (baseUrlResult instanceof Response) return baseUrlResult;

  const accessTokenResult = await resolveAccessToken(request);
  if (accessTokenResult instanceof Response) return accessTokenResult;

  const endpoint = `${baseUrlResult.apiBaseUrl}${apiPath}${queryString ? `?${queryString}` : ''}`;

  try {
    const apiResponse = await fetch(endpoint, {
      headers: {
        Authorization: `Bearer ${accessTokenResult}`,
        'Content-Type': 'application/json',
      },
    });

    if (!apiResponse.ok) {
      const errorText = await apiResponse.text();
      return jsonError(
        `Backend error: ${apiResponse.status} ${apiResponse.statusText}`,
        apiResponse.status,
        errorText
      );
    }

    return await apiResponse.json();
  } catch (error) {
    return jsonError(
      'Failed to fetch from backend',
      HttpStatusConstants.SERVICE_UNAVAILABLE,
      String(error)
    );
  }
};

export const action = async ({ request }: ActionFunctionArgs) => {
  const url = new URL(request.url);
  const apiPath = url.searchParams.get('path');
  if (!apiPath) {
    return jsonError("Missing 'path' parameter", HttpStatusConstants.BAD_REQUEST);
  }
  url.searchParams.delete('path');
  const queryString = url.searchParams.toString();

  const baseUrlResult = resolveApiBaseUrl();
  if (baseUrlResult instanceof Response) return baseUrlResult;

  const accessTokenResult = await resolveAccessToken(request);
  if (accessTokenResult instanceof Response) return accessTokenResult;

  const endpoint = `${baseUrlResult.apiBaseUrl}${apiPath}${queryString ? `?${queryString}` : ''}`;

  try {
    const incomingContentType = request.headers.get('Content-Type') ?? '';
    const isMultipart = incomingContentType.includes('multipart/form-data');

    const body: BodyInit | undefined = isMultipart
      ? await request.arrayBuffer()
      : (await request.text()) || undefined;

    const forwardHeaders: HeadersInit = {
      Authorization: `Bearer ${accessTokenResult}`,
      ...(isMultipart
        ? { 'Content-Type': incomingContentType }
        : { 'Content-Type': 'application/json' }),
    };

    const apiResponse = await fetch(endpoint, {
      method: request.method,
      headers: forwardHeaders,
      body,
    });

    if (!apiResponse.ok) {
      const errorText = await apiResponse.text();
      return jsonError(
        `Backend error: ${apiResponse.status} ${apiResponse.statusText}`,
        apiResponse.status,
        errorText
      );
    }

    const contentType = apiResponse.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      return await apiResponse.json();
    }
    return { success: true };
  } catch (error) {
    return jsonError(
      'Failed to fetch from backend',
      HttpStatusConstants.SERVICE_UNAVAILABLE,
      String(error)
    );
  }
};

export const shouldRevalidate = () => false;
