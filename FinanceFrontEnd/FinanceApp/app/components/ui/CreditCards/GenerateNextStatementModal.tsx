import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/shadcn/button';
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  ModalFooter,
} from '@/components/ui/utils/Modal';
import { Checkbox } from '@/components/ui/utils/Checkbox';
import urls from '@/utils/urls';
import type { CreditCardStatementDraft } from '@/types/creditCard';
import type { DraftTransaction } from './EditStatementModal';

export interface GeneratedStatementDraft {
  closureDate: string;
  expiringDate: string;
  draftTxs: DraftTransaction[];
}

export default function GenerateNextStatementModal({
  show,
  onHide,
  creditCardId,
  onContinue,
}: {
  show: boolean;
  onHide: () => void;
  creditCardId: string;
  onContinue: (draft: GeneratedStatementDraft) => void;
}) {
  const [loading, setLoading] = useState(false);
  const [draft, setDraft] = useState<CreditCardStatementDraft | null>(null);
  const [checkedIds, setCheckedIds] = useState<Set<string>>(new Set());
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!show) return;
    setLoading(true);
    setError(null);
    setDraft(null);
    setCheckedIds(new Set());
    fetch(String(urls.creditCardStatements.nextDraft.with({ CreditCardId: creditCardId })))
      .then(async (r) => {
        if (!r.ok) throw new Error(await r.text());
        return r.json();
      })
      .then((data: CreditCardStatementDraft) => setDraft(data))
      .catch((err) => setError(String(err)))
      .finally(() => setLoading(false));
  }, [show, creditCardId]);

  const toggle = (id: string) => {
    setCheckedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleContinue = () => {
    if (!draft) return;

    const suggestedDate = draft.suggestedClosureDate.slice(0, 10);

    const installmentRows: DraftTransaction[] = draft.continuingInstallments.map((row) => ({
      timestamp: suggestedDate,
      concept: `${row.baseConcept} (${row.nextInstallmentNumber}/${row.totalInstallments})`,
      amount: String(row.suggestedAmount),
      currencyId: row.currencyId,
      isDeleted: false,
      isModified: false,
    }));

    const otherRows: DraftTransaction[] = draft.eligibleNonPlanTransactions
      .filter((tx) => checkedIds.has(tx.id))
      .map((tx) => ({
        timestamp: suggestedDate,
        concept: tx.concept,
        amount: String(tx.amount),
        currencyId: tx.currencyId,
        isDeleted: false,
        isModified: false,
      }));

    onContinue({
      closureDate: draft.suggestedClosureDate,
      expiringDate: draft.suggestedExpiringDate,
      draftTxs: [...installmentRows, ...otherRows],
    });
  };

  return (
    <Modal show={show} onHide={onHide} size="2xl">
      <ModalHeader closeButton>
        <ModalTitle>
          <h3 className="text-lg font-medium">Generar Próximo Resúmen</h3>
        </ModalTitle>
      </ModalHeader>
      <ModalBody>
        {loading && <p className="text-sm text-muted-foreground">Cargando...</p>}
        {error && <p className="text-sm text-destructive">{error}</p>}
        {draft && (
          <div className="space-y-4">
            <p className="text-sm">
              Se incluirán automáticamente {draft.continuingInstallments.length} cuota(s) en curso.
            </p>
            <div>
              <p className="text-sm font-medium mb-2">
                Otros movimientos del resúmen actual (opcional)
              </p>
              {draft.eligibleNonPlanTransactions.length === 0 ? (
                <p className="text-sm text-muted-foreground">No hay otros movimientos.</p>
              ) : (
                <div className="space-y-1 max-h-72 overflow-y-auto">
                  {draft.eligibleNonPlanTransactions.map((tx) => (
                    <label
                      key={tx.id}
                      className="flex items-center gap-2 text-sm px-2 py-1 rounded hover:bg-muted cursor-pointer"
                    >
                      <Checkbox checked={checkedIds.has(tx.id)} onCheckedChange={() => toggle(tx.id)} />
                      <span className="flex-1">{tx.concept}</span>
                      <span className="text-muted-foreground">{tx.amount}</span>
                    </label>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}
      </ModalBody>
      <ModalFooter>
        <div className="flex justify-end gap-2 mt-6">
          <Button type="button" variant="outline" onClick={onHide}>
            Cancelar
          </Button>
          <Button type="button" disabled={!draft} onClick={handleContinue}>
            Continuar
          </Button>
        </div>
      </ModalFooter>
    </Modal>
  );
}
