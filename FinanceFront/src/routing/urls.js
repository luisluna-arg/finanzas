const baseUrl = "https://localhost:7005";
const apiBaseUrl = `${baseUrl}/api`;

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
    endpoint: `${apiBaseUrl}/debits/`,
    paginated: `${apiBaseUrl}/debits/paginated`,
    latest: `${apiBaseUrl}/debits/latest`,
  },
  debitOrigins: {
    endpoint: `${apiBaseUrl}/debit-origins`
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
    endpoint: `${apiBaseUrl}/iol-investment-asset/`
  },
  iolInvestmentAssetTypes: {
    endpoint: `${apiBaseUrl}/iol-investment-asset-type/`
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
    general: `${apiBaseUrl}/summary/general`
  }
};

export default urls;
