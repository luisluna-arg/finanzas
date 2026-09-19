import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import EditStatementModal from '@/components/ui/CreditCards/EditStatementModal';
import type { DraftTransaction } from '@/components/ui/CreditCards/EditStatementModal';
import type { CreditCardStatement, CreditCardTransaction } from '@/types/creditCard';

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
    creditCardStatements: { endpoint: '/api/statements' },
    creditCardTransactions: { endpoint: '/api/transactions' },
    catalog: { currencies: { endpoint: '/api/catalog/currencies' } },
  },
}));

const mockCurrencies = [
  { id: 'cur-ars', name: 'ARS' },
  { id: 'cur-usd', name: 'USD' },
];

const mockStatement: CreditCardStatement = {
  id: 'stmt-1',
  creditCardId: 'card-1',
  closureDate: '2025-06-30T00:00:00Z',
  expiringDate: '2025-07-10T00:00:00Z',
  deactivated: false,
};

const mockTransactions: CreditCardTransaction[] = [
  {
    id: 'tx-1',
    creditCardId: 'card-1',
    timestamp: '2025-06-01T00:00:00Z',
    concept: 'Netflix',
    amount: 100,
    convertedAmount: 100,
    currencyId: 'cur-ars',
    installmentNumber: 1,
  },
  {
    id: 'tx-2',
    creditCardId: 'card-1',
    timestamp: '2025-06-05T00:00:00Z',
    concept: 'Spotify',
    amount: 50,
    convertedAmount: 50,
    currencyId: 'cur-ars',
    installmentNumber: 1,
  },
];

function buildFetchMock() {
  return vi.fn().mockImplementation((url: string) => {
    if (url === '/api/catalog/currencies') {
      return Promise.resolve({ ok: true, json: () => Promise.resolve(mockCurrencies) });
    }
    if (url === '/api/statements') {
      return Promise.resolve({
        ok: true,
        text: () => Promise.resolve(''),
        json: () => Promise.resolve({ id: 'stmt-new' }),
      });
    }
    if (url === '/api/transactions') {
      return Promise.resolve({ ok: true, text: () => Promise.resolve('') });
    }
    return Promise.resolve({ ok: true, text: () => Promise.resolve(''), json: () => Promise.resolve({}) });
  });
}

beforeEach(() => {
  vi.clearAllMocks();
  global.alert = vi.fn();
  global.confirm = vi.fn().mockReturnValue(true);
  global.fetch = buildFetchMock();
});

