import { LoaderFunction, LoaderFunctionArgs, redirect } from "@remix-run/node";
import urls from "@/utils/urls";
import AnnualDebits from "@/components/ui/Debits/Annual";
import { SessionContants } from "@/services/auth/auth.constants";
import { sessionStorage } from "@/services/auth/session.server";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const cookie = request.headers.get("Cookie");
    const session = await sessionStorage.getSession(cookie);
    const user = session.get(SessionContants.USER_KEY);

    if (!user) return redirect("/auth/login");
    const pesosModuleId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
    const dollarsModuleId = "03cc66c7-921c-4e05-810e-9764cd365c1d";

    const pesosEndpoint = `${urls.debits.annual.paginated}?AppModuleId=${pesosModuleId}`;
    const dollarsEndpoint = `${urls.debits.annual.paginated}?AppModuleId=${dollarsModuleId}`;

    return {
        pesosEndpoint,
        dollarsEndpoint,
        pesosModuleId,
        dollarsModuleId,
    };
};

export default AnnualDebits;
