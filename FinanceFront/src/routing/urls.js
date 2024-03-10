const baseUrl = "https://localhost:7005/";

const urls = {
  appModules: {
    endpoint: `${baseUrl}api/app-modules/`,
  },
  banks: {
    endpoint: `${baseUrl}api/banks/`,
  },
  currencies: {
    endpoint: `${baseUrl}api/currencies/`,
  },
  creditCards: {
    get: `${baseUrl}api/credit-cards/`,
  },
  creditCardMovements: {
    endpoint: `${baseUrl}api/credit-card-movements`,
    latest: `${baseUrl}api/credit-card-movements/latest`,
    paginated: `${baseUrl}api/credit-card-movements/paginated`,
    upload: `${baseUrl}api/credit-card-movements/upload`,
  },
  debits: {
    endpoint: `${baseUrl}api/debits/`,
    paginated: `${baseUrl}api/debits/paginated`,
    latest: `${baseUrl}api/debits/latest`,
  },
  debitOrigins: {
    endpoint: `${baseUrl}api/debit-origins`
  },
  funds: {
    endpoint: `${baseUrl}api/funds/`,
    upload: `${baseUrl}api/funds/upload`,
  },
  iolInvestments: {
    endpoint: `${baseUrl}api/iol-investment/`,
    upload: `${baseUrl}api/iol-investment/upload`,
    paginated: `${baseUrl}api/iol-investment/paginated`,
  },
  iolInvestmentAssets: {
    endpoint: `${baseUrl}api/iol-investment-asset/`
  },
  iolInvestmentAssetTypes: {
    endpoint: `${baseUrl}api/iol-investment-asset-type/`
  },
  movements: {
    endpoint: `${baseUrl}api/movements/`,
    paginated: `${baseUrl}api/movements/paginated`,
    upload: `${baseUrl}api/movements/upload`,
  },
  summary: {
    totalExpenses: `${baseUrl}api/summary/totalExpenses`,
    currentInvestments: `${baseUrl}api/summary/currentInvestments`
  }
};

export default urls;
