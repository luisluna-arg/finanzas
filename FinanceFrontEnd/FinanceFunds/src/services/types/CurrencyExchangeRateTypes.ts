import type { Currency } from './CurrencyTypes';

/**
 * Represents a currency exchange rate entity
 */
export interface CurrencyExchangeRate {
  id: string;
  baseCurrency: Currency;
  quoteCurrency: Currency;
  buyRate: number;
  sellRate: number;
  timeStamp: string;
  deactivated?: boolean;
}

/**
 * Represents the response from the currency exchange rates API
 */
export type CurrencyExchangeRatesResponse = CurrencyExchangeRate[];

/**
 * Command for creating a new currency exchange rate
 */
export interface CreateCurrencyExchangeRateCommand {
  baseCurrencyId: string;
  quoteCurrencyId: string;
  buyRate: number;
  sellRate: number;
}

/**
 * Command for updating an existing currency exchange rate
 */
export interface UpdateCurrencyExchangeRateCommand {
  id: string;
  baseCurrencyId: string;
  quoteCurrencyId: string;
  buyRate: number;
  sellRate: number;
}

/**
 * Query parameters for getting currency exchange rates
 */
export interface GetCurrencyExchangeRatesQuery {
  quoteCurrencyShortName?: string;
  baseCurrencyShortName?: string;
  pageNumber?: number;
  pageSize?: number;
}
