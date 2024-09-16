import { Dictionary } from "@/app/utils/common";
import { FetchTableColumn } from '@/app/components/ui/utils/FetchTableColumn';
import { InputType } from '@/app/components/ui/utils/InputType';
import { loader } from '@/app/routes/dashboard';
import { Outlet } from '@remix-run/react';
import { useLoaderData } from '@remix-run/react';
import FetchTable from '@/app/components/ui/utils/FetchTable';
import urls from '@/app/utils/urls';

interface ValueHolder {
    value?: number
}

const backgroundClasses = [
    "orange-bg",
    "blue-bg",
    "tomato-bg",
    "lime-bg"
];

const intFormatter = (r: number) => r ? r : 0;

const moneyFormatter = (r: number) => (r ? r : 0).toFixed(2);

const getSafeValue = (v: ValueHolder) => v?.value ?? (v ?? 0);

const DecimalColumn = (id: string, label: string, mapper?: Function, totalsReducer?: Function) => {
    let localMapper = mapper ?? getSafeValue;

    let localTotalsReducer = totalsReducer ?? ((acc: number, r: ValueHolder | number) => acc + localMapper(r));

    var result = {
        id: id,
        label: label,
        class: ["text-end"],
        headerClass: ["text-end"],
        type: InputType.Decimal,
        mapper,
        formatter: moneyFormatter,
        totals: {
            formatter: moneyFormatter,
            reducer: localTotalsReducer
        }
    };

    return result;
};

const PaymentPlanColumn = (columnName: String, label: String, mapper?: Function, totalsReducer?: Function) => {
    return {
        id: columnName,
        label: label,
        type: InputType.Integer,
        class: ["text-end"],
        headerClass: ["text-end"],
        mapper: mapper ?? getSafeValue,
        formatter: intFormatter,
        totals: {
            formatter: intFormatter,
            reducer: totalsReducer
        }
    };
};

const dollarCalculator = function (r: any, creditCardConversion: any) {
    return (r?.value ?? r ?? 0) * creditCardConversion.sellRate;
};

