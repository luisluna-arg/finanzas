import urls from "@/utils/urls";
import { Agent } from "https";
import { BasePaginatedQuery } from "./base/BasePaginatedQuery";

export class PaginatedDebitsQuery extends BasePaginatedQuery {
  constructor(httpsAgent: Agent) {
    super(httpsAgent, {
      get: urls.debits.monthly.endpoint,
      getPaginated: urls.debits.monthly.paginated,
    });
  }
}
