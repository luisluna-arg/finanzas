import { AuthConstants } from "@/services/auth/auth.constants";
import { OAuth2Tokens } from "arctic";
import { Authenticator } from "remix-auth";
import { Auth0Strategy } from "remix-auth-auth0";
import { jwtDecode } from "jwt-decode";

interface Auth0IdTokenPayload {
    sub: string;
    name: string;
    email: string;
    picture: string;
}

export let authenticator = new Authenticator<any>();

async function getUser(tokens: OAuth2Tokens, request: Request) {
    const idToken = tokens.idToken();
    if (!idToken) {
        throw new Error("No idToken found in tokens");
    }
    const decoded = jwtDecode<Auth0IdTokenPayload>(idToken);
    return {
        id: decoded.sub,
        name: decoded.name,
        email: decoded.email,
        picture: decoded.picture,
    };
}

console.log(AuthConstants)

authenticator.use(
    new Auth0Strategy(
        {
            domain: AuthConstants.DOMAIN,
            clientId: AuthConstants.CLIENT_ID,
            clientSecret: AuthConstants.CLIENT_SECRET,
            redirectURI: AuthConstants.REDIRECT_URI,
            scopes: AuthConstants.SCOPES,
            audience: AuthConstants.AUDIENCE,
        },
        async ({ tokens, request }) => {
            let user = await getUser(tokens, request);
            // console.log("USER INFO", user);
            return {
                ...user,
                accessToken: tokens.accessToken(),
                refreshToken: tokens.hasRefreshToken()
                    ? tokens.refreshToken()
                    : null,
            };
        }
    ),
    AuthConstants.PROVIDER
);

// // app/utils/auth.server.ts
// import { AuthConstants } from "@/services/auth/auth.constants";
// // import { User } from "@/services/auth/types/User";

// import { Authenticator } from "remix-auth";
// import { Auth0Strategy } from "remix-auth-auth0";
// import { createCookieSessionStorage } from "@remix-run/node";
// import { jwtDecode } from "jwt-decode";

// // npm install jwt-decode

// export interface User {
//   id: string;
//   name: string;
//   email: string;
//   picture: string;
// }

// interface Auth0IdTokenPayload {
//   sub: string;
//   name: string;
//   email: string;
//   picture: string;
// }

// export const sessionStorage = createCookieSessionStorage({
//   cookie: {
//     name: "_session",
//     sameSite: "lax",
//     path: "/",
//     httpOnly: true,
//     secrets: [process.env.SESSION_SECRET!],
//     secure: process.env.NODE_ENV === "production",
//   },
// });

// export const authenticator = new Authenticator<User>();

// const auth0Strategy = new Auth0Strategy(
//   {
//     redirectURI: `${process.env.APP_URL}/auth/auth0/callback`,
//     domain: process.env.AUTH0_DOMAIN!,
//     clientId: process.env.AUTH0_CLIENT_ID!,
//     clientSecret: process.env.AUTH0_CLIENT_SECRET!,
//     scopes: ["openid", "profile", "email"],
//   },
//   async ({ tokens }) => {
//     // ❗ KEY CHANGE: Pass the expected payload type as a generic to jwtDecode.
//     // This correctly types the `decoded` constant.
//     const decoded = jwtDecode<Auth0IdTokenPayload>(tokens.idToken());

//     const user: User = {
//       id: decoded.sub,
//       name: decoded.name,
//       email: decoded.email,
//       picture: decoded.picture,
//     };

//     return user;
//   }
// );

// authenticator.use(auth0Strategy);

// export const { getSession, commitSession, destroySession } = sessionStorage;
