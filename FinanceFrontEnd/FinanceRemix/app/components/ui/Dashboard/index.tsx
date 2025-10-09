import { loader } from "@/routes/dashboard";
import { Outlet, useLoaderData } from "@remix-run/react";

import urls from "@/utils/urls";
import { Dictionary, toNumber } from "@/utils/common";
import { FetchTableColumn } from "@/components/ui/utils/FetchTableColumn";
import { InputType } from "@/components/ui/utils/InputType";
import FetchTable from "@/components/ui/utils/FetchTable";

interface ValueHolder {
    value?: number;
}

const backgroundIntensity = 500;

const backgroundClasses = [
    "bg-amber-500",
    "bg-blue-500",
    "bg-cyan-500",
    "bg-emerald-500",
    "bg-fuchsia-500",
    "bg-green-500",
    "bg-indigo-500",
    "bg-lime-500",
    "bg-orange-500",
    "bg-purple-500",
    "bg-pink-500",
    "bg-red-500",
    "bg-rose-500",
    "bg-sky-500",
    "bg-teal-500",
    "bg-violet-500",
    "bg-yellow-500",
    //   "bg-slate-500",
    //   "bg-zinc-500",
    //   "bg-neutral-500",
    //   "bg-stone-500",
    //   "bg-gray-500",
];

const intFormatter = (r: number) => (r ? r : 0);

const moneyFormatter = (r: number) =>
    (typeof r === "number" ? r : 0).toFixed(2);

const getSafeValue = (v: ValueHolder) => toNumber(v);

const DecimalColumn = (
    id: string,
    label: string,
    mapper?: Function,
    totalsReducer?: Function
) => {
    const localMapper = mapper ?? getSafeValue;

    const localTotalsReducer =
        totalsReducer ??
        ((acc: number, r: ValueHolder | number) => acc + localMapper(r));

    const result = {
        id: id,
        label: label,
        class: ["text-end"],
        headerClass: ["text-end"],
        type: InputType.Decimal,
        mapper,
        formatter: moneyFormatter,
        totals: {
            formatter: moneyFormatter,
            reducer: localTotalsReducer,
        },
    };

    return result;
};

const PaymentPlanColumn = (
    columnName: string,
    label: string,
    mapper?: Function,
    totalsReducer?: Function
) => {
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
            reducer: totalsReducer,
        },
    };
};

const dollarCalculator = function (r: any, creditCardConversion: any) {
    return toNumber(r) * toNumber(creditCardConversion.sellRate, 1);
};

