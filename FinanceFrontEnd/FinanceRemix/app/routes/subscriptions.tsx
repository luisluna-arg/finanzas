import { LoaderFunctionArgs } from '@remix-run/node';
import Subscriptions from '@/components/ui/Subscriptions';
import { getBackendClient } from '@/data/getBackendClient';
import { PaginatedQueryFilters } from '@/data/base/BasePaginatedQuery';
import { requireAuth } from '@/services/auth/session.server';

interface SubscriptionFilters {
  IncludeDeactivated?: boolean;
}

interface PaginatedSubscriptionFilters extends PaginatedQueryFilters, SubscriptionFilters {}

export const meta = () => {
  return [
    {
      title: 'Subscriptions',
      description: 'Manage your subscriptions',
    },
  ];
};

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await requireAuth(request);

  if (!user.accessToken) {
    throw new Error('No access token available');
  }

  const client = await getBackendClient(user.accessToken!);

  const subscriptions = await client
    .GetPaginatedSubscriptionsQuery()
    .getPaginated<PaginatedSubscriptionFilters>({
      PageSize: 100,
      Page: 1,
    });

  const currencies = await client.GetCurrenciesQuery().get();
  const frequencies = await client.GetFrequenciesQuery().get();

  return {
    subscriptions,
    currencies,
    frequencies,
  };
};

export default Subscriptions;
