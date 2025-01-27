import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "./base/BaseQuery";

export class CreditCardQuery extends BaseQuery {
  constructor(httpsAgent: Agent) {
    super(httpsAgent, {
      get: urls.creditCards.get
    });
  }
}
