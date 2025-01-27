import { getBackendClient } from "@/data/getBackendClient";
import Dashboard from "../components/ui/Dashboard";

export const loader = async () => {
  try {
    let backendClient = await getBackendClient();

    let [creditCards, latestCurrencyExchangeRates] = await Promise.all([
      backendClient.CreditCardsQuery.get(),
      backendClient.CurrencyExchangeRatesQuery.get(),
    ]);

    return {
      creditCards,
      latestCurrencyExchangeRates,
    };
  } catch (error) {
    throw new Error("Failed to retrieve CreditCards. " + error);
  }
};

export const meta = () => {
  return [{
    title: "Dashboard",
    description: "Home of the Finance app",
  }];
};

export default Dashboard;
