import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  isRouteErrorResponse,
  useRouteError,
  useLocation,
  useLoaderData,
  LinksFunction,
  redirect,
  LoaderFunction,
} from 'react-router';

import { library } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons';
import Navigation from '@/components/ui/Navigation';
import stylesheet from '@/tailwind.css?url';
import app from '@/app.css?url';
import SafeLogger from './utils/SafeLogger';

library.add(fas);

// Conditional redirect based on the current path
export const loader: LoaderFunction = async ({ request }) => {
  const url = new URL(request.url);

  if (url.pathname === '/') {
    // Check if user is authenticated before redirecting to dashboard
    const { getUserFromSession } = await import('@/services/auth/session.server');
    const user = await getUserFromSession(request);

    if (user) {
      return redirect('/dashboard');
    } else {
      return redirect('/auth/login');
    }
  }

  // Check authentication status for showing navigation
  const { getUserFromSession } = await import('@/services/auth/session.server');
  const user = await getUserFromSession(request);

  return { isAuthenticated: !!user };
};

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
        <script
          dangerouslySetInnerHTML={{
            __html: `
                            (function() {
                                const theme = localStorage.getItem('theme');
                                if (theme === 'dark') {
                                    document.documentElement.classList.add('dark');
                                }
                            })();
                        `,
          }}
        />
      </head>
      <body className="bg-background text-foreground">
        {children}
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

export default function App() {
  const location = useLocation();
  const data = useLoaderData<{ isAuthenticated: boolean }>();

  // Don't show navigation on auth routes
  const isAuthRoute = location.pathname.startsWith('/auth');
  const showNavigation = data?.isAuthenticated && !isAuthRoute;

  return (
    <>
      {showNavigation && <Navigation />}
      <Outlet />
    </>
  );
}

export const links: LinksFunction = () => [
  { rel: 'stylesheet', href: stylesheet },
  { rel: 'app', href: app },
];

export function ErrorBoundary() {
  const error = useRouteError();

  SafeLogger.error('Global error boundary caught:', error);

  if (isRouteErrorResponse(error)) {
    return (
      <html lang="en">
        <head>
          <meta charSet="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>
            {error.status} {error.statusText}
          </title>
          <Meta />
          <Links />
        </head>
        <body>
          <div style={{ padding: '2rem', fontFamily: 'system-ui' }}>
            <h1 style={{ color: '#dc2626' }}>
              {error.status} {error.statusText}
            </h1>
            <p>{error.data}</p>
            <a href="/" style={{ color: '#2563eb', textDecoration: 'underline' }}>
              Go back home
            </a>
          </div>
          <Scripts />
        </body>
      </html>
    );
  }

  const errorMessage = error instanceof Error ? error.message : 'Unknown error';
  const errorStack = error instanceof Error ? error.stack : undefined;

  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <title>Application Error</title>
        <Meta />
        <Links />
      </head>
      <body>
        <div style={{ padding: '2rem', fontFamily: 'system-ui' }}>
          <h1 style={{ color: '#dc2626' }}>Application Error</h1>
          <p style={{ color: '#991b1b', fontWeight: 'bold' }}>{errorMessage}</p>
          {errorStack && (
            <pre
              style={{
                background: '#f3f4f6',
                padding: '1rem',
                overflow: 'auto',
                fontSize: '0.875rem',
              }}
            >
              {errorStack}
            </pre>
          )}
          <a href="/" style={{ color: '#2563eb', textDecoration: 'underline' }}>
            Go back home
          </a>
        </div>
        <Scripts />
      </body>
    </html>
  );
}
