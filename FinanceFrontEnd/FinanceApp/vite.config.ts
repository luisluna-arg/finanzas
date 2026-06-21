import path from 'path';
import { reactRouter } from '@react-router/dev/vite';
import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig(({ mode }) => ({
  plugins: mode === 'test' ? [tsconfigPaths()] : [reactRouter(), tsconfigPaths()],
  server: {
    port: parseInt(process.env.PORT || '5100'),
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './app'),
    },
  },
  optimizeDeps: {
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
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./tests/setup.ts'],
    include: ['tests/**/*.{test,spec}.{ts,tsx}'],
    alias: {
      '@': path.resolve(__dirname, './app'),
    },
  },
}));
