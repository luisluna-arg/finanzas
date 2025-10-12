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
  },
});
