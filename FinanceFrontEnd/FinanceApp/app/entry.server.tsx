/**
 * By default, Remix will handle generating the HTTP Response for you.
 * You are free to delete this file if you'd like to, but if you ever want it revealed again, you can run `npx remix reveal` ✨
 * For more information, see https://remix.run/file-conventions/entry.server
 */

// Initialize server-side OpenTelemetry before any other imports
import '@/telemetry/tracing.server';

import { PassThrough } from 'node:stream';

import type { AppLoadContext, EntryContext } from 'react-router';
import { createReadableStreamFromReadable } from '@react-router/node';
import { ServerRouter } from 'react-router';
import { isbot } from 'isbot';
import { renderToPipeableStream } from 'react-dom/server';
import { HttpStatusConstants } from '@/services/auth/auth.constants';
import serverLogger from '@/utils/logger.server';
import { logger } from '@/middleware/logger.server';
import SafeLogger from './utils/SafeLogger';

const ABORT_DELAY = 5_000;

export default function handleRequest(
  request: Request,
  responseStatusCode: number,
  responseHeaders: Headers,
  remixContext: EntryContext,
  // This is ignored so we can keep it in the template for visibility.  Feel
  // free to delete this parameter in your app if you're not using it!
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  loadContext: AppLoadContext
) {
  // Log every request with cookies and response headers
  const url = new URL(request.url);
  const cookies = request.headers.get('cookie');
  const setCookie = responseHeaders.get('set-cookie');
  const userAgent = request.headers.get('user-agent');
  const referer = request.headers.get('referer');

  logger.info(
    {
      method: request.method,
      path: url.pathname,
      status: responseStatusCode,
      hasCookies: !!cookies,
      cookiePreview: cookies?.substring(0, 50),
      hasSetCookie: !!setCookie,
      setCookiePreview: setCookie?.substring(0, 100),
      userAgent: userAgent?.substring(0, 80),
      referer,
    },
    `${request.method} ${url.pathname} → ${responseStatusCode}`
  );

  return isbot(request.headers.get('user-agent') || '')
    ? handleBotRequest(request, responseStatusCode, responseHeaders, remixContext)
    : handleBrowserRequest(request, responseStatusCode, responseHeaders, remixContext);
}

function handleBotRequest(
  request: Request,
  responseStatusCode: number,
  responseHeaders: Headers,
  remixContext: EntryContext
) {
  return new Promise((resolve, reject) => {
    let shellRendered = false;
    const { pipe, abort } = renderToPipeableStream(
      <ServerRouter context={remixContext} url={request.url} />,
      {
        onAllReady() {
          shellRendered = true;
          const body = new PassThrough();
          const stream = createReadableStreamFromReadable(body);

          responseHeaders.set('Content-Type', 'text/html');

          resolve(
            new Response(stream, {
              headers: responseHeaders,
              status: responseStatusCode,
            })
          );

          pipe(body);
        },
        onShellError(error: unknown) {
          SafeLogger.error('[Bot Request] Shell Error:', error);
          resolve(
            new Response('<!DOCTYPE html><html><body><h1>Server Error</h1></body></html>', {
              status: 500,
              headers: { 'Content-Type': 'text/html' },
            })
          );
        },
        onError(error: unknown) {
          SafeLogger.error('[Bot Request] Render Error:', error);
          responseStatusCode = HttpStatusConstants.INTERNAL_SERVER_ERROR;
          if (shellRendered) {
            serverLogger.error(error);
          }
        },
      }
    );

    setTimeout(abort, ABORT_DELAY);
  });
}

function handleBrowserRequest(
  request: Request,
  responseStatusCode: number,
  responseHeaders: Headers,
  remixContext: EntryContext
) {
  return new Promise((resolve) => {
    let shellRendered = false;
    const { pipe, abort } = renderToPipeableStream(
      <ServerRouter context={remixContext} url={request.url} />,
      {
        onShellReady() {
          shellRendered = true;
          const body = new PassThrough();
          const stream = createReadableStreamFromReadable(body);

          responseHeaders.set('Content-Type', 'text/html');

          resolve(
            new Response(stream, {
              headers: responseHeaders,
              status: responseStatusCode,
            })
          );

          pipe(body);
        },
        onShellError(error: unknown) {
          SafeLogger.error('[Browser Request] Shell Error:', error);
          resolve(
            new Response('<!DOCTYPE html><html><body><h1>Server Error</h1></body></html>', {
              status: 500,
              headers: { 'Content-Type': 'text/html' },
            })
          );
        },
        onError(error: unknown) {
          responseStatusCode = HttpStatusConstants.INTERNAL_SERVER_ERROR;
          // Log streaming rendering errors from inside the shell.  Don't log
          // errors encountered during initial shell rendering since they'll
          // reject and get logged in handleDocumentRequest.
          if (shellRendered) {
            serverLogger.error(error);
          }
        },
      }
    );

    setTimeout(abort, ABORT_DELAY);
  });
}
