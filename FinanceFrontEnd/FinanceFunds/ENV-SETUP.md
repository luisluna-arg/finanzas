# Environment Variables for Auth0

This project uses environment variables to configure Auth0 authentication. This approach keeps sensitive information out of your codebase and makes it easier to configure different environments.

## Available Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `VITE_AUTH0_DOMAIN` | Your Auth0 domain (e.g., `dev-abc123.us.auth0.com`) | Yes |
| `VITE_AUTH0_CLIENT_ID` | Your Auth0 client ID | Yes |
| `VITE_AUTH0_REDIRECT_URI` | The URI to redirect to after authentication (defaults to window.location.origin) | No |
| `VITE_AUTH0_AUDIENCE` | The API identifier if you're calling an API | No |

## Setting Up Environment Variables

1. Copy the `.env.example` file to create a new `.env` file:
   ```
   cp .env.example .env
   ```

2. Edit the `.env` file with your Auth0 credentials:
   ```
   VITE_AUTH0_DOMAIN=your-auth0-domain.auth0.com
   VITE_AUTH0_CLIENT_ID=your-auth0-client-id
   VITE_AUTH0_REDIRECT_URI=http://localhost:5200
   # VITE_AUTH0_AUDIENCE=your-api-identifier  # Uncomment if needed
   ```

3. Save the file and restart the development server if it's already running.

## Security Notes

- The `.env` file is listed in `.gitignore` to prevent committing sensitive credentials to your repository.
- Environment variables in Vite that start with `VITE_` are exposed to your client-side code.
- For production, set these environment variables on your hosting platform rather than relying on the `.env` file.

## Different Environments

For different environments (development, staging, production), you can create environment-specific files:

- `.env.development` - Used during development
- `.env.production` - Used during production build

Vite will automatically load the appropriate file based on the environment.

### Development Environment

When running in development mode (`npm run dev`), Vite loads variables from:
1. `.env`
2. `.env.development`
3. `.env.local`
4. `.env.development.local`

### Production Environment

When building for production (`npm run build`), Vite loads variables from:
1. `.env`
2. `.env.production`
3. `.env.local`
4. `.env.production.local`

## Debug Tools

In development mode, an Auth0Debug panel is available in the bottom-right corner of the screen. This panel shows:

- Current authentication status
- User information (when authenticated)
- Environment configuration
- Any authentication errors

This debug panel is automatically hidden in production builds.
