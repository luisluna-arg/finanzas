import { useAuth } from '../auth';
import { Button } from '@mantine/core';

export const LogoutButton = () => {
  const { logout } = useAuth();

  return (
    <Button onClick={() => logout()} variant="outline" color="red">
      Log Out
    </Button>
  );
};
