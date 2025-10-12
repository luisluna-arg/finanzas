// Simple runtime configuration - no build-time complexity needed
const baseUrl = process.env.API_URL || (process.env.NODE_ENV === 'production' 
    ? 'http://backend:5000'  // Docker internal network - updated to port 5000
    : 'http://localhost:5000'); // Local development
const apiBaseUrl = `${baseUrl}/api`;
const debitsBaseUrl = `${apiBaseUrl}/debits`;

const urls = {
    appModules: {
        endpoint: `${apiBaseUrl}/app-modules/`,
    },
    banks: {
        endpoint: `${apiBaseUrl}/banks/`,
    },
    currencies: {
        endpoint: `${apiBaseUrl}/currencies/`,
    },
    currencyExchangeRates: {
        endpoint: `${apiBaseUrl}/currencies/exchange-rates`,
        latest: `${apiBaseUrl}/currencies/exchange-rates/latest`,
        paginated: `${apiBaseUrl}/currencies/exchange-rates/paginated`,
        latestByShortName: `${apiBaseUrl}/currencies/exchange-rates`,
    },
    creditCards: {
        get: `${apiBaseUrl}/credit-cards/`,
    },
    creditCardMovements: {
        endpoint: `${apiBaseUrl}/credit-card-movements`,
        latest: `${apiBaseUrl}/credit-card-movements/latest`,
        paginated: `${apiBaseUrl}/credit-card-movements/paginated`,
        upload: `${apiBaseUrl}/credit-card-movements/upload`,
    },
    debits: {
        monthly: {
            endpoint: `${debitsBaseUrl}/monthly/`,
            paginated: `${debitsBaseUrl}/monthly/paginated`,
            latest: `${debitsBaseUrl}/monthly/latest`,
        },
        annual: {
            endpoint: `${debitsBaseUrl}/annual/`,
            paginated: `${debitsBaseUrl}/annual/paginated`,
            latest: `${debitsBaseUrl}/annual/latest`,
        },
    },
    debitOrigins: {
        endpoint: `${apiBaseUrl}/debit-origins`,
    },
    funds: {
        endpoint: `${apiBaseUrl}/funds/`,
        upload: `${apiBaseUrl}/funds/upload`,
        paginated: `${apiBaseUrl}/funds/paginated`,
    },
    incomes: {
        endpoint: `${apiBaseUrl}/incomes/`,
        upload: `${apiBaseUrl}/incomes/upload`,
        paginated: `${apiBaseUrl}/incomes/paginated`,
    },
    iolInvestments: {
        endpoint: `${apiBaseUrl}/iol-investment/`,
        upload: `${apiBaseUrl}/iol-investment/upload`,
        paginated: `${apiBaseUrl}/iol-investment/paginated`,
    },
    iolInvestmentAssets: {
        endpoint: `${apiBaseUrl}/iol-investment-asset/`,
    },
    iolInvestmentAssetTypes: {
        endpoint: `${apiBaseUrl}/iol-investment-asset-type/`,
    },
    movements: {
        endpoint: `${apiBaseUrl}/movements/`,
        paginated: `${apiBaseUrl}/movements/paginated`,
        upload: `${apiBaseUrl}/movements/upload`,
    },
    summary: {
        currentFunds: `${apiBaseUrl}/summary/currentFunds`,
        totalExpenses: `${apiBaseUrl}/summary/totalExpenses`,
        currentInvestments: `${apiBaseUrl}/summary/currentInvestments`,
        general: `${apiBaseUrl}/summary/general`,
    },

    /**
     * Returns a proxy URL for a backend API path and optional params object.
     * Example: urls.proxy('/summary/general', { DailyUse: true })
     */
    /**
     * Accepts a full backend URL or a path, but always extracts just the path for the proxy.
     */
    proxy: (urlOrPath: string, params?: Record<string, unknown>) => {
        // If urlOrPath is a full URL, extract the path and search
        let path = urlOrPath;
        try {
            if (urlOrPath.startsWith("http")) {
                const u = new URL(urlOrPath);
                path = u.pathname + (u.search || "");
            }
        } catch {
            // ignore invalid URL and treat input as a path
        }

        const search = params
            ? "&" +
              Object.entries(params)
                  .map(
                      ([k, v]) =>
                          `${encodeURIComponent(k)}=${encodeURIComponent(
                              String(v)
                          )}`
                  )
                  .join("&")
            : "";
        return `/api/proxy?path=${encodeURIComponent(path)}${search}`;
    },
};

export default urls;
