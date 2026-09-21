import { defineConfig } from 'vite';
import { reactRouter } from '@react-router/dev/vite';
import { resolve } from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [reactRouter()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
    },
  },
  server: {
    port: parseInt(process.env.PORT || '5200'),
    host: '0.0.0.0',
  },
});
