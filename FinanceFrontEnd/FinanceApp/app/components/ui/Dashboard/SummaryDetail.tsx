import FetchTable from '@/components/ui/utils/FetchTable';
import CustomButton from '@/components/ui/utils/CustomButton';
import urls from '@/utils/urls';
import {
  buildFundsColumns,
  buildInvestmentsColumns,
  buildSummaryColumns,
  getSafeValueFrom,
  moneyFormatter,
} from './dashboardHelpers';

type Props = {
  onClose: () => void;
  tableClasses: string[];
  tableContainer: string;
  defaultCurrencySymbol: string;
};

export default function SummaryDetail({ onClose, tableClasses, tableContainer, defaultCurrencySymbol }: Props) {
  const summaryColumns = buildSummaryColumns(defaultCurrencySymbol);
  const investmentsColumns = buildInvestmentsColumns(defaultCurrencySymbol);
  const fundsColumns = buildFundsColumns(defaultCurrencySymbol);

  return (
    <div>
      <div className="flex justify-end mb-4">
        <CustomButton onClick={onClose} className="bg-gray-500 text-white hover:bg-gray-600">
          Ver menos
        </CustomButton>
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          {urls.summary.general && (
            <div className={tableContainer}>
              <FetchTable
                name="Summary"
                title={{ text: `Resúmen`, class: `text-center bg-blue-500 text-white` }}
                url={urls.summary.general.with({ DailyUse: true })}
                columns={summaryColumns}
                classes={tableClasses}
                showTotals={false}
              />
            </div>
          )}
        </div>
        <div>
          {urls.summary.currentFunds && (
            <div className={tableContainer}>
              <FetchTable
                name="Funds"
                title={{ text: `Fondos`, class: `text-center bg-indigo-500 text-white` }}
                url={urls.summary.currentFunds.with({ DailyUse: true })}
                columns={fundsColumns}
                classes={tableClasses}
                collapsible={{
                  defaultCollapsed: true,
                  summary: (rows) => {
                    const total = rows.reduce((acc, r) => acc + getSafeValueFrom(r, 'quoteCurrencyValue'), 0);
                    return `Fondos: ${defaultCurrencySymbol}${moneyFormatter(total)}`;
                  }
                }}
              />
            </div>
          )}
          {urls.summary.currentFunds && (
            <div className={tableContainer}>
              <FetchTable
                name="OtherFunds"
                title={{ text: `Otros Fondos`, class: `text-center bg-indigo-500 text-white` }}
                url={urls.summary.currentFunds.with({ DailyUse: false })}
                columns={fundsColumns}
                classes={tableClasses}
                collapsible={{
                  defaultCollapsed: true,
                  summary: (rows) => {
                    const total = rows.reduce((acc, r) => acc + getSafeValueFrom(r, 'quoteCurrencyValue'), 0);
                    return `Otros Fondos: ${defaultCurrencySymbol}${moneyFormatter(total)}`;
                  }
                }}
              />
            </div>
          )}
          {urls.summary.currentInvestments && (
            <div className={tableContainer}>
              <FetchTable
                name="Investments"
                title={{ text: `Inversiones`, class: `text-center medium bg-purple-500 text-white` }}
                url={urls.summary.currentInvestments}
                columns={investmentsColumns as unknown[]}
                classes={tableClasses}
                collapsible={{
                  defaultCollapsed: true,
                  summary: (rows) => {
                    const total = rows.reduce((acc, r) => acc + getSafeValueFrom(r, 'valuedDefaultCurrency'), 0);
                    return `Inversiones: ${defaultCurrencySymbol}${moneyFormatter(total)}`;
                  }
                }}
              />
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
