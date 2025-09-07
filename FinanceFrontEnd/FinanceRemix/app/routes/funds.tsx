import { getBackendClient } from "@/data/getBackendClient";
import Funds from "@/components/ui/Funds";
import { LoaderFunctionArgs, redirect } from "@remix-run/node";
import { SessionContants } from "@/services/auth/auth.constants";
import { sessionStorage } from "@/services/auth/session.server";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const cookie = request.headers.get("Cookie");
    const session = await sessionStorage.getSession(cookie);
    const user = session.get(SessionContants.USER_KEY);

    if (!user) return redirect("/auth/login");

    let client = await getBackendClient(user.accessToken);

    let banks = await client.GetBanksQuery().get();

    let currencies = await client.GetCurrenciesQuery().get();

    return {
        banks,
        currencies,
    };
};

export const meta = () => {
    return [
        {
            title: "Fondos",
            description: "",
        },
    ];
};

export default Funds;
