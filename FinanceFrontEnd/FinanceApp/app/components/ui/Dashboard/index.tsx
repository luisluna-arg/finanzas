import type { loader } from '@/routes/dashboard';
import { Outlet, useLoaderData, useMatch, useNavigate } from 'react-router';

import urls from '@/utils/urls';
import FetchTable from '@/components/ui/utils/FetchTable';
import CustomButton from '@/components/ui/utils/CustomButton';
import {
  backgroundClasses,
  buildCreditCardColumns,
  buildCurrencyRatesColumns,
  buildDebitColumns,
  buildExpensesColumns,
  buildFundsColumns,
  buildInvestmentsColumns,
  buildSummaryColumns,
  DEBIT_BACKGROUND_CLASSES,
  DEBIT_MODULES,
  DEBIT_TABLE_NAMES,
  DEBIT_TABLE_TITLES,
} from './dashboardHelpers';
import SummaryDetail from './SummaryDetail';

export default function Dashboard() {
  const { creditCards, latestCurrencyExchangeRates, defaultCurrency } = useLoaderData<typeof loader>();
  const defaultCurrencySymbol = defaultCurrency?.symbol ?? '$';
  const navigate = useNavigate();
  const isSummary = !!useMatch('/dashboard/summary');

  const tableClasses: string[] = [];
  const tableContainer = 'w-auto mb-4 overflow-hidden';

  const summaryColumns = buildSummaryColumns(defaultCurrencySymbol);
  const investmentsColumns = buildInvestmentsColumns(defaultCurrencySymbol);
  const fundsColumns = buildFundsColumns(defaultCurrencySymbol);
  const expensesColumns = buildExpensesColumns(defaultCurrencySymbol);
  const debitColumns = buildDebitColumns();
  const currencyRatesColumns = buildCurrencyRatesColumns();
  const creditCardColumns = buildCreditCardColumns(latestCurrencyExchangeRates);

  return (
    <>
      <Outlet />
      <main>
        <section id="expenses-actions">
          <div className="container-fluid mt-4 ml-4 mr-4">
            {isSummary ? (
              <SummaryDetail
                onClose={() => navigate('/dashboard')}
                tableClasses={tableClasses}
                tableContainer={tableContainer}
                defaultCurrencySymbol={defaultCurrencySymbol}
              />
            ) : (
              <div className="grid grid-cols-5 gap-4">
                <div id="investments-section" className="column flex-wrap justify-content-center">
                  {urls.currencyExchangeRates.latest && (
                    <div className={tableContainer}>
                      <FetchTable
                        name="CurrencyRates"
                        title={{ text: `Conversiones de moneda`, class: `text-center bg-sky-500 text-white` }}
                        url={urls.currencyExchangeRates.latest}
                        columns={currencyRatesColumns}
                        classes={tableClasses}
                        showTotals={false}
                        collapsible
                      />
                    </div>
                  )}
                  {!urls.summary.currentFunds && <div>AASAS</div>}
                </div>
                <div id="summary-section" className="col-span-2 column flex-wrap">
                  {urls.summary.general && (
                    <div className={tableContainer}>
                      <FetchTable
                        name="Summary"
                        title={{ text: `Resúmen`, class: `text-center bg-blue-500 text-white` }}
                        url={urls.summary.general.with({ DailyUse: true })}
                        columns={summaryColumns}
                        classes={tableClasses}
                        showTotals={false}
                        collapsible
                      />
                    </div>
                  )}
                  <div className="flex justify-center mt-2 mb-4">
                    <CustomButton onClick={() => navigate('/dashboard/summary')} className="bg-blue-500 text-white hover:bg-blue-600">
                      Ver más
                    </CustomButton>
                  </div>
                  {urls.summary.totalExpenses && (
                    <div className={tableContainer}>
                      <FetchTable
                        name="Expenses"
                        title={{ text: `Gastos`, class: `text-center bg-red-500 text-white` }}
                        url={urls.summary.totalExpenses}
                        columns={expensesColumns}
                        classes={tableClasses}
                        collapsible
                      />
                    </div>
                  )}
                  <div className={tableContainer}>
                    {urls.debits.monthly.latest &&
                      DEBIT_MODULES.map((appModuleId) => {
                        const url = urls.debits.monthly.latest.with({
                          AppModuleId: appModuleId,
                          IncludeDeactivated: false,
                        });
                        return (
                          <FetchTable
                            key={appModuleId}
                            name={DEBIT_TABLE_NAMES[appModuleId]}
                            title={{
                              text: DEBIT_TABLE_TITLES[appModuleId],
                              class: `text-center ${DEBIT_BACKGROUND_CLASSES[appModuleId]}`,
                            }}
                            url={url}
                            columns={debitColumns as unknown as unknown[]}
                            classes={tableClasses}
                            hideIfEmpty={true}
                            wrapper={{ classes: tableContainer.split(' ') }}
                            collapsible
                          />
                        );
                      })}
                  </div>
                </div>
                <div id="credit-cards-section" className="col-span-2 justify-content-center">
                  {creditCards &&
                    creditCards.map((c: unknown, _index: number) => {
                      const data = c as { id?: string; name?: string; bank?: { name?: string } };
                      const url = urls.creditCardMovements.latest.with({
                        CreditCardId: data.id,
                        Page: 1,
                        PageSize: 10,
                      });
                      const idx =
                        _index < backgroundClasses.length
                          ? _index
                          : backgroundClasses.length - _index - 1;
                      return (
                        <FetchTable
                          name={`${data.name}-${data.bank?.name}-table`}
                          key={_index}
                          title={{
                            text: `${data.name} ${data.bank?.name}`,
                            class: `text-center ${backgroundClasses[idx]} text-white`,
                          }}
                          url={url}
                          columns={creditCardColumns as unknown as unknown[]}
                          classes={tableClasses}
                          hideIfEmpty={true}
                          wrapper={{ classes: tableContainer.split(' ') }}
                          collapsible
                        />
                      );
                    })}
                </div>
              </div>
            )}
          </div>
        </section>
      </main>
    </>
  );
}
