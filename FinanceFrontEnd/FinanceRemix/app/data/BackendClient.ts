import axios from "axios";
import { Agent } from "https";
import { BanksQuery } from "./queries/BanksQuery";
import { CreditCardQuery } from "./queries/CreditCardQuery";
import { CurrenciesQuery } from "./queries/CurrenciesQuery";
import { CurrencyExchangeRatesQuery } from "./queries/CurrencyExchangeRatesQuery";
import { DebitsQuery } from "./queries/DebitsQuery";
import { PaginatedDebitsQuery } from "./queries/PaginatedDebitsQuery";

const httpsAgent = new Agent({ rejectUnauthorized: false });

export class BackendClient {
    private HttpsAgent: Agent;
    private AccessToken: string;

    private BanksQuery: BanksQuery;
    private DebitsQuery: DebitsQuery;
    private PaginatedDebitsQuery: PaginatedDebitsQuery;
    private CreditCardsQuery: CreditCardQuery;
    private CurrenciesQuery: CurrenciesQuery;
    private CurrencyExchangeRatesQuery: CurrencyExchangeRatesQuery;

    constructor(accessToken: string) {
        this.HttpsAgent = httpsAgent;
        this.AccessToken = accessToken;

        this.BanksQuery = new BanksQuery(httpsAgent, this.AccessToken);
        this.DebitsQuery = new DebitsQuery(httpsAgent, this.AccessToken);
        this.PaginatedDebitsQuery = new PaginatedDebitsQuery(
            httpsAgent,
            this.AccessToken
        );
        this.CreditCardsQuery = new CreditCardQuery(httpsAgent, this.AccessToken);
        this.CurrenciesQuery = new CurrenciesQuery(httpsAgent, this.AccessToken);
        this.CurrencyExchangeRatesQuery = new CurrencyExchangeRatesQuery(
            httpsAgent,
            this.AccessToken
        );
    }

    public GetBanksQuery = (): BanksQuery => this.BanksQuery;
    public GetDebitsQuery = (): DebitsQuery => this.DebitsQuery;
    public GetPaginatedDebitsQuery = (): PaginatedDebitsQuery => this.PaginatedDebitsQuery;
    public GetCreditCardsQuery = (): CreditCardQuery => this.CreditCardsQuery;
    public GetCurrenciesQuery = (): CurrenciesQuery => this.CurrenciesQuery;
    public GetCurrencyExchangeRatesQuery = (): CurrencyExchangeRatesQuery => this.CurrencyExchangeRatesQuery;


    async get<TFilter>(endpoint: string, filters?: TFilter) {
        try {
            const response = await axios.get(endpoint, {
                params: filters ?? {},
                httpsAgent: this.HttpsAgent,
                headers: {
                    Authorization: `Bearer ${this.AccessToken}`,
                },
            });

            return response.data;
        } catch (error) {
            console.error("Error:", error);
            throw error;
        }
    }
}
