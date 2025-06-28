import type { ReactNode } from "react";
import { MantineProvider, createTheme } from "@mantine/core";
import { Notifications } from "@mantine/notifications";
import { useTheme } from "./useTheme";

interface MantineThemeProviderProps {
  children: ReactNode;
}

export function MantineThemeProvider({ children }: MantineThemeProviderProps) {
  const { colorScheme } = useTheme();
  // Create Mantine theme
  const theme = createTheme({
    fontFamily: "Inter, system-ui, Avenir, Helvetica, Arial, sans-serif",
    primaryColor: "blue",
    colors: {
      // Add some custom colors if needed
      blue: [
        "#e6f7ff",
        "#bae7ff",
        "#91d5ff",
        "#69c0ff",
        "#40a9ff",
        "#1890ff",
        "#096dd9",
        "#0050b3",
        "#003a8c",
        "#002766",
      ],
    },
    components: {
      Button: {
        defaultProps: {
          radius: "md",
        },
      },
    },
    defaultRadius: "sm",
    breakpoints: {
      xs: "576px",
      sm: "768px",
      md: "992px",
      lg: "1200px",
      xl: "1400px",
    },
  });

  return (
    <MantineProvider
      theme={theme}
      defaultColorScheme={colorScheme}
      forceColorScheme={colorScheme}
    >
      <Notifications position="top-right" />
      {children}
    </MantineProvider>
  );
}
