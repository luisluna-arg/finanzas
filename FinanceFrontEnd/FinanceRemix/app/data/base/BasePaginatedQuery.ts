import axios from "axios";
import { Agent } from "https";
import { BaseQuery, QueryEndpoints } from "./BaseQuery";

export interface PaginatedQueryEndpoints extends QueryEndpoints {
  getPaginated: string;
}

export interface PaginatedQueryFilters {
  From?: string;
  To?: string;
  Page: number;
  PageSize: number;
}

export class BasePaginatedQuery extends BaseQuery {
  protected getPaginatedEndpoint: string;

  constructor(httpsAgent: Agent, endpoints: PaginatedQueryEndpoints) {
    super(httpsAgent, endpoints);

    this.getPaginatedEndpoint = endpoints.getPaginated;
  }

  async getPaginated<T extends PaginatedQueryFilters>(filters: T) {
    try {
      const response = await axios.get(this.getPaginatedEndpoint, {
        params: filters,
        httpsAgent: this.httpsAgent,
      });

      return response.data;
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
  }
}
