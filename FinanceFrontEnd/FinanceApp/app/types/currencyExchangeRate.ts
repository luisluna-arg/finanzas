export interface CurrencyData {
  id: string;
  name: string;
  shortName: string;
}

export interface CurrencyExchangeRateRecord {
  id: string;
  createdAt: string;
  updatedAt: string;
  timeStamp: string;
  deactivated: boolean;
  buyRate: number;
  sellRate: number;
  baseCurrencyId: string;
  quoteCurrencyId: string;
  baseCurrency: CurrencyData;
  quoteCurrency: CurrencyData;
}
