import { getBackendClient } from '@/app/data/getBackendClient';
import Funds from "@/app/components/ui/Funds";

export const loader = async () => {
    
    let client = await getBackendClient();

    let banks = await client.BanksQuery.get()

    let currencies = await client.CurrenciesQuery.get()

    return {
        banks,
        currencies
    }
};

export default Funds;