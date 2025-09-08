import { getBackendClient } from "@/data/getBackendClient";
import { LoaderFunctionArgs, redirect } from "@remix-run/node";
import { SessionContants } from "@/services/auth/auth.constants";
import { sessionStorage } from "@/services/auth/session.server";
import Dashboard from "../components/ui/Dashboard";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    // Get the session cookie from the request
    const cookie = request.headers.get("Cookie");
    const session = await sessionStorage.getSession(cookie);
    const user = session.get(SessionContants.USER_KEY);

    if (!user) {
        // Only log in non-production to avoid exposing session details in logs
        // Use server logger utility
        const { default: serverLogger } = await import("@/utils/logger.server");
        serverLogger.info("[dashboard loader] No user in session, redirecting to login");
        return redirect("/auth/login");
    }

    // Fetch creditCards and latestCurrencyExchangeRates from your backend API
    // Replace these with your actual API calls as needed
    let creditCards = [];
    let latestCurrencyExchangeRates = null;
    try {
        const backendClient = await getBackendClient(user.accessToken);
        // console.log("[dashboard loader] Access token (first 50 chars):", user.accessToken?.substring(0, 50));
        // console.log("[dashboard loader] User object:", JSON.stringify(user, null, 2));
        creditCards = await backendClient.GetCreditCardsQuery().get();
        latestCurrencyExchangeRates = await backendClient.GetCurrencyExchangeRatesQuery().get();
    } catch (e) {
    const { default: serverLogger } = await import("@/utils/logger.server");
    serverLogger.error("[dashboard loader] Error fetching dashboard data", e);
    }

    return { creditCards, latestCurrencyExchangeRates };
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
