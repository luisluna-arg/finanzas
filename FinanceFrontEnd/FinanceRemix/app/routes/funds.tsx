import { getBackendClient } from "@/data/getBackendClient";
import Funds from "@/components/ui/Funds";
import { LoaderFunctionArgs } from "@remix-run/node";
import { requireAuth } from "@/services/auth/session.server";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const user = await requireAuth(request);

    if (!user.accessToken) {
        throw new Error("No access token available");
    }

    const client = await getBackendClient(user.accessToken!);

    const banks = await client.GetBanksQuery().get();

    const currencies = await client.GetCurrenciesQuery().get();

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
