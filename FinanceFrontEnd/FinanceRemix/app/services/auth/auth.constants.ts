const {
    AUTH0_AUDIENCE,
    AUTH0_CLIENT_ID,
    AUTH0_CLIENT_SECRET,
    AUTH0_DOMAIN,
    AUTH0_SCOPES,
    BASE_URL,
    PORT,
} = process.env;

if (!AUTH0_AUDIENCE) throw new Error("AUTH0_AUDIENCE is not set");
if (!AUTH0_CLIENT_ID) throw new Error("AUTH0_AUDIENCE is not set");
if (!AUTH0_CLIENT_SECRET) throw new Error("AUTH0_AUDIENCE is not set");
if (!AUTH0_DOMAIN) throw new Error("AUTH0_DOMAIN is not set");
if (!BASE_URL) throw new Error("BASE_URL is not set");
if (!PORT) throw new Error("PORT is not set");

const AuthConstants = {
    PROVIDER: "auth0",
    AUDIENCE: `${AUTH0_AUDIENCE}`,
    DOMAIN: `${AUTH0_DOMAIN}`,
    CLIENT_ID: `${AUTH0_CLIENT_ID}`,
    CLIENT_SECRET: `${AUTH0_CLIENT_SECRET}`,
    LOGOUT_URL: `https://${AUTH0_DOMAIN}/oidc/logout`,
    REDIRECT_URI:
        `${BASE_URL}/auth/callback` || `http://localhost:${PORT}/auth/callback`,
    RETURN_TO_URL: BASE_URL || `http://localhost:${PORT}`,
    SCOPES: AUTH0_SCOPES?.split(",") ?? ["openid", "email", "profile"],
};

const SessionContants = {
    COOKIE_HEADER: "Cookie",
    SET_COOKIE_HEADER: "Set-Cookie",
    USER_KEY: "User",
    ID_TOKEN: "idToken",
    ACCESS_TOKEN: "accessToken"
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
    SERVICE_UNAVAILABLE: 503
};

export { AuthConstants, SessionContants, HttpStatusConstants };
