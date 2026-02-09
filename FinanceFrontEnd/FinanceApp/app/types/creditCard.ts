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
  amountDollars?: number;
  reference?: string;
}

export interface CreditCardsData {
  creditCards: CreditCard[];
}
