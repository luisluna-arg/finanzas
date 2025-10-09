// app/routes/auth/logout.ts
import type { ActionFunctionArgs, LoaderFunctionArgs } from "@remix-run/node";
import { destroyUserSession } from "@/services/auth/session.server";

export async function action({ request }: ActionFunctionArgs) {
    return destroyUserSession(request);
}

export async function loader({ request }: LoaderFunctionArgs) {
    return destroyUserSession(request);
}
