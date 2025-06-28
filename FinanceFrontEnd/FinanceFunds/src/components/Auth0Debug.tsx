import { useAuth } from "../auth";
import {
  Title,
  Text,
  Paper,
  Stack,
  Group,
  Badge,
  Alert,
  Divider,
  Box,
  Code,
} from "@mantine/core";
// Importing only what's needed

export const Auth0Debug = () => {
  const { isLoading, isAuthenticated, user, error } = useAuth();

  return (
    <Paper withBorder radius="md" p="md" mb="xl">
      <Title order={3} mb="md">
        Auth0 Debug Panel
      </Title>
      <Stack gap="md">
        <Group>
          <Text fw={500}>Environment:</Text>
          <Badge
            color={import.meta.env.MODE === "development" ? "blue" : "green"}
          >
            {import.meta.env.MODE}
          </Badge>
        </Group>

        <Group>
          <Text fw={500}>Auth Status:</Text>
          <Badge color={isLoading ? "gray" : isAuthenticated ? "green" : "red"}>
            {isLoading
              ? "Loading..."
              : isAuthenticated
                ? "Authenticated"
                : "Not authenticated"}
          </Badge>
        </Group>

        {error && (
          <Alert title="Auth Error" color="red">
            <Text>{error.message}</Text>
          </Alert>
        )}

        {isAuthenticated && user && (
          <Box>
            <Divider label="User Information" labelPosition="center" mb="xs" />
            <Stack gap="xs">
              <Group>
                <Text fw={500}>User:</Text>
                <Text>
                  {user.name} ({user.email})
                </Text>
              </Group>
              <Group>
                <Text fw={500}>User ID:</Text>
                <Code>{user.sub}</Code>
              </Group>
            </Stack>
          </Box>
        )}

        <Box>
          <Divider
            label="Environment Variables"
            labelPosition="center"
            mb="xs"
          />
          <Stack gap="xs">
            <Group>
              <Text fw={500}>Auth0 Domain:</Text>
              <Text>{import.meta.env.VITE_AUTH0_DOMAIN || "Not set"}</Text>
            </Group>
            <Group>
              <Text fw={500}>Redirect URI:</Text>
              <Text>
                {import.meta.env.VITE_AUTH0_REDIRECT_URI ||
                  window.location.origin}
              </Text>
            </Group>
          </Stack>
        </Box>
      </Stack>
    </Paper>
  );
};
