# API Services

This directory contains the API client services for interacting with the FinanzaBackend API.

## Structure

- `ApiClient.ts` - Base HTTP client for making API requests
- `FundService.ts` - Service for fund-related API endpoints
- `UserService.ts` - Service for user-related API endpoints
- `types/` - TypeScript interfaces for API requests and responses
- `index.ts` - Exports all services for easy importing

## Usage

Import the services in your components:

```typescript
import { FundService } from "../services";

// Then use them in your component
const fetchData = async () => {
  try {
    const funds = await FundService.getCurrentFunds();
    // Do something with the funds data
  } catch (error) {
    console.error("Error fetching funds:", error);
  }
};
```

## API Endpoints

- `api/summary/currentFunds` - Get current funds summary
  - Optional query parameters:
    - `dailyUse` (boolean) - Filter by daily use flag
    - `currencyId` (string) - Filter by currency ID
