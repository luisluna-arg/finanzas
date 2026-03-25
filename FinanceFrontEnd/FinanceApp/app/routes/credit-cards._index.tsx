import { LoaderFunctionArgs } from 'react-router';
import CreditCards from '@/components/ui/CreditCards';
import { getBackendClient } from '@/data/getBackendClient';
import { requireAuth } from '@/services/auth/session.server';
import type { CreditCard, CreditCardStatement } from '@/types/creditCard';

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);
  const client = await getBackendClient(user.accessToken!);

  const [creditCards, latestStatements] = await Promise.all([
    client.GetCreditCardsQuery().get(),
    client.GetCreditCardStatementsQuery().getLatest(),
  ]);

  const statementsById = new Map<string, CreditCardStatement>(
    (latestStatements as CreditCardStatement[]).map((s: CreditCardStatement) => [s.creditCardId, s])
  );

  const enrichedCards = (creditCards as CreditCard[]).map((card: CreditCard) => ({
    ...card,
    creditCardStatement: statementsById.get(card.id) ?? null,
  }));

  return { creditCards: enrichedCards };
};

export const meta = () => {
  return [
    {
      title: 'Tarjetas de Crédito',
      description: 'Credit cards management',
    },
  ];
};

export default CreditCards;
