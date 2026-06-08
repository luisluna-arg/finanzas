import { useState, useEffect, useCallback } from 'react';
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
import type {
  CreditCardStatementImportTemplate,
  StatementImportConfig,
} from '@/types/creditCardStatementImportTemplate';
import type { CatalogItem } from '@/types/catalog';

const DEFAULT_CONFIG: StatementImportConfig = {
  skipRows: 1,
  skipLastRows: 0,
  dateColumn: 0,
  dateFormat: 'd/M/yyyy',
  conceptColumn: 1,
  amountColumn: 2,
  defaultCurrencyId: '',
  decimalSeparator: ',',
  thousandsSeparator: '.',
  amountNegate: false,
};

function TemplateForm({
  initial,
  currencies,
  creditCardId,
  onSave,
  onCancel,
}: {
  initial?: CreditCardStatementImportTemplate | null;
  currencies: CatalogItem[];
  creditCardId?: string;
  onSave: () => void;
  onCancel: () => void;
}) {
  const [name, setName] = useState(initial?.name ?? '');
  const [isSystem, setIsSystem] = useState(initial?.isSystem ?? false);
  const [config, setConfig] = useState<StatementImportConfig>(() => {
    if (initial?.configJson) {
      try {
        return JSON.parse(initial.configJson) as StatementImportConfig;
      } catch {
        return { ...DEFAULT_CONFIG };
      }
    }
    return { ...DEFAULT_CONFIG };
  });
  const [saving, setSaving] = useState(false);

  const updateConfig = <K extends keyof StatementImportConfig>(
    key: K,
    value: StatementImportConfig[K]
  ) => setConfig((prev) => ({ ...prev, [key]: value }));

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const body = { name, isSystem, configJson: JSON.stringify(config) };
      const method = initial ? 'PUT' : 'POST';
      const payload = initial
        ? { id: initial.id, ...body }
        : { ...body, creditCardId: creditCardId ?? null };
      const res = await fetch(String(urls.creditCardStatementImportTemplates.endpoint), {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });
      if (!res.ok) throw new Error(await res.text());
      onSave();
    } catch (err) {
      alert(`Error al guardar plantilla: ${err}`);
    } finally {
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSave} className="space-y-4 border rounded-md p-4 bg-muted/30">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <Label className="block mb-1">Nombre</Label>
          <Input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div className="flex items-end gap-2">
          <label className="flex items-center gap-2 text-sm cursor-pointer">
            <input
              type="checkbox"
              checked={isSystem}
              onChange={(e) => setIsSystem(e.target.checked)}
            />
            Plantilla del sistema
          </label>
        </div>
      </div>
      <Separator />
      <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
        Configuración de columnas (índice 0 = primera columna)
      </p>
      <div className="grid grid-cols-3 gap-3">
        <div>
          <Label className="block mb-1 text-sm">Filas a omitir (inicio)</Label>
          <Input
            type="number"
            min={0}
            value={config.skipRows}
            onChange={(e) => updateConfig('skipRows', Number(e.target.value))}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Filas a omitir (final)</Label>
          <Input
            type="number"
            min={0}
            value={config.skipLastRows}
            onChange={(e) => updateConfig('skipLastRows', Number(e.target.value))}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Col. Fecha</Label>
          <Input
            type="number"
            min={0}
            value={config.dateColumn}
            onChange={(e) => updateConfig('dateColumn', Number(e.target.value))}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Formato fecha</Label>
          <Input
            value={config.dateFormat}
            onChange={(e) => updateConfig('dateFormat', e.target.value)}
            placeholder="d/M/yyyy"
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Col. Concepto</Label>
          <Input
            type="number"
            min={0}
            value={config.conceptColumn}
            onChange={(e) => updateConfig('conceptColumn', Number(e.target.value))}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Col. Monto</Label>
          <Input
            type="number"
            min={0}
            value={config.amountColumn}
            onChange={(e) => updateConfig('amountColumn', Number(e.target.value))}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Moneda por defecto</Label>
          <select
            value={config.defaultCurrencyId}
            onChange={(e) => updateConfig('defaultCurrencyId', e.target.value)}
            className="h-9 w-full text-sm rounded-md border border-input bg-background px-2"
            required
          >
            <option value="">Seleccionar...</option>
            {currencies.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
        <div>
          <Label className="block mb-1 text-sm">Sep. decimal</Label>
          <Input
            value={config.decimalSeparator}
            maxLength={1}
            onChange={(e) => updateConfig('decimalSeparator', e.target.value)}
          />
        </div>
        <div>
          <Label className="block mb-1 text-sm">Sep. miles</Label>
          <Input
            value={config.thousandsSeparator}
            maxLength={1}
            onChange={(e) => updateConfig('thousandsSeparator', e.target.value)}
          />
        </div>
        <div className="flex items-end pb-1">
          <label className="flex items-center gap-2 text-sm cursor-pointer">
            <input
              type="checkbox"
              checked={config.amountNegate}
              onChange={(e) => updateConfig('amountNegate', e.target.checked)}
            />
            Invertir signo
          </label>
        </div>
      </div>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" size="sm" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" size="sm" disabled={saving}>
          {saving ? 'Guardando...' : 'Guardar plantilla'}
        </Button>
      </div>
    </form>
  );
}

