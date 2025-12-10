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
  
  // Server-side: use environment variable API_URL if available
  // This is critical for container environments where localhost refers to the container itself
  if (process.env.API_URL) {
    return process.env.API_URL;
  }
  
  // Fallback for local development without environment variable
  return 'http://localhost:5000';
};

// Use getter functions to ensure environment variables are read at runtime, not at module load time
const getApiBaseUrl = () => `${getBaseUrl()}/api`;
const getDebitsBaseUrl = () => `${getApiBaseUrl()}/debits`;

const urls = {
  appModules: {
    get endpoint() { return `${getApiBaseUrl()}/app-modules/`; },
  },
  banks: {
    get endpoint() { return `${getApiBaseUrl()}/banks/`; },
  },
  currencies: {
    get endpoint() { return `${getApiBaseUrl()}/currencies/`; },
  },
  currencyExchangeRates: {
    get endpoint() { return `${getApiBaseUrl()}/currencies/exchange-rates`; },
    get latest() { return `${getApiBaseUrl()}/currencies/exchange-rates/latest`; },
    get paginated() { return `${getApiBaseUrl()}/currencies/exchange-rates/paginated`; },
    get latestByShortName() { return `${getApiBaseUrl()}/currencies/exchange-rates`; },
  },
  creditCards: {
    get get() { return `${getApiBaseUrl()}/credit-cards/`; },
  },
  creditCardTransactions: {
    get endpoint() { return `${getApiBaseUrl()}/credit-card-transactions`; },
    get latest() { return `${getApiBaseUrl()}/credit-card-transactions/latest`; },
    get paginated() { return `${getApiBaseUrl()}/credit-card-transactions/paginated`; },
  },
  creditCardStatements: {
    get endpoint() { return `${getApiBaseUrl()}/credit-card-statements`; },
    get latest() { return `${getApiBaseUrl()}/credit-card-statements/latest`; },
    get paginated() { return `${getApiBaseUrl()}/credit-card-statements/paginated`; },
  },
  creditCardPayments: {
    get endpoint() { return `${getApiBaseUrl()}/credit-card-payments`; },
    get latest() { return `${getApiBaseUrl()}/credit-card-payments/latest`; },
    get paginated() { return `${getApiBaseUrl()}/credit-card-payments/paginated`; },
  },
  // Keep old creditCardMovements for backward compatibility, but point to statement transactions for dashboard
  creditCardMovements: {
    get endpoint() { return `${getApiBaseUrl()}/credit-card-statement-transactions`; },
    get latest() { return `${getApiBaseUrl()}/credit-card-statement-transactions/latest`; },
    get paginated() { return `${getApiBaseUrl()}/credit-card-statement-transactions/paginated`; },
    get upload() { return `${getApiBaseUrl()}/credit-card-transactions/upload`; },
  },
  debits: {
    monthly: {
      get endpoint() { return `${getDebitsBaseUrl()}/monthly/`; },
      get paginated() { return `${getDebitsBaseUrl()}/monthly/paginated`; },
      get latest() { return `${getDebitsBaseUrl()}/monthly/latest`; },
    },
    annual: {
      get endpoint() { return `${getDebitsBaseUrl()}/annual/`; },
      get paginated() { return `${getDebitsBaseUrl()}/annual/paginated`; },
      get latest() { return `${getDebitsBaseUrl()}/annual/latest`; },
    },
  },
  debitOrigins: {
    get endpoint() { return `${getApiBaseUrl()}/debit-origins`; },
  },
  frequencies: {
    get endpoint() { return `${getApiBaseUrl()}/frequencies/`; },
  },
  funds: {
    get endpoint() { return `${getApiBaseUrl()}/funds/`; },
    get upload() { return `${getApiBaseUrl()}/funds/upload`; },
    get paginated() { return `${getApiBaseUrl()}/funds/paginated`; },
  },
  incomes: {
    get endpoint() { return `${getApiBaseUrl()}/incomes/`; },
    get upload() { return `${getApiBaseUrl()}/incomes/upload`; },
    get paginated() { return `${getApiBaseUrl()}/incomes/paginated`; },
  },
  iolInvestments: {
    get endpoint() { return `${getApiBaseUrl()}/iol-investment/`; },
    get upload() { return `${getApiBaseUrl()}/iol-investment/upload`; },
    get paginated() { return `${getApiBaseUrl()}/iol-investment/paginated`; },
  },
  iolInvestmentAssets: {
    get endpoint() { return `${getApiBaseUrl()}/iol-investment-asset/`; },
  },
  iolInvestmentAssetTypes: {
    get endpoint() { return `${getApiBaseUrl()}/iol-investment-asset-type/`; },
  },
  movements: {
    get endpoint() { return `${getApiBaseUrl()}/movements/`; },
    get paginated() { return `${getApiBaseUrl()}/movements/paginated`; },
    get upload() { return `${getApiBaseUrl()}/movements/upload`; },
  },
  summary: {
    get currentFunds() { return `${getApiBaseUrl()}/summary/currentFunds`; },
    get totalExpenses() { return `${getApiBaseUrl()}/summary/totalExpenses`; },
    get currentInvestments() { return `${getApiBaseUrl()}/summary/currentInvestments`; },
    get general() { return `${getApiBaseUrl()}/summary/general`; },
  },
  subscriptions: {
    get endpoint() { return `${getApiBaseUrl()}/subscriptions/`; },
    get paginated() { return `${getApiBaseUrl()}/subscriptions/paginated`; },
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
