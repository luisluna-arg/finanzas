import { LoaderFunctionArgs } from 'react-router';
import { getBackendClient } from '@/data/getBackendClient';
import urls from '@/utils/urls';
import CurrencyExchangeRates from '@/components/ui/CurrencyExchangeRates';
import { requireAuth } from '@/services/auth/session.server';
import { CURRENCY_IDS } from '@/utils/currency.constants';

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 100;

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);

  if (!user.accessToken) {
    throw new Error('No access token available');
  }

  const url = new URL(request.url);
  const page = Number(url.searchParams.get('page') ?? DEFAULT_PAGE);
  const pageSize = Number(url.searchParams.get('pageSize') ?? DEFAULT_PAGE_SIZE);
  let selectedBaseCurrencyId = url.searchParams.get('baseCurrencyId') ?? undefined;
  let selectedQuoteCurrencyId = url.searchParams.get('quoteCurrencyId') ?? undefined;

  const client = await getBackendClient(user.accessToken!);

  const getDataPromise = (baseCurrencyId: string, quoteCurrencyId: string) => {
    const endpoint = urls.currencyExchangeRates.paginated
      .with({
        Page: page,
        PageSize: pageSize,
        BaseCurrencyId: baseCurrencyId,
        QuoteCurrencyId: quoteCurrencyId,
      })
      .toRaw();
    return client.get(endpoint);
  };

  const currencies = await client.GetCurrenciesQuery().get();

  let data = null;
  if (selectedBaseCurrencyId && selectedQuoteCurrencyId) {
    data = await getDataPromise(selectedBaseCurrencyId, selectedQuoteCurrencyId);
  }

  if (!data && currencies?.length >= 2) {
    selectedBaseCurrencyId ??= CURRENCY_IDS.ARS;
    selectedQuoteCurrencyId ??= CURRENCY_IDS.USD;
    data = await getDataPromise(selectedBaseCurrencyId!, selectedQuoteCurrencyId!);
  }

  return {
    currencies,
    data: data ?? [],
    baseCurrencyId: selectedBaseCurrencyId,
    quoteCurrencyId: selectedQuoteCurrencyId,
  };
};

export const meta = () => {
  return [
    {
      title: 'Tipos de Cambio',
      description: '',
    },
  ];
};

export default CurrencyExchangeRates;
