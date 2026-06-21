import { LoaderFunctionArgs, redirect } from 'react-router';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';
import type { CreditCard } from '@/types/creditCard';
import type { SessionInfo } from '@/data/queries/SessionQuery';
import { CURRENCY_IDS } from '@/utils/currency.constants';
import CreditCardDetail from '@/components/ui/CreditCards/CreditCardDetail';

export const loader = async ({ request, params }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);
  const client = await getBackendClient(user.accessToken!);

  const [cards, session, currencyResult] = await Promise.all([
    client.GetCreditCardsQuery().get() as Promise<CreditCard[]>,
    client.GetSessionQuery().get<void>().catch(() => null) as Promise<SessionInfo | null>,
    client.GetCurrenciesQuery().getById(CURRENCY_IDS.ARS).catch(() => null),
  ]);

  const card = cards.find((c) => c.id === params.id);
  if (!card) return redirect('/credit-cards');

  const isAdmin = session?.roles?.includes('Admin') ?? false;
  const symbol = (currencyResult as { defaultSymbol?: string } | null)?.defaultSymbol;
  const defaultCurrency: { id: string; symbol: string } | null = symbol
    ? { id: CURRENCY_IDS.ARS, symbol }
    : null;

  return { card, cards, isAdmin, defaultCurrency };
};

export const meta = ({ data }: { data: { card: CreditCard } | undefined }) => {
  return [
    {
      title: data?.card ? `${data.card.name} — Tarjeta de Crédito` : 'Tarjeta de Crédito',
    },
  ];
};

export default CreditCardDetail;
