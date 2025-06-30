/**
 * Currency constants and configuration
 */

// Default currency IDs - these should match the GUIDs from your backend
const DEFAULT_USD_ID = 'efbf50bc-34d4-43e9-96f9-9f6213ea11b5';
const DEFAULT_ARS_ID = '6d189135-7040-45a1-b713-b1aa6cad1720';

/**
 * Currency IDs that can be overridden by environment variables
 */
export const CURRENCY_IDS = {
  USD: import.meta.env.VITE_USD_CURRENCY_ID || DEFAULT_USD_ID,
  ARS: import.meta.env.VITE_ARS_CURRENCY_ID || DEFAULT_ARS_ID,
} as const;

/**
 * Currencies that should use limited decimal places (2 instead of 8)
 */
export const LIMITED_DECIMAL_CURRENCIES = [CURRENCY_IDS.USD, CURRENCY_IDS.ARS];

/**
 * Check if a currency should use limited decimal places
 */
export const shouldUseLimitedDecimals = (currencyId?: string): boolean => {
  return currencyId ? LIMITED_DECIMAL_CURRENCIES.includes(currencyId) : false;
};

/**
 * Get the maximum number of decimal places for a currency
 */
export const getMaxDecimals = (currencyId?: string): number => {
  return shouldUseLimitedDecimals(currencyId) ? 2 : 8;
};

/**
 * Export individual currency IDs for convenience
 */
export const { USD: USD_CURRENCY_ID, ARS: ARS_CURRENCY_ID } = CURRENCY_IDS;
