import { useEffect } from 'react';
import type { ReactNode } from 'react';
import { useAuth } from './useAuth';

interface ProtectedRouteProps {
  children: ReactNode;
}

export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isLoading, isAuthenticated, login } = useAuth();

  // Use useEffect to handle redirection to avoid issues with React's rendering lifecycle
  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      login({
        appState: { returnTo: window.location.pathname },
      });
    }
  }, [isLoading, isAuthenticated, login]);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  // Don't render anything while redirecting to login
  if (!isAuthenticated) {
    return <div>Redirecting to login...</div>;
  }

  return <>{children}</>;
};
