import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  useLoaderData,
  useLocation,
} from 'react-router';
import type { LoaderFunctionArgs } from 'react-router';
import { AppShell, Container, Box } from '@mantine/core';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import '@mantine/notifications/styles.css';
import './index.css';
import './App.css';
import './responsive.css';
import { Navigation } from '@/components';
import { ThemeProvider } from '@/context/ThemeContext';
import { MantineThemeProvider } from '@/context/MantineThemeProvider';
import { getUserFromSession } from '@/services/auth/session.server';

export const loader = async ({ request }: LoaderFunctionArgs) => {
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
      </head>
      <body>
        <ThemeProvider>
          <MantineThemeProvider>{children}</MantineThemeProvider>
        </ThemeProvider>
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

export default function App() {
  const { isAuthenticated } = useLoaderData<typeof loader>();
  const location = useLocation();
  const isAuthRoute = location.pathname.startsWith('/auth');
  const showNavigation = isAuthenticated && !isAuthRoute;

  if (!showNavigation) {
    return <Outlet />;
  }

  return (
    <AppShell header={{ height: 60 }} padding="0">
      <AppShell.Header>
        <Navigation />
      </AppShell.Header>
      <AppShell.Main>
        <Container size="xl" py="md" px="md" mx="auto" className="app-container">
          <Box style={{ width: '100%' }}>
            <Outlet />
          </Box>
        </Container>
      </AppShell.Main>
    </AppShell>
  );
}
