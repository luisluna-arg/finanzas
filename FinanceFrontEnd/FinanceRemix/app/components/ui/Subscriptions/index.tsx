import { useState } from 'react';
import { useFetcher, useLoaderData, useRevalidator } from '@remix-run/react';
import Table, { TableColumn } from '@/components/ui/utils/Table';
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

interface Subscription {
  id: string;
  name: string;
  price: number;
  frequency: 'Monthly' | 'Annual';
  currencyId: string;
  currency?: {
    id: string;
    name: string;
    shortName: string;
  };
}

interface SubscriptionsData {
  subscriptions: {
    items: Subscription[];
    totalCount: number;
  };
  currencies: Array<{
    id: string;
    name: string;
    shortName: string;
  }>;
  frequencies: Array<{
    id: string;
    name: string;
  }>;
}

interface SubscriptionModalProps {
  show: boolean;
  onHide: () => void;
  subscription?: Subscription | null;
  currencies: Array<{ id: string; name: string; shortName: string }>;
  frequencies: Array<{ id: string; name: string }>;
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
    frequency: subscription?.frequency || 'Monthly',
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
              {subscription ? 'Edit Subscription' : 'New Subscription'}
            </h3>
          </ModalTitle>
        </ModalHeader>
        <ModalBody>
          <div className="space-y-4">
            <div>
              <Label htmlFor="subscription-name" className="block mb-2">
                Name
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
                Price
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
                Frequency
              </Label>
              <select
                id="subscription-frequency"
                required
                value={formData.frequency}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    frequency: e.target.value as 'Monthly' | 'Annual',
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
                Currency
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
                    {currency.name} ({currency.shortName})
                  </option>
                ))}
              </select>
            </div>
          </div>
        </ModalBody>
        <ModalFooter>
          <div className="flex justify-end space-x-2 mt-6">
            <Button type="button" variant="outline" onClick={onHide}>
              Cancel
            </Button>
            <Button type="submit">Save</Button>
          </div>
        </ModalFooter>
      </form>
    </Modal>
  );
}

function Subscriptions() {
  const { subscriptions, currencies, frequencies } = useLoaderData<SubscriptionsData>();
  const [showModal, setShowModal] = useState(false);
  const [editingSubscription, setEditingSubscription] = useState<Subscription | null>(null);
  const fetcher = useFetcher();
  const revalidator = useRevalidator();

  const handleAdd = () => {
    setEditingSubscription(null);
    setShowModal(true);
  };

  const handleEdit = (subscription: Subscription) => {
    setEditingSubscription(subscription);
    setShowModal(true);
  };

  const handleDelete = async (subscription: Subscription) => {
    if (!confirm(`Are you sure you want to delete subscription "${subscription.name}"?`)) {
      return;
    }

    try {
      const response = await fetch(urls.proxy(urls.subscriptions.endpoint), {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ ids: [subscription.id] }),
      });

      if (response.ok) {
        revalidator.revalidate();
      } else {
        const error = await response.text();
        alert(`Error deleting subscription: ${error}`);
      }
    } catch (error) {
      alert(`Error deleting subscription: ${error}`);
    }
  };

  const handleSave = async (data: Partial<Subscription>) => {
    const endpoint = urls.proxy(urls.subscriptions.endpoint);
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
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        setShowModal(false);
        setEditingSubscription(null);
        revalidator.revalidate();
      } else {
        const error = await response.text();
        alert(`Error saving subscription: ${error}`);
      }
    } catch (error) {
      alert(`Error saving subscription: ${error}`);
    }
  };

  const TableColumns: Array<TableColumn> = [
    {
      id: 'name',
      label: 'Name',
      className: 'w-auto',
      mapper: (record: unknown) => {
        if (typeof record === 'object' && record !== null) {
          const r = record as Record<string, unknown>;
          return (r.name as string) ?? '-';
        }
        return '-';
      },
    },
    {
      id: 'price',
      label: 'Price',
      className: 'text-right w-32',
      headerClassName: 'text-right',
      mapper: (d: unknown) =>
        toNumber(
          (d && typeof d === 'object' ? (d as Record<string, unknown>).price : d) as ValueLike,
          0
        ),
    },
    {
      id: 'currency',
      label: 'Currency',
      className: 'w-32',
      mapper: (record: unknown) => {
        if (typeof record === 'object' && record !== null) {
          const r = record as Record<string, unknown>;
          const currency = r.currency as Record<string, unknown> | undefined;
          return currency?.shortName ? (currency.shortName as string) : '-';
        }
        return '-';
      },
    },
    {
      id: 'frequency',
      label: 'Frequency',
      className: 'w-32',
      mapper: (record: unknown) => {
        if (typeof record === 'object' && record !== null) {
          const r = record as Record<string, unknown>;
          return (r.frequency as string) ?? '-';
        }
        return '-';
      },
    },
    {
      id: 'actions',
      label: 'Actions',
      className: 'text-center w-32',
      headerClassName: 'text-center',
      mapper: (record: unknown) => {
        const subscription = record as Subscription;
        return (
          <div className="flex justify-center space-x-2">
            <Button
              onClick={() => handleEdit(subscription)}
              variant="outline"
              size="sm"
              className="border-primary text-primary hover:bg-primary/10"
            >
              Edit
            </Button>
            <Button
              onClick={() => handleDelete(subscription)}
              variant="outline"
              size="sm"
              className="border-destructive text-destructive hover:bg-destructive/10"
            >
              Delete
            </Button>
          </div>
        );
      },
    },
  ];

  return (
    <div className={cn(['py-10', 'px-40'])}>
      <div className="row flex justify-between items-center mb-4">
        <h1 className="text-base font-medium">Subscriptions</h1>
        <Button
          onClick={handleAdd}
          variant="outline"
          className="border-primary text-primary hover:bg-primary/10"
        >
          Add Subscription
        </Button>
      </div>
      <Separator className="my-5" />
      <Table columns={TableColumns} data={subscriptions?.items ?? []} />

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
