import { LoaderFunctionArgs } from "@remix-run/node";
import { getBackendClient } from "@/data/getBackendClient";
import urls from "@/utils/urls";
import Incomes from "@/components/ui/Incomes/Index";
import CommonUtils from "@/utils/common";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 100;

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const url = new URL(request.url);
  const page = url.searchParams.get("page") ?? DEFAULT_PAGE;
  const pageSize = url.searchParams.get("pageSize") ?? DEFAULT_PAGE_SIZE;
  let selectedBankId = url.searchParams.get("bankId") ?? undefined;
  let selectedCurrencyId = url.searchParams.get("currencyId") ?? undefined;

  urls.incomes.paginated;

  let client = await getBackendClient();

  const getDataPromise = (bankId: string, currencyId: string) => {
    const url = `${urls.incomes.paginated}?${CommonUtils.Params({
      Page: page,
      PageSize: pageSize,
      BankId: bankId,
      CurrencyId: currencyId,
    })}`;
    return client.get(url);
  };

  let queries = [
    await client.BanksQuery.get(),
    await client.CurrenciesQuery.get(),
  ];

  if (selectedBankId && selectedCurrencyId) {
    queries.push(getDataPromise(selectedBankId, selectedCurrencyId));
  }

  let [banks, currencies, data] = await Promise.all(queries);

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
      title: "Ingresos",
      description: "",
    },
  ];
};

export default Incomes;
