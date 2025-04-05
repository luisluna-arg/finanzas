import axios from "axios";
import { Agent } from "https";
import { BanksQuery } from "./BanksQuery";
import { CreditCardQuery } from "./CreditCardQuery";
import { CurrenciesQuery } from "./CurrenciesQuery";
import { CurrencyExchangeRatesQuery } from "./CurrencyExchangeRatesQuery";
import { DebitsQuery } from "./DebitsQuery";
import { PaginatedDebitsQuery } from "./PaginatedDebitsQuery";

const httpsAgent = new Agent({ rejectUnauthorized: false });

export class BackendClient {
  private HttpsAgent: Agent;
  
  BanksQuery: BanksQuery = new BanksQuery(httpsAgent);
  DebitsQuery: DebitsQuery = new DebitsQuery(httpsAgent);
  PaginatedDebitsQuery: PaginatedDebitsQuery = new PaginatedDebitsQuery(httpsAgent);
  CreditCardsQuery: CreditCardQuery = new CreditCardQuery(httpsAgent);
  CurrenciesQuery: CurrenciesQuery = new CurrenciesQuery(httpsAgent);
  CurrencyExchangeRatesQuery: CurrencyExchangeRatesQuery = new CurrencyExchangeRatesQuery(httpsAgent);

  constructor() {
    this.HttpsAgent = httpsAgent;
  }

  async get<TFilter>(endpoint: string, filters?: TFilter) {
    try {
      const response = await axios.get(endpoint, {
        params: filters ?? {},
        httpsAgent: this.HttpsAgent,
      });

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
