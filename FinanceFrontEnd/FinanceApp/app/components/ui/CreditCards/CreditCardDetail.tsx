import { useState, useEffect, useCallback } from 'react';
import { useLoaderData, useNavigate } from 'react-router';
import FetchTable from '@/components/ui/utils/FetchTable';
import { InputType } from '@/components/ui/utils/InputType';
import { Button } from '@/components/ui/shadcn/button';
import { Input } from '@/components/ui/shadcn/input';
import { Label } from '@/components/ui/shadcn/label';
import { Separator } from '@/components/ui/shadcn/separator';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselPrevious,
  CarouselNext,
  type CarouselApi,
} from '@/components/ui/shadcn/carousel';
import { toNumber, ValueLike } from '@/utils/common';
import SafeLogger from '@/utils/SafeLogger';
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  ModalFooter,
} from '@/components/ui/utils/Modal';
import urls from '@/utils/urls';
import type { CreditCard, CreditCardStatement, CreditCardTransaction } from '@/types/creditCard';
import EditStatementModal from './EditStatementModal';
import ImportStatementModal from './ImportStatementModal';

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleDateString('es-AR');
};

const formatMoney = (value: unknown) => {
  return toNumber(value as ValueLike, 0).toFixed(2);
};

function StatementPicker({
  statements,
  currentIndex,
  onNavigate,
}: {
  statements: CreditCardStatement[];
  currentIndex: number;
  onNavigate: (index: number) => void;
}) {
  const [api, setApi] = useState<CarouselApi>();

  useEffect(() => {
    if (!api) return;
    api.scrollTo(currentIndex, true);
  }, [api, currentIndex]);

  if (statements.length === 0) return null;

  const lastIndex = statements.length - 1;

  return (
    <div className="flex items-center justify-center gap-2 mb-6 px-4">
      <Button
        variant="outline"
        size="sm"
        disabled={currentIndex === 0}
        onClick={() => onNavigate(0)}
      >
        Último
      </Button>
      <Button
        variant="outline"
        size="sm"
        disabled={currentIndex === 0}
        onClick={() => onNavigate(currentIndex - 1)}
      >
        ←
      </Button>
      <div className="flex-1 max-w-sm">
        <Carousel
          opts={{ align: 'center', loop: false }}
          setApi={setApi}
          className="w-full"
        >
          <CarouselContent>
            {statements.map((s, i) => (
              <CarouselItem key={s.id} onClick={() => onNavigate(i)} className="cursor-pointer">
                <div className={`text-sm font-medium px-4 py-2 rounded-md border text-center transition-colors ${
                  i === currentIndex ? 'bg-muted' : 'text-muted-foreground'
                }`}>
                  Cierre: {formatDate(s.closureDate)} — Vto: {formatDate(s.expiringDate)}
                </div>
              </CarouselItem>
            ))}
          </CarouselContent>
        </Carousel>
      </div>
      <Button
        variant="outline"
        size="sm"
        disabled={currentIndex === lastIndex}
        onClick={() => onNavigate(currentIndex + 1)}
      >
        →
      </Button>
      <Button
        variant="outline"
        size="sm"
        disabled={currentIndex === lastIndex}
        onClick={() => onNavigate(lastIndex)}
      >
        Primero
      </Button>
    </div>
  );
}

