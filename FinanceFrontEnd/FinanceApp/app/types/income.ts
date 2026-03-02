export interface BankData {
  id: string;
  name: string;
  deactivated: boolean;
  creditCards: unknown[];
}

export interface CurrencyData {
  id: string;
  name: string;
  shortName: string;
  deactivated: boolean;
  baseExchangeRates: unknown[];
  quoteExchangeRates: unknown[];
  iolInvestmentAssets: unknown[];
  subscriptions: unknown[];
  symbols: unknown[];
}

export interface IncomeRecord {
  id: string;
  createdAt: string;
  updatedAt: string;
  timeStamp: string;
  deactivated: boolean;
  amount: number;
  bankId: string;
  currencyId: string;
  bank: BankData;
  currency: CurrencyData;
}