describe('EditStatementModal', () => {
  describe('edit mode', () => {
    const editProps = {
      show: true,
      onHide: vi.fn(),
      statement: mockStatement,
      transactions: mockTransactions,
      onSaved: vi.fn(),
    };

    it('does not render when show is false', () => {
      render(<EditStatementModal {...editProps} show={false} />);
      expect(screen.queryByTestId('modal')).toBeNull();
    });

    it('shows the edit title and seeds rows from transactions', async () => {
      render(<EditStatementModal {...editProps} />);
      expect(screen.getByText('Editar Resúmen')).toBeDefined();
      await waitFor(() => {
        expect(screen.getByDisplayValue('Netflix')).toBeDefined();
        expect(screen.getByDisplayValue('Spotify')).toBeDefined();
      });
    });

    it('shows the delete-statement button', () => {
      render(<EditStatementModal {...editProps} />);
      expect(screen.getByRole('button', { name: 'Eliminar resúmen' })).toBeDefined();
    });

    it('removes a row from view when its delete button is clicked, and it stays removed', async () => {
      render(<EditStatementModal {...editProps} />);
      await waitFor(() => screen.getByDisplayValue('Netflix'));

      const deleteButtons = screen.getAllByRole('button', { name: '×' });
      await userEvent.click(deleteButtons[0]);

      expect(screen.queryByDisplayValue('Netflix')).toBeNull();
      expect(screen.getByDisplayValue('Spotify')).toBeDefined();

      // Regression guard: a later re-render (e.g. typing in the surviving row)
      // must not resurrect the deleted one.
      await userEvent.type(screen.getByDisplayValue('Spotify'), '!');
      expect(screen.queryByDisplayValue('Netflix')).toBeNull();
    });

    it('sends a DELETE for removed existing rows and no POST/PUT for them on save', async () => {
      render(<EditStatementModal {...editProps} />);
      await waitFor(() => screen.getByDisplayValue('Netflix'));

      await userEvent.click(screen.getAllByRole('button', { name: '×' })[0]);
      await act(async () => {
        fireEvent.submit(screen.getByTestId('modal').querySelector('form')!);
      });

      await waitFor(() => {
        expect(global.fetch).toHaveBeenCalledWith(
          '/api/transactions',
          expect.objectContaining({
            method: 'DELETE',
            body: JSON.stringify({ id: 'tx-1' }),
          })
        );
        expect(editProps.onSaved).toHaveBeenCalled();
      });
    });

    it('adds a new empty row when clicking + Agregar fila', async () => {
      render(<EditStatementModal {...editProps} />);
      await waitFor(() => screen.getByDisplayValue('Netflix'));

      await userEvent.click(screen.getByRole('button', { name: '+ Agregar fila' }));

      // 2 seeded rows + 1 new empty row, all sharing the "Concepto" placeholder.
      expect(screen.getAllByPlaceholderText('Concepto')).toHaveLength(3);
    });
  });

  describe('draft mode', () => {
    const initialDraftTxs: DraftTransaction[] = [
      {
        timestamp: '2025-07-01',
        concept: 'Amazon (2/3)',
        amount: '300',
        currencyId: 'cur-ars',
        isDeleted: false,
        isModified: false,
      },
      {
        timestamp: '2025-07-01',
        concept: 'Gym',
        amount: '20',
        currencyId: 'cur-ars',
        isDeleted: false,
        isModified: false,
      },
    ];

    const draftProps = {
      show: true,
      onHide: vi.fn(),
      mode: 'draft' as const,
      creditCardId: 'card-1',
      initialClosureDate: '2025-07-30T00:00:00Z',
      initialExpiringDate: '2025-08-10T00:00:00Z',
      initialDraftTxs,
      onSaved: vi.fn(),
    };

    it('shows the draft title and seeds rows from initialDraftTxs', () => {
      render(<EditStatementModal {...draftProps} />);
      expect(screen.getByText('Generar Próximo Resúmen')).toBeDefined();
      expect(screen.getByDisplayValue('Amazon (2/3)')).toBeDefined();
      expect(screen.getByDisplayValue('Gym')).toBeDefined();
    });

    it('does not show the delete-statement button', () => {
      render(<EditStatementModal {...draftProps} />);
      expect(screen.queryByRole('button', { name: 'Eliminar resúmen' })).toBeNull();
    });

    it('removes a row when its delete button is clicked and it does not come back on re-render', async () => {
      render(<EditStatementModal {...draftProps} />);

      const deleteButtons = screen.getAllByRole('button', { name: '×' });
      await userEvent.click(deleteButtons[0]);

      expect(screen.queryByDisplayValue('Amazon (2/3)')).toBeNull();
      expect(screen.getByDisplayValue('Gym')).toBeDefined();

      // Regression guard for the bug where the default `transactions = []` parameter
      // recreated a new array every render, causing the seeding effect to re-fire and
      // silently restore the row that had just been deleted.
      await userEvent.type(screen.getByDisplayValue('Gym'), '!');
      expect(screen.queryByDisplayValue('Amazon (2/3)')).toBeNull();
      expect(screen.getByDisplayValue('Gym!')).toBeDefined();
    });

    it('creates the statement then posts each remaining row on save', async () => {
      render(<EditStatementModal {...draftProps} />);

      await userEvent.click(screen.getAllByRole('button', { name: '×' })[0]);
      await act(async () => {
        fireEvent.submit(screen.getByTestId('modal').querySelector('form')!);
      });

      await waitFor(() => {
        expect(global.fetch).toHaveBeenCalledWith(
          '/api/statements',
          expect.objectContaining({ method: 'POST' })
        );
        expect(global.fetch).toHaveBeenCalledWith(
          '/api/transactions',
          expect.objectContaining({
            method: 'POST',
            body: expect.stringContaining('"concept":"Gym"'),
          })
        );
        expect(global.fetch).not.toHaveBeenCalledWith(
          '/api/transactions',
          expect.objectContaining({ method: 'DELETE' })
        );
        expect(draftProps.onSaved).toHaveBeenCalled();
      });
    });
  });
});
