import ApiClient from "./ApiClient";
import type { TotalFunds, FundsQueryParams } from "./types/FundTypes";

/**
 * Service for interacting with fund-related API endpoints
 */
class FundService {
  /**
   * Fetch current funds from the API
   *
   * @param params - Optional query parameters (dailyUse and currencyId)
   * @returns Promise with the TotalFunds response
   */
  async getCurrentFunds(params?: FundsQueryParams): Promise<TotalFunds> {
    return await ApiClient.get<TotalFunds>("api/summary/currentFunds", params);
  }
}

export default new FundService();
