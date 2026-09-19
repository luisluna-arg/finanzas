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
  paymentPlanId?: string | null;
  installmentNumber: number;
}

export interface CreditCardsData {
  creditCards: CreditCard[];
}

export interface CreditCardInstallmentPattern {
  id: string;
  name: string;
  regexPattern: string;
  isSystem: boolean;
  userId?: string | null;
}

export interface CreditCardStatementDraftRow {
  paymentPlanId: string;
  baseConcept: string;
  nextInstallmentNumber: number;
  totalInstallments: number;
  suggestedAmount: number;
  currencyId: string;
  sourceTransactionId: string;
}

export interface CreditCardStatementDraft {
  creditCardId: string;
  currentStatementId: string;
  suggestedClosureDate: string;
  suggestedExpiringDate: string;
  continuingInstallments: CreditCardStatementDraftRow[];
  eligibleNonPlanTransactions: CreditCardTransaction[];
}
