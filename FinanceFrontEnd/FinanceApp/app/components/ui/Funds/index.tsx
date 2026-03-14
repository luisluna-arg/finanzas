import React from 'react';
import { useLoaderData, useLocation, useNavigate } from 'react-router';
import dayjs from 'dayjs';
import urls from '@/utils/urls';
import BankCurrencySelector from '@/components/ui/utils/BankCurrencySelector';
import PaginatedTable, { Column, Row } from '@/components/ui/utils/PaginatedTable';
import { InputType } from '@/components/ui/utils/InputType';
import CommonUtils, { toNumber } from '@/utils/common';
import { cn } from '@/lib/utils';
import type { FundRecord } from '@/types/fund';

// Define types for the props and states
interface PickerData {
  id: string;
  name: string;
}

interface LoaderData {
  banks: PickerData[];
  currencies: PickerData[];
  data: FundRecord[];
  bankId: string;
  currencyId: string;
}

const dateFormat = 'DD/MM/YYYY';

const Funds: React.FC = () => {
  const { banks, currencies, data, bankId, currencyId } = useLoaderData<LoaderData>();
  const navigate = useNavigate();
  const location = useLocation();

  const reload = ({
    currentBankId,
    currentCurrencyId,
  }: {
    currentBankId?: string;
    currentCurrencyId?: string;
  }) => {
    const params = CommonUtils.Params({
      bankId: currentBankId ?? bankId,
      currencyId: currentCurrencyId ?? currencyId,
    });
    navigate(`${location.pathname}?${params}`);
  };

  const onBankPickerChange = (picker: { value: string }) => {
    reload({ currentBankId: picker.value });
  };

  const onCurrencyPickerChange = (picker: { value: string }) => {
    reload({ currentCurrencyId: picker.value });
  };

  const valueConditionalClass = {
    class: 'text-success fw-bold',
    eval: (field: unknown) => field != null && toNumber(field) > 0,
  };

  const valueMapper = (field: unknown) => (field != null ? toNumber(field) : null);

  const numericHeader = {
    classes: 'text-end',
    style: {
      width: '140px',
    },
  };

  const TableColumns: Column[] = [
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
        style: {
          width: '160px',
        },
      },
    },
    {
      id: 'bank',
      label: 'Banco/Entidad',
      placeholder: 'Seleccione un banco',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.banks.endpoint,
      mapper: {
        id: 'id',
        label: (record) => (record as unknown as FundRecord).bank.name,
      },
    },
    {
      id: 'currency',
      label: 'Moneda',
      placeholder: 'Seleccione una moneda',
      editable: false,
      type: InputType.Dropdown,
      endpoint: urls.currencies.endpoint,
      mapper: {
        id: 'id',
        label: (record) => (record as unknown as FundRecord).currency.name,
      },
    },
    {
      id: 'amount',
      label: 'Monto',
      placeholder: 'Monto',
      type: InputType.Decimal,
      min: 0.0,
      header: numericHeader,
      class: 'text-end',
      editable: {
        defaultValue: 0.0,
      },
      mapper: valueMapper,
      conditionalClass: valueConditionalClass,
    },
    {
      id: 'dailyUse',
      label: 'Use diario',
      placeholder: 'Use diario',
      type: InputType.Boolean,
      header: {
        classes: '',
        style: {
          width: '80px',
        },
      },
      class: 'text-center',
      editable: {
        defaultValue: false,
      },
    },
  ];

  const paginatedData = Array.isArray(data)
    ? {
        items: data,
        totalPages: 1,
      }
    : data;

  return (
    <div className={cn(['py-10', 'px-40'])}>
      <BankCurrencySelector
        banks={banks}
        currencies={currencies}
        bankId={bankId}
        currencyId={currencyId}
        onBankChange={onBankPickerChange}
        onCurrencyChange={onCurrencyPickerChange}
      />
      <PaginatedTable
        name={'funds-table'}
        columns={TableColumns}
        data={paginatedData}
        onAdd={() => reload({})}
        onDelete={() => reload({})}
        admin={{
          endpoint: urls.funds.endpoint,
          key: [
            {
              id: 'BankId',
              value: bankId,
            },
            {
              id: 'CurrencyId',
              value: currencyId,
            },
          ],
        }}
      />
    </div>
  );
};

export default Funds;
