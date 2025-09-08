import { redirect, ActionFunctionArgs } from "@remix-run/node";
import { authenticator } from "@/services/auth/auth.server";
import { AuthConstants } from "@/services/auth/auth.constants";

export const loader = () => redirect("/login");

export const action = ({ request }: ActionFunctionArgs) => {
    return authenticator.authenticate(AuthConstants.PROVIDER, request);
};
