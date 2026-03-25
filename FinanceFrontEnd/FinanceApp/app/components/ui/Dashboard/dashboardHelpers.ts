import { Dictionary, toNumber, ValueLike } from '@/utils/common';
import { InputType } from '@/components/ui/utils/InputType';
import { FetchTableColumn } from '@/components/ui/utils/FetchTableColumn';

export const backgroundClasses = [
  'bg-amber-500',
  'bg-blue-500',
  'bg-cyan-500',
  'bg-emerald-500',
  'bg-fuchsia-500',
  'bg-green-500',
  'bg-indigo-500',
  'bg-lime-500',
  'bg-orange-500',
  'bg-purple-500',
  'bg-pink-500',
  'bg-red-500',
  'bg-rose-500',
  'bg-sky-500',
  'bg-teal-500',
  'bg-violet-500',
  'bg-yellow-500',
];

export const moneyFormatter = (r: number) => (typeof r === 'number' ? r : 0).toFixed(2);

export const getSafeValue = (v: unknown) => toNumber(v as ValueLike);

export const safeProp = (obj: unknown, prop: string): unknown => {
  if (obj && typeof obj === 'object') {
    return (obj as Record<string, unknown>)[prop];
  }
  return undefined;
};

export const getOriginOrName = (record: unknown): string => {
  const origin = safeProp(record, 'origin');
  if (origin && typeof origin === 'object') {
    const name = safeProp(origin, 'name');
    if (typeof name === 'string') return name;
  }
  const name = safeProp(record, 'name');
  return typeof name === 'string' ? name : '-';
};

export const getNestedName = (obj: unknown, path: string[]): string => {
  let cur: unknown = obj;
  for (const p of path) {
    cur = safeProp(cur, p);
    if (cur === undefined) return '-';
  }
  return typeof cur === 'string' ? cur : '-';
};

export const getSafeValueFrom = (v: unknown, prop?: string) => {
  const value = prop ? safeProp(v, prop) : v;
  return toNumber(value as ValueLike);
};

export type Mapper = (v: unknown) => number;
export type Reducer = (acc: number, v: unknown) => number;

export const DecimalColumn = (id: string, label: string, mapper?: Mapper, totalsReducer?: Reducer) => {
  const localMapper: Mapper = mapper ?? ((v: unknown) => toNumber(v as ValueLike));
  const localTotalsReducer: Reducer =
    totalsReducer ?? ((acc: number, r: unknown) => acc + localMapper(r));

  return {
    id,
    label,
    class: ['text-end'],
    headerClass: ['text-end'],
    type: InputType.Decimal,
    mapper: localMapper,
    formatter: moneyFormatter,
    totals: {
      formatter: moneyFormatter,
      reducer: localTotalsReducer,
    },
  };
};

export const dollarCalculator = (r: ValueLike, creditCardConversion: unknown) => {
  const sellRate = safeProp(creditCardConversion, 'sellRate') as ValueLike;
  return toNumber(r) * toNumber(sellRate, 1);
};

export const DEBIT_MODULE_PESOS = '4c1ee918-e8f9-4bed-8301-b4126b56cfc0';
export const DEBIT_MODULE_DOLLARS = '03cc66c7-921c-4e05-810e-9764cd365c1d';
export const DEBIT_MODULES = [DEBIT_MODULE_PESOS, DEBIT_MODULE_DOLLARS];

export const DEBIT_BACKGROUND_CLASSES: Dictionary<string> = {
  [DEBIT_MODULE_PESOS]: 'bg-violet-500 text-white',
  [DEBIT_MODULE_DOLLARS]: 'bg-orange-500 text-white',
};
export const DEBIT_TABLE_NAMES: Dictionary<string> = {
  [DEBIT_MODULE_PESOS]: 'debit-pesos-table',
  [DEBIT_MODULE_DOLLARS]: 'debit-dollars-table',
};
export const DEBIT_TABLE_TITLES: Dictionary<string> = {
  [DEBIT_MODULE_PESOS]: 'Débitos Pesos',
  [DEBIT_MODULE_DOLLARS]: 'Débitos Dólares',
};

