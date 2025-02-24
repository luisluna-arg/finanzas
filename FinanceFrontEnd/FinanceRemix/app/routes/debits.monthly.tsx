import { LoaderFunction } from "@remix-run/node";
import MonthlyDebits from "@/components/ui/Debits/Monthly";
import { getBackendClient } from "@/data/getBackendClient";
import { PaginatedQueryFilters } from "@/data/base/BasePaginatedQuery";

type DebitFrequency = "monthly" | "annual";

interface DebitFilters {
  AppModuleId?: string;
  Frequency?: DebitFrequency;
  IncludeDeactivated?: boolean;
}

interface PaginatedDebitFilters extends PaginatedQueryFilters, DebitFilters {}

export const loader: LoaderFunction = async () => {
  const pesosModuleId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
  const dollarsModuleId = "03cc66c7-921c-4e05-810e-9764cd365c1d";

  let client = await getBackendClient();

  let pesoDebits =
    await client.PaginatedDebitsQuery.getPaginated<PaginatedDebitFilters>({
      AppModuleId: pesosModuleId,
      Frequency: "monthly",
      PageSize: 10,
      Page: 1,
    });

  let dollarDebits =
    await client.PaginatedDebitsQuery.getPaginated<PaginatedDebitFilters>({
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
