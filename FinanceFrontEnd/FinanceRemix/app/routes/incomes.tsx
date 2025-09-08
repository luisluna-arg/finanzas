import { LoaderFunctionArgs, redirect } from "@remix-run/node";
import { getBackendClient } from "@/data/getBackendClient";
import urls from "@/utils/urls";
import Incomes from "@/components/ui/Incomes/Index";
import CommonUtils from "@/utils/common";
import { SessionContants } from "@/services/auth/auth.constants";
import { sessionStorage } from "@/services/auth/session.server";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 100;

export const loader = async ({ request }: LoaderFunctionArgs) => {
    // Get the session cookie and user from session storage (same pattern as dashboard)
    const cookie = request.headers.get("Cookie");
    const session = await sessionStorage.getSession(cookie);
    const user = session.get(SessionContants.USER_KEY);

    if (!user) {
        return redirect("/auth/login");
    }

    const url = new URL(request.url);
    const page = url.searchParams.get("page") ?? DEFAULT_PAGE;
    const pageSize = url.searchParams.get("pageSize") ?? DEFAULT_PAGE_SIZE;
    let selectedBankId = url.searchParams.get("bankId") ?? undefined;
    let selectedCurrencyId = url.searchParams.get("currencyId") ?? undefined;

    const client = await getBackendClient(user.accessToken);

    const getDataPromise = (bankId: string, currencyId: string) => {
        const url = `${urls.incomes.paginated}?${CommonUtils.Params({
            Page: page,
            PageSize: pageSize,
            BankId: bankId,
            CurrencyId: currencyId,
        })}`;
        return client.get(url);
    };

    const queries = [
        await client.GetBanksQuery().get(),
        await client.GetCurrenciesQuery().get(),
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
