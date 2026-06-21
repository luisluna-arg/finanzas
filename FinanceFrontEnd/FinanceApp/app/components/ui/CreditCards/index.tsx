import { useLoaderData, useNavigate } from 'react-router';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/shadcn/table';
import { Separator } from '@/components/ui/shadcn/separator';
import type { CreditCard, CreditCardsData } from '@/types/creditCard';

const formatDate = (dateStr: string) => {
  const [y, m, d] = dateStr.slice(0, 10).split('-').map(Number);
  return new Date(y, m - 1, d).toLocaleDateString();
};

function CreditCardList({
  creditCards,
  onSelectCard,
}: {
  creditCards: CreditCard[];
  onSelectCard: (card: CreditCard) => void;
}) {
  return (
    <>
      <div className="row flex justify-between items-center mb-4">
        <h1 className="text-base font-medium">Tarjetas de Crédito</h1>
      </div>
      <Separator className="my-5" />
      {/* TODO: This should use the table component from ui/utils/Table, but it needs some adjustments to support this implementation. */}
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Nombre</TableHead>
            <TableHead>Banco</TableHead>
            <TableHead className="text-right">Último Cierre</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {!creditCards || creditCards.length === 0 ? (
            <TableRow>
              <TableCell colSpan={3} className="text-center text-muted-foreground">
                No se encontraron tarjetas de crédito
              </TableCell>
            </TableRow>
          ) : (
            creditCards.map((card) => (
              <TableRow
                key={card.id}
                className="cursor-pointer hover:bg-primary/5 transition-colors"
                onClick={() => onSelectCard(card)}
              >
                <TableCell className="font-medium">{card.name}</TableCell>
                <TableCell>{card.bank?.name ?? '-'}</TableCell>
                <TableCell className="text-right">
                  {card.creditCardStatement
                    ? formatDate(card.creditCardStatement.closureDate)
                    : '-'}
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </>
  );
}

function CreditCards() {
  const { creditCards } = useLoaderData<CreditCardsData>();
  const navigate = useNavigate();

  return (
    <CreditCardList
      creditCards={creditCards ?? []}
      onSelectCard={(card) => navigate(`/credit-cards/statement/${card.id}`)}
    />
  );
}

export default CreditCards;
