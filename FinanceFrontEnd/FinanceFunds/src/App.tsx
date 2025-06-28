import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import "./App.css";
import "./responsive.css"; // Import responsive styles
import { useAuth } from "./auth";
import { Navigation, Auth0Debug } from "./components";
import { ProtectedRoute } from "./auth";
import { Dashboard } from "./pages";
import { AppShell, Container, Loader, Center, Box } from "@mantine/core";

function App() {
  const { isLoading } = useAuth();

  if (isLoading) {
    return (
      <Center style={{ height: "100vh" }}>
        <Loader size="xl" />
      </Center>
    );
  }

  const isDevelopment = import.meta.env.MODE === "development";

  return (
    <Router>
      <AppShell header={{ height: 60 }} padding="0">
        <AppShell.Header>
          <Navigation />
        </AppShell.Header>
        <AppShell.Main>
          <Container
            size="xl"
            py="md"
            px="md"
            mx="auto"
            className="app-container"
          >
            <Box style={{ width: "100%" }}>
              <Routes>
                <Route
                  path="/"
                  element={
                    <ProtectedRoute>
                      <Dashboard />
                    </ProtectedRoute>
                  }
                />
                <Route path="*" element={<Navigate to="/" />} />
              </Routes>

              {/* Auth0Debug Panel at the bottom of the page */}
              {isDevelopment && (
                <Box mt="xl">
                  <Auth0Debug />
                </Box>
              )}
            </Box>
          </Container>
        </AppShell.Main>
      </AppShell>
    </Router>
  );
}

export default App;
