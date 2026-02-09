import { LoaderFunctionArgs } from 'react-router';
import CreditCards from '@/components/ui/CreditCards';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);
  const client = await getBackendClient(user.accessToken!);
  const creditCards = await client.GetCreditCardsQuery().get();
  return { creditCards };
};

export default CreditCards;
