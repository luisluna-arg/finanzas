import { LoaderFunctionArgs } from "@remix-run/node";
import MonthlyDebits from "@/components/ui/Debits/Monthly";
import { getBackendClient } from "@/data/getBackendClient";
import { PaginatedQueryFilters } from "@/data/base/BasePaginatedQuery";
import { requireAuth } from "@/services/auth/session.server";

type DebitFrequency = "monthly" | "annual";

interface DebitFilters {
    AppModuleId?: string;
    Frequency?: DebitFrequency;
    IncludeDeactivated?: boolean;
}

interface PaginatedDebitFilters extends PaginatedQueryFilters, DebitFilters {}

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const user = await requireAuth(request);

    if (!user.accessToken) {
        throw new Error("No access token available");
    }

    const pesosModuleId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
    const dollarsModuleId = "03cc66c7-921c-4e05-810e-9764cd365c1d";

    const client = await getBackendClient(user.accessToken!);

    const pesoDebits = await client
        .GetPaginatedDebitsQuery()
        .getPaginated<PaginatedDebitFilters>({
            AppModuleId: pesosModuleId,
            Frequency: "monthly",
            PageSize: 10,
            Page: 1,
        });

    const dollarDebits = await client
        .GetPaginatedDebitsQuery()
        .getPaginated<PaginatedDebitFilters>({
            AppModuleId: dollarsModuleId,
            Frequency: "monthly",
            Page: 1,
            PageSize: 10,
        });

    return {
        pesoDebits,
        dollarDebits,
        pesosModuleId,
        dollarsModuleId,
    };
};

export default MonthlyDebits;
