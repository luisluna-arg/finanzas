import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "@/data/base/BaseQuery";

export class BanksQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.banks.endpoint
    });
  }
}