const TRANSACTION_COLUMNS = [
  {
    id: 'timestamp',
    label: 'Fecha',
    formatter: (v: unknown) => formatDate(String(v ?? '')),
  },
  {
    id: 'concept',
    label: 'Concepto',
  },
  {
    id: 'currency',
    label: 'Moneda',
    headerClass: 'w-20',
    mapper: (r: unknown) => {
      const tx = r as CreditCardTransaction;
      return tx.currency?.defaultSymbol ?? tx.currency?.shortName ?? '';
    },
  },
  {
    id: 'amount',
    label: 'Monto',
    class: 'text-right',
    headerClass: 'w-32 text-right',
    type: InputType.Decimal,
    mapper: (r: unknown) => r,
    formatter: (v: unknown) => {
      const tx = v as CreditCardTransaction;
      const symbol = tx.currency?.defaultSymbol ?? tx.currency?.shortName ?? '';
      const amount = formatMoney(tx.amount);
      return symbol ? `${symbol} ${amount}` : amount;
    },
    totals: {
      reducer: (acc: unknown, row: unknown) =>
        (acc as number) + toNumber((row as CreditCardTransaction).convertedAmount as unknown as ValueLike, 0),
      formatter: (v: unknown) => formatMoney(v),
    },
  },
];

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
      const res = await fetch(String(urls.creditCardStatements.endpoint), {
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

function CreditCardDetail() {
  const { card, cards, isAdmin } = useLoaderData<{ card: CreditCard; cards: CreditCard[]; isAdmin: boolean }>();
  const navigate = useNavigate();
  const [carouselApi, setCarouselApi] = useState<CarouselApi>();
  const [statements, setStatements] = useState<CreditCardStatement[]>([]);
  const [statementIndex, setStatementIndex] = useState(0);
  const [transactions, setTransactions] = useState<CreditCardTransaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showImportModal, setShowImportModal] = useState(false);

  useEffect(() => {
    if (!carouselApi) return;
    const currentIndex = cards.findIndex((c) => c.id === card.id);
    carouselApi.scrollTo(currentIndex, true);
    const onSelect = () => {
      const idx = carouselApi.selectedScrollSnap();
      if (cards[idx]?.id !== card.id) {
        navigate(`/credit-cards/statement/${cards[idx].id}`);
      }
    };
    carouselApi.on('select', onSelect);
    return () => { carouselApi.off('select', onSelect); };
  }, [carouselApi, card.id, cards, navigate]);

  const fetchStatements = useCallback(async () => {
    try {
      const res = await fetch(
        String(
          urls.creditCardStatements.endpoint.with({
            CreditCardId: card.id,
          })
        )
      );
      const data = await res.json();
      const items: CreditCardStatement[] = Array.isArray(data) ? data : (data.items ?? []);
      items.sort((a, b) => new Date(b.closureDate).getTime() - new Date(a.closureDate).getTime());
      setStatements(items);
      setStatementIndex(0);
    } catch (error) {
      SafeLogger.error('Error fetching statements:', error);
    }
  }, [card.id]);

  useEffect(() => {
    fetchStatements();
  }, [fetchStatements]);

  const fetchTransactions = useCallback(async () => {
    if (statements.length === 0) return;

    setLoading(true);
    try {
      const currentStatement = statements[statementIndex];
      const params: Record<string, unknown> = { StatementId: currentStatement.id };
      const url = urls.creditCardTransactions.endpoint.append('/all').with(params);

      const res = await fetch(String(url));
      const data = await res.json();
      const items: CreditCardTransaction[] = Array.isArray(data) ? data : (data.items ?? []);
      setTransactions(items);
    } catch (error) {
      SafeLogger.error('Error fetching transactions:', error);
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
          <Button variant="outline" size="sm" onClick={() => navigate('/credit-cards')}>
            ← Volver
          </Button>
          <div className="flex items-center gap-2 px-8">
            <Carousel
              opts={{ align: 'center', loop: false }}
              setApi={setCarouselApi}
              className="w-64"
            >
              <CarouselContent>
                {cards.map((c) => (
                  <CarouselItem key={c.id}>
                    <div className={`text-center text-sm font-medium px-2 py-1 rounded-md transition-colors ${
                      c.id === card.id ? 'text-foreground' : 'text-muted-foreground'
                    }`}>
                      {c.bank?.name ?? '-'} — {c.name}
                    </div>
                  </CarouselItem>
                ))}
              </CarouselContent>
              <CarouselPrevious />
              <CarouselNext />
            </Carousel>
          </div>
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
          <Button
            variant="outline"
            size="sm"
            className="border-primary text-primary hover:bg-primary/10"
            onClick={() => setShowImportModal(true)}
          >
            Importar Resúmen
          </Button>
          {statements.length > 0 && (
            <Button
              variant="outline"
              size="sm"
              className="border-primary text-primary hover:bg-primary/10"
              onClick={() => setShowEditModal(true)}
            >
              Editar Resúmen
            </Button>
          )}
        </div>
      </div>
      <Separator className="my-5" />

      <StatementPicker
        statements={statements}
        currentIndex={statementIndex}
        onNavigate={setStatementIndex}
      />

      {loading ? (
        <div className="text-center py-10 text-muted-foreground">Cargando...</div>
      ) : (
        <FetchTable name="transactions" data={transactions as never[]} columns={TRANSACTION_COLUMNS} />
      )}

      <CreateStatementModal
        show={showCreateModal}
        onHide={() => setShowCreateModal(false)}
        creditCardId={card.id}
        onCreated={fetchStatements}
      />

      {statements.length > 0 && (
        <EditStatementModal
          show={showEditModal}
          onHide={() => setShowEditModal(false)}
          statement={statements[statementIndex]}
          transactions={transactions}
          onSaved={() => {
            fetchStatements();
            fetchTransactions();
          }}
        />
      )}

      <ImportStatementModal
        show={showImportModal}
        onHide={() => setShowImportModal(false)}
        creditCardId={card.id}
        defaultTemplateId={card.defaultImportTemplateId ?? null}
        isAdmin={isAdmin}
        onImported={() => {
          fetchStatements();
          fetchTransactions();
        }}
      />
    </>
  );
}

export default CreditCardDetail;
