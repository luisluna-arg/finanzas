import type { LoaderFunctionArgs } from '@remix-run/node';
import { getUserAndTokens } from '@/services/auth/session.server';
import { HttpStatusConstants } from '@/services/auth/auth.constants';
import { JsonErrorResponse } from '@/utils/JsonResponse';

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const url = new URL(request.url);
  const apiPath = url.searchParams.get('path');
  if (!apiPath) {
    throw JsonErrorResponse("Missing 'path' parameter", HttpStatusConstants.BAD_REQUEST);
  }

  // Remove 'path' from search params for backend API
  url.searchParams.delete('path');
  const queryString = url.searchParams.toString();

  // Build the external API URL
  const apiBaseUrl = process.env.API_URL;

  // In production/staging require an explicit API URL and enforce HTTPS
  const allowInsecure = process.env.ALLOW_INSECURE_API === 'true';

  if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
    if (!apiBaseUrl) {
      return JsonErrorResponse(
        'Server misconfiguration: missing API_URL. Contact administrators.',
        HttpStatusConstants.SERVICE_UNAVAILABLE
      );
    }
    try {
      const parsed = new URL(apiBaseUrl);
      if (parsed.protocol !== 'https:' && !allowInsecure) {
        return JsonErrorResponse(
          'Insecure backend configuration: API base URL must use HTTPS in production unless ALLOW_INSECURE_API is enabled.',
          HttpStatusConstants.SERVICE_UNAVAILABLE
        );
      }
    } catch (err) {
      return JsonErrorResponse(
        `Invalid API_URL: ${String(err)}`,
        HttpStatusConstants.SERVICE_UNAVAILABLE
      );
    }
  }

  const endpoint = `${apiBaseUrl}${apiPath}${queryString ? `?${queryString}` : ''}`;

  // Get access token from server-side session (Redis)
  const result = await getUserAndTokens(request);
  if (!result) {
    throw JsonErrorResponse('Not authenticated', HttpStatusConstants.UNAUTHORIZED);
  }

  const { tokens } = result;
  const accessToken = tokens.accessToken;

  if (!accessToken) {
    throw JsonErrorResponse('Not authenticated', HttpStatusConstants.UNAUTHORIZED);
  }

  try {
    // Fetch from external API
    const apiResponse = await fetch(endpoint, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
        'Content-Type': 'application/json',
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
      'Failed to fetch from backend',
      HttpStatusConstants.SERVICE_UNAVAILABLE,
      String(error)
    );
  }
};

export const shouldRevalidate = () => false;