export default function Dashboard() {
    const { creditCards, latestCurrencyExchangeRates } =
        useLoaderData<typeof loader>();

    const tableClasses: string[] = [];

    const debitModulePesos = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
    const debitModuleDollars = "03cc66c7-921c-4e05-810e-9764cd365c1d";

    const debitModules = [debitModulePesos, debitModuleDollars];

    const debitBackgroundClasses: Dictionary<string> = {};
    debitBackgroundClasses[debitModulePesos] = "bg-violet-500 text-white";
    debitBackgroundClasses[debitModuleDollars] = "bg-orange-500 text-white";

    const debitTableNames: Dictionary<string> = {};
    debitTableNames[debitModulePesos] = "debit-pesos-table";
    debitTableNames[debitModuleDollars] = "debit-dollars-table";

    const debitTableTitles: Dictionary<string> = {};
    debitTableTitles[debitModulePesos] = "Débitos Pesos";
    debitTableTitles[debitModuleDollars] = "Débitos Dólares";

    const DebitTableSettings = {
        columns: [
            new FetchTableColumn(
                "origin",
                "Débito/Servicio",
                (record: any) => record?.origin?.name ?? record?.name ?? "-"
            ),
            DecimalColumn("amount", "Monto", (a: any) =>
                toNumber(a?.amount ?? a)
            ),
        ],
    };

    const FundsTableSettings = {
        columns: [
            new FetchTableColumn("label", "Origen"),
            DecimalColumn("value", "Monto", getSafeValue),
        ],
    };

    const CurrencyExchangeRatesTableSettings = {
        columns: [
            new FetchTableColumn(
                "label",
                "Origen",
                (v: any) => v?.baseCurrency?.name ?? "-",
                (v: any) => v?.baseCurrency?.name ?? "-"
            ),
            new FetchTableColumn(
                "label",
                "Destino",
                (v: any) => v?.quoteCurrency?.name ?? "-",
                (v: any) => v?.quoteCurrency?.name ?? "-"
            ),
            DecimalColumn("value", "Compra", (v: any) =>
                getSafeValue(v.buyRate)
            ),
            DecimalColumn("value", "Venta", (v: any) =>
                getSafeValue(v.sellRate)
            ),
        ],
    };

    const SummaryTableSettings = {
        columns: [
            new FetchTableColumn("label", "Dato"),
            DecimalColumn("value", "Monto", getSafeValue),
        ],
    };

    const ExpensesTableSettings = {
        columns: [
            new FetchTableColumn("label", "Gasto/Servicio"),
            DecimalColumn("value", "Monto ($)", getSafeValue),
        ],
    };

    const InvestmentsTableSettings = {
        columns: [
            new FetchTableColumn("symbol", "Activo"),
            DecimalColumn(
                "averageReturn",
                "Rend. prom.",
                (v: any) => v.averageReturn ?? v
            ),
            DecimalColumn("valued", "Valorado", (v: any) => v.valued ?? v),
        ],
    };

    const CreditCardTableSettings = {
        columns: [
            {
                id: "concept",
                label: "Concepto",
            },
            DecimalColumn("amount", "Monto", (v: any) =>
                toNumber(v?.amount ?? v, 0)
            ),
            DecimalColumn("amountDollars", "Dólares", (v: any) =>
                toNumber(v?.amountDollars ?? v, 0)
            ),
            DecimalColumn(
                "totalAmount",
                "Total",
                (v: any) => {
                    if (v) {
                        const amount = toNumber(v?.amount ?? v, 0);
                        const amountDollars = toNumber(
                            v?.amountDollars ?? v,
                            0
                        );
                        return (
                            amount +
                            dollarCalculator(
                                amountDollars,
                                latestCurrencyExchangeRates
                            )
                        );
                    }

                    return 0;
                },
                (acc: number, v: any) => {
                    if (v) {
                        const amount = toNumber(v?.amount ?? v, 0);
                        const amountDollars = toNumber(
                            v?.amountDollars ?? v,
                            0
                        );
                        return (
                            acc +
                            (amount +
                                dollarCalculator(
                                    amountDollars,
                                    latestCurrencyExchangeRates
                                ))
                        );
                    }

                    return acc;
                }
            ),
            PaymentPlanColumn(
                "paymentNumber",
                "Nro. Cuota",
                (v: any) => toNumber(v?.paymentNumber ?? v, 0),
                (acc: number, v: any) =>
                    acc + toNumber(v?.paymentNumber ?? v, 0)
            ),
            PaymentPlanColumn(
                "planSize",
                "Cuotas",
                (v: any) => toNumber(v?.planSize ?? v, 0),
                (acc: number, v: any) => acc + toNumber(v?.planSize ?? v, 0)
            ),
        ],
    };

    return (
        <>
            <Outlet />
            <main>
                <section id="expenses-actions">
                    <div className="container-fluid">
                        <div className="grid grid-cols-4 gap-4">
                            <div className="col-3 column flex-wrap">
                                {urls.summary.general && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        <FetchTable
                                            name={`Summary`}
                                            title={{
                                                text: `Resúmen`,
                                                class: `text-center bg-blue-500 text-white`,
                                            }}
                                            url={urls.proxy(
                                                urls.summary.general,
                                                { DailyUse: true }
                                            )}
                                            columns={
                                                SummaryTableSettings.columns
                                            }
                                            classes={tableClasses}
                                            showTotals={false}
                                        />
                                    </div>
                                )}
                                {urls.summary.totalExpenses && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        <FetchTable
                                            name={`Expenses`}
                                            title={{
                                                text: `Gastos`,
                                                class: `text-center bg-red-500 text-white`,
                                            }}
                                            url={urls.proxy(
                                                urls.summary.totalExpenses
                                            )}
                                            columns={
                                                ExpensesTableSettings.columns
                                            }
                                            classes={tableClasses}
                                        />
                                    </div>
                                )}
                                {urls.currencyExchangeRates.latest && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        <FetchTable
                                            name={`CurrencyRates`}
                                            title={{
                                                text: `Conversiones de moneda`,
                                                class: `text-center bg-sky-500 text-white`,
                                            }}
                                            url={urls.proxy(
                                                urls.currencyExchangeRates
                                                    .latest
                                            )}
                                            columns={
                                                CurrencyExchangeRatesTableSettings.columns
                                            }
                                            classes={tableClasses}
                                            showTotals={false}
                                        />
                                    </div>
                                )}
                                {urls.summary.currentFunds && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        <FetchTable
                                            name={`Funds`}
                                            title={{
                                                text: `Fondos`,
                                                class: `text-center bg-indigo-500 text-white`,
                                            }}
                                            url={urls.proxy(
                                                urls.summary.currentFunds,
                                                { DailyUse: true }
                                            )}
                                            columns={FundsTableSettings.columns}
                                            classes={tableClasses}
                                        />
                                    </div>
                                )}
                                {urls.summary.currentFunds && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        <FetchTable
                                            name={`OtherFunds`}
                                            title={{
                                                text: `Otros Fondos`,
                                                class: `text-center bg-indigo-500 text-white`,
                                            }}
                                            url={urls.proxy(
                                                urls.summary.currentFunds,
                                                { DailyUse: false }
                                            )}
                                            columns={FundsTableSettings.columns}
                                            classes={tableClasses}
                                        />
                                    </div>
                                )}
                                {debitModules && (
                                    <div className="w-auto me-2 overflow-hidden">
                                        {urls.debits.monthly.latest &&
                                            debitModules.map(
                                                (appModuleId, index) => {
                                                    const url = urls.proxy(
                                                        urls.debits.monthly
                                                            .latest,
                                                        {
                                                            AppModuleId:
                                                                appModuleId,
                                                            IncludeDeactivated:
                                                                false,
                                                        }
                                                    );
                                                    const bgClass =
                                                        debitBackgroundClasses[
                                                            appModuleId
                                                        ];
                                                    const tableName =
                                                        debitTableNames[
                                                            appModuleId
                                                        ];
                                                    const title =
                                                        debitTableTitles[
                                                            appModuleId
                                                        ];

                                                    return (
                                                        <FetchTable
                                                            key={appModuleId}
                                                            name={`${tableName}`}
                                                            title={{
                                                                text: title,
                                                                class: `text-center ${bgClass}`,
                                                            }}
                                                            url={url}
                                                            columns={
                                                                DebitTableSettings.columns
                                                            }
                                                            classes={
                                                                tableClasses
                                                            }
                                                            hideIfEmpty={true}
                                                            wrapper={{
                                                                classes: [
                                                                    "w-auto",
                                                                    "overflow-hidden",
                                                                ],
                                                            }}
                                                        />
                                                    );
                                                }
                                            )}
                                    </div>
                                )}
                            </div>
                            <div className="col-span-2 justify-content-center">
                                {creditCards &&
                                    creditCards.map(
                                        (data: any, index: number) => {
                                            const url = urls.proxy(
                                                urls.creditCardMovements.latest,
                                                { CreditCardId: data.id }
                                            );
                                            const bgClass =
                                                backgroundClasses[
                                                    index <
                                                    backgroundClasses.length
                                                        ? index
                                                        : backgroundClasses.length -
                                                          index -
                                                          1
                                                ];
                                            return (
                                                <FetchTable
                                                    name={`${data.name}-${data.bank.name}-table`}
                                                    key={index}
                                                    title={{
                                                        text: `${data.name} ${data.bank.name}`,
                                                        class: `text-center ${bgClass} text-white`,
                                                    }}
                                                    url={url}
                                                    columns={
                                                        CreditCardTableSettings.columns
                                                    }
                                                    classes={tableClasses}
                                                    hideIfEmpty={true}
                                                    wrapper={{
                                                        classes: [
                                                            "w-auto",
                                                            "overflow-hidden",
                                                        ],
                                                    }}
                                                />
                                            );
                                        }
                                    )}
                            </div>
                            <div className="col-3 column flex-wrap justify-content-center">
                                {urls.summary.currentInvestments && (
                                    <FetchTable
                                        name={`Investments`}
                                        title={{
                                            text: `Inversiones`,
                                            class: `text-center medium bg-purple-500 text-white`,
                                        }}
                                        url={urls.proxy(
                                            urls.summary.currentInvestments
                                        )}
                                        columns={
                                            InvestmentsTableSettings.columns
                                        }
                                        classes={tableClasses}
                                    />
                                )}
                                {!urls.summary.currentFunds && <div>AASAS</div>}
                            </div>
                        </div>
                    </div>
                </section>
            </main>
        </>
    );
}
