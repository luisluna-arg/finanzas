export interface StatementImportConfig {
  skipRows: number;
  skipLastRows: number;
  dateColumn: number;
  dateFormat: string;
  conceptColumn: number;
  amountColumn: number;
  defaultCurrencyId: string;
  decimalSeparator: string;
  thousandsSeparator: string;
  amountNegate: boolean;
  installmentsColumn?: number;
  installmentsPattern?: string;
}

export interface CreditCardStatementImportTemplate {
  id: string;
  name: string;
  isSystem: boolean;
  userId: string | null;
  configJson: string;
  deactivated: boolean;
}
