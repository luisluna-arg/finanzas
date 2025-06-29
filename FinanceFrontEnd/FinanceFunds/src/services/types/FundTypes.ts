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
 * Represents the request to create a fund
 */
export interface CreateFundRequest {
  bankId: string;
  currencyId: string;
  timeStamp: string;
  amount: number;
  dailyUse?: boolean;
}

/**
 * Represents the total funds response from the API
 */
export interface TotalFunds {
  items: Fund[];
}

/**
 * Represents the response for a list of funds
 */
export interface FundsResponse {
  items: Fund[];
  totalItems?: number;
  pageNumber?: number;
  pageSize?: number;
  totalPages?: number;
}

/**
 * Represents the response for a single fund
 */
export interface FundResponse {
  item: Fund;
}

/**
 * Parameters for funds API queries
 */
export interface FundsQueryParams {
  dailyUse?: boolean;
  currencyId?: string;
  [key: string]: string | number | boolean | null | undefined;
}
