import type { CatalogItem } from '@/types/catalog';

export enum SubscriptionFrequency {
  Monthly = 'monthly',
  Annual = 'annual',
}

export type SubscriptionFrequencyName = 'monthly' | 'annual';

export interface Currency {
  id: string;
  name: string;
  shortName: string;
}

export interface Subscription {
  id: string;
  name: string;
  price: number;
  frequency: SubscriptionFrequency | SubscriptionFrequencyName;
  currencyId: string;
  currency?: Currency;
  paymentDate?: string | null;
}

export interface SubscriptionsData {
  subscriptions: {
    items: Subscription[];
    totalCount: number;
  };
  currencies: CatalogItem[];
  frequencies: Array<{
    id: number;
    name: string;
  }>;
}
