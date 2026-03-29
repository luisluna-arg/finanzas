import { getBackendClient } from '@/data/getBackendClient';
import { LoaderFunctionArgs, redirect } from 'react-router';

import { requireAuth } from '@/services/auth/session.server';
import Dashboard from '../components/ui/Dashboard';
import serverLogger from '@/utils/logger.server';
import { CURRENCY_IDS } from '@/utils/currency.constants';

export const loader = async ({ request }: LoaderFunctionArgs) => {
  try {
    const user = await requireAuth(request);

    let creditCards = [];
    let latestCurrencyExchangeRates = null;
    let defaultCurrency: { id: string; symbol: string } | null = null;
    try {
      if (!user.accessToken) {
        throw new Error('No access token available');
      }
      const backendClient = await getBackendClient(user.accessToken!);
      const [creditCardsResult, exchangeRatesResult, currencyResult] = await Promise.all([
        backendClient.GetCreditCardsQuery().get(),
        backendClient.GetCurrencyExchangeRatesQuery().get(),
        backendClient.GetCurrenciesQuery().getById(CURRENCY_IDS.ARS),
      ]);
      creditCards = creditCardsResult;
      latestCurrencyExchangeRates = exchangeRatesResult;
      const symbol = (currencyResult as { defaultSymbol?: string } | undefined)?.defaultSymbol;
      defaultCurrency = symbol ? { id: CURRENCY_IDS.ARS, symbol } : null;
    } catch (e) {
      serverLogger.error('[dashboard loader] Error fetching dashboard data', e);
    }

    return { creditCards, latestCurrencyExchangeRates, defaultCurrency };
  } catch (authError) {
    serverLogger.error('[dashboard] Authentication failed:', authError);
    return redirect('/auth/login');
  }
};

export const meta = () => {
  return [
    {
      title: 'Dashboard',
      description: 'Home of the Finance app',
    },
  ];
};

export default Dashboard;
