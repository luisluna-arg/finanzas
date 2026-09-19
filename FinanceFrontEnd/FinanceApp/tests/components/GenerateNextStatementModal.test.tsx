import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import GenerateNextStatementModal from '@/components/ui/CreditCards/GenerateNextStatementModal';
import type { GeneratedStatementDraft } from '@/components/ui/CreditCards/GenerateNextStatementModal';

vi.mock('@/components/ui/utils/Modal', () => ({
  Modal: ({ show, children }: { show: boolean; children: React.ReactNode }) =>
    show ? <div data-testid="modal">{children}</div> : null,
  ModalHeader: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  ModalTitle: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  ModalBody: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  ModalFooter: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

vi.mock('@/utils/urls', () => ({
  default: {
    creditCardStatements: {
      nextDraft: { with: (params: Record<string, unknown>) => `/api/statements/next-draft?${new URLSearchParams(params as Record<string, string>).toString()}` },
    },
  },
}));

const mockDraft = {
  creditCardId: 'card-1',
  currentStatementId: 'stmt-1',
  suggestedClosureDate: '2025-07-30T00:00:00Z',
  suggestedExpiringDate: '2025-08-10T00:00:00Z',
  continuingInstallments: [
    {
      paymentPlanId: 'plan-1',
      baseConcept: 'Amazon',
      nextInstallmentNumber: 2,
      totalInstallments: 3,
      suggestedAmount: 300,
      currencyId: 'cur-ars',
      sourceTransactionId: 'tx-1',
    },
  ],
  eligibleNonPlanTransactions: [
    {
      id: 'tx-2',
      creditCardId: 'card-1',
      timestamp: '2025-06-05T00:00:00Z',
      concept: 'Gym',
      amount: 20,
      convertedAmount: 20,
      currencyId: 'cur-ars',
      installmentNumber: 1,
    },
  ],
};

function buildFetchMock(ok = true, data: unknown = mockDraft) {
  return vi.fn().mockImplementation(() =>
    Promise.resolve({
      ok,
      text: () => Promise.resolve(ok ? '' : 'boom'),
      json: () => Promise.resolve(data),
    })
  );
}

const defaultProps = {
  show: true,
  onHide: vi.fn(),
  creditCardId: 'card-1',
  onContinue: vi.fn(),
};

beforeEach(() => {
  vi.clearAllMocks();
  global.fetch = buildFetchMock();
});

describe('GenerateNextStatementModal', () => {
  it('does not render when show is false', () => {
    render(<GenerateNextStatementModal {...defaultProps} show={false} />);
    expect(screen.queryByTestId('modal')).toBeNull();
  });

  it('fetches the draft for the given credit card on open', async () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('/api/statements/next-draft'));
      expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('CreditCardId=card-1'));
    });
  });

  it('shows the count of auto-included continuing installments', async () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(screen.getByText(/incluirán automáticamente 1 cuota/)).toBeDefined();
    });
  });

  it('lists eligible non-plan transactions as an unchecked checklist', async () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('Gym'));

    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).not.toBeChecked();
  });

  it('shows a message when there are no other transactions to pick from', async () => {
    global.fetch = buildFetchMock(true, { ...mockDraft, eligibleNonPlanTransactions: [] });
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(screen.getByText('No hay otros movimientos.')).toBeDefined();
    });
  });

  it('shows an error message when the draft fetch fails', async () => {
    global.fetch = buildFetchMock(false);
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(screen.getByText(/boom/)).toBeDefined();
    });
  });

  it('disables Continuar until the draft has loaded', () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    expect(screen.getByRole('button', { name: 'Continuar' })).toBeDisabled();
  });

  it('continues with only the auto-included installment when nothing is checked', async () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('Gym'));

    await userEvent.click(screen.getByRole('button', { name: 'Continuar' }));

    expect(defaultProps.onContinue).toHaveBeenCalledTimes(1);
    const draft = defaultProps.onContinue.mock.calls[0][0] as GeneratedStatementDraft;
    expect(draft.closureDate).toBe(mockDraft.suggestedClosureDate);
    expect(draft.expiringDate).toBe(mockDraft.suggestedExpiringDate);
    expect(draft.draftTxs).toHaveLength(1);
    expect(draft.draftTxs[0]).toMatchObject({
      concept: 'Amazon (2/3)',
      amount: '300',
      currencyId: 'cur-ars',
    });
  });

  it('includes checked non-plan transactions when continuing', async () => {
    render(<GenerateNextStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('Gym'));

    await userEvent.click(screen.getByRole('checkbox'));
    await userEvent.click(screen.getByRole('button', { name: 'Continuar' }));

    const draft = defaultProps.onContinue.mock.calls[0][0] as GeneratedStatementDraft;
    expect(draft.draftTxs).toHaveLength(2);
    expect(draft.draftTxs.map((t) => t.concept)).toEqual(
      expect.arrayContaining(['Amazon (2/3)', 'Gym'])
    );
  });
});
