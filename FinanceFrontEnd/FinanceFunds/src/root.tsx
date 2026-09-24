import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  isRouteErrorResponse,
  useLoaderData,
  useLocation,
  useRouteError,
} from 'react-router';
import type { LoaderFunctionArgs } from 'react-router';
import { AppShell, Container, Box, Center, Paper, Stack, Title, Text, Button } from '@mantine/core';
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
import { ApiRequestError } from '@/services/ApiClient';
import SafeLogger from '@/utils/SafeLogger';

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

export function ErrorBoundary() {
  const error = useRouteError();

  SafeLogger.error('Global error boundary caught:', error);

  const isUnauthorized =
    (isRouteErrorResponse(error) && (error.status === 401 || error.status === 403)) ||
    (error instanceof ApiRequestError && (error.status === 401 || error.status === 403));

  const title = isUnauthorized ? 'Not authorized' : 'Something went wrong';
  const message = isUnauthorized
    ? "You don't have permission to view this page."
    : 'An unexpected error occurred. Please try again later.';

  return (
    <Center style={{ minHeight: '100vh' }}>
      <Paper withBorder shadow="sm" p="xl" radius="md" w={420}>
        <Stack gap="md" align="center">
          <Title order={2}>{title}</Title>
          <Text c="dimmed" size="sm" ta="center">
            {message}
          </Text>
          <Button component="a" href="/">
            Go back home
          </Button>
        </Stack>
      </Paper>
    </Center>
  );
}
