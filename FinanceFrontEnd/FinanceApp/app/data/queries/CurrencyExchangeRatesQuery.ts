import urls from '@/utils/urls';
import { Agent } from 'https';
import { BaseQuery } from '../base/BaseQuery';

export class CurrencyExchangeRatesQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      // TODO: 'USDTC' is hardcoded here — consider making this configurable or
      // deriving it from a user/app setting so the dashboard CC dollar conversion
      // rate isn't silently broken if the short name ever changes.
      get: urls.currencyExchangeRates.latestByShortName.append('/USDTC/latest'),
    });
  }
}
