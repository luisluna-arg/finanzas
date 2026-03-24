import urls from "@/utils/urls";
import axios from "axios";
import { Agent } from "https";
import { BaseQuery } from "../base/BaseQuery";

export class CurrenciesQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.currencies.endpoint,
    });
  }

  async getById(id: string) {
    const url = urls.currencies.getById(id).toRaw();
    const response = await axios.get(url, this.axiosConfig());
    return response.data;
  }
}
