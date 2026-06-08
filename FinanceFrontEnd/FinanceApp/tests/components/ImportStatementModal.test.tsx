import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import ImportStatementModal from '@/components/ui/CreditCards/ImportStatementModal';

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
    creditCardStatementImportTemplates: { endpoint: '/api/templates' },
    creditCardStatements: {
      endpoint: '/api/statements',
      import: '/api/statements/import',
    },
    creditCards: {
      defaultImportTemplate: (id: string) => `/api/credit-cards/${id}/default-import-template`,
    },
    catalog: { currencies: { endpoint: '/api/catalog/currencies' } },
  },
}));

const mockTemplates = [
  { id: 'tmpl-1', name: 'My Template', isSystem: false, configJson: '{}' },
  { id: 'tmpl-sys', name: 'System Template', isSystem: true, configJson: '{}' },
];

const mockCurrencies = [
  { id: 'cur-ars', name: 'ARS' },
  { id: 'cur-usd', name: 'USD' },
];

function buildFetchMock() {
  return vi.fn().mockImplementation((url: string) => {
    if (url.startsWith('/api/templates')) {
      return Promise.resolve({
        ok: true,
        json: () => Promise.resolve(mockTemplates),
      });
    }
    if (url === '/api/catalog/currencies') {
      return Promise.resolve({
        ok: true,
        json: () => Promise.resolve(mockCurrencies),
      });
    }
    if (url === '/api/statements') {
      return Promise.resolve({
        ok: true,
        text: () => Promise.resolve(''),
        json: () => Promise.resolve({ id: 'stmt-new' }),
      });
    }
    if (url === '/api/statements/import') {
      return Promise.resolve({ ok: true, text: () => Promise.resolve('') });
    }
    if (typeof url === 'string' && url.includes('default-import-template')) {
      return Promise.resolve({ ok: true });
    }
    return Promise.resolve({ ok: true, json: () => Promise.resolve([]) });
  });
}

const defaultProps = {
  show: true,
  onHide: vi.fn(),
  creditCardId: 'card-123',
  defaultTemplateId: null as string | null,
  onImported: vi.fn(),
};

beforeEach(() => {
  vi.clearAllMocks();
  global.alert = vi.fn();
  global.confirm = vi.fn().mockReturnValue(true);
  global.fetch = buildFetchMock();
});

function getForm() {
  return screen.getByTestId('modal').querySelector('form')!;
}

describe('ImportStatementModal', () => {
  it('does not render when show is false', () => {
    render(<ImportStatementModal {...defaultProps} show={false} />);
    expect(screen.queryByTestId('modal')).toBeNull();
  });

  it('renders the modal title when show is true', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    expect(screen.getByTestId('modal')).toBeDefined();
    expect(screen.getByText('Importar Resúmen desde archivo')).toBeDefined();
  });

  it('fetches templates and currencies on open', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('/api/templates'));
      expect(global.fetch).toHaveBeenCalledWith('/api/catalog/currencies');
    });
  });

  it('renders template list after loading', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(screen.getByText('My Template')).toBeDefined();
      expect(screen.getByText('System Template')).toBeDefined();
    });
  });

  it('pre-selects the default template on open', async () => {
    render(<ImportStatementModal {...defaultProps} defaultTemplateId="tmpl-1" />);
    await waitFor(() => screen.getByText('My Template'));

    const templateRow = screen.getByText('My Template').closest('[role="button"]');
    expect(templateRow?.className).toContain('border-primary');
  });

  it('selects a template when clicked', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    await userEvent.click(screen.getByText('My Template').closest('[role="button"]')!);

    expect(
      screen.getByText('My Template').closest('[role="button"]')?.className
    ).toContain('border-primary');
  });

  it('shows new template form when clicking Nueva plantilla', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('+ Nueva plantilla'));

    await userEvent.click(screen.getByText('+ Nueva plantilla'));

    expect(screen.getByPlaceholderText('d/M/yyyy')).toBeDefined();
  });

  it('hides template form when clicking Cancelar inside the form', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('+ Nueva plantilla'));

    await userEvent.click(screen.getByText('+ Nueva plantilla'));
    expect(screen.getByPlaceholderText('d/M/yyyy')).toBeDefined();

    const cancelButtons = screen.getAllByRole('button', { name: 'Cancelar' });
    await userEvent.click(cancelButtons[0]);
    expect(screen.queryByPlaceholderText('d/M/yyyy')).toBeNull();
  });

  it('shows alert when submitting without a file selected', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    fireEvent.change(screen.getByLabelText('Fecha de Cierre'), {
      target: { value: '2025-06-30' },
    });
    fireEvent.change(screen.getByLabelText('Fecha de Vencimiento'), {
      target: { value: '2025-07-10' },
    });
    await userEvent.click(screen.getByText('My Template').closest('[role="button"]')!);

    fireEvent.submit(getForm());

    expect(global.alert).toHaveBeenCalledWith('Seleccioná un archivo');
  });

  it('shows alert when submitting without a template selected', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    fireEvent.change(screen.getByLabelText('Fecha de Cierre'), {
      target: { value: '2025-06-30' },
    });
    fireEvent.change(screen.getByLabelText('Fecha de Vencimiento'), {
      target: { value: '2025-07-10' },
    });
    const fileInput = screen.getByLabelText('Archivo (.xls, .xlsx, .csv)');
    const file = new File(['col1,col2'], 'test.csv', { type: 'text/csv' });
    fireEvent.change(fileInput, { target: { files: [file] } });

    fireEvent.submit(getForm());

    expect(global.alert).toHaveBeenCalledWith('Seleccioná una plantilla');
  });

  it('calls create-statement and import APIs on successful submit', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    fireEvent.change(screen.getByLabelText('Fecha de Cierre'), {
      target: { value: '2025-06-30' },
    });
    fireEvent.change(screen.getByLabelText('Fecha de Vencimiento'), {
      target: { value: '2025-07-10' },
    });
    const file = new File(['Date,Concept,Amount'], 'test.csv', { type: 'text/csv' });
    fireEvent.change(screen.getByLabelText('Archivo (.xls, .xlsx, .csv)'), {
      target: { files: [file] },
    });
    await userEvent.click(screen.getByText('My Template').closest('[role="button"]')!);

    await act(async () => {
      fireEvent.submit(getForm());
    });

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/statements',
        expect.objectContaining({ method: 'POST' })
      );
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/statements/import',
        expect.objectContaining({ method: 'POST' })
      );
      expect(defaultProps.onHide).toHaveBeenCalled();
      expect(defaultProps.onImported).toHaveBeenCalled();
    });
  });

  it('patches default template after import when selected differs from default', async () => {
    render(<ImportStatementModal {...defaultProps} defaultTemplateId="other-tmpl" />);
    await waitFor(() => screen.getByText('My Template'));

    fireEvent.change(screen.getByLabelText('Fecha de Cierre'), {
      target: { value: '2025-06-30' },
    });
    fireEvent.change(screen.getByLabelText('Fecha de Vencimiento'), {
      target: { value: '2025-07-10' },
    });
    const file = new File(['Date,Concept'], 'test.csv', { type: 'text/csv' });
    fireEvent.change(screen.getByLabelText('Archivo (.xls, .xlsx, .csv)'), {
      target: { files: [file] },
    });
    await userEvent.click(screen.getByText('My Template').closest('[role="button"]')!);

    await act(async () => {
      fireEvent.submit(getForm());
    });

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/credit-cards/card-123/default-import-template',
        expect.objectContaining({ method: 'PATCH' })
      );
    });
  });

  it('shows edit button only for non-system templates', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    expect(screen.queryAllByRole('button', { name: 'Editar' })).toHaveLength(1);
  });

  it('calls delete API and reloads templates when delete clicked', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => screen.getByText('My Template'));

    await userEvent.click(screen.getByRole('button', { name: '×' }));

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/templates',
        expect.objectContaining({ method: 'DELETE' })
      );
    });
  });
});
