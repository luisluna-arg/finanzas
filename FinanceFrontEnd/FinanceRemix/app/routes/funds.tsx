import { getBackendClient } from '@/data/getBackendClient';
import Funds from "@/components/ui/Funds";

export const loader = async () => {
    
    let client = await getBackendClient();

    let banks = await client.BanksQuery.get()

    let currencies = await client.CurrenciesQuery.get()

    return {
        banks,
        currencies
    }
};

export const meta = () => {
    return [{
      title: "Fondos",
      description: "",
    }];
  };
  
export default Funds;