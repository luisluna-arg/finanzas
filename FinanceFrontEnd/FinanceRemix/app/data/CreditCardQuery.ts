import urls from "@/app/utils/urls";
import axios from "axios";
import { Agent } from "https";

export class CreditCardQuery {
  httpsAgent: Agent;

  constructor(httpsAgent: Agent) {
    this.httpsAgent = httpsAgent;
  }

  async getCreditCards() {
    try {
      const response = await axios.get(urls.creditCards.get, {
        httpsAgent: this.httpsAgent,
      });

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
