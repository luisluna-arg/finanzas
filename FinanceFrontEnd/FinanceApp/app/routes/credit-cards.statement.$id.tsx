import { LoaderFunctionArgs, redirect } from 'react-router';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';
import type { CreditCard } from '@/types/creditCard';
import type { SessionInfo } from '@/data/queries/SessionQuery';
import CreditCardDetail from '@/components/ui/CreditCards/CreditCardDetail';

export const loader = async ({ request, params }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);
  const client = await getBackendClient(user.accessToken!);

  const [cards, session] = await Promise.all([
    client.GetCreditCardsQuery().get() as Promise<CreditCard[]>,
    client.GetSessionQuery().get<void>().catch(() => null) as Promise<SessionInfo | null>,
  ]);

  const card = cards.find((c) => c.id === params.id);
  if (!card) return redirect('/credit-cards');

  const isAdmin = session?.roles?.includes('Admin') ?? false;

  return { card, cards, isAdmin };
};

export const meta = ({ data }: { data: { card: CreditCard } | undefined }) => {
  return [
    {
      title: data?.card ? `${data.card.name} — Tarjeta de Crédito` : 'Tarjeta de Crédito',
    },
  ];
};

export default CreditCardDetail;
