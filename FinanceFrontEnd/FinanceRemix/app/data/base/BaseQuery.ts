import axios from "axios";
import { Agent } from "https";

export interface QueryEndpoints {
    get: string;
}

export class BaseQuery {
    protected httpsAgent: Agent;
    protected getEndpoint: string;
    protected accessToken: string;

    constructor(
        httpsAgent: Agent,
        accessToken: string,
        endpoints: QueryEndpoints
    ) {
        this.httpsAgent = httpsAgent;
        this.getEndpoint = endpoints.get;
        this.accessToken = accessToken;
    }

    async get<TFilter>(filters?: TFilter) {
        try {
            const config = {
                params: filters ?? {},
                httpsAgent: this.httpsAgent,
                headers: {
                    Authorization: `Bearer ${this.accessToken}`,
                },
            };
            const response = await axios.get(this.getEndpoint, config);

            return response.data;
        } catch (error) {
                const { default: serverLogger } = await import("@/utils/logger.server");
                serverLogger.error("this.getEndpoint:", this.getEndpoint);
                serverLogger.error("Error:", error);
            throw error;
        }
    }
}
