import ApiClient from './ApiClient';
import type { CreateFundRequest, Fund, FundResponse, FundsResponse } from './types/FundTypes';

// Cache management
const CACHE_TIMEOUT = 60000; // 1 minute
let fundsCache: FundsResponse | null = null;
let fundsCacheTimestamp: number = 0;

const FundService = {
  /**
   * Gets all funds with optional caching
   * @param forceRefresh Whether to bypass cache
   * @returns Promise with funds response
   */
  getAllFunds: async (forceRefresh = false): Promise<FundsResponse> => {
    const now = Date.now();

    // Check if we have a valid cache
    if (!forceRefresh && fundsCache && now - fundsCacheTimestamp < CACHE_TIMEOUT) {
      return fundsCache;
    }

    try {
      const response = await ApiClient.get<FundsResponse>('/api/summary/currentFunds');

      // Update cache
      fundsCache = response;
      fundsCacheTimestamp = now;

      return response;
    } catch (error) {
      console.error('Error fetching funds:', error);
      throw error;
    }
  },

  /**
   * Gets a fund by ID
   * @param id Fund ID
   * @returns Promise with fund response
   */
  getFund: async (id: string): Promise<FundResponse> => {
    try {
      return await ApiClient.get<FundResponse>(`/api/funds/${id}`);
    } catch (error) {
      console.error(`Error fetching fund ${id}:`, error);
      throw error;
    }
  },

  /**
   * Creates a new fund
   * @param fund Fund data to create
   * @returns Promise with created fund
   */
  createFund: async (fund: CreateFundRequest): Promise<Fund> => {
    try {
      const response = await ApiClient.post<Fund>('/api/funds', fund);

      // Invalidate cache
      fundsCache = null;
      fundsCacheTimestamp = 0;

      return response;
    } catch (error) {
      console.error('Error creating fund:', error);
      throw error;
    }
  },

  /**
   * Invalidates the funds cache
   */
  invalidateCache: (): void => {
    fundsCache = null;
    fundsCacheTimestamp = 0;
  },
};

export default FundService;
