import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "../base/BaseQuery";

export class CatalogFrequenciesQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.catalog.frequencies.endpoint,
    });
  }
}
