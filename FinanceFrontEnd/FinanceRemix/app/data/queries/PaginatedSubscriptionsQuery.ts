import urls from "@/utils/urls";
import { Agent } from "https";
import { BasePaginatedQuery } from "@/data/base/BasePaginatedQuery";

export class PaginatedSubscriptionsQuery extends BasePaginatedQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.subscriptions.endpoint,
      getPaginated: urls.subscriptions.paginated,
    });
  }
}
