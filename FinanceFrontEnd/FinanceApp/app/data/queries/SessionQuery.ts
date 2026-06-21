import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "../base/BaseQuery";

export interface SessionInfo {
  userId: string;
  fullName: string;
  username: string;
  sourceId: string;
  roles: string[];
}

export class SessionQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.session.me,
    });
  }
}
