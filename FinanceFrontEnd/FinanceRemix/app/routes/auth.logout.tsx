// app/routes/auth/logout.ts
import type { LoaderFunctionArgs } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { destroySession, getSession } from "@/services/auth/session.server";
import { AuthConstants, SessionContants } from "@/services/auth/auth.constants";

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const session = await getSession(request.headers.get(SessionContants.SET_COOKIE_HEADER));
  const logoutURL = new URL(AuthConstants.LOGOUT_URL);

  logoutURL.searchParams.set("client_id", AuthConstants.CLIENT_ID);
  logoutURL.searchParams.set("post_logout_redirect_uri", AuthConstants.RETURN_TO_URL);

  return redirect(logoutURL.toString(), {
    headers: {
      [SessionContants.SET_COOKIE_HEADER]: await destroySession(session),
    },
  });
};