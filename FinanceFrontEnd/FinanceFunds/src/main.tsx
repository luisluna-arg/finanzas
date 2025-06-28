import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { AuthProvider } from "./auth";
import { ThemeProvider } from "./context/ThemeContext";
import { MantineThemeProvider } from "./context/MantineThemeProvider";
import "@mantine/core/styles.css";
import "@mantine/dates/styles.css";
import "@mantine/notifications/styles.css";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider>
      <MantineThemeProvider>
        <AuthProvider>
          <App />
        </AuthProvider>
      </MantineThemeProvider>
    </ThemeProvider>
  </StrictMode>,
);
