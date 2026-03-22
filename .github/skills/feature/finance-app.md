# FinanceApp Feature — React Router SSR Frontend

Step-by-step guide for wiring a new feature into the `FinanceFrontEnd/FinanceApp` application.
This app uses React Router v7 with a server-side loader pattern (similar to Remix / Next.js).

---

## Step 1 — URL Registry

**File:** `FinanceFrontEnd/FinanceApp/app/utils/urls.ts`

Add an entry for each backend endpoint the feature needs:

```ts
<entityNamePlural>: {
  get endpoint() {
    return new BackendUrl(`${getApiBaseUrl()}/<entity-route>`);
  },
  get paginated() {
    return new BackendUrl(`${getApiBaseUrl()}/<entity-route>/paginated`);
  },
},
```

---

## Step 2 — Data Query Class

**Location:** `FinanceFrontEnd/FinanceApp/app/data/queries/<EntityName>sQuery.ts`

```ts
import urls from "@/utils/urls";
import { Agent } from "https";
import { BaseQuery } from "@/data/base/BaseQuery";

export class <EntityName>sQuery extends BaseQuery {
  constructor(httpsAgent: Agent, accessToken: string) {
    super(httpsAgent, accessToken, {
      get: urls.<entityNamePlural>.endpoint,
    });
  }
}
```

**File:** `FinanceFrontEnd/FinanceApp/app/data/BackendClient.ts`

Register the new query:

1. Import at the top:
   ```ts
   import { <EntityName>sQuery } from "./queries/<EntityName>sQuery";
   ```
2. Add private field:
   ```ts
   private <EntityName>sQuery: <EntityName>sQuery;
   ```
3. Instantiate in the constructor:
   ```ts
   this.<EntityName>sQuery = new <EntityName>sQuery(httpsAgent, this.AccessToken);
   ```
4. Expose via getter:
   ```ts
   Get<EntityName>sQuery() { return this.<EntityName>sQuery; }
   ```

---

## Step 3 — Route File

**Location:** `FinanceFrontEnd/FinanceApp/app/routes/<entityNamePlural>.tsx`

```tsx
import { getBackendClient } from "@/data/getBackendClient";
import <EntityName>s from "@/components/ui/<EntityName>s";
import { LoaderFunctionArgs } from "react-router";
import { requireAuth } from "@/services/auth/session.server";

export const loader = async ({ request }: LoaderFunctionArgs) => {
    const user = await requireAuth(request);

    if (!user.accessToken) {
        throw new Error("No access token available");
    }

    const client = await getBackendClient(user.accessToken!);

    // fetch any data that the page component needs upfront
    const items = await client.Get<EntityName>sQuery().get();

    return { items };
};

export const meta = () => [
    { title: "<Page Title>" },
];

export default <EntityName>s;
```

**File:** `FinanceFrontEnd/FinanceApp/app/routes.ts`

Register the new route following the existing entries:

```ts
route("<path>", "<entityNamePlural>.tsx"),
```

---

## Step 4 — UI Component

**Location:** `FinanceFrontEnd/FinanceApp/app/components/ui/<EntityName>s/index.tsx`

General conventions:
- Use `useLoaderData<LoaderData>()` to access data from the loader
- Use `PaginatedTable` with a typed `Column[]` array for lists
- Use shadcn primitives (`Button`, `Input`, `Select`, etc.) for forms
- Wire mutations via `fetch` to the proxy route (`/api/proxy` → `POST /api/<entity-route>`)

Minimal structure:

```tsx
import React from 'react';
import { useLoaderData } from 'react-router';

interface LoaderData {
  items: { id: string; /* ... */ }[];
}

const <EntityName>s: React.FC = () => {
  const { items } = useLoaderData<LoaderData>();

  return (
    <div>
      {/* render list / table / form */}
    </div>
  );
};

export default <EntityName>s;
```

---

## Step 5 — Verify

Run both checks from `FinanceFrontEnd/FinanceApp` and fix any errors before committing:

```powershell
npm run typecheck
npm run lint
```

## Checklist

- [ ] URL entry added in `utils/urls.ts`
- [ ] Query class created in `data/queries/`
- [ ] Query registered in `BackendClient.ts` (field + constructor + getter)
- [ ] Route file created in `app/routes/`
- [ ] Route registered in `app/routes.ts`
- [ ] UI component created in `app/components/ui/<EntityName>s/`
- [ ] `npm run typecheck` passes with no errors
- [ ] `npm run lint` passes with no errors
