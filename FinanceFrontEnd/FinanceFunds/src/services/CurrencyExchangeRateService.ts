import ApiClient from './ApiClient';
import type {
  CurrencyExchangeRate,
  CurrencyExchangeRatesResponse,
  CreateCurrencyExchangeRateCommand,
  UpdateCurrencyExchangeRateCommand,
  GetCurrencyExchangeRatesQuery,
} from './types/CurrencyExchangeRateTypes';
import SafeLogger from '@/utils/SafeLogger';

// Define cache keys
const EXCHANGE_RATES_CACHE_KEY = 'all_exchange_rates';
const LATEST_EXCHANGE_RATES_CACHE_KEY = 'latest_exchange_rates';

/**
 * Service for interacting with currency exchange rate-related API endpoints
 */
class CurrencyExchangeRateService {
  // Static shared cache storage
  private static cache: {
    data: Map<string, unknown>;
    timestamps: Map<string, number>;
  } = {
    data: new Map<string, unknown>(),
    timestamps: new Map(),
  };

  // Cache expiration in milliseconds (5 minutes for exchange rates as they change frequently)
  private readonly CACHE_EXPIRATION = 5 * 60 * 1000;

  /**
   * Fetch all currency exchange rates from the API
   *
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the CurrencyExchangeRatesResponse
   */
  async getAllExchangeRates(forceRefresh = false): Promise<CurrencyExchangeRatesResponse> {
    const now = Date.now();
    const { data, timestamps } = CurrencyExchangeRateService.cache;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(EXCHANGE_RATES_CACHE_KEY) &&
      timestamps.has(EXCHANGE_RATES_CACHE_KEY) &&
      now - (timestamps.get(EXCHANGE_RATES_CACHE_KEY) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(EXCHANGE_RATES_CACHE_KEY) as CurrencyExchangeRatesResponse;
    }

    try {
      const response = await ApiClient.get<CurrencyExchangeRatesResponse>(
        '/api/currencies/exchange-rates'
      );

      // Set response in cache
      data.set(EXCHANGE_RATES_CACHE_KEY, response);
      timestamps.set(EXCHANGE_RATES_CACHE_KEY, now);

      // Cache individual exchange rates as well
      if (Array.isArray(response)) {
        response.forEach(rate => {
          data.set(`exchange_rate_${rate.id}`, rate);
          timestamps.set(`exchange_rate_${rate.id}`, now);
        });
      }

      return response;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(EXCHANGE_RATES_CACHE_KEY)) {
        SafeLogger.warn('Returning stale exchange rates data due to API error');
        return data.get(EXCHANGE_RATES_CACHE_KEY) as CurrencyExchangeRatesResponse;
      }

      SafeLogger.error('Error fetching exchange rates:', error);
      throw error;
    }
  }

  /**
   * Fetch latest currency exchange rates from the API
   *
   * @param query - Query parameters for filtering
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the CurrencyExchangeRatesResponse
   */
  async getLatestExchangeRates(
    query: GetCurrencyExchangeRatesQuery = {},
    forceRefresh = false
  ): Promise<CurrencyExchangeRatesResponse> {
    const now = Date.now();
    const { data, timestamps } = CurrencyExchangeRateService.cache;
    const cacheKey = `${LATEST_EXCHANGE_RATES_CACHE_KEY}_${JSON.stringify(query)}`;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(cacheKey) &&
      timestamps.has(cacheKey) &&
      now - (timestamps.get(cacheKey) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(cacheKey) as CurrencyExchangeRatesResponse;
    }

    try {
      const searchParams = new URLSearchParams();
      if (query.quoteCurrencyShortName)
        searchParams.append('quoteCurrencyShortName', query.quoteCurrencyShortName);
      if (query.baseCurrencyShortName)
        searchParams.append('baseCurrencyShortName', query.baseCurrencyShortName);

      const url = `/api/currencies/exchange-rates/latest${searchParams.toString() ? `?${searchParams.toString()}` : ''}`;
      const response = await ApiClient.get<CurrencyExchangeRatesResponse>(url);

      // Set response in cache
      data.set(cacheKey, response);
      timestamps.set(cacheKey, now);

      return response;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(cacheKey)) {
        SafeLogger.warn('Returning stale latest exchange rates data due to API error');
        return data.get(cacheKey) as CurrencyExchangeRatesResponse;
      }

      SafeLogger.error('Error fetching latest exchange rates:', error);
      throw error;
    }
  }

  /**
   * Fetch a specific currency exchange rate by ID
   *
   * @param id - The exchange rate ID
   * @param forceRefresh - Whether to bypass cache and force a fresh API call
   * @returns Promise with the CurrencyExchangeRate
   */
  async getExchangeRate(id: string, forceRefresh = false): Promise<CurrencyExchangeRate> {
    if (!id) throw new Error('Exchange rate ID is required');

    const now = Date.now();
    const { data, timestamps } = CurrencyExchangeRateService.cache;
    const cacheKey = `exchange_rate_${id}`;

    // Check cache validity
    if (
      !forceRefresh &&
      data.has(cacheKey) &&
      timestamps.has(cacheKey) &&
      now - (timestamps.get(cacheKey) || 0) < this.CACHE_EXPIRATION
    ) {
      return data.get(cacheKey) as CurrencyExchangeRate;
    }

    try {
      const exchangeRate = await ApiClient.get<CurrencyExchangeRate>(
        `/api/currencies/exchange-rates/${id}`
      );

      // Update cache
      data.set(cacheKey, exchangeRate);
      timestamps.set(cacheKey, now);

      return exchangeRate;
    } catch (error) {
      // If we have stale data, return it rather than failing completely
      if (data.has(cacheKey)) {
        SafeLogger.warn(`Returning stale data for exchange rate ${id} due to API error`);
        return data.get(cacheKey) as CurrencyExchangeRate;
      }

      SafeLogger.error(`Error fetching exchange rate with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new currency exchange rate
   *
   * @param command - The create command
   * @returns Promise with the created CurrencyExchangeRate
   */
  async createExchangeRate(
    command: CreateCurrencyExchangeRateCommand
  ): Promise<CurrencyExchangeRate> {
    try {
      const response = await ApiClient.post<CurrencyExchangeRate>(
        '/api/currencies/exchange-rates',
        command
      );

      // Invalidate relevant caches
      this.invalidateCache();

      return response;
    } catch (error) {
      SafeLogger.error('Error creating exchange rate:', error);
      throw error;
    }
  }

  /**
   * Update an existing currency exchange rate
   *
   * @param command - The update command
   * @returns Promise with the updated CurrencyExchangeRate
   */
  async updateExchangeRate(
    command: UpdateCurrencyExchangeRateCommand
  ): Promise<CurrencyExchangeRate> {
    try {
      const response = await ApiClient.put<CurrencyExchangeRate>(
        '/api/currencies/exchange-rates',
        command
      );

      // Invalidate relevant caches
      this.invalidateCache(command.id);

      return response;
    } catch (error) {
      SafeLogger.error('Error updating exchange rate:', error);
      throw error;
    }
  }

  /**
   * Delete a currency exchange rate
   *
   * @param id - The exchange rate ID to delete
   * @returns Promise void
   */
  async deleteExchangeRate(id: string): Promise<void> {
    if (!id) throw new Error('Exchange rate ID is required');

    try {
      await ApiClient.delete(`/api/currencies/exchange-rates`, { id });

      // Invalidate relevant caches
      this.invalidateCache(id);
    } catch (error) {
      SafeLogger.error(`Error deleting exchange rate with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Activate a currency exchange rate
   *
   * @param id - The exchange rate ID to activate
   * @returns Promise void
   */
  async activateExchangeRate(id: string): Promise<void> {
    if (!id) throw new Error('Exchange rate ID is required');

    try {
      await ApiClient.patch(`/api/currencies/exchange-rates/activate/${id}`);

      // Invalidate relevant caches
      this.invalidateCache(id);
    } catch (error) {
      SafeLogger.error(`Error activating exchange rate with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Deactivate a currency exchange rate
   *
   * @param id - The exchange rate ID to deactivate
   * @returns Promise void
   */
  async deactivateExchangeRate(id: string): Promise<void> {
    if (!id) throw new Error('Exchange rate ID is required');

    try {
      await ApiClient.patch(`/api/currencies/exchange-rates/deactivate/${id}`);

      // Invalidate relevant caches
      this.invalidateCache(id);
    } catch (error) {
      SafeLogger.error(`Error deactivating exchange rate with ID ${id}:`, error);
      throw error;
    }
  }

  /**
   * Invalidate all exchange rate caches or a specific exchange rate cache
   * @param exchangeRateId Optional exchange rate ID to invalidate specific cache
   */
  invalidateCache(exchangeRateId?: string): void {
    const { data, timestamps } = CurrencyExchangeRateService.cache;

    if (exchangeRateId) {
      // Invalidate only specific exchange rate
      const cacheKey = `exchange_rate_${exchangeRateId}`;
      data.delete(cacheKey);
      timestamps.delete(cacheKey);
    } else {
      // Invalidate all exchange rate caches
      data.delete(EXCHANGE_RATES_CACHE_KEY);
      timestamps.delete(EXCHANGE_RATES_CACHE_KEY);

      // Find and remove all exchange rate related keys
      const exchangeRateKeys = Array.from(data.keys()).filter(
        key => key.startsWith('exchange_rate_') || key.startsWith(LATEST_EXCHANGE_RATES_CACHE_KEY)
      );
      exchangeRateKeys.forEach(key => {
        data.delete(key);
        timestamps.delete(key);
      });
    }
  }
}

export default new CurrencyExchangeRateService();
