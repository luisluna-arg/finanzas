import React from 'react';
import urls from '@/utils/urls';
import dayjs from 'dayjs';
import CommonUtils from '@/utils/common';
import Picker from '@/components/ui/utils/Picker';
import { Label } from '@/components/ui/shadcn/label';
import { useLoaderData, useLocation, useNavigate } from 'react-router';
import { InputType } from '@/components/ui/utils/InputType';
import PaginatedTable, {
  Column,
  ConditionalClass,
  Row,
} from '@/components/ui/utils/PaginatedTable';
import { cn } from '@/lib/utils';
import type { CurrencyExchangeRateRecord, CurrencyData } from '@/types/currencyExchangeRate';
import type { CatalogItem } from '@/types/catalog';

interface LoaderData {
  currencies: CatalogItem[];
  data: CurrencyExchangeRateRecord[] | { items: CurrencyExchangeRateRecord[]; totalPages: number };
  baseCurrencyId: string;
  quoteCurrencyId: string;
}

interface CurrencyExchangeRateRow extends CurrencyExchangeRateRecord, Omit<Row, 'id'> {}

const dateFormat = 'DD/MM/YYYY';

const CurrencyExchangeRates: React.FC = () => {
  const { currencies, data, baseCurrencyId, quoteCurrencyId } = useLoaderData<LoaderData>();
  const navigate = useNavigate();
  const location = useLocation();

  const reload = ({
    currentBaseCurrencyId,
    currentQuoteCurrencyId,
  }: {
    currentBaseCurrencyId?: string;
    currentQuoteCurrencyId?: string;
  }) => {
    const params = CommonUtils.Params({
      baseCurrencyId: currentBaseCurrencyId ?? baseCurrencyId,
      quoteCurrencyId: currentQuoteCurrencyId ?? quoteCurrencyId,
    });
    navigate(`${location.pathname}?${params}`);
  };

  const numericHeader = {
    classes: 'text-end',
    style: { width: '180px' },
  };

  const positiveValueClass: ConditionalClass = {
    class: 'text-success fw-bold',
    eval: (field: unknown) => field != null && Number(String(field)) > 0,
  };

  const TableColumns: Column<CurrencyExchangeRateRow>[] = [
    {
      id: 'timeStamp',
      label: 'Fecha',
      placeholder: 'Fecha',
      type: InputType.DateTime,
      editable: {
        defaultValue: () => dayjs().format(dateFormat),
      },
      datetime: {
        timeFormat: 'HH:mm',
        timeIntervals: 15,
        dateFormat: dateFormat,
        placeholder: 'Seleccionar fecha',
      },
      header: {
        style: { width: '160px' },
      },
    },
    {
      id: 'baseCurrency',
      label: 'Moneda base',
      placeholder: 'Seleccione moneda base',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.currencies.endpoint,
      mapper: {
        id: 'id',
        label: (record) => record.baseCurrency.name,
      },
    },
    {
      id: 'quoteCurrency',
      label: 'Moneda cotización',
      placeholder: 'Seleccione moneda cotización',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.currencies.endpoint,
      mapper: {
        id: 'id',
        label: (record) => record.quoteCurrency.name,
      },
    },
    {
      id: 'buyRate',
      label: 'Compra',
      placeholder: 'Compra',
      type: InputType.Decimal,
      min: 0.0,
      header: numericHeader,
      class: 'text-end',
      editable: {
        defaultValue: 0.0,
      },
      mapper: (record) => (record.buyRate != null ? Number(record.buyRate) : null),
      conditionalClass: positiveValueClass,
    },
    {
      id: 'sellRate',
      label: 'Venta',
      placeholder: 'Venta',
      type: InputType.Decimal,
      min: 0.0,
      header: numericHeader,
      class: 'text-end',
      editable: {
        defaultValue: 0.0,
      },
      mapper: (record) => (record.sellRate != null ? Number(record.sellRate) : null),
      conditionalClass: positiveValueClass,
    },
  ];

  const paginatedData = Array.isArray(data)
    ? { items: data, totalPages: 1 }
    : data;

  return (
    <div className={cn(['py-10', 'px-40'])}>
      <div className="flex flex-row justify-center gap-10">
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="base-currency-picker">Moneda base</Label>
          <Picker
            id="base-currency-picker"
            placeholder="Moneda base..."
            value={baseCurrencyId}
            data={currencies}
            mapper={{
              id: 'id',
              label: (record: unknown) => `${(record as CatalogItem).name}`,
            }}
            onChange={(picker: { value: string }) => reload({ currentBaseCurrencyId: picker.value })}
            className="w-60"
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <Label htmlFor="quote-currency-picker">Moneda cotización</Label>
          <Picker
            id="quote-currency-picker"
            placeholder="Moneda cotización..."
            value={quoteCurrencyId}
            data={currencies}
            mapper={{
              id: 'id',
              label: (record: unknown) => `${(record as CatalogItem).name}`,
            }}
            onChange={(picker: { value: string }) => reload({ currentQuoteCurrencyId: picker.value })}
            className="w-60"
          />
        </div>
      </div>
      <hr className={cn('py-1', 'mb-5', 'mt-5')} />
      <PaginatedTable<CurrencyExchangeRateRow>
        name="currency-exchange-rates-table"
        columns={TableColumns}
        data={paginatedData}
        onAdd={() => reload({})}
        onDelete={() => reload({})}
        admin={{
          endpoint: urls.currencyExchangeRates.endpoint,
          key: [
            {
              id: 'BaseCurrencyId',
              value: baseCurrencyId,
            },
            {
              id: 'QuoteCurrencyId',
              value: quoteCurrencyId,
            },
          ],
        }}
      />
    </div>
  );
};

export default CurrencyExchangeRates;