export default function ImportStatementModal({
  show,
  onHide,
  creditCardId,
  defaultTemplateId,
  onImported,
}: {
  show: boolean;
  onHide: () => void;
  creditCardId: string;
  defaultTemplateId: string | null;
  onImported: () => void;
}) {
  const [closureDate, setClosureDate] = useState('');
  const [expiringDate, setExpiringDate] = useState('');
  const [templates, setTemplates] = useState<CreditCardStatementImportTemplate[]>([]);
  const [selectedTemplateId, setSelectedTemplateId] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [currencies, setCurrencies] = useState<CatalogItem[]>([]);
  const [showTemplateForm, setShowTemplateForm] = useState(false);
  const [editingTemplate, setEditingTemplate] = useState<CreditCardStatementImportTemplate | null>(null);
  const [otherTemplates, setOtherTemplates] = useState<CreditCardStatementImportTemplate[]>([]);

  const loadTemplates = useCallback(async () => {
    try {
      const cardUrl = `${urls.creditCardStatementImportTemplates.endpoint}?creditCardId=${creditCardId}`;
      const [cardRes, allRes] = await Promise.all([
        fetch(cardUrl),
        fetch(String(urls.creditCardStatementImportTemplates.endpoint)),
      ]);
      const cardData = await cardRes.json();
      const allData = await allRes.json();
      const cardTemplates: CreditCardStatementImportTemplate[] = Array.isArray(cardData) ? cardData : (cardData.items ?? []);
      const allList: CreditCardStatementImportTemplate[] = Array.isArray(allData) ? allData : (allData.items ?? []);
      const cardIds = new Set(cardTemplates.map((t) => t.id));
      setTemplates(cardTemplates);
      setOtherTemplates(allList.filter((t) => !t.isSystem && !cardIds.has(t.id)));
    } catch {
      setTemplates([]);
      setOtherTemplates([]);
    }
  }, [creditCardId]);

  useEffect(() => {
    if (show && defaultTemplateId) {
      setSelectedTemplateId(defaultTemplateId);
    }
  }, [show, defaultTemplateId]);

  useEffect(() => {
    if (!show) return;
    loadTemplates();
    fetch(String(urls.catalog.currencies.endpoint))
      .then((r) => r.json())
      .then((data: CatalogItem[]) => setCurrencies(data))
      .catch(() => {});
  }, [show, loadTemplates]);

  const handleAssociateTemplate = async (templateId: string) => {
    try {
      await fetch(String(urls.creditCardStatementImportTemplates.associate(templateId)), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ creditCardId }),
      });
      await loadTemplates();
      setSelectedTemplateId(templateId);
    } catch (err) {
      alert(`Error al asociar plantilla: ${err}`);
    }
  };

  const handleDeleteTemplate = async (id: string) => {
    if (!confirm('¿Eliminar esta plantilla?')) return;
    try {
      await fetch(String(urls.creditCardStatementImportTemplates.endpoint), {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ids: [id] }),
      });
      await loadTemplates();
      if (selectedTemplateId === id) setSelectedTemplateId('');
    } catch (err) {
      alert(`Error al eliminar plantilla: ${err}`);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file) { alert('Seleccioná un archivo'); return; }
    if (!selectedTemplateId) { alert('Seleccioná una plantilla'); return; }

    setSubmitting(true);
    try {
      const stmtRes = await fetch(String(urls.creditCardStatements.endpoint), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          creditCardId,
          closureDate: new Date(closureDate).toISOString(),
          expiringDate: new Date(expiringDate).toISOString(),
        }),
      });
      if (!stmtRes.ok) throw new Error(`Error creando resúmen: ${await stmtRes.text()}`);
      const stmt = await stmtRes.json();

      const form = new FormData();
      form.append('file', file);
      form.append('templateId', selectedTemplateId);
      form.append('statementId', stmt.id);

      const importRes = await fetch(String(urls.creditCardStatements.import), {
        method: 'POST',
        body: form,
      });
      if (!importRes.ok) throw new Error(`Error importando: ${await importRes.text()}`);

      if (selectedTemplateId !== defaultTemplateId) {
        await fetch(String(urls.creditCards.defaultImportTemplate(creditCardId)), {
          method: 'PATCH',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ templateId: selectedTemplateId }),
        }).catch(() => {});
      }

      onHide();
      onImported();
    } catch (err) {
      alert(`${err}`);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal show={show} onHide={onHide} size="4xl">
      <form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>
            <h3 className="text-lg font-medium">Importar Resúmen desde archivo</h3>
          </ModalTitle>
        </ModalHeader>
        <ModalBody>
          <div className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="import-closure-date" className="block mb-2">
                  Fecha de Cierre
                </Label>
                <Input
                  id="import-closure-date"
                  type="date"
                  required
                  value={closureDate}
                  onChange={(e) => setClosureDate(e.target.value)}
                />
              </div>
              <div>
                <Label htmlFor="import-expiring-date" className="block mb-2">
                  Fecha de Vencimiento
                </Label>
                <Input
                  id="import-expiring-date"
                  type="date"
                  required
                  value={expiringDate}
                  onChange={(e) => setExpiringDate(e.target.value)}
                />
              </div>
            </div>

            <Separator />

            <div>
              <div className="flex items-center justify-between mb-3">
                <p className="text-sm font-medium">Plantilla de importación</p>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={() => { setEditingTemplate(null); setShowTemplateForm(true); }}
                >
                  + Nueva plantilla
                </Button>
              </div>

              {showTemplateForm && (
                <div className="mb-4">
                  <TemplateForm
                    initial={editingTemplate}
                    currencies={currencies}
                    creditCardId={creditCardId}
                    onSave={async () => { await loadTemplates(); setShowTemplateForm(false); setEditingTemplate(null); }}
                    onCancel={() => { setShowTemplateForm(false); setEditingTemplate(null); }}
                  />
                </div>
              )}

              <div className="space-y-1">
                {templates.length === 0 && (
                  <p className="text-sm text-muted-foreground">
                    No hay plantillas disponibles. Creá una nueva.
                  </p>
                )}
                {templates.map((t) => (
                  <div
                    key={t.id}
                    role="button"
                    tabIndex={0}
                    className={`flex items-center justify-between px-3 py-2 rounded-md border cursor-pointer text-sm transition-colors ${
                      selectedTemplateId === t.id
                        ? 'border-primary bg-primary/10'
                        : 'hover:bg-muted'
                    }`}
                    onClick={() => setSelectedTemplateId(t.id)}
                    onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') setSelectedTemplateId(t.id); }}
                  >
                    <div className="flex items-center gap-2">
                      <span>{t.name}</span>
                      {t.isSystem && (
                        <span className="text-xs bg-muted px-1.5 py-0.5 rounded">Sistema</span>
                      )}
                    </div>
                    {!t.isSystem && (
                      <div className="flex gap-1">
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="h-6 px-2 text-xs"
                          onClick={(e) => {
                            e.stopPropagation();
                            setEditingTemplate(t);
                            setShowTemplateForm(true);
                          }}
                        >
                          Editar
                        </Button>
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="h-6 px-2 text-xs text-destructive hover:text-destructive"
                          onClick={(e) => { e.stopPropagation(); handleDeleteTemplate(t.id); }}
                        >
                          ×
                        </Button>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>

            {otherTemplates.length > 0 && (
              <div>
                <p className="text-sm font-medium mb-2 text-muted-foreground">
                  Otras plantillas disponibles
                </p>
                <div className="space-y-1">
                  {otherTemplates.map((t) => (
                    <div
                      key={t.id}
                      className="flex items-center justify-between px-3 py-2 rounded-md border text-sm bg-muted/20"
                    >
                      <span>{t.name}</span>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        className="h-6 px-2 text-xs"
                        onClick={() => handleAssociateTemplate(t.id)}
                      >
                        Usar
                      </Button>
                    </div>
                  ))}
                </div>
              </div>
            )}

            <Separator />

            <div>
              <Label htmlFor="import-file" className="block mb-2">
                Archivo (.xls, .xlsx, .csv)
              </Label>
              <Input
                id="import-file"
                type="file"
                accept=".xls,.xlsx,.csv"
                required
                onChange={(e) => setFile(e.target.files?.[0] ?? null)}
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
              {submitting ? 'Importando...' : 'Importar'}
            </Button>
          </div>
        </ModalFooter>
      </form>
    </Modal>
  );
}
