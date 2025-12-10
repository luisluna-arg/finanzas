import path from 'path';
import tailwindcss from '@tailwindcss/vite';
import { reactRouter } from '@react-router/dev/vite';
import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
  plugins: [reactRouter(), tsconfigPaths(), tailwindcss()],
  server: {
    port: parseInt(process.env.PORT || '5100'),
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './app'),
    },
  },
});
