import axios from "axios";
import { Agent } from "https";
import serverLogger from "@/utils/logger.server";
import { buildAxiosConfig } from "./base/axiosConfig";
import { BanksQuery } from "./queries/BanksQuery";
import { CatalogBanksQuery } from "./queries/CatalogBanksQuery";
import { CatalogCurrenciesQuery } from "./queries/CatalogCurrenciesQuery";
import { CatalogFrequenciesQuery } from "./queries/CatalogFrequenciesQuery";
import { CreditCardQuery } from "./queries/CreditCardQuery";
import { CreditCardStatementQuery } from "./queries/CreditCardStatementQuery";
import { CurrenciesQuery } from "./queries/CurrenciesQuery";
import { FrequenciesQuery } from "./queries/FrequenciesQuery";
import { CurrencyExchangeRatesQuery } from "./queries/CurrencyExchangeRatesQuery";
import { DebitsQuery } from "./queries/DebitsQuery";
import { PaginatedDebitsQuery } from "./queries/PaginatedDebitsQuery";
import { PaginatedSubscriptionsQuery } from "./queries/PaginatedSubscriptionsQuery";
import { SessionQuery } from "./queries/SessionQuery";

const httpsAgent = new Agent({ rejectUnauthorized: false });

export class BackendClient {
    private HttpsAgent: Agent;
    private AccessToken: string;

    private BanksQuery: BanksQuery;
    private CatalogBanksQuery: CatalogBanksQuery;
    private CatalogCurrenciesQuery: CatalogCurrenciesQuery;
    private CatalogFrequenciesQuery: CatalogFrequenciesQuery;
    private DebitsQuery: DebitsQuery;
    private PaginatedDebitsQuery: PaginatedDebitsQuery;
    private PaginatedSubscriptionsQuery: PaginatedSubscriptionsQuery;
    private CreditCardsQuery: CreditCardQuery;
    private CreditCardStatementsQuery: CreditCardStatementQuery;
    private CurrenciesQuery: CurrenciesQuery;
    private FrequenciesQuery: FrequenciesQuery;
    private CurrencyExchangeRatesQuery: CurrencyExchangeRatesQuery;
    private SessionQuery: SessionQuery;

    constructor(accessToken: string) {
        this.HttpsAgent = httpsAgent;
        this.AccessToken = accessToken;

        this.BanksQuery = new BanksQuery(httpsAgent, this.AccessToken);
        this.CatalogBanksQuery = new CatalogBanksQuery(httpsAgent, this.AccessToken);
        this.CatalogCurrenciesQuery = new CatalogCurrenciesQuery(httpsAgent, this.AccessToken);
        this.CatalogFrequenciesQuery = new CatalogFrequenciesQuery(httpsAgent, this.AccessToken);
        this.DebitsQuery = new DebitsQuery(httpsAgent, this.AccessToken);
        this.PaginatedDebitsQuery = new PaginatedDebitsQuery(
            httpsAgent,
            this.AccessToken
        );
        this.PaginatedSubscriptionsQuery = new PaginatedSubscriptionsQuery(
            httpsAgent,
            this.AccessToken
        );
        this.CreditCardsQuery = new CreditCardQuery(
            httpsAgent,
            this.AccessToken
        );
        this.CreditCardStatementsQuery = new CreditCardStatementQuery(
            httpsAgent,
            this.AccessToken
        );
        this.CurrenciesQuery = new CurrenciesQuery(
            httpsAgent,
            this.AccessToken
        );
        this.FrequenciesQuery = new FrequenciesQuery(
            httpsAgent,
            this.AccessToken
        );
        this.CurrencyExchangeRatesQuery = new CurrencyExchangeRatesQuery(
            httpsAgent,
            this.AccessToken
        );
        this.SessionQuery = new SessionQuery(httpsAgent, this.AccessToken);
    }

    public GetBanksQuery = (): BanksQuery => this.BanksQuery;
    public GetCatalogBanksQuery = (): CatalogBanksQuery => this.CatalogBanksQuery;
    public GetCatalogCurrenciesQuery = (): CatalogCurrenciesQuery => this.CatalogCurrenciesQuery;
    public GetCatalogFrequenciesQuery = (): CatalogFrequenciesQuery => this.CatalogFrequenciesQuery;
    public GetDebitsQuery = (): DebitsQuery => this.DebitsQuery;
    public GetPaginatedDebitsQuery = (): PaginatedDebitsQuery =>
        this.PaginatedDebitsQuery;
    public GetPaginatedSubscriptionsQuery = (): PaginatedSubscriptionsQuery =>
        this.PaginatedSubscriptionsQuery;
    public GetCreditCardsQuery = (): CreditCardQuery => this.CreditCardsQuery;
    public GetCreditCardStatementsQuery = (): CreditCardStatementQuery => this.CreditCardStatementsQuery;
    public GetCurrenciesQuery = (): CurrenciesQuery => this.CurrenciesQuery;
    public GetFrequenciesQuery = (): FrequenciesQuery => this.FrequenciesQuery;
    public GetCurrencyExchangeRatesQuery = (): CurrencyExchangeRatesQuery =>
        this.CurrencyExchangeRatesQuery;
    public GetSessionQuery = (): SessionQuery => this.SessionQuery;

    async get<TFilter>(endpoint: string, filters?: TFilter) {
        try {
            const response = await axios.get(endpoint, buildAxiosConfig(this.HttpsAgent, this.AccessToken, filters ?? {}));

            return response.data;
        } catch (error) {
            serverLogger.error("Error:", error);
            throw error;
        }
    }
}
