import { getBackendClient } from '@/app/data/getBackendClient';
import Dashboard from '../components/ui/Dashboard';

export const loader = async () => {
    try {
        let backendClient = await getBackendClient();

        let [
            creditCards,
            latestCurrencyExchangeRates
        ] = await Promise.all([
            backendClient.CreditCardsQuery.getCreditCards(),
            backendClient.CurrencyExchangeRatesQuery.getLatest()
        ]);

        return {
            creditCards,
            latestCurrencyExchangeRates
        };
    }
    catch (error) {
        throw new Error("Failed to retrieve CreditCards. " + error);
    }
}

export default Dashboard;