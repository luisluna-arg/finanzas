# FinanceFunds Feature — Vite SPA Frontend

Step-by-step guide for adding a new feature to `FinanceFrontEnd/FinanceFunds`.
This app is a Vite + React SPA with Mantine UI, Auth0 authentication, and a direct `ApiClient` for backend calls.

---

## Step 1 — Types

**Location:** `FinanceFrontEnd/FinanceFunds/src/services/types/<EntityName>Types.ts`

```ts
export interface <EntityName> {
  id: string;
  // ...
}

export interface <EntityName>Response {
  data: <EntityName>;
}

export interface <EntityName>sResponse {
  data: <EntityName>[];
}

export interface Create<EntityName>Request {
  // fields required to create
}

export interface Update<EntityName>Request {
  id: string;
  // fields required to update
}
```

---

## Step 2 — Service

**Location:** `FinanceFrontEnd/FinanceFunds/src/services/<EntityName>Service.ts`

```ts
import ApiClient from './ApiClient';
import type {
  Create<EntityName>Request,
  <EntityName>,
  <EntityName>Response,
  <EntityName>sResponse,
} from './types/<EntityName>Types';
import SafeLogger from '@/utils/SafeLogger';

const <EntityName>Service = {
  getAll: async (): Promise<<EntityName>sResponse> => {
    try {
      return await ApiClient.get<<EntityName>sResponse>('/api/<entity-route>');
    } catch (error) {
      SafeLogger.error('Error fetching <entityNamePlural>:', error);
      throw error;
    }
  },

  getById: async (id: string): Promise<<EntityName>Response> => {
    try {
      return await ApiClient.get<<EntityName>Response>(`/api/<entity-route>/${id}`);
    } catch (error) {
      SafeLogger.error(`Error fetching <entityName> ${id}:`, error);
      throw error;
    }
  },

  create: async (data: Create<EntityName>Request): Promise<<EntityName>> => {
    try {
      return await ApiClient.post<<EntityName>>('/api/<entity-route>', data);
    } catch (error) {
      SafeLogger.error('Error creating <entityName>:', error);
      throw error;
    }
  },

  update: async (id: string, data: Partial<Create<EntityName>Request>): Promise<<EntityName>> => {
    try {
      return await ApiClient.put<<EntityName>>(`/api/<entity-route>/${id}`, data);
    } catch (error) {
      SafeLogger.error(`Error updating <entityName> ${id}:`, error);
      throw error;
    }
  },

  delete: async (id: string): Promise<void> => {
    try {
      await ApiClient.delete(`/api/<entity-route>/${id}`);
    } catch (error) {
      SafeLogger.error(`Error deleting <entityName> ${id}:`, error);
      throw error;
    }
  },
};

export default <EntityName>Service;
```

**File:** `FinanceFrontEnd/FinanceFunds/src/services/index.ts`

Export the new service:
```ts
export { default as <EntityName>Service } from './<EntityName>Service';
```

---

## Step 3 — Create Modal (optional)

**Location:** `FinanceFrontEnd/FinanceFunds/src/components/Create<EntityName>Modal.tsx`

Follow the pattern of `CreateFundModal.tsx`:
- Accept `opened` + `onClose` + `onCreated` props
- Use Mantine `Modal`, `TextInput`, `NumberInput`, `Select`, `Button`
- Call `<EntityName>Service.create(...)` on submit
- Call `onCreated()` on success to trigger a parent refresh

---

## Step 4 — Page Component

**Location:** `FinanceFrontEnd/FinanceFunds/src/pages/<EntityName>sDashboard.tsx`

Conventions:
- Use `useState` + `useEffect` for data loading
- Use `useCallback` for stable handler references
- Use `useMemo` for derived/display config values
- Mantine components: `Card`, `Table`, `Stack`, `Group`, `Loader`, `Center`, `Button`, `ScrollArea`
- TablerIcons for iconography
- Call `SafeLogger.error(...)` instead of `console.error`

Minimal structure:

```tsx
import { useState, useEffect, useCallback } from 'react';
import { Title, Card, Table, Loader, Center, Button } from '@mantine/core';
import <EntityName>Service from '@/services/<EntityName>Service';
import SafeLogger from '@/utils/SafeLogger';
import type { <EntityName> } from '@/services/types/<EntityName>Types';

const <EntityName>sDashboard = () => {
  const [items, setItems] = useState<<EntityName>[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadItems = useCallback(async () => {
    try {
      setLoading(true);
      const response = await <EntityName>Service.getAll();
      setItems(response.data);
    } catch (err) {
      SafeLogger.error('Failed to load <entityNamePlural>', err);
      setError('Failed to load <entityNamePlural>');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadItems(); }, [loadItems]);

  if (loading) return <Center><Loader /></Center>;

  return (
    <Card>
      <Title><EntityName>s</Title>
      {/* Table / form */}
    </Card>
  );
};

export default <EntityName>sDashboard;
```

**File:** `FinanceFrontEnd/FinanceFunds/src/pages/index.ts`

Export the new page:
```ts
export { default as <EntityName>sDashboard } from './<EntityName>sDashboard';
```

---

## Step 5 — Register in App Router

**File:** `FinanceFrontEnd/FinanceFunds/src/App.tsx`

Add a route for the new page following the existing pattern:

```tsx
import { <EntityName>sDashboard } from '@/pages';

// inside the Routes / router config:
<Route path="/<entity-route>" element={<<EntityName>sDashboard />} />
```

Add a navigation link in `src/components/Navigation.tsx` if the page should appear in the sidebar.

---

## Checklist

- [ ] Types file created in `services/types/`
- [ ] Service created and exported from `services/index.ts`
- [ ] Create modal added (if CRUD is needed)
- [ ] Dashboard page created and exported from `pages/index.ts`
- [ ] Route registered in `App.tsx`
- [ ] Navigation link added (if applicable)
