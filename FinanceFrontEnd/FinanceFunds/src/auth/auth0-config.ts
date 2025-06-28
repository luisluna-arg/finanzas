// Auth0 configuration

// Check if required environment variables are set
const requireEnvVar = (name: string): string => {
  const value = import.meta.env[name];
  if (!value) {
    console.warn(`Missing environment variable: ${name}`);
    return "";
  }
  return value;
};

export const auth0Config = {
  domain: requireEnvVar("VITE_AUTH0_DOMAIN"),
  clientId: requireEnvVar("VITE_AUTH0_CLIENT_ID"),
  authorizationParams: {
    redirect_uri:
      import.meta.env.VITE_AUTH0_REDIRECT_URI || window.location.origin,
    // Only add audience if it's defined in environment variables
    ...(import.meta.env.VITE_AUTH0_AUDIENCE
      ? { audience: import.meta.env.VITE_AUTH0_AUDIENCE }
      : {}),
  },
};
