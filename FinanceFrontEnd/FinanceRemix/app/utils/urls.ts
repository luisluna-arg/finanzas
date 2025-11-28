// Configuration for both local development and containerized environments
const getBaseUrl = () => {
  // Check if we're on the client side
  if (typeof window !== 'undefined') {
    // Client-side: determine based on current hostname
    const hostname = window.location.hostname;
    
    if (hostname === 'localhost' || hostname === '127.0.0.1') {
      // Local development
      return 'http://localhost:5000';
    } else {
      // Container environment - use the same hostname but different port
      // or use the backend service name if running in container network
      return `http://${hostname}:5000`;
    }
  }
  
  // Server-side: use environment variables or defaults
  // In containers, this might be 'http://backend:5000' for internal communication
  // In local development, this should be 'http://localhost:5000'
  return 'http://localhost:5000';
};

const baseUrl = getBaseUrl();
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
  creditCardTransactions: {
    endpoint: `${apiBaseUrl}/credit-card-transactions`,
    latest: `${apiBaseUrl}/credit-card-transactions/latest`,
    paginated: `${apiBaseUrl}/credit-card-transactions/paginated`,
  },
  creditCardStatements: {
    endpoint: `${apiBaseUrl}/credit-card-statements`,
    latest: `${apiBaseUrl}/credit-card-statements/latest`,
    paginated: `${apiBaseUrl}/credit-card-statements/paginated`,
  },
  creditCardPayments: {
    endpoint: `${apiBaseUrl}/credit-card-payments`,
    latest: `${apiBaseUrl}/credit-card-payments/latest`,
    paginated: `${apiBaseUrl}/credit-card-payments/paginated`,
  },
  // Keep old creditCardMovements for backward compatibility, but point to statement transactions for dashboard
  creditCardMovements: {
    endpoint: `${apiBaseUrl}/credit-card-statement-transactions`,
    latest: `${apiBaseUrl}/credit-card-statement-transactions/latest`,
    paginated: `${apiBaseUrl}/credit-card-statement-transactions/paginated`,
    upload: `${apiBaseUrl}/credit-card-transactions/upload`,
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
  frequencies: {
    endpoint: `${apiBaseUrl}/frequencies/`,
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
  subscriptions: {
    endpoint: `${apiBaseUrl}/subscriptions/`,
    paginated: `${apiBaseUrl}/subscriptions/paginated`,
  },

  /**
   * Returns a proxy URL for a backend API path and optional params object.
   * Example: urls.proxy('/summary/general', { DailyUse: true })
   *
   * Accepts a full backend URL or a path, but always extracts just the path for the proxy.
   */
  proxy: (urlOrPath: string, params?: Record<string, unknown>) => {
    // If urlOrPath is a full URL, extract the path and search
    let path = urlOrPath;
    try {
      if (urlOrPath.startsWith('http')) {
        const u = new URL(urlOrPath);
        path = u.pathname + (u.search || '');
      }
    } catch {
      // ignore invalid URL and treat input as a path
    }

    const search = params
      ? '&' +
        Object.entries(params)
          .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
          .join('&')
      : '';
    return `/api/proxy?path=${encodeURIComponent(path)}${search}`;
  },
};

export default urls;
