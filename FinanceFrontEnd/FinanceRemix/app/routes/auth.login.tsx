import type { ActionFunctionArgs, LoaderFunctionArgs } from "react-router";
import { authenticator } from "@/services/auth/auth.server";
import { Form } from "@remix-run/react";
import { AuthConstants } from "@/services/auth/auth.constants";

export async function action({ request }: ActionFunctionArgs) {
    return authenticator.authenticate(AuthConstants.PROVIDER, request);
}

export async function loader({ request }: LoaderFunctionArgs) {
    console.log("🚀 Login loader called");

    // In remix-auth, redirects are "thrown" as Response objects
    // This is normal behavior, not an error
    return authenticator.authenticate(AuthConstants.PROVIDER, request);
}

export default function Login() {
    return (
        <Form action="/auth/auth0" method="post">
            <button>Login with Auth0</button>
        </Form>
    );
}
