import path from 'path';
import { reactRouter } from '@react-router/dev/vite';
import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
  plugins: [reactRouter(), tsconfigPaths()],
  server: {
    port: parseInt(process.env.PORT || '5100'),
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './app'),
    },
  },
  optimizeDeps: {
    // Force Vite to pre-bundle these CJS packages so named exports resolve in the browser
    include: [
      '@opentelemetry/resources',
      '@opentelemetry/sdk-trace-web',
      '@opentelemetry/sdk-trace-base',
      '@opentelemetry/exporter-trace-otlp-http',
      '@opentelemetry/instrumentation-fetch',
      '@opentelemetry/instrumentation-document-load',
      '@opentelemetry/instrumentation',
      '@opentelemetry/semantic-conventions',
      '@opentelemetry/api',
    ],
  },
});
