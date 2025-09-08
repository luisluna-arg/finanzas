import ApiClient from './ApiClient';
import type { CurrenciesResponse } from './types/CurrencyTypes';
import type { Currency } from './types/CurrencyTypes';
import SafeLogger from '@/utils/SafeLogger';

// Define a memory-efficient cache key
const CURRENCIES_CACHE_KEY = 'all_currencies';

/**
 * Service for interacting with currency-related API endpoints
 */
class CurrencyService {
  // Static shared cache storage
  private static cache: {
    data: Map<string, unknown>;
    timestamps: Map<string, number>;
  } = {
    data: new Map<string, unknown>(),
    timestamps: new Map(),
  };

  // Cache expiration in milliseconds (15 minutes since currency data rarely changes)
  private readonly CACHE_EXPIRATION = 15 * 60 * 1000;

  /**
   * Fetch all currencies from the API
   *
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the CurrenciesResponse
   */
  async getAllCurrencies(forceRefresh = false): Promise<CurrenciesResponse> {
    const now = Date.now();
    const { data, timestamps } = CurrencyService.cache;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(CURRENCIES_CACHE_KEY) &&
      timestamps.has(CURRENCIES_CACHE_KEY) &&
      now - (timestamps.get(CURRENCIES_CACHE_KEY) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(CURRENCIES_CACHE_KEY) as CurrenciesResponse;
    }

    try {
      const response = await ApiClient.get<CurrenciesResponse>('/api/currencies');

      // Set response in cache
      data.set(CURRENCIES_CACHE_KEY, response);
      timestamps.set(CURRENCIES_CACHE_KEY, now);

      // Cache individual currencies as well
      if (Array.isArray(response)) {
        response.forEach(currency => {
          data.set(`currency_${currency.id}`, currency);
          timestamps.set(`currency_${currency.id}`, now);
        });
      }

      return response;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(CURRENCIES_CACHE_KEY)) {
        SafeLogger.warn('Returning stale currency data due to API error');
        return data.get(CURRENCIES_CACHE_KEY) as CurrenciesResponse;
      }

      SafeLogger.error('Error fetching currencies:', error);
      throw error;
    }
  }

  /**
   * Fetch a specific currency by ID
   *
   * @param id - The currency ID
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the Currency
   */
  async getCurrency(id: string, forceRefresh = false): Promise<Currency> {
    if (!id) throw new Error('Currency ID is required');

    const now = Date.now();
    const { data, timestamps } = CurrencyService.cache;
    const currencyCacheKey = `currency_${id}`;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(currencyCacheKey) &&
      timestamps.has(currencyCacheKey) &&
      now - (timestamps.get(currencyCacheKey) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(currencyCacheKey) as Currency;
    }

    try {
      // Try to get from getAllCurrencies first to avoid extra API call
      if (!forceRefresh) {
        try {
          const allCurrencies = await this.getAllCurrencies(false);
          const foundCurrency = Array.isArray(allCurrencies)
            ? allCurrencies.find((c: Currency) => c.id === id)
            : undefined;
          if (foundCurrency) return foundCurrency;
        } catch {
          // If this fails, continue with direct API call
        }
      }

      // Direct API call if needed
      const currency = await ApiClient.get<Currency>(`api/currencies/${id}`);

      // Update cache
      data.set(currencyCacheKey, currency);
      timestamps.set(currencyCacheKey, now);

      return currency;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(currencyCacheKey)) {
        SafeLogger.warn(`Returning stale data for currency ${id} due to API error`);
        return data.get(currencyCacheKey) as Currency;
      }

      SafeLogger.error(`Error fetching currency with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Invalidate all currency caches or a specific currency cache
   * @param currencyId Optional currency ID to invalidate specific cache
   */
  invalidateCache(currencyId?: string): void {
    const { data, timestamps } = CurrencyService.cache;

    if (currencyId) {
      // Invalidate only specific currency
      const currencyCacheKey = `currency_${currencyId}`;
      data.delete(currencyCacheKey);
      timestamps.delete(currencyCacheKey);
    } else {
      // Invalidate all currency caches
      data.delete(CURRENCIES_CACHE_KEY);
      timestamps.delete(CURRENCIES_CACHE_KEY);

      // Find and remove all currency_ keys
      const currencyKeys = Array.from(data.keys()).filter(key => key.startsWith('currency_'));
      currencyKeys.forEach(key => {
        data.delete(key);
        timestamps.delete(key);
      });
    }
  }
}

export default new CurrencyService();
