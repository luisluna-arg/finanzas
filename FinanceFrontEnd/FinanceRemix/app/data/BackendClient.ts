import { Agent } from "https";
import { BanksQuery } from "./BanksQuery";
import { CreditCardQuery } from "./CreditCardQuery";
import { CurrenciesQuery } from "./CurrenciesQuery";
import { CurrencyExchangeRatesQuery } from "./CurrencyExchangeRatesQuery";

const httpsAgent = new Agent({ rejectUnauthorized: false });

export class BackendClient {
  BanksQuery: BanksQuery = new BanksQuery(httpsAgent);
  CreditCardsQuery: CreditCardQuery = new CreditCardQuery(httpsAgent);
  CurrenciesQuery: CurrenciesQuery = new CurrenciesQuery(httpsAgent);
  CurrencyExchangeRatesQuery: CurrencyExchangeRatesQuery = new CurrencyExchangeRatesQuery(httpsAgent);

  BackendClient() {
  }
}
