import { useState, useEffect, useCallback } from 'react';
import { useLoaderData } from 'react-router';
import {
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/shadcn/table';
import { Button } from '@/components/ui/shadcn/button';
import { Input } from '@/components/ui/shadcn/input';
import { Label } from '@/components/ui/shadcn/label';
import { Separator } from '@/components/ui/shadcn/separator';
import { cn } from '@/lib/utils';
import { toNumber, ValueLike } from '@/utils/common';
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  ModalFooter,
} from '@/components/ui/utils/Modal';
import urls from '@/utils/urls';
import type {
  CreditCard,
  CreditCardStatement,
  CreditCardTransaction,
  CreditCardsData,
} from '@/types/creditCard';

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleDateString('es-AR');
};

const formatMoney = (value: unknown) => {
  return toNumber(value as ValueLike, 0).toFixed(2);
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
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Nombre</TableHead>
            <TableHead>Banco</TableHead>
            <TableHead className="text-right">Movimientos</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {(!creditCards || creditCards.length === 0) ? (
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
                <TableCell className="text-right">{card.recordCount}</TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </>
  );
}

function StatementPicker({
  statements,
  currentIndex,
  onNavigate,
}: {
  statements: CreditCardStatement[];
  currentIndex: number;
  onNavigate: (index: number) => void;
}) {
  if (statements.length === 0) return null;

  const current = statements[currentIndex];
  const canGoPrev = currentIndex < statements.length - 1;
  const canGoNext = currentIndex > 0;

  return (
    <div className="flex items-center justify-center gap-4 mb-6">
      <Button
        variant="outline"
        size="sm"
        disabled={!canGoPrev}
        onClick={() => onNavigate(currentIndex + 1)}
      >
        ← Anterior
      </Button>
      <div className="text-sm font-medium px-4 py-2 rounded-md border bg-muted min-w-[300px] text-center">
        Cierre: {formatDate(current.closureDate)} — Vto: {formatDate(current.expiringDate)}
      </div>
      <Button
        variant="outline"
        size="sm"
        disabled={!canGoNext}
        onClick={() => onNavigate(currentIndex - 1)}
      >
        Siguiente →
      </Button>
      {currentIndex !== 0 && (
        <Button
          variant="outline"
          size="sm"
          onClick={() => onNavigate(0)}
        >
          Último
        </Button>
      )}
    </div>
  );
}

function TransactionsTable({
  transactions,
}: {
  transactions: CreditCardTransaction[];
}) {
  const totalAmount = transactions.reduce(
    (acc, tx) => acc + toNumber(tx.amount as unknown as ValueLike, 0),
    0
  );

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Fecha</TableHead>
          <TableHead>Concepto</TableHead>
          <TableHead className="text-right">Monto</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {transactions.length === 0 ? (
          <TableRow>
            <TableCell colSpan={3} className="text-center text-muted-foreground">
              No se encontraron movimientos
            </TableCell>
          </TableRow>
        ) : (
          transactions.map((tx) => (
            <TableRow key={tx.id}>
              <TableCell>{formatDate(tx.timestamp)}</TableCell>
              <TableCell>{tx.concept}</TableCell>
              <TableCell className="text-right">
                {formatMoney(tx.amount)}
              </TableCell>
            </TableRow>
          ))
        )}
      </TableBody>
      {transactions.length > 0 && (
        <TableFooter>
          <TableRow>
            <TableCell className="font-medium">Total</TableCell>
            <TableCell></TableCell>
            <TableCell className="text-right font-medium">
              {formatMoney(totalAmount)}
            </TableCell>
          </TableRow>
        </TableFooter>
      )}
    </Table>
  );
}

function CreateStatementModal({
  show,
  onHide,
  creditCardId,
  onCreated,
}: {
  show: boolean;
  onHide: () => void;
  creditCardId: string;
  onCreated: () => void;
}) {
  const [closureDate, setClosureDate] = useState('');
  const [expiringDate, setExpiringDate] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const res = await fetch(urls.proxy(urls.creditCardStatements.endpoint), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          creditCardId,
          closureDate: new Date(closureDate).toISOString(),
          expiringDate: new Date(expiringDate).toISOString(),
        }),
      });
      if (res.ok) {
        onHide();
        onCreated();
      } else {
        const error = await res.text();
        alert(`Error al crear resúmen: ${error}`);
      }
    } catch (error) {
      alert(`Error al crear resúmen: ${error}`);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal show={show} onHide={onHide}>
      <form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>
            <h3 className="text-lg font-medium">Nuevo Resúmen</h3>
          </ModalTitle>
        </ModalHeader>
        <ModalBody>
          <div className="space-y-4">
            <div>
              <Label htmlFor="closure-date" className="block mb-2">
                Fecha de Cierre
              </Label>
              <Input
                id="closure-date"
                type="date"
                required
                value={closureDate}
                onChange={(e) => setClosureDate(e.target.value)}
              />
            </div>
            <div>
              <Label htmlFor="expiring-date" className="block mb-2">
                Fecha de Vencimiento
              </Label>
              <Input
                id="expiring-date"
                type="date"
                required
                value={expiringDate}
                onChange={(e) => setExpiringDate(e.target.value)}
              />
            </div>
          </div>
        </ModalBody>
        <ModalFooter>
          <div className="flex justify-end space-x-2 mt-6">
            <Button type="button" variant="outline" onClick={onHide}>
              Cancelar
            </Button>
            <Button type="submit" disabled={submitting}>
              {submitting ? 'Creando...' : 'Crear'}
            </Button>
          </div>
        </ModalFooter>
      </form>
    </Modal>
  );
}

