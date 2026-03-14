import axios from 'axios';
import { Agent } from 'https';
import serverLogger from '@/utils/logger.server';
import { BackendUrl } from '@/utils/BackendUrl';
import { buildAxiosConfig } from './axiosConfig';

export interface QueryEndpoints {
  get: BackendUrl | string;
}

export class BaseQuery {
  protected httpsAgent: Agent;
  protected getEndpoint: string | BackendUrl;
  protected accessToken: string;

  constructor(httpsAgent: Agent, accessToken: string, endpoints: QueryEndpoints) {
    this.httpsAgent = httpsAgent;
    this.getEndpoint = endpoints.get;
    this.accessToken = accessToken;
  }

  protected axiosConfig(params?: object) {
    return buildAxiosConfig(this.httpsAgent, this.accessToken, params);
  }

  async get<TFilter>(filters?: TFilter) {
    try {
      const config = this.axiosConfig(filters ?? {});
      const endpointUrl =
        this.getEndpoint instanceof BackendUrl
          ? this.getEndpoint.toRaw()
          : String(this.getEndpoint);
      const response = await axios.get(endpointUrl, config);

      return response.data;
    } catch (error) {
      serverLogger.error('this.getEndpoint:', this.getEndpoint);
      serverLogger.error('Error:', error);
      throw error;
    }
  }
}
