/**
 * Represents a bank entity
 */
export interface Bank {
  id: string;
  name: string;
  deactivated?: boolean;
}

/**
 * Represents the response from the banks API
 * Note: The API returns an array directly, not wrapped in an object
 */
export type BanksResponse = Bank[];
