import type { ReactNode } from "react";
import { useState, useEffect } from "react";
import { Auth0Provider } from "@auth0/auth0-react";
import { auth0Config } from "./auth0-config";

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [isConfigValid, setIsConfigValid] = useState<boolean>(false);
  const [configError, setConfigError] = useState<string>("");

  useEffect(() => {
    // Validate Auth0 configuration
    if (!auth0Config.domain) {
      setConfigError("Missing Auth0 domain. Please check your .env file.");
      return;
    }

    if (!auth0Config.clientId) {
      setConfigError("Missing Auth0 client ID. Please check your .env file.");
      return;
    }

    setIsConfigValid(true);
  }, []);

  if (!isConfigValid) {
    return (
      <div className="auth-error">
        <h2>Auth0 Configuration Error</h2>
        <p>
          {configError ||
            "Invalid Auth0 configuration. Please check your .env file."}
        </p>
        <p>
          Please refer to the AUTH0-SETUP.md and ENV-SETUP.md files for setup
          instructions.
        </p>
      </div>
    );
  }
  interface AppState {
    returnTo?: string;
    [key: string]: unknown;
  }
  
  const onRedirectCallback = (appState: AppState | undefined) => {
    // Handle redirect after login
    window.history.replaceState(
      {},
      document.title,
      appState?.returnTo || window.location.pathname,
    );
  };
  return (
    <Auth0Provider
      domain={auth0Config.domain}
      clientId={auth0Config.clientId}
      authorizationParams={auth0Config.authorizationParams}
      onRedirectCallback={onRedirectCallback}
    >
      {children}
    </Auth0Provider>
  );
};
