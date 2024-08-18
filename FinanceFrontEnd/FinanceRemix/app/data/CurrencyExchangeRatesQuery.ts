import urls from "@/app/utils/urls";
import axios from "axios";
import { Agent } from "https";

export class CurrencyExchangeRatesQuery {
  httpsAgent: Agent;

  constructor(httpsAgent: Agent) {
    this.httpsAgent = httpsAgent;
  }

  async getLatest() {
    try {
      const response = await axios.get(
        `${urls.currencyExchangeRates.latestByShortName}/USDTC/latest`,
        {
          httpsAgent: this.httpsAgent,
        }
      );

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
