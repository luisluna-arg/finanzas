import type { RouteConfig } from '@react-router/dev/routes';
import { index, route } from '@react-router/dev/routes';

export default [
  index('routes/index.tsx'),
  route('funds', 'pages/FundsDashboard.tsx'),
  route('exchange-rates', 'pages/CurrencyExchangeDashboard.tsx'),
  route('auth/login', 'routes/auth.login.tsx'),
  route('auth/auth0', 'routes/auth.auth0.tsx'),
  route('auth/callback', 'routes/auth.callback.tsx'),
  route('auth/logout', 'routes/auth.logout.tsx'),
  route('api/proxy', 'routes/api.proxy.ts'),
  route('*', 'routes/catch-all.tsx'),
] satisfies RouteConfig;
