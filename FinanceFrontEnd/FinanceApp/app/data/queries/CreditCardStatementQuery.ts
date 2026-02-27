import axios from 'axios';
import urls from '@/utils/urls';
import { Agent } from 'https';
import { BaseQuery } from '../base/BaseQuery';
import serverLogger from '@/utils/logger.server';

export class CreditCardStatementQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.creditCardStatements.endpoint,
    });
  }

  async getLatest() {
    try {
      const config = {
        httpsAgent: this.httpsAgent,
        headers: {
          Authorization: `Bearer ${this.accessToken}`,
        },
      };
      const response = await axios.get(urls.creditCardStatements.latest.toRaw(), config);
      return response.data;
    } catch (error) {
      serverLogger.error('Error fetching latest statements:', error);
      throw error;
    }
  }
}
