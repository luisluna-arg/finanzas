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
    get: `${baseUrl}api/credit-card-movements`,
  },
  funds: {
    endpoint: `${baseUrl}api/funds/`,
    upload: `${baseUrl}api/funds/upload`,
  },
  movements: {
    paginated: `${baseUrl}api/movements/paginated`,
    upload: `${baseUrl}api/movements/upload`,
  }
};

export default urls;
