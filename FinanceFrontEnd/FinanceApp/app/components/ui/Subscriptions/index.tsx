import { useState } from 'react';
import { useLoaderData, useRevalidator } from 'react-router';
import type { Subscription, SubscriptionsData } from '@/types/subscription';
import { SubscriptionFrequency } from '@/types/subscription';
import type { CatalogItem } from '@/types/catalog';
import {
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/shadcn/table';
import { toNumber, ValueLike } from '@/utils/common';
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  ModalFooter,
} from '@/components/ui/utils/Modal';
import { Input } from '@/components/ui/shadcn/input';
import { Button } from '@/components/ui/shadcn/button';
import { Label } from '@/components/ui/shadcn/label';
import { Separator } from '@/components/ui/shadcn/separator';
import { cn } from '@/lib/utils';
import urls from '@/utils/urls';

interface SubscriptionModalProps {
  show: boolean;
  onHide: () => void;
  subscription?: Subscription | null;
  currencies: CatalogItem[];
  frequencies: Array<{ id: number; name: string }>;
  onSave: (data: Partial<Subscription>) => void;
}

function SubscriptionModal({
  show,
  onHide,
  subscription,
  currencies,
  frequencies,
  onSave,
}: SubscriptionModalProps) {
  const [formData, setFormData] = useState<Partial<Subscription>>({
    name: subscription?.name || '',
    price: subscription?.price || 0,
    frequency: subscription?.frequency || 'monthly',
    currencyId: subscription?.currencyId || currencies?.[0]?.id || '',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      ...formData,
      ...(subscription?.id ? { id: subscription.id } : {}),
    });
  };

  return (
    <Modal show={show} onHide={onHide}>
      <form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>
            <h3 className="text-lg font-medium">
              {subscription ? 'Editar Suscripción' : 'Nueva Suscripción'}
            </h3>
          </ModalTitle>
        </ModalHeader>
        <ModalBody>
          <div className="space-y-4">
            <div>
              <Label htmlFor="subscription-name" className="block mb-2">
                Nombre
              </Label>
              <Input
                id="subscription-name"
                type="text"
                required
                value={formData.name}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    name: e.target.value,
                  })
                }
                autoComplete="off"
                spellCheck={false}
                data-gramm="false"
                data-gramm_editor="false"
                data-enable-grammarly="false"
                data-1p-ignore="true"
                data-lpignore="true"
                data-form-type="other"
                role="textbox"
              />
            </div>
            <div>
              <Label htmlFor="subscription-price" className="block mb-2">
                Precio
              </Label>
              <Input
                id="subscription-price"
                type="number"
                step="0.01"
                required
                value={formData.price}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    price: parseFloat(e.target.value),
                  })
                }
              />
            </div>
            <div>
              <Label htmlFor="subscription-frequency" className="block mb-2">
                Frecuencia
              </Label>
              <select
                id="subscription-frequency"
                required
                value={String(formData.frequency ?? '')}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    frequency: e.target.value.toLowerCase() as 'monthly' | 'annual',
                  })
                }
                className={cn(
                  'flex h-9 w-full rounded-md border border-gray-200 bg-transparent px-3 py-1 text-base shadow-sm transition-colors',
                  'focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-gray-950',
                  'disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer'
                )}
              >
                {frequencies.map((frequency) => (
                  <option key={frequency.id} value={frequency.name}>
                    {frequency.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <Label htmlFor="subscription-currency" className="block mb-2">
                Moneda
              </Label>
              <select
                id="subscription-currency"
                required
                value={formData.currencyId}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    currencyId: e.target.value,
                  })
                }
                className={cn(
                  'flex h-9 w-full rounded-md border border-gray-200 bg-transparent px-3 py-1 text-base shadow-sm transition-colors',
                  'focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-gray-950',
                  'disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer'
                )}
              >
                {currencies.map((currency) => (
                  <option key={currency.id} value={currency.id}>
                    {currency.name}
                  </option>
                ))}
              </select>
            </div>
          </div>
        </ModalBody>
        <ModalFooter>
          <div className="flex justify-end space-x-2 mt-6">
            <Button type="button" variant="outline" onClick={onHide}>
              Cancelar
            </Button>
            <Button type="submit">Guardar</Button>
          </div>
        </ModalFooter>
      </form>
    </Modal>
  );
}

