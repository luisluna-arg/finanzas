import { Agent } from "https";
import { CreditCardQuery } from "./CreditCardQuery";
import { CurrencyExchangeRatesQuery } from "./CurrencyExchangeRatesQuery";

const httpsAgent = new Agent({ rejectUnauthorized: false });

export class BackendClient {
  CreditCardsQuery: CreditCardQuery = new CreditCardQuery(httpsAgent);
  CurrencyExchangeRatesQuery: CurrencyExchangeRatesQuery = new CurrencyExchangeRatesQuery(httpsAgent);

  BackendClient() {
  }
}
