import { useState } from 'react';
import { useLoaderData } from 'react-router';
import urls from '@/utils/urls';
import Uploader from '@/components/ui/utils/Uploader';
import PaginatedTable, { Column } from '@/components/ui/utils/PaginatedTable';
import { InputType } from '@/components/ui/utils/InputType';

interface CurrencySymbol {
  id: string;
  symbol: string;
}

interface Currency {
  id: string;
  name: string;
  shortName: string;
  symbols: CurrencySymbol[];
}

interface LoaderData {
  currencies: Currency[];
}

type InvestmentField = {
  symbol: string;
  description: string;
  currencyId?: string;
};

function Movements() {
  const { currencies } = useLoaderData<LoaderData>();
  const currencyById = new Map(currencies.map((c) => [c.id, c]));
  const [reloadData, setReloadData] = useState<number>(0);

  const IOLInvestmentsUploadEndpoint = urls.iolInvestments.upload.with({
    timezoneOffset: -new Date().getTimezoneOffset() / 60,
  });
  const iolInvestmentsEndpoint = urls.iolInvestments.paginated;

  const onFetchInvestmentsTable = () => {};

  const numericColumn = (columnId: string, label: string, visible: boolean): Column => ({
    id: columnId,
    label: label,
    placeholder,
    header: {
      classes: ['text-end'],
    },
    class: 'text-end',
    editable: true,
    visible: visible,
    mapper: (record: unknown) => {
      const field = (record as Record<string, unknown>)[columnId];
      const num = typeof field === 'number' ? field : Number(String(field));
      return Number.isFinite(num) ? parseFloat(num.toFixed(2)) : num;
    },
    conditionalClass: [
      {
        class: 'text-success fw-bold',
        eval: (record: unknown) => {
          const field = (record as Record<string, unknown>)[columnId];
          const num = typeof field === 'number' ? field : Number(String(field));
          return Number(num) > 0;
        },
      },
      {
        class: 'text-danger fw-bold',
        eval: (record: unknown) => {
          const field = (record as Record<string, unknown>)[columnId];
          const num = typeof field === 'number' ? field : Number(String(field));
          return Number(num) < 0;
        },
      },
    ],
  });

  const placeholder = 'Ingrese un valor';

  const investmentsTableColumns: Column[] = [
    {
      id: 'timeStamp',
      label: 'Fecha',
      placeholder,
      type: InputType.DateTime,
      editable: true,
      visible: true,
      datetime: {
        timeIntervals: 15,
        dateFormat: 'DD/MM/yyyy',
        timeFormat: 'HH:mm',
        placeholder: 'Seleccionar fecha',
      },
      header: {
        style: {
          width: '160px',
        },
      },
    },
    {
      id: 'asset',
      label: 'Activo',
      placeholder,
      editable: true,
      visible: true,
      mapper: (field: unknown) => {
        const f = (field as Record<string, unknown>).asset as InvestmentField;
        return `${f?.symbol ?? ''} - ${f?.description ?? ''}`;
      },
    },
    {
      id: 'currencySymbol',
      label: 'Moneda',
      editable: false,
      visible: true,
      type: InputType.Text,
      mapper: (record: unknown) => {
        const asset = (record as Record<string, unknown>).asset as InvestmentField;
        const currency = asset?.currencyId ? currencyById.get(asset.currencyId) : undefined;
        return currency?.symbols?.[0]?.symbol ?? null;
      },
    },
    numericColumn('alarms', 'Alarmas', false),
    numericColumn('quantity', 'Cantidad', true),
    numericColumn('assets', 'Activos comp.', false),
    numericColumn('dailyVariation', 'Variación diaria', false),
    numericColumn('lastPrice', 'Último precio', true),
    numericColumn('averageBuyPrice', 'Precio prom. compra', true),
    numericColumn('averageReturnPercent', 'Rendimiento (%)', true),
    numericColumn('averageReturn', 'Rendimiento prom.', true),
    numericColumn('valued', 'Valorado', true),
    {
      id: 'valuedConversion',
      label: 'Valorado ($)',
      visible: true,
      editable: false,
      header: { classes: ['text-end'] },
      class: 'text-end',
      mapper: (record: unknown) => {
        const r = record as Record<string, unknown>;
        return parseFloat(Number(r.valuedConversion).toFixed(2));
      },
      conditionalClass: [
        {
          class: 'text-success fw-bold',
          eval: (record: unknown) => Number((record as Record<string, unknown>).valued) > 0,
        },
        {
          class: 'text-danger fw-bold',
          eval: (record: unknown) => Number((record as Record<string, unknown>).valued) < 0,
        },
      ],
    },
  ];

  const enabled = Boolean(iolInvestmentsEndpoint);

  return (
    <>
      <div className="container pt-3 pb-3">
        {!enabled && <div>Cargando...</div>}
        {enabled && (
          <div>
            <hr className="py-1" />
            <Uploader
              url={IOLInvestmentsUploadEndpoint}
              extensions={['.xls', '.xlsx']}
              onSuccess={() => {
                setReloadData((n) => n + 1);
              }}
            />
            <hr className="py-1" />
            <PaginatedTable
              name={'investments-table'}
              url={iolInvestmentsEndpoint}
              rowCount={20}
              admin={{
                endpoint: urls.iolInvestments.endpoint,
                addEnabled: false,
              }}
              onFetch={onFetchInvestmentsTable}
              columns={investmentsTableColumns}
              reloadData={reloadData}
            />
          </div>
        )}
      </div>
    </>
  );
}

export default Movements;
