import axios from 'axios';
import { Agent } from 'https';
import { BaseQuery, QueryEndpoints } from './BaseQuery';
import serverLogger from '@/utils/logger.server';
import { BackendUrl } from '@/utils/BackendUrl';

export interface PaginatedQueryEndpoints extends QueryEndpoints {
  getPaginated: string | BackendUrl;
}

export interface PaginatedQueryFilters {
  From?: string;
  To?: string;
  Page: number;
  PageSize: number;
}

export class BasePaginatedQuery extends BaseQuery {
  protected getPaginatedEndpoint: string | BackendUrl;

  constructor(httpsAgent: Agent, accessToken: string, endpoints: PaginatedQueryEndpoints) {
    super(httpsAgent, accessToken, endpoints);

    this.getPaginatedEndpoint = endpoints.getPaginated;
  }

  async getPaginated<T extends PaginatedQueryFilters>(filters: T) {
    try {
      const endpointUrl =
        this.getPaginatedEndpoint instanceof BackendUrl
          ? this.getPaginatedEndpoint.toRaw()
          : String(this.getPaginatedEndpoint);
      const response = await axios.get(endpointUrl, this.axiosConfig(filters));

      return response.data;
    } catch (error) {
      serverLogger.error('Error:', error);
      throw error;
    }
  }
}
