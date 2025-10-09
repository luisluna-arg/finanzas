import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth';
import { LoginButton, LogoutButton } from './index';
import { ThemeToggle } from './ThemeToggle';
import {
  Group,
  Title,
  Container,
  Flex,
  Box,
  Anchor,
  useMantineTheme,
  useMantineColorScheme,
  Burger,
  Drawer,
  Stack,
} from '@mantine/core';

export const Navigation = () => {
  const { isAuthenticated, isLoading } = useAuth();
  const theme = useMantineTheme();
  const { colorScheme } = useMantineColorScheme();
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [menuOpened, setMenuOpened] = useState(false);

  // Effect for media query
  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
    };

    window.addEventListener('resize', handleResize);
    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  const NavLinks = () => (
    <Group className="nav-links" gap="lg">
      <Anchor component={Link} to="/funds" fw={500}>
        Funds
      </Anchor>
      <Anchor component={Link} to="/exchange-rates" fw={500}>
        Exchange Rates
      </Anchor>
    </Group>
  );

  const AuthButtons = () => (
    <Group className="nav-auth" gap="md">
      <ThemeToggle />
      {!isLoading && (isAuthenticated ? <LogoutButton /> : <LoginButton />)}
    </Group>
  );

  // Mobile menu drawer
  const MobileMenu = () => (
    <Drawer
      opened={menuOpened}
      onClose={() => setMenuOpened(false)}
      size="xs"
      position="right"
      title="Menu"
      styles={{
        header: {
          color: theme.colors[theme.primaryColor][colorScheme === 'dark' ? 4 : 6],
          fontWeight: 600,
        },
        body: {
          backgroundColor: colorScheme === 'dark' ? theme.colors.dark[7] : theme.white,
        },
      }}
    >
      <Stack gap="xl" p="md">
        <NavLinks />
        <AuthButtons />
      </Stack>
    </Drawer>
  );

  return (
    <Box
      component="nav"
      className="navigation"
      py="md"
      px="xl"
      style={{
        backgroundColor: colorScheme === 'dark' ? theme.colors.dark[7] : theme.white,
        borderBottom: `1px solid ${colorScheme === 'dark' ? theme.colors.dark[5] : theme.colors.gray[2]}`,
      }}
    >
      <Container size="xl" className="app-container">
        <Flex justify="space-between" align="center" style={{ width: '100%' }}>
          <Box className="nav-brand">
            <Title order={3} c={theme.primaryColor}>
              FinanceFunds
            </Title>
          </Box>

          {/* Desktop navigation */}
          {!isMobile && (
            <Flex justify="space-between" align="center" gap="xl">
              <NavLinks />
              <AuthButtons />
            </Flex>
          )}

          {/* Mobile burger menu */}
          {isMobile && (
            <Burger
              opened={menuOpened}
              onClick={() => setMenuOpened(!menuOpened)}
              size="sm"
              color={theme.colors.gray[6]}
            />
          )}
        </Flex>
      </Container>

      {/* Mobile menu drawer */}
      {isMobile && <MobileMenu />}
    </Box>
  );
};
