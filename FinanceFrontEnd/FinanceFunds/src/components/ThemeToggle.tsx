import { ActionIcon, useMantineColorScheme } from '@mantine/core';
import { useTheme } from '../context/useTheme';
import { IconSun, IconMoon } from '@tabler/icons-react';

export function ThemeToggle() {
  const { colorScheme, toggleColorScheme } = useTheme();
  const mantineColorScheme = useMantineColorScheme();

  // Handle theme toggle
  const handleToggle = () => {
    const newColorScheme = colorScheme === 'dark' ? 'light' : 'dark';
    toggleColorScheme();
    mantineColorScheme.setColorScheme(newColorScheme);
  };

  return (
    <ActionIcon
      variant="outline"
      color={colorScheme === 'dark' ? 'yellow' : 'blue'}
      onClick={handleToggle}
      size="md"
      radius="md"
      title={`Toggle ${colorScheme === 'dark' ? 'light' : 'dark'} mode`}
    >
      {colorScheme === 'dark' ? <IconSun size="1.1rem" /> : <IconMoon size="1.1rem" />}
    </ActionIcon>
  );
}
