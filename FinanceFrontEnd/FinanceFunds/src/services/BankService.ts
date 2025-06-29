import ApiClient from './ApiClient';
import type { Bank, BanksResponse } from './types/BankTypes';

// Define a memory-efficient cache key
const BANKS_CACHE_KEY = 'all_banks';

/**
 * Service for interacting with bank-related API endpoints
 */
class BankService {
  // Static shared cache storage
  private static cache: {
    data: Map<string, any>;
    timestamps: Map<string, number>;
  } = {
    data: new Map(),
    timestamps: new Map(),
  };

  // Cache expiration in milliseconds (10 minutes since bank data rarely changes)
  private readonly CACHE_EXPIRATION = 10 * 60 * 1000;

  /**
   * Fetch all banks from the API
   *
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the BanksResponse
   */
  async getAllBanks(forceRefresh = false): Promise<BanksResponse> {
    const now = Date.now();
    const { data, timestamps } = BankService.cache;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(BANKS_CACHE_KEY) &&
      timestamps.has(BANKS_CACHE_KEY) &&
      now - (timestamps.get(BANKS_CACHE_KEY) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(BANKS_CACHE_KEY) as BanksResponse;
    }

    try {
      const response = await ApiClient.get<BanksResponse>('/api/banks');

      // Set response in cache
      data.set(BANKS_CACHE_KEY, response);
      timestamps.set(BANKS_CACHE_KEY, now);

      // Cache individual banks as well
      if (Array.isArray(response)) {
        response.forEach(bank => {
          data.set(`bank_${bank.id}`, bank);
          timestamps.set(`bank_${bank.id}`, now);
        });
      }

      return response;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(BANKS_CACHE_KEY)) {
        console.warn('Returning stale bank data due to API error');
        return data.get(BANKS_CACHE_KEY) as BanksResponse;
      }

      console.error('Error fetching banks:', error);
      throw error;
    }
  }

  /**
   * Fetch a specific bank by ID
   *
   * @param id - The bank ID
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the Bank
   */
  async getBank(id: string, forceRefresh = false): Promise<Bank> {
    if (!id) throw new Error('Bank ID is required');

    const now = Date.now();
    const { data, timestamps } = BankService.cache;
    const bankCacheKey = `bank_${id}`;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(bankCacheKey) &&
      timestamps.has(bankCacheKey) &&
      now - (timestamps.get(bankCacheKey) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(bankCacheKey) as Bank;
    }

    try {
      // Try to get from getAllBanks first to avoid extra API call
      if (!forceRefresh) {
        try {
          const allBanks = await this.getAllBanks(false);
          const foundBank = Array.isArray(allBanks)
            ? allBanks.find((b: Bank) => b.id === id)
            : undefined;
          if (foundBank) return foundBank;
        } catch {
          // If this fails, continue with direct API call
        }
      }

      // Direct API call if needed
      const bank = await ApiClient.get<Bank>(`/api/banks/${id}`);

      // Update cache
      data.set(bankCacheKey, bank);
      timestamps.set(bankCacheKey, now);

      return bank;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(bankCacheKey)) {
        console.warn(`Returning stale data for bank ${id} due to API error`);
        return data.get(bankCacheKey) as Bank;
      }

      console.error(`Error fetching bank with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Invalidate all bank caches or a specific bank cache
   * @param bankId Optional bank ID to invalidate specific cache
   */
  invalidateCache(bankId?: string): void {
    const { data, timestamps } = BankService.cache;

    if (bankId) {
      // Invalidate only specific bank
      const bankCacheKey = `bank_${bankId}`;
      data.delete(bankCacheKey);
      timestamps.delete(bankCacheKey);
    } else {
      // Invalidate all bank caches
      data.delete(BANKS_CACHE_KEY);
      timestamps.delete(BANKS_CACHE_KEY);

      // Find and remove all bank_ keys
      const bankKeys = Array.from(data.keys()).filter(key => key.startsWith('bank_'));
      bankKeys.forEach(key => {
        data.delete(key);
        timestamps.delete(key);
      });
    }
  }
}

export default new BankService();
