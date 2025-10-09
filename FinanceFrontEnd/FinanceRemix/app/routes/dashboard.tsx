import { getBackendClient } from "@/data/getBackendClient";
import { LoaderFunctionArgs, redirect } from "@remix-run/node";
import { requireAuth } from "@/services/auth/session.server";
import Dashboard from "../components/ui/Dashboard";
import serverLogger from "@/utils/logger.server";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    try {
        const user = await requireAuth(request);

        // Fetch creditCards and latestCurrencyExchangeRates from your backend API
        // Replace these with your actual API calls as needed
        let creditCards = [];
        let latestCurrencyExchangeRates = null;
        try {
            if (!user.accessToken) {
                throw new Error("No access token available");
            }
            const backendClient = await getBackendClient(user.accessToken!);
            creditCards = await backendClient.GetCreditCardsQuery().get();
            latestCurrencyExchangeRates = await backendClient
                .GetCurrencyExchangeRatesQuery()
                .get();
        } catch (e) {
            serverLogger.error(
                "[dashboard loader] Error fetching dashboard data",
                e
            );
        }

        return { creditCards, latestCurrencyExchangeRates };
    } catch (authError) {
        serverLogger.error("[dashboard] Authentication failed:", authError);
        // If authentication fails, redirect to login
        return redirect("/auth/login");
    }
};

export const meta = () => {
    return [
        {
            title: "Dashboard",
            description: "Home of the Finance app",
        },
    ];
};

export default Dashboard;
