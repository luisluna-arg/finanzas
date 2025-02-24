import axios from "axios";
import { Agent } from "https";

export interface QueryEndpoints {
  get: string;
}

export class BaseQuery {
  protected httpsAgent: Agent;
  protected getEndpoint: string;

  constructor(httpsAgent: Agent, endpoints: QueryEndpoints) {
    this.httpsAgent = httpsAgent;
    this.getEndpoint = endpoints.get;
  }

  async get<TFilter>(filters?: TFilter) {
    try {
      const response = await axios.get(this.getEndpoint, {
        params: filters ?? {},
        httpsAgent: this.httpsAgent,
      });

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
