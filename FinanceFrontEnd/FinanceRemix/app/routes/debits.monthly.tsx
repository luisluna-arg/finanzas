import { LoaderFunction } from "@remix-run/node";
import urls from "@/app/utils/urls";
import MonthlyDebits from "@/app/components/ui/Debits/Monthly"

export const loader: LoaderFunction = async () => {
  const pesosModuleId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
  const dollarsModuleId = "03cc66c7-921c-4e05-810e-9764cd365c1d";

  const pesosEndpoint = `${urls.debits.monthly.paginated}?AppModuleId=${pesosModuleId}`;
  const dollarsEndpoint = `${urls.debits.monthly.paginated}?AppModuleId=${dollarsModuleId}`;

  return {
    pesosEndpoint,
    dollarsEndpoint,
    pesosModuleId,
    dollarsModuleId,
  };
};

export default MonthlyDebits;