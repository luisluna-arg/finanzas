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
    creditCardStatementImportTemplates: {
      endpoint: '/api/templates',
      associate: (id: string) => `/api/templates/${id}/associate`,
    },
    creditCardInstallmentPatterns: { endpoint: '/api/patterns' },
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

const mockPatterns = [
  { id: 'pat-1', name: 'Parens N/M', regexPattern: '(N/M)', isSystem: true },
];

function buildFetchMock() {
  return vi.fn().mockImplementation((url: string) => {
    if (url.startsWith('/api/templates')) {
      return Promise.resolve({
        ok: true,
        text: () => Promise.resolve(''),
        json: () => Promise.resolve(mockTemplates),
      });
    }
    if (url === '/api/patterns') {
      return Promise.resolve({
        ok: true,
        text: () => Promise.resolve(''),
        json: () => Promise.resolve(mockPatterns),
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
    return Promise.resolve({ ok: true, text: () => Promise.resolve(''), json: () => Promise.resolve([]) });
  });
}

const defaultProps = {
  show: true,
  onHide: vi.fn(),
  creditCardId: 'card-123',
  defaultTemplateId: null as string | null,
  statements: [] as import('@/types/creditCard').CreditCardStatement[],
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

async function openCreateTab() {
  await waitFor(() => screen.getByText('My Template'));
  await userEvent.click(screen.getByRole('button', { name: 'Crear nueva' }));
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

  it('fetches templates, currencies, and patterns on open', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(expect.stringContaining('/api/templates'));
      expect(global.fetch).toHaveBeenCalledWith('/api/catalog/currencies');
      expect(global.fetch).toHaveBeenCalledWith('/api/patterns');
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

  it('shows template form when clicking the Crear nueva tab', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await openCreateTab();

    expect(screen.getByPlaceholderText('d/M/yyyy')).toBeDefined();
  });

  it('switches back to the template list when clicking the Seleccionar tab', async () => {
    render(<ImportStatementModal {...defaultProps} />);
    await openCreateTab();
    expect(screen.getByPlaceholderText('d/M/yyyy')).toBeDefined();

    await userEvent.click(screen.getByRole('button', { name: 'Seleccionar' }));
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
    const fileInput = screen.getByLabelText('Archivo (.xls, .xlsx, .csv, .pdf)');
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
    fireEvent.change(screen.getByLabelText('Archivo (.xls, .xlsx, .csv, .pdf)'), {
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
    fireEvent.change(screen.getByLabelText('Archivo (.xls, .xlsx, .csv, .pdf)'), {
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

  describe('installment pattern picker', () => {
    it('lists existing patterns in the picker once loaded', async () => {
      render(<ImportStatementModal {...defaultProps} />);
      await openCreateTab();

      await waitFor(() => {
        expect(screen.getByText('Parens N/M')).toBeDefined();
      });
    });

    it('shows the inline new-pattern form when clicking + Nuevo patrón', async () => {
      render(<ImportStatementModal {...defaultProps} />);
      await openCreateTab();

      await userEvent.click(screen.getByRole('button', { name: '+ Nuevo patrón' }));

      expect(screen.getByRole('button', { name: 'Guardar patrón' })).toBeDefined();
    });

    it('hides the inline new-pattern form when clicking it again (Cancelar)', async () => {
      render(<ImportStatementModal {...defaultProps} />);
      await openCreateTab();

      const toggle = screen.getByRole('button', { name: '+ Nuevo patrón' });
      await userEvent.click(toggle);
      expect(screen.getByRole('button', { name: 'Guardar patrón' })).toBeDefined();

      await userEvent.click(toggle);
      expect(screen.queryByRole('button', { name: 'Guardar patrón' })).toBeNull();
    });

    it('disables the save-pattern button until name and regex are filled', async () => {
      render(<ImportStatementModal {...defaultProps} />);
      await openCreateTab();
      await userEvent.click(screen.getByRole('button', { name: '+ Nuevo patrón' }));

      const saveButton = screen.getByRole('button', { name: 'Guardar patrón' });
      expect(saveButton).toBeDisabled();

      const regexInput = screen.getByPlaceholderText(String.raw`^(?<base>.*?)\s*\((?<n>\d+)/(?<m>\d+)\)\s*$`);
      await userEvent.type(regexInput, '(N/M)');
      expect(saveButton).toBeDisabled();
    });

    it('POSTs a new pattern and selects it on save', async () => {
      global.fetch = vi.fn().mockImplementation((url: string, opts?: RequestInit) => {
        if (url === '/api/patterns' && opts?.method === 'POST') {
          return Promise.resolve({
            ok: true,
            text: () => Promise.resolve(''),
            json: () =>
              Promise.resolve({ id: 'pat-new', name: 'Cuota N de M', regexPattern: 'x', isSystem: false }),
          });
        }
        return buildFetchMock()(url);
      });

      render(<ImportStatementModal {...defaultProps} />);
      await openCreateTab();
      await userEvent.click(screen.getByRole('button', { name: '+ Nuevo patrón' }));

      await userEvent.type(screen.getByLabelText('Nombre'), 'Cuota N de M');
      await userEvent.type(
        screen.getByPlaceholderText(String.raw`^(?<base>.*?)\s*\((?<n>\d+)/(?<m>\d+)\)\s*$`),
        'x'
      );

      await userEvent.click(screen.getByRole('button', { name: 'Guardar patrón' }));

      await waitFor(() => {
        expect(global.fetch).toHaveBeenCalledWith(
          '/api/patterns',
          expect.objectContaining({ method: 'POST' })
        );
      });

      // Inline form closes and the new pattern becomes selected/visible in the picker.
      await waitFor(() => {
        expect(screen.queryByRole('button', { name: 'Guardar patrón' })).toBeNull();
      });
    });
  });
});