function getFrequencyName(frequency: SubscriptionFrequency | string | number): string {
  if (typeof frequency === 'string') {
    // Return Spanish translation for display
    return frequency.toLowerCase() === 'monthly' ? 'Mensual' : 'Anual';
  }
  if (typeof frequency === 'number') {
    return frequency === 0 ? 'Mensual' : 'Anual';
  }
  if (frequency && typeof frequency === 'object' && 'name' in frequency) {
    const name = String((frequency as { name: unknown }).name).toLowerCase();
    return name === 'monthly' ? 'Mensual' : 'Anual';
  }
  return 'Desconocido';
}

function Subscriptions() {
  const { subscriptions, currencies, frequencies } = useLoaderData<SubscriptionsData>();
  const [showModal, setShowModal] = useState(false);
  const [editingSubscription, setEditingSubscription] = useState<Subscription | null>(null);
  const revalidator = useRevalidator();

  const allItems = subscriptions?.items ?? [];

  const isMonthly = (s: Subscription) => {
    if (typeof s.frequency === 'string') {
      return (
        s.frequency.toLowerCase() === 'monthly' || s.frequency === SubscriptionFrequency.Monthly
      );
    }
    if (typeof s.frequency === 'number') {
      return s.frequency === 0;
    }
    return false;
  };
  const isAnnual = (s: Subscription) => {
    if (typeof s.frequency === 'string') {
      return s.frequency.toLowerCase() === 'annual' || s.frequency === SubscriptionFrequency.Annual;
    }
    if (typeof s.frequency === 'number') {
      return s.frequency === 1;
    }
    return false;
  };

  const monthlySubscriptions = allItems.filter(isMonthly);
  const annualSubscriptions = allItems.filter(isAnnual);

  // Group monthly by currency
  const monthlyCurrencyGroups: Record<string, Subscription[]> = {};
  for (const sub of monthlySubscriptions) {
    const key = sub.currency?.shortName ?? 'Unknown';
    if (!monthlyCurrencyGroups[key]) monthlyCurrencyGroups[key] = [];
    monthlyCurrencyGroups[key].push(sub);
  }

  const handleAdd = () => {
    setEditingSubscription(null);
    setShowModal(true);
  };

  const handleEdit = (subscription: Subscription) => {
    setEditingSubscription(subscription);
    setShowModal(true);
  };

  const handleDelete = async (subscription: Subscription) => {
    if (!confirm(`¿Estás seguro de que deseas eliminar la suscripción "${subscription.name}"?`)) {
      return;
    }

    try {
      const response = await fetch(String(urls.subscriptions.endpoint), {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ids: [subscription.id] }),
      });

      if (response.ok) {
        revalidator.revalidate();
      } else {
        const error = await response.text();
        alert(`Error al eliminar suscripción: ${error}`);
      }
    } catch (error) {
      alert(`Error al eliminar suscripción: ${error}`);
    }
  };

  const handleSave = async (data: Partial<Subscription>) => {
    const endpoint = String(urls.subscriptions.endpoint);
    const method = editingSubscription ? 'PUT' : 'POST';

    const payload = editingSubscription
      ? {
          subscriptionId: editingSubscription.id,
          name: data.name,
          price: data.price,
          frequency: data.frequency,
          currencyId: data.currencyId,
        }
      : {
          name: data.name,
          price: data.price,
          frequency: data.frequency,
          currencyId: data.currencyId,
        };

    try {
      const response = await fetch(endpoint, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        setShowModal(false);
        setEditingSubscription(null);
        revalidator.revalidate();
      } else {
        const error = await response.text();
        alert(`Error al guardar suscripción: ${error}`);
      }
    } catch (error) {
      alert(`Error al guardar suscripción: ${error}`);
    }
  };

  const SubscriptionTable = ({
    subs,
    title,
    showTotal = false,
    showCurrency = true,
    showFrequency = false,
    showPaymentDate = false,
  }: {
    subs: Subscription[];
    title: string;
    showTotal?: boolean;
    showCurrency?: boolean;
    showFrequency?: boolean;
    showPaymentDate?: boolean;
  }) => {
    const total = subs.reduce((acc, sub) => acc + sub.price, 0);

    return (
      <div className="mb-10">
        {title && <h2 className="text-lg font-medium mb-2">{title}</h2>}
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-auto">Nombre</TableHead>
              <TableHead className="text-right w-32">Precio</TableHead>
              {showCurrency && <TableHead className="w-32">Moneda</TableHead>}
              {showFrequency && <TableHead className="w-32">Frecuencia</TableHead>}
              {showPaymentDate && <TableHead className="w-32">Fecha de Pago</TableHead>}
              <TableHead className="text-center w-32">Acciones</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {subs.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={
                    3 + (showCurrency ? 1 : 0) + (showFrequency ? 1 : 0) + (showPaymentDate ? 1 : 0)
                  }
                  className="text-center text-gray-500"
                >
                  No se encontraron suscripciones
                </TableCell>
              </TableRow>
            ) : (
              subs.map((sub) => (
                <TableRow key={sub.id}>
                  <TableCell>{sub.name}</TableCell>
                  <TableCell className="text-right">
                    {toNumber(sub.price as ValueLike, 0)}
                  </TableCell>
                  {showCurrency && <TableCell>{sub.currency?.shortName ?? '-'}</TableCell>}
                  {showFrequency && <TableCell>{getFrequencyName(sub.frequency)}</TableCell>}
                  {showPaymentDate && <TableCell>{sub.paymentDate ?? 'N/D'}</TableCell>}
                  <TableCell className="text-center">
                    <div className="flex justify-center space-x-2">
                      <Button
                        onClick={() => handleEdit(sub)}
                        variant="outline"
                        size="sm"
                        className="border-primary text-primary hover:bg-primary/10"
                      >
                        Edit
                      </Button>
                      <Button
                        onClick={() => handleDelete(sub)}
                        variant="outline"
                        size="sm"
                        className="border-destructive text-destructive hover:bg-destructive/10"
                      >
                        Delete
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
          {showTotal && subs.length > 0 && (
            <TableFooter>
              <TableRow>
                <TableCell className="font-medium">Total</TableCell>
                <TableCell className="text-right font-medium">
                  {toNumber(total as ValueLike, 0)}
                </TableCell>
                <TableCell></TableCell>
                <TableCell></TableCell>
              </TableRow>
            </TableFooter>
          )}
        </Table>
      </div>
    );
  };

  return (
    <div className={cn(['py-10', 'px-40'])}>
      <div className="row flex justify-between items-center mb-4">
        <h1 className="text-base font-medium">Suscripciones</h1>
        <Button
          onClick={handleAdd}
          variant="outline"
          className="border-primary text-primary hover:bg-primary/10"
        >
          Agregar Suscripción
        </Button>
      </div>
      <Separator className="my-5" />

      {Object.keys(monthlyCurrencyGroups).length > 0 && (
        <div className="mb-10">
          <h2 className="text-lg font-medium mb-4">Suscripciones Mensuales</h2>
          {Object.entries(monthlyCurrencyGroups).map(([currency, subs]) => (
            <div key={`monthly-${currency}`} className="mb-6">
              <h3 className="text-sm font-semibold uppercase tracking-wide text-gray-600 mb-3 px-2">
                {currency}
              </h3>
              <SubscriptionTable subs={subs} title="" showTotal={true} showCurrency={false} />
            </div>
          ))}
        </div>
      )}

      <SubscriptionTable
        subs={annualSubscriptions}
        title="Suscripciones Anuales"
        showPaymentDate={true}
      />

      <SubscriptionModal
        show={showModal}
        onHide={() => {
          setShowModal(false);
          setEditingSubscription(null);
        }}
        subscription={editingSubscription}
        currencies={currencies}
        frequencies={frequencies}
        onSave={handleSave}
      />
    </div>
  );
}

export default Subscriptions;
