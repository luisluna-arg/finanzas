import { BackendUrl } from '@/utils/BackendUrl';

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
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/app-modules/`);
    },
  },
  banks: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/banks/`);
    },
  },
  currencies: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/currencies/`);
    },
  },
  currencyExchangeRates: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/currencies/exchange-rates`);
    },
    get latest() {
      return new BackendUrl(`${getApiBaseUrl()}/currencies/exchange-rates/latest`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/currencies/exchange-rates/paginated`);
    },
    get latestByShortName() {
      return new BackendUrl(`${getApiBaseUrl()}/currencies/exchange-rates`);
    },
  },
  creditCards: {
    get get() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-cards/`);
    },
  },
  creditCardTransactions: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-transactions`);
    },
    get latest() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-transactions/latest`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-transactions/paginated`);
    },
  },
  creditCardStatements: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statements`);
    },
    get latest() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statements/latest`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statements/paginated`);
    },
  },
  creditCardPayments: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-payments`);
    },
    get latest() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-payments/latest`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-payments/paginated`);
    },
  },
  // Keep old creditCardMovements for backward compatibility, but point to statement transactions for dashboard
  creditCardMovements: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statement-transactions`);
    },
    get latest() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statement-transactions/latest`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-statement-transactions/paginated`);
    },
    get upload() {
      return new BackendUrl(`${getApiBaseUrl()}/credit-card-transactions/upload`);
    },
  },
  debits: {
    monthly: {
      get endpoint() {
        return new BackendUrl(`${getDebitsBaseUrl()}/monthly/`);
      },
      get paginated() {
        return new BackendUrl(`${getDebitsBaseUrl()}/monthly/paginated`);
      },
      get latest() {
        return new BackendUrl(`${getDebitsBaseUrl()}/monthly/latest`);
      },
    },
    annual: {
      get endpoint() {
        return new BackendUrl(`${getDebitsBaseUrl()}/annual/`);
      },
      get paginated() {
        return new BackendUrl(`${getDebitsBaseUrl()}/annual/paginated`);
      },
      get latest() {
        return new BackendUrl(`${getDebitsBaseUrl()}/annual/latest`);
      },
    },
  },
  debitOrigins: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/debit-origins`);
    },
  },
  frequencies: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/frequencies/`);
    },
  },
  funds: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/funds/`);
    },
    get upload() {
      return new BackendUrl(`${getApiBaseUrl()}/funds/upload`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/funds/paginated`);
    },
  },
  incomes: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/incomes/`);
    },
    get upload() {
      return new BackendUrl(`${getApiBaseUrl()}/incomes/upload`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/incomes/paginated`);
    },
  },
  iolInvestments: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/iol-investment/`);
    },
    get upload() {
      return new BackendUrl(`${getApiBaseUrl()}/iol-investment/upload`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/iol-investment/paginated`);
    },
  },
  iolInvestmentAssets: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/iol-investment-asset/`);
    },
  },
  iolInvestmentAssetTypes: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/iol-investment-asset-type/`);
    },
  },
  movements: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/movements/`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/movements/paginated`);
    },
    get upload() {
      return new BackendUrl(`${getApiBaseUrl()}/movements/upload`);
    },
  },
  summary: {
    get currentFunds() {
      return new BackendUrl(`${getApiBaseUrl()}/summary/currentFunds`);
    },
    get totalExpenses() {
      return new BackendUrl(`${getApiBaseUrl()}/summary/totalExpenses`);
    },
    get currentInvestments() {
      return new BackendUrl(`${getApiBaseUrl()}/summary/currentInvestments`);
    },
    get general() {
      return new BackendUrl(`${getApiBaseUrl()}/summary/general`);
    },
  },
  subscriptions: {
    get endpoint() {
      return new BackendUrl(`${getApiBaseUrl()}/subscriptions/`);
    },
    get paginated() {
      return new BackendUrl(`${getApiBaseUrl()}/subscriptions/paginated`);
    },
  },
};

export default urls;
