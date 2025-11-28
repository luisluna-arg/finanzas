import path from 'path';
import tailwindcss from '@tailwindcss/vite';
import { vitePlugin as remix } from '@remix-run/dev';
import { installGlobals } from '@remix-run/node';
import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

installGlobals();

export default defineConfig({
  plugins: [remix(), tsconfigPaths(), tailwindcss()],
  server: {
    port: parseInt(process.env.PORT || '5100'),
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './app'),
    },
  },
});
