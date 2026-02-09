import { useState } from 'react';
import { useLoaderData } from 'react-router';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/shadcn/table';
import { Separator } from '@/components/ui/shadcn/separator';
import { cn } from '@/lib/utils';
import CreditCardDetail from './CreditCardDetail';
import type { CreditCard, CreditCardsData } from '@/types/creditCard';

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
                    ? new Date(card.creditCardStatement.closureDate).toLocaleDateString('es-AR')
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
  const [selectedCard, setSelectedCard] = useState<CreditCard | null>(null);

  return (
    <div className={cn('py-10', 'px-40')}>
      {selectedCard ? (
        <CreditCardDetail card={selectedCard} onBack={() => setSelectedCard(null)} />
      ) : (
        <CreditCardList creditCards={creditCards ?? []} onSelectCard={setSelectedCard} />
      )}
    </div>
  );
}

export default CreditCards;
