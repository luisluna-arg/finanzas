import type { LoaderFunctionArgs } from 'react-router';
import { redirect } from 'react-router';
import { Button, Center, Paper, Stack, Text, Title } from '@mantine/core';
import { getUserFromSession } from '@/services/auth/session.server';

export async function loader({ request }: LoaderFunctionArgs) {
  const user = await getUserFromSession(request);
  if (user) {
    return redirect('/funds');
  }
  return {};
}

export const meta = () => [{ title: 'Log In — FinanceFunds' }];

export default function Login() {
  return (
    <Center style={{ minHeight: '100vh' }}>
      <Paper withBorder shadow="sm" p="xl" radius="md" w={360}>
        <Stack gap="md" align="center">
          <Title order={2}>FinanceFunds</Title>
          <Text c="dimmed" size="sm">
            Sign in to your account
          </Text>
          <form action="/auth/auth0" method="post" style={{ width: '100%' }}>
            <Button type="submit" fullWidth>
              Log In
            </Button>
          </form>
        </Stack>
      </Paper>
    </Center>
  );
}
