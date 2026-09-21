import type { ActionFunctionArgs, LoaderFunctionArgs } from 'react-router';
import { authenticator } from '@/services/auth/auth.server';
import { AuthConstants } from '@/services/auth/auth.constants';
import { createUserSession } from '@/services/auth/session.server';

export async function loader({ request }: LoaderFunctionArgs) {
  const user = await authenticator.authenticate(AuthConstants.PROVIDER, request);
  return createUserSession(user, '/funds');
}

export const action = ({ request }: ActionFunctionArgs) => {
  return authenticator.authenticate(AuthConstants.PROVIDER, request);
};