function CreditCardDetail({
  card,
  onBack,
}: {
  card: CreditCard;
  onBack: () => void;
}) {
  const [statements, setStatements] = useState<CreditCardStatement[]>([]);
  const [statementIndex, setStatementIndex] = useState(0);
  const [transactions, setTransactions] = useState<CreditCardTransaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);

  const fetchStatements = useCallback(async () => {
    try {
      const res = await fetch(
        urls.proxy(urls.creditCardStatements.endpoint, {
          CreditCardId: card.id,
        })
      );
      const data = await res.json();
      const items: CreditCardStatement[] = Array.isArray(data)
        ? data
        : (data.items ?? []);
      items.sort(
        (a, b) =>
          new Date(b.closureDate).getTime() -
          new Date(a.closureDate).getTime()
      );
      setStatements(items);
      setStatementIndex(0);
    } catch (error) {
      console.error('Error fetching statements:', error);
    }
  }, [card.id]);

  useEffect(() => {
    fetchStatements();
  }, [fetchStatements]);

  const fetchTransactions = useCallback(async () => {
    if (statements.length === 0) return;

    setLoading(true);
    try {
      let url: string;

      if (statementIndex === 0) {
        url = urls.proxy(urls.creditCardMovements.latest, {
          CreditCardId: card.id,
          Page: 1,
          PageSize: 200,
        });
      } else {
        const currentStatement = statements[statementIndex];
        const prevStatement = statements[statementIndex + 1];
        const params: Record<string, unknown> = {
          CreditCardId: card.id,
          To: currentStatement.closureDate,
        };
        if (prevStatement) {
          params.From = prevStatement.closureDate;
        }
        url = urls.proxy(
          urls.creditCardTransactions.endpoint + '/all',
          params
        );
      }

      const res = await fetch(url);
      const data = await res.json();
      const items: CreditCardTransaction[] = Array.isArray(data)
        ? data
        : (data.items ?? []);
      setTransactions(items);
    } catch (error) {
      console.error('Error fetching transactions:', error);
      setTransactions([]);
    } finally {
      setLoading(false);
    }
  }, [card.id, statements, statementIndex]);

  useEffect(() => {
    fetchTransactions();
  }, [fetchTransactions]);

  return (
    <>
      <div className="row flex justify-between items-center mb-4">
        <div className="flex items-center gap-4">
          <Button variant="outline" size="sm" onClick={onBack}>
            ← Volver
          </Button>
          <h1 className="text-base font-medium">
            {card.name} — {card.bank?.name}
          </h1>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="border-primary text-primary hover:bg-primary/10"
            onClick={() => alert('No implementado')}
          >
            Generar Último
          </Button>
          <Button
            variant="outline"
            size="sm"
            className="border-primary text-primary hover:bg-primary/10"
            onClick={() => setShowCreateModal(true)}
          >
            Nuevo Resúmen
          </Button>
        </div>
      </div>
      <Separator className="my-5" />

      <StatementPicker
        statements={statements}
        currentIndex={statementIndex}
        onNavigate={setStatementIndex}
      />

      {loading ? (
        <div className="text-center py-10 text-muted-foreground">
          Cargando...
        </div>
      ) : (
        <TransactionsTable transactions={transactions} />
      )}

      <CreateStatementModal
        show={showCreateModal}
        onHide={() => setShowCreateModal(false)}
        creditCardId={card.id}
        onCreated={fetchStatements}
      />
    </>
  );
}

function CreditCards() {
  const { creditCards } = useLoaderData<CreditCardsData>();
  const [selectedCard, setSelectedCard] = useState<CreditCard | null>(null);

  return (
    <div className={cn('py-10', 'px-40')}>
      {selectedCard ? (
        <CreditCardDetail
          card={selectedCard}
          onBack={() => setSelectedCard(null)}
        />
      ) : (
        <CreditCardList
          creditCards={creditCards ?? []}
          onSelectCard={setSelectedCard}
        />
      )}
    </div>
  );
}

export default CreditCards;
