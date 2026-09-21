/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_USD_CURRENCY_ID?: string;
  readonly VITE_ARS_CURRENCY_ID?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
