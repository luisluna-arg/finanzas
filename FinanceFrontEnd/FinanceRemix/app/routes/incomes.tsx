import { getBackendClient } from '@/data/getBackendClient';
import Incomes from "../components/ui/Incomes/Index";

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
      title: "Ingresos",
      description: "",
    }];
  };
  
export default Incomes;