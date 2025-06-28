/**
 * Represents a fund item in the summary
 */
export interface Fund {
  id: string;
  label: string;
  value: number;
  baseCurrency: string;
  baseCurrencySymbol: string;
  baseCurrencyId: string;
  quoteCurrencyValue: number;
  defaultCurrency: string;
  defaultCurrencySymbol: string;
  defaultCurrencyId: string;
}

/**
 * Represents the total funds response from the API
 */
export interface TotalFunds {
  items: Fund[];
}

/**
 * Parameters for funds API queries
 */
export interface FundsQueryParams {
  dailyUse?: boolean;
  currencyId?: string;
}
