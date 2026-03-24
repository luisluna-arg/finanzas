import { LoaderFunctionArgs } from 'react-router';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';
import Investments from "../components/ui/Investments";

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);

  if (!user.accessToken) {
    throw new Error('No access token available');
  }

  const client = await getBackendClient(user.accessToken!);
  const currencies = await client.GetCurrenciesQuery().get();

  return { currencies };
};

export const meta = () => {
    return [
        {
            title: "Inversiones",
            description: "",
        },
    ];
};

export default Investments;
