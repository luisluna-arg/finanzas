import type { LoaderFunctionArgs } from 'react-router';
import { redirect } from 'react-router';
import { authenticator } from '@/services/auth/auth.server';
import { AuthConstants } from '@/services/auth/auth.constants';
import { createUserSession } from '@/services/auth/session.server';
import SafeLogger from '@/utils/SafeLogger';

export async function loader({ request }: LoaderFunctionArgs) {
  try {
    const user = await authenticator.authenticate(AuthConstants.PROVIDER, request);
    return createUserSession(user, '/funds');
  } catch (error) {
    if (error instanceof Response) {
      throw error;
    }
    SafeLogger.error('[auth.callback] Authentication failed:', error);
    return redirect('/auth/login?error=callback_failed');
  }
}