export const buildSummaryColumns = (symbol: string) => [
  new FetchTableColumn('label', 'Dato'),
  DecimalColumn('value', 'Monto', getSafeValue),
  DecimalColumn('convertedValue', `Monto (${symbol})`, (v: unknown) => getSafeValueFrom(v, 'convertedValue')),
];

export const buildFundsColumns = (symbol: string) => {
  const { totals: _t, type: _ty, ...montoCol } = DecimalColumn('value', 'Monto', getSafeValue);
  return [
    new FetchTableColumn('label', 'Origen'),
    montoCol,
    DecimalColumn('quoteCurrencyValue', `Monto (${symbol})`, (v: unknown) => getSafeValueFrom(v, 'quoteCurrencyValue')),
  ];
};

export const buildExpensesColumns = (symbol: string) => [
  new FetchTableColumn('label', 'Gasto/Servicio'),
  DecimalColumn('value', `Monto (${symbol})`, getSafeValue),
];

export const buildDebitColumns = () => [
  new FetchTableColumn('origin', 'Débito/Servicio', (record: unknown) => getOriginOrName(record)),
  DecimalColumn('amount', 'Monto', (a: unknown) => getSafeValueFrom(a, 'amount')),
];

export const buildInvestmentsColumns = (symbol: string) => [
  new FetchTableColumn('symbol', 'Activo', (record: unknown) => getNestedName(record, ['label'])),
  DecimalColumn('averageReturn', 'Rend. prom.', (v: unknown) => {
    const val = safeProp(v, 'averageReturn');
    return typeof val === 'number' ? val : getSafeValueFrom(v);
  }),
  DecimalColumn('valued', 'Valorado', (v: unknown) => {
    const val = safeProp(v, 'valued');
    return typeof val === 'number' ? val : getSafeValueFrom(v);
  }),
  DecimalColumn('valuedDefaultCurrency', `Valorado (${symbol})`, (v: unknown) => {
    const val = safeProp(v, 'valuedDefaultCurrency');
    return typeof val === 'number' ? val : getSafeValueFrom(v);
  }),
];

export const buildCurrencyRatesColumns = () => [
  new FetchTableColumn('label', 'Base', (v: unknown) => getNestedName(v, ['baseCurrency', 'name']), (v: unknown) => getNestedName(v, ['baseCurrency', 'name'])),
  new FetchTableColumn('label', 'Cotización', (v: unknown) => getNestedName(v, ['quoteCurrency', 'name']), (v: unknown) => getNestedName(v, ['quoteCurrency', 'name'])),
  DecimalColumn('value', 'Compra', (v: unknown) => getSafeValueFrom(safeProp(v, 'buyRate'))),
  DecimalColumn('value', 'Venta', (v: unknown) => getSafeValueFrom(safeProp(v, 'sellRate'))),
];

export const buildCreditCardColumns = (latestCurrencyExchangeRates: unknown) => [
  {
    id: 'timestamp',
    label: 'Fecha',
    mapper: (v: unknown) => {
      const date = safeProp(v, 'timestamp') as string;
      return date ? new Date(date).toLocaleDateString() : '';
    },
  },
  { id: 'concept', label: 'Concepto' },
  DecimalColumn('amount', 'Monto', (v: unknown) => toNumber(safeProp(v, 'amount') as ValueLike, 0)),
  DecimalColumn('amountDollars', 'Dólares', (v: unknown) => toNumber(safeProp(v, 'amountDollars') as ValueLike, 0)),
  DecimalColumn(
    'totalAmount',
    'Total',
    (v: unknown) => {
      if (!v) return 0;
      const amount = toNumber(safeProp(v, 'amount') as ValueLike, 0);
      const amountDollars = toNumber(safeProp(v, 'amountDollars') as ValueLike, 0);
      return amount + dollarCalculator(amountDollars, latestCurrencyExchangeRates);
    },
    (acc: number, v: unknown) => {
      if (!v) return acc;
      const amount = toNumber(safeProp(v, 'amount') as ValueLike, 0);
      const amountDollars = toNumber(safeProp(v, 'amountDollars') as ValueLike, 0);
      return acc + amount + dollarCalculator(amountDollars, latestCurrencyExchangeRates);
    }
  ),
];
