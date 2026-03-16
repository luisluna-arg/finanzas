import { useState, useEffect } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/shadcn/table';
import { Button } from '@/components/ui/shadcn/button';
import { Input } from '@/components/ui/shadcn/input';
import { Label } from '@/components/ui/shadcn/label';
import { Separator } from '@/components/ui/shadcn/separator';
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  ModalFooter,
} from '@/components/ui/utils/Modal';
import urls from '@/utils/urls';
import type { CreditCardStatement, CreditCardTransaction } from '@/types/creditCard';

interface DraftTransaction {
  id?: string;
  timestamp: string;
  concept: string;
  amount: string;
  isDeleted: boolean;
  isModified: boolean;
}

export default function EditStatementModal({
  show,
  onHide,
  statement,
  transactions,
  onSaved,
}: {
  show: boolean;
  onHide: () => void;
  statement: CreditCardStatement;
  transactions: CreditCardTransaction[];
  onSaved: () => void;
}) {
  const [closureDate, setClosureDate] = useState('');
  const [expiringDate, setExpiringDate] = useState('');
  const [draftTxs, setDraftTxs] = useState<DraftTransaction[]>([]);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (show) {
      setClosureDate(statement.closureDate.slice(0, 10));
      setExpiringDate(statement.expiringDate.slice(0, 10));
      setDraftTxs(
        transactions.map((tx) => ({
          id: tx.id,
          timestamp: tx.timestamp.slice(0, 10),
          concept: tx.concept,
          amount: String(tx.amount),
          isDeleted: false,
          isModified: false,
        }))
      );
    }
  }, [show, statement, transactions]);

  const updateTx = (
    index: number,
    field: keyof Pick<DraftTransaction, 'timestamp' | 'concept' | 'amount'>,
    value: string
  ) => {
    setDraftTxs((prev) =>
      prev.map((tx, i) => (i === index ? { ...tx, [field]: value, isModified: true } : tx))
    );
  };

  const deleteTx = (index: number) => {
    setDraftTxs((prev) =>
      prev.map((tx, i) => (i === index ? { ...tx, isDeleted: true } : tx))
    );
  };

  const addRow = () => {
    setDraftTxs((prev) => [
      ...prev,
      {
        timestamp: new Date().toISOString().slice(0, 10),
        concept: '',
        amount: '0',
        isDeleted: false,
        isModified: false,
      },
    ]);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const origClosure = statement.closureDate.slice(0, 10);
      const origExpiring = statement.expiringDate.slice(0, 10);
      if (closureDate !== origClosure || expiringDate !== origExpiring) {
        const res = await fetch(String(urls.creditCardStatements.endpoint), {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            id: statement.id,
            creditCardId: statement.creditCardId,
            closureDate: new Date(closureDate).toISOString(),
            expiringDate: new Date(expiringDate).toISOString(),
          }),
        });
        if (!res.ok) throw new Error(await res.text());
      }

      for (const tx of draftTxs.filter((t) => t.isDeleted && t.id !== undefined)) {
        const res = await fetch(String(urls.creditCardTransactions.endpoint), {
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ id: tx.id }),
        });
        if (!res.ok) throw new Error(await res.text());
      }

      for (const tx of draftTxs.filter((t) => !t.isDeleted && t.id === undefined)) {
        const res = await fetch(String(urls.creditCardTransactions.endpoint), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            creditCardId: statement.creditCardId,
            timestamp: new Date(tx.timestamp).toISOString(),
            concept: tx.concept,
            amount: Number(tx.amount),
            transactionType: 0,
          }),
        });
        if (!res.ok) throw new Error(await res.text());
      }

      for (const tx of draftTxs.filter(
        (t) => !t.isDeleted && t.id !== undefined && t.isModified
      )) {
        const res = await fetch(String(urls.creditCardTransactions.endpoint), {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            id: tx.id,
            timestamp: new Date(tx.timestamp).toISOString(),
            concept: tx.concept,
            amount: Number(tx.amount),
          }),
        });
        if (!res.ok) throw new Error(await res.text());
      }

      onSaved();
      onHide();
    } catch (err) {
      alert(`Error al guardar: ${err}`);
    } finally {
      setSubmitting(false);
    }
  };

  const visibleTxs = draftTxs.filter((t) => !t.isDeleted);

  return (
    <Modal show={show} onHide={onHide} size="4xl">
      <form onSubmit={handleSave}>
        <ModalHeader closeButton>
          <ModalTitle>
            <h3 className="text-lg font-medium">Editar Resúmen</h3>
          </ModalTitle>
        </ModalHeader>
        <ModalBody>
          <div className="space-y-6">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="edit-closure-date" className="block mb-2">
                  Fecha de Cierre
                </Label>
                <Input
                  id="edit-closure-date"
                  type="date"
                  required
                  value={closureDate}
                  onChange={(e) => setClosureDate(e.target.value)}
                />
              </div>
              <div>
                <Label htmlFor="edit-expiring-date" className="block mb-2">
                  Fecha de Vencimiento
                </Label>
                <Input
                  id="edit-expiring-date"
                  type="date"
                  required
                  value={expiringDate}
                  onChange={(e) => setExpiringDate(e.target.value)}
                />
              </div>
            </div>
            <Separator />
            <div>
              <p className="text-sm font-medium mb-3">Movimientos</p>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-36">Fecha</TableHead>
                    <TableHead>Concepto</TableHead>
                    <TableHead className="w-28 text-right">Monto</TableHead>
                    <TableHead className="w-10"></TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {visibleTxs.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={4} className="text-center text-muted-foreground">
                        Sin movimientos
                      </TableCell>
                    </TableRow>
                  ) : (
                    visibleTxs.map((tx) => {
                      const realIndex = draftTxs.indexOf(tx);
                      return (
                        <TableRow key={tx.id ?? `new-${realIndex}`}>
                          <TableCell>
                            <Input
                              type="date"
                              value={tx.timestamp}
                              onChange={(e) => updateTx(realIndex, 'timestamp', e.target.value)}
                              className="h-8 text-sm"
                            />
                          </TableCell>
                          <TableCell>
                            <Input
                              value={tx.concept}
                              onChange={(e) => updateTx(realIndex, 'concept', e.target.value)}
                              className="h-8 text-sm"
                              placeholder="Concepto"
                            />
                          </TableCell>
                          <TableCell>
                            <Input
                              type="number"
                              step="0.01"
                              value={tx.amount}
                              onChange={(e) => updateTx(realIndex, 'amount', e.target.value)}
                              className="h-8 text-sm text-right"
                            />
                          </TableCell>
                          <TableCell>
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              onClick={() => deleteTx(realIndex)}
                              className="text-destructive hover:text-destructive h-8 w-8 p-0"
                            >
                              ×
                            </Button>
                          </TableCell>
                        </TableRow>
                      );
                    })
                  )}
                </TableBody>
              </Table>
              <Button type="button" variant="outline" size="sm" className="mt-2" onClick={addRow}>
                + Agregar fila
              </Button>
            </div>
          </div>
        </ModalBody>
        <ModalFooter>
          <div className="flex justify-end space-x-2 mt-6">
            <Button type="button" variant="outline" onClick={onHide}>
              Cancelar
            </Button>
            <Button type="submit" disabled={submitting}>
              {submitting ? 'Guardando...' : 'Guardar'}
            </Button>
          </div>
        </ModalFooter>
      </form>
    </Modal>
  );
}
