import { getBackendClient } from '@/data/getBackendClient';
import Funds from '@/components/ui/Funds';
import { LoaderFunctionArgs } from 'react-router';
import { requireAuth } from '@/services/auth/session.server';
import urls from '@/utils/urls';

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 10;

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);

  if (!user.accessToken) {
    throw new Error('No access token available');
  }

  const requestUrl = new URL(request.url);
  const page = Number(requestUrl.searchParams.get('page') ?? DEFAULT_PAGE);
  const pageSize = Number(requestUrl.searchParams.get('pageSize') ?? DEFAULT_PAGE_SIZE);
  let selectedBankId = requestUrl.searchParams.get('bankId') ?? undefined;
  let selectedCurrencyId = requestUrl.searchParams.get('currencyId') ?? undefined;

  const client = await getBackendClient(user.accessToken!);

  const getDataPromise = (bankId: string, currencyId: string) => {
    const dataUrl = urls.funds.paginated
      .with({
        Page: page,
        PageSize: pageSize,
        BankId: bankId,
        CurrencyId: currencyId,
      })
      .toRaw();
    return client.get(dataUrl);
  };

  const queries = [await client.GetBanksQuery().get(), await client.GetCurrenciesQuery().get()];

  if (selectedBankId && selectedCurrencyId) {
    queries.push(getDataPromise(selectedBankId, selectedCurrencyId));
  }

  const results = await Promise.all(queries);
  const banks = results[0];
  const currencies = results[1];
  let data = results[2] ?? null;

  if (!data && banks?.length > 0 && currencies?.length > 0) {
    selectedBankId ??= banks[0].id;
    selectedCurrencyId ??= currencies[0].id;
    data = await getDataPromise(selectedBankId!, selectedCurrencyId!);
  }

  return {
    banks,
    currencies,
    data: data ?? [],
    bankId: selectedBankId,
    currencyId: selectedCurrencyId,
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
