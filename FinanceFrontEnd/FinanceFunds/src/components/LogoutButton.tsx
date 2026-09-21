import { Button } from '@mantine/core';

export const LogoutButton = () => {
  return (
    <form action="/auth/logout" method="post">
      <Button type="submit" variant="outline" color="red">
        Log Out
      </Button>
    </form>
  );
};
