import { redirect } from 'react-router';
import type { LoaderFunctionArgs } from 'react-router';
import { getUserFromSession } from '@/services/auth/session.server';

export const loader = async ({ request }: LoaderFunctionArgs) => {
  const user = await getUserFromSession(request);
  return redirect(user ? '/funds' : '/auth/login');
};
