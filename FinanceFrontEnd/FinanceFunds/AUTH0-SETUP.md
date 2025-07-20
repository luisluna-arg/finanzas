# Auth0 Integration Setup Guide for FinanceFunds

This guide will help you configure Auth0 for your FinanceFunds application.

## Step 1: Create an Auth0 Account

If you don't already have an Auth0 account, sign up for one at [https://auth0.com/](https://auth0.com/).

## Step 2: Create a New Application in Auth0

1. Once logged in, navigate to the Auth0 Dashboard
2. Click on "Applications" in the left sidebar
3. Click "Create Application"
4. Give your application a name (e.g., "FinanceFunds")
5. Select "Single Page Application" as the application type
6. Click "Create"

## Step 3: Configure the Application

1. In your application settings, set the following URLs:
   - Allowed Callback URLs: `http://localhost:5200, http://localhost:5200/`
   - Allowed Logout URLs: `http://localhost:5200, http://localhost:5200/`
   - Allowed Web Origins: `http://localhost:5200`

2. Save your changes

## Step 4: Configure Environment Variables

1. Copy the `.env.example` file to a new file named `.env`
2. Update the `.env` file with your Auth0 credentials:

```
VITE_AUTH0_DOMAIN=your-auth0-domain.auth0.com
VITE_AUTH0_CLIENT_ID=your-auth0-client-id
VITE_AUTH0_REDIRECT_URI=http://localhost:5200
# VITE_AUTH0_AUDIENCE=your-api-identifier  # Uncomment and add only if you're calling an API
```

The `auth0-config.ts` file is already set up to use these environment variables.
```

## Step 5: Run Your Application

With this configuration in place, you can now run your application and test the authentication flow.

```bash
npm run dev
```

## Optional: Add API Configuration

If you need to call a protected API:

1. In Auth0 Dashboard, go to "APIs" and create a new API
2. Get your API identifier (audience)
3. Uncomment and update the audience in your auth0-config.ts file

## Using Auth0 in Your Application

### Protected Routes

Use the `ProtectedRoute` component to ensure that users must be authenticated to access certain routes:

```tsx
import { ProtectedRoute } from './auth';

function App() {
  return (
    // ...
    <ProtectedRoute>
      <Dashboard />
    </ProtectedRoute>
    // ...
  );
}
```

### User Information

Access user information with the `useAuth` hook:

```tsx
import { useAuth } from './auth';

function Profile() {
  const { user, isAuthenticated } = useAuth();
  
  if (!isAuthenticated) return null;
  
  return <div>Hello, {user.name}!</div>;
}
```

### API Calls

For API calls that require authentication:

```tsx
import { useAuth } from './auth';

function ApiComponent() {
  const { getAccessToken } = useAuth();
  
  const callApi = async () => {
    try {
      const token = await getAccessToken();
      const response = await fetch('https://your-api.com/endpoint', {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      const data = await response.json();
      // Handle the data
    } catch (error) {
      console.error('API call failed:', error);
    }
  };
  
  return <button onClick={callApi}>Call API</button>;
}
```