export default function Dashboard() {
    const { creditCards, latestCurrencyExchangeRates } = useLoaderData<typeof loader>();

    const tableClasses = ["table", "mt-2", "small", "table-sm"];

    const debitModulePesos = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
    const debitModuleDollars = "03cc66c7-921c-4e05-810e-9764cd365c1d";

    const debitModules = [
        debitModulePesos,
        debitModuleDollars
    ];

    const debitBackgroundClasses: Dictionary<string> = {};
    debitBackgroundClasses[debitModulePesos] = "violet-bg";
    debitBackgroundClasses[debitModuleDollars] = "orange-bg";

    const debitTableNames: Dictionary<string> = {};
    debitTableNames[debitModulePesos] = "debit-pesos-table";
    debitTableNames[debitModuleDollars] = "debit-dollars-table";

    const debitTableTitles: Dictionary<string> = {};
    debitTableTitles[debitModulePesos] = "Débitos Pesos";
    debitTableTitles[debitModuleDollars] = "Débitos Dólares";

    const DebitTableSettings = {
        columns: [
            new FetchTableColumn("origin", "Débito/Servicio"),
            DecimalColumn("amount", "Monto", (a: any) => a?.amount?.value ?? a?.value ?? a)
        ],
    };

    const FundsTableSettings = {
        columns: [
            new FetchTableColumn("label", "Origen"),
            DecimalColumn("value", "Monto", getSafeValue)
        ],
    };

    const SummaryTableSettings = {
        columns: [
            new FetchTableColumn("label", "Dato"),
            DecimalColumn("value", "Monto", getSafeValue)
        ],
    };

    const ExpensesTableSettings = {
        columns: [
            new FetchTableColumn("label", "Gasto/Servicio"),
            DecimalColumn("value", "Monto ($)", getSafeValue)
        ],
    };

    const InvestmentsTableSettings = {
        columns: [
            new FetchTableColumn("symbol", "Activo"),
            DecimalColumn("averageReturn", "Rend. prom.", (v: any) => v.averageReturn ?? v),
            DecimalColumn("valued", "Valorado", (v: any) => v.valued ?? v)
        ],
    };

    const CreditCardTableSettings = {
        columns: [
            {
                id: "concept",
                label: "Concepto"
            },
            DecimalColumn("amount", "Monto", (v: any) => v?.amount?.value ?? v?.value ?? v ?? 0),
            DecimalColumn("amountDollars", "Dólares", (v: any) => v?.amountDollars?.value ?? v?.value ?? v ?? 0),
            DecimalColumn(
                "totalAmount",
                "Total",
                (v: any) => {
                    if (v) {
                        let amount = v?.amount?.value ?? v?.value ?? v ?? 0;
                        let amountDollars = v?.amountDollars?.value ?? v?.value ?? v ?? 0;
                        return amount + dollarCalculator(amountDollars, latestCurrencyExchangeRates);
                    }

                    return 0;
                },
                (acc: number, v: any) => {
                    if (v) {
                        let amount = v?.amount?.value ?? v?.value ?? v ?? 0;
                        let amountDollars = v?.amountDollars?.value ?? v?.value ?? v ?? 0;
                        return acc + (amount + dollarCalculator(amountDollars, latestCurrencyExchangeRates));
                    }

                    return acc;
                }
            ),
            PaymentPlanColumn(
                "paymentNumber",
                "Nro. Cuota",
                (v: any) => v?.paymentNumber?.value ?? v?.paymentNumber ?? v?.value ?? v ?? 0,
                (acc: number, v: any) => acc + (v?.paymentNumber?.value ?? v?.paymentNumber ?? v?.value ?? v ?? 0)),
            PaymentPlanColumn(
                "planSize",
                "Cuotas",
                (v: any) => v?.planSize?.value ?? v?.planSize ?? v?.value ?? v ?? 0,
                (acc: number, v: any) => acc + (v?.planSize?.value ?? v?.planSize ?? v?.value ?? v ?? 0)),
        ],
    };

    return (
        <>
            <Outlet />
            <main>
                <section id="expenses-actions">
                    <div className="container-fluid">
                        <div className="row">
                            <div className="col-3 column flex-wrap">
                                {urls.summary.general && <div className="w-auto me-2 overflow-hidden">
                                    <FetchTable
                                        name={`Summary`}
                                        title={{
                                            text: `Resúmen`,
                                            class: `text-center cornflowerblue-bg text-light`
                                        }}
                                        url={`${urls.summary.general}?DailyUse=true`}
                                        columns={SummaryTableSettings.columns}
                                        classes={tableClasses}
                                        showTotals={false}
                                    />
                                </div>
                                }
                                {urls.summary.currentFunds && <div className="w-auto me-2 overflow-hidden">
                                    <FetchTable
                                        name={`Funds`}
                                        title={{
                                            text: `Fondos`,
                                            class: `text-center coral-bg text-light`
                                        }}
                                        url={`${urls.summary.currentFunds}?DailyUse=true`}
                                        columns={FundsTableSettings.columns}
                                        classes={tableClasses}
                                    />
                                </div>
                                }
                                {urls.summary.currentFunds && <div className="w-auto me-2 overflow-hidden">
                                    <FetchTable
                                        name={`OtherFunds`}
                                        title={{
                                            text: `Otros Fondos`,
                                            class: `text-center coral-bg text-light`
                                        }}
                                        url={`${urls.summary.currentFunds}?DailyUse=false`}
                                        columns={FundsTableSettings.columns}
                                        classes={tableClasses}
                                    />
                                </div>
                                }
                                {urls.summary.totalExpenses && <div className="w-auto me-2 overflow-hidden">
                                    <FetchTable
                                        name={`Expenses`}
                                        title={{
                                            text: `Gastos`,
                                            class: `text-center red-bg text-light`
                                        }}
                                        url={urls.summary.totalExpenses}
                                        columns={ExpensesTableSettings.columns}
                                        classes={tableClasses}
                                    />
                                </div>
                                }
                                {debitModules && <div className="w-auto me-2 overflow-hidden">
                                    {urls.debits.latest && debitModules.map((appModuleId, index) => {
                                        const url = `${urls.debits.latest}?AppModuleId=${appModuleId}&IncludeDeactivated=false`;
                                        const bgClass = debitBackgroundClasses[appModuleId];
                                        const tableName = debitTableNames[appModuleId];
                                        const title = debitTableTitles[appModuleId];
                                        return (<FetchTable
                                            key={appModuleId}
                                            name={`${tableName}`}
                                            title={{
                                                text: title,
                                                class: `text-center ${bgClass}`
                                            }}
                                            url={url}
                                            columns={DebitTableSettings.columns}
                                            classes={tableClasses}
                                            hideIfEmpty={true}
                                            wrapper={
                                                {
                                                    classes: ["w-auto", "overflow-hidden"]
                                                }
                                            }
                                        />);
                                    })}
                                </div>
                                }
                            </div>
                            <div className="col-6 column flex-wrap justify-content-center">
                                {
                                    creditCards && creditCards.map((data: any, index: number) => {
                                        const url = `${urls.creditCardMovements.latest}?CreditCardId=${data.id}`;
                                        const bgClass = backgroundClasses[index < backgroundClasses.length ? index : backgroundClasses.length - index - 1];
                                        return (<FetchTable
                                            name={`${data.name}-${data.bank.name}-table`}
                                            key={index}
                                            title={{
                                                text: `${data.name} ${data.bank.name}`,
                                                class: `text-center ${bgClass} text-light`
                                            }}
                                            url={url}
                                            columns={CreditCardTableSettings.columns}
                                            classes={tableClasses}
                                            hideIfEmpty={true}
                                            wrapper={
                                                {
                                                    classes: ["w-auto", "overflow-hidden"]
                                                }
                                            }
                                        />);
                                    })
                                }
                            </div>
                            <div className="col-3 column flex-wrap justify-content-center">
                                {urls.summary.currentInvestments && <FetchTable
                                    name={`Investments`}
                                    title={{
                                        text: `Inversiones`,
                                        class: `text-center mediumpurple-bg text-light`
                                    }}
                                    url={urls.summary.currentInvestments}
                                    columns={InvestmentsTableSettings.columns}
                                    classes={tableClasses}
                                />}
                                {!urls.summary.currentFunds && <div>AASAS</div>}
                            </div>
                        </div>
                    </div>
                </section>
            </main >
        </>
    );
}