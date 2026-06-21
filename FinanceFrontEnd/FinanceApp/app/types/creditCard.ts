export interface CreditCard {
  id: string;
  bankId: string;
  bank?: {
    id: string;
    name: string;
  };
  name: string;
  recordCount: number;
  creditCardStatement?: CreditCardStatement;
  deactivated: boolean;
  defaultImportTemplateId?: string | null;
}

export interface CreditCardStatement {
  id: string;
  creditCardId: string;
  closureDate: string;
  expiringDate: string;
  deactivated: boolean;
}

export interface CreditCardTransaction {
  id: string;
  creditCardId: string;
  timestamp: string;
  concept: string;
  amount: number;
  convertedAmount: number;
  currencyId: string;
  currency?: { id: string; name: string; shortName: string; defaultSymbol?: string };
  amountDollars?: number;
  reference?: string;
}

export interface CreditCardsData {
  creditCards: CreditCard[];
}
