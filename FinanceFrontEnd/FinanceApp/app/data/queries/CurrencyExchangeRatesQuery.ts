import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "../base/BaseQuery";

export class CurrencyExchangeRatesQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: `${urls.currencyExchangeRates.latestByShortName}/USDTC/latest`,
    });
  }
}
