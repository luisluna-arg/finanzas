/**
 * Represents a currency entity
 */
export interface Currency {
  id: string;
  name: string;
  shortName: string;
  symbols?: CurrencySymbol[];
  deactivated?: boolean;
}

/**
 * Represents a currency symbol
 */
export interface CurrencySymbol {
  id: string;
  symbol: string;
}

/**
 * Represents the response from the currencies API
 * Note: The API returns an array directly, not wrapped in an object
 */
export type CurrenciesResponse = Currency[];
