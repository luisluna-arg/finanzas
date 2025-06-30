# Currency Configuration

This document explains how to configure currency IDs for the Finance Funds application.

## Overview

The application uses specific currency IDs (GUIDs) to determine decimal formatting:
- **USD and ARS**: Maximum 2 decimal places
- **All other currencies**: Maximum 8 decimal places

## Configuration

### Environment Variables

You can override the default currency IDs using environment variables:

```bash
# US Dollar currency ID
VITE_USD_CURRENCY_ID=your-usd-currency-guid

# Argentine Peso currency ID  
VITE_ARS_CURRENCY_ID=your-ars-currency-guid
```

### Default Values

If no environment variables are provided, the application uses these default values:
- USD: `efbf50bc-34d4-43e9-96f9-9f6213ea11b5`
- ARS: `6d189135-7040-45a1-b713-b1aa6cad1720`

### Setup Instructions

1. Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

2. Update the currency IDs in `.env` to match your backend database:
   ```bash
   VITE_USD_CURRENCY_ID=your-actual-usd-guid
   VITE_ARS_CURRENCY_ID=your-actual-ars-guid
   ```

3. Restart the development server for changes to take effect.

## Adding New Limited Decimal Currencies

To add more currencies that should use 2 decimal places instead of 8:

1. Add the new environment variable to `.env.example` and your `.env` file
2. Update `src/constants/currencies.ts`:
   ```typescript
   export const CURRENCY_IDS = {
     USD: import.meta.env.VITE_USD_CURRENCY_ID || DEFAULT_USD_ID,
     ARS: import.meta.env.VITE_ARS_CURRENCY_ID || DEFAULT_ARS_ID,
     EUR: import.meta.env.VITE_EUR_CURRENCY_ID || DEFAULT_EUR_ID, // Add new currency
   } as const;

   export const LIMITED_DECIMAL_CURRENCIES = [
     CURRENCY_IDS.USD, 
     CURRENCY_IDS.ARS,
     CURRENCY_IDS.EUR, // Add to limited decimals list
   ];
   ```

## File Structure

- `src/constants/currencies.ts` - Currency configuration and utility functions
- `.env.example` - Example environment variables
- `.env` - Your local environment variables (not committed to git)
