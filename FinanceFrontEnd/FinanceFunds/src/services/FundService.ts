import apiClient, { ApiClient } from './ApiClient';
import type { CreateFundRequest, Fund, FundResponse, FundsResponse } from './types/FundTypes';
import SafeLogger from '@/utils/SafeLogger';

// Cache management — only used for the default (browser) client. A request-scoped
// server client (passed explicitly from a loader) always bypasses it, since a
// module-level cache would otherwise leak one user's data to another on the server.
const CACHE_TIMEOUT = 60000; // 1 minute
let fundsCache: FundsResponse | null = null;
let fundsCacheTimestamp: number = 0;

const FundService = {
  /**
   * Gets all funds with optional caching
   * @param forceRefresh Whether to bypass cache
   * @param client Request-scoped client (server loaders); defaults to the browser singleton
   * @returns Promise with funds response
   */
  getAllFunds: async (
    forceRefresh = false,
    client: ApiClient = apiClient
  ): Promise<FundsResponse> => {
    const useCache = client === apiClient;
    const now = Date.now();

    if (useCache && !forceRefresh && fundsCache && now - fundsCacheTimestamp < CACHE_TIMEOUT) {
      return fundsCache;
    }

    try {
      const response = await client.get<FundsResponse>('/api/summary/currentFunds');

      if (useCache) {
        fundsCache = response;
        fundsCacheTimestamp = now;
      }

      return response;
    } catch (error) {
      SafeLogger.error('Error fetching funds:', error);
      throw error;
    }
  },

  /**
   * Gets a fund by ID
   * @param id Fund ID
   * @param client Request-scoped client (server loaders); defaults to the browser singleton
   * @returns Promise with fund response
   */
  getFund: async (id: string, client: ApiClient = apiClient): Promise<FundResponse> => {
    try {
      return await client.get<FundResponse>(`/api/funds/${id}`);
    } catch (error) {
      SafeLogger.error(`Error fetching fund ${id}:`, error);
      throw error;
    }
  },

  /**
   * Creates a new fund
   * @param fund Fund data to create
   * @param client Request-scoped client (server loaders); defaults to the browser singleton
   * @returns Promise with created fund
   */
  createFund: async (fund: CreateFundRequest, client: ApiClient = apiClient): Promise<Fund> => {
    try {
      const response = await client.post<Fund>('/api/funds', fund);

      // Invalidate cache
      fundsCache = null;
      fundsCacheTimestamp = 0;

      return response;
    } catch (error) {
      SafeLogger.error('Error creating fund:', error);
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
