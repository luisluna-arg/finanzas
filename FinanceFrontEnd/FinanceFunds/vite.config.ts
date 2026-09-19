import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';
import { resolve } from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
    },
  },
  server: {
    port: parseInt(process.env.PORT || '5200'),
    host: '0.0.0.0',
  },
  preview: {
    port: parseInt(process.env.PORT || '5200'),
    host: '0.0.0.0',
    // Vite rejects any request Host header that isn't localhost-like unless
    // explicitly allowed. Behind Traefik, requests arrive with the real
    // public domain, so allow it (and its www subdomain) when known.
    allowedHosts: process.env.APP_DOMAIN
      ? [process.env.APP_DOMAIN, `www.${process.env.APP_DOMAIN}`]
      : undefined,
  },
});
