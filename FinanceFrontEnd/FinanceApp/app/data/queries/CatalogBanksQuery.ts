import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "@/data/base/BaseQuery";

export class CatalogBanksQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.catalog.banks.endpoint,
    });
  }
}
