import { useAuth0 } from '@auth0/auth0-react';

export const useAuth = () => {
  const {
    isLoading,
    isAuthenticated,
    error,
    user,
    loginWithRedirect,
    logout,
    getAccessTokenSilently,
  } = useAuth0();

  return {
    isLoading,
    isAuthenticated,
    error,
    user,
    login: loginWithRedirect,
    logout: () =>
      logout({
        logoutParams: {
          returnTo: window.location.origin,
        },
      }),
    getAccessToken: getAccessTokenSilently,
  };
};
