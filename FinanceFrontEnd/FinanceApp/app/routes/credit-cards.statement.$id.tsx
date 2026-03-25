import { LoaderFunctionArgs, redirect } from 'react-router';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';
import type { CreditCard } from '@/types/creditCard';
import CreditCardDetail from '@/components/ui/CreditCards/CreditCardDetail';

export const loader = async ({ request, params }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);
  const client = await getBackendClient(user.accessToken!);

  const cards = (await client.GetCreditCardsQuery().get()) as CreditCard[];
  const card = cards.find((c) => c.id === params.id);
  if (!card) return redirect('/credit-cards');

  return { card, cards };
};

export const meta = ({ data }: { data: { card: CreditCard } | undefined }) => {
  return [
    {
      title: data?.card ? `${data.card.name} — Tarjeta de Crédito` : 'Tarjeta de Crédito',
    },
  ];
};

export default CreditCardDetail;
