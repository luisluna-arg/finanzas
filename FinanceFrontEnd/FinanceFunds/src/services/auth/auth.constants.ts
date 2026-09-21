const {
  AUTH0_AUDIENCE,
  AUTH0_CLIENT_ID,
  AUTH0_CLIENT_SECRET,
  AUTH0_DOMAIN,
  AUTH0_SCOPES,
  BASE_URL,
  PORT,
} = process.env;

if (!AUTH0_CLIENT_ID) throw new Error('AUTH0_CLIENT_ID is not set');
if (!AUTH0_CLIENT_SECRET) throw new Error('AUTH0_CLIENT_SECRET is not set');
if (!AUTH0_DOMAIN) throw new Error('AUTH0_DOMAIN is not set');
if (!BASE_URL) throw new Error('BASE_URL is not set');

const AuthConstants = {
  PROVIDER: 'auth0',
  AUDIENCE: AUTH0_AUDIENCE ?? '',
  DOMAIN: AUTH0_DOMAIN,
  CLIENT_ID: AUTH0_CLIENT_ID,
  CLIENT_SECRET: AUTH0_CLIENT_SECRET,
  LOGOUT_URL: `https://${AUTH0_DOMAIN}/oidc/logout`,
  REDIRECT_URI: `${BASE_URL}/auth/callback`,
  RETURN_TO_URL: BASE_URL,
  // offline_access is required so Auth0 issues a refresh token, which
  // tokenRefresh.server.ts needs — always included even if AUTH0_SCOPES is set
  // without it.
  SCOPES: (() => {
    const scopes = AUTH0_SCOPES
      ? AUTH0_SCOPES.split(',')
          .map(s => s.trim())
          .filter(Boolean)
      : ['openid', 'email', 'profile'];
    if (!scopes.includes('offline_access')) scopes.push('offline_access');
    return scopes;
  })(),
  PORT: PORT ?? '5200',
};

const HttpStatusConstants = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500,
  BAD_GATEWAY: 502,
  SERVICE_UNAVAILABLE: 503,
};

export { AuthConstants, HttpStatusConstants };
