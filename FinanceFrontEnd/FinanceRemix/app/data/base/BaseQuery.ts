import urls from "@/app/utils/urls";
import axios from "axios";
import { Agent } from "https";

export interface QueryEndpoints {
  get: string;
}

export class BaseQuery {
  private httpsAgent: Agent;
  private endpoints: QueryEndpoints;

  constructor(httpsAgent: Agent, endpoints: QueryEndpoints) {
    this.httpsAgent = httpsAgent;
    this.endpoints = endpoints;
  }

  async get() {
    try {
      const response = await axios.get(this.endpoints.get, {
        httpsAgent: this.httpsAgent,
      });

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
