import type { ActionFunctionArgs, LoaderFunctionArgs } from 'react-router';
import { authenticator } from '@/services/auth/auth.server';
import { AuthConstants } from '@/services/auth/auth.constants';
import serverLogger from '@/utils/logger.server';
import { Button } from '@/components/ui/shadcn/button';
import CustomButton from '@/components/ui/utils/CustomButton';

export async function action({ request }: ActionFunctionArgs) {
  return authenticator.authenticate(AuthConstants.PROVIDER, request);
}

export async function loader({ request }: LoaderFunctionArgs) {
  serverLogger.info('Login loader called');

  // Check if user is already authenticated
  const { getUserFromSession } = await import('@/services/auth/session.server');
  const user = await getUserFromSession(request);

  if (user) {
    // User is already logged in, redirect to dashboard
    const { redirect } = await import('react-router');
    return redirect('/dashboard');
  }

  // Just return empty object, don't auto-authenticate
  return {};
}

export default function Login() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Finance App</h1>
          <p className="mt-2 text-sm text-gray-600">Sign in to your account</p>
        </div>

        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <form action="/auth/auth0" method="post" className="space-y-6">
            <div>
              <CustomButton
                type="submit"
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Login with Auth0
              </CustomButton>
            </div>

            <div className="text-center">
              <p className="text-sm text-gray-600">
                Click the button above to authenticate securely
              </p>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
