import type { Config } from '@react-router/dev/config';

export default {
  // FinanceFunds keeps its existing `src/` layout instead of the RR7 default `app/`.
  appDirectory: 'src',

  // Ensure .server files are treated as server-only, so Redis/Auth0 secrets
  // and other server-only code are never bundled into the client.
  serverModuleFormat: 'esm',
} satisfies Config;
