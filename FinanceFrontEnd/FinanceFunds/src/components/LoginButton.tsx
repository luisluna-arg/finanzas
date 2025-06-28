import { useAuth } from "../auth";
import { Button } from "@mantine/core";

export const LoginButton = () => {
  const { login } = useAuth();

  return (
    <Button onClick={() => login()} variant="filled">
      Log In
    </Button>
  );
};
