import type { BankData, CurrencyData } from './income';

export interface FundRecord {
  id: string;
  createdAt: string;
  updatedAt: string;
  timeStamp: string;
  deactivated: boolean;
  amount: number;
  bankId: string;
  currencyId: string;
  dailyUse: boolean;
  bank: BankData;
  currency: CurrencyData;
}
