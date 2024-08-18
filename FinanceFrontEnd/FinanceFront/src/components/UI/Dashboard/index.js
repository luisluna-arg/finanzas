import React, { useState, useEffect } from 'react';
import FetchTable from "../../utils/FetchTable";
import { InputControlTypes } from "../../utils/InputControl";
import urls from "../../../routing/urls";

const intFormatter = (r) => r ? r : 0;

const moneyFormatter = r => (r ? r : 0).toFixed(2);

const getSafeValue = (v) => v?.value ?? (v ?? 0);

const PaymentPlanColumn = (columnName, label, mapper, totalsReducer) => {
  return {
    id: columnName,
    label: label,
    type: InputControlTypes.Integer,
    class: ["text-end"],
    headerClass: ["text-end"],
    mapper: mapper ?? getSafeValue,
    formatter: intFormatter,
    totals: {
      formatter: moneyFormatter,
      reducer: totalsReducer
    }
  };
};

const DecimalColumn = (id, label, mapper, totalsReducer) => {
  let localMapper = (r) => {
    console.log("   id", id);
    console.log("   mapper", mapper);
    console.log("   getSafeValue", getSafeValue);
    // console.log("DecimalColumn.localMapper", "(mapper ?? getSafeValue)(r)", (mapper ?? getSafeValue)(r), "r", r);
    console.log("   (mapper ?? getSafeValue)(r)", (mapper ?? getSafeValue)(r), "r", r);
    return (mapper ?? getSafeValue)(r);
  };

  localMapper = mapper ?? getSafeValue;

  let localTotalsReducer =
    totalsReducer ??
    ((acc, r) => acc + localMapper(r));

  var result = {
    id: id,
    label: label,
    class: ["text-end"],
    headerClass: ["text-end"],
    type: InputControlTypes.Decimal,
    mapper,
    formatter: moneyFormatter,
    totals: {
      formatter: moneyFormatter,
      reducer: localTotalsReducer
    }
  };

  return result;
};

const CreditCardTableSettings = {
  columns: [
    {
      id: "concept",
      label: "Concepto"
    },
    DecimalColumn("amount", "Monto", r => r?.amount?.value),
    DecimalColumn("amountDollars", "Dólares", (r) => r?.amountDollars?.value ?? 0),
    DecimalColumn("totalAmount", "Total"),
    PaymentPlanColumn("paymentNumber", "Nro. Cuota", (r) => r.paymentNumber),
    PaymentPlanColumn("planSize", "Cuotas", (r) => r.planSize)
  ],
};

const DebitTableSettings = {
  columns: [
    {
      id: "origin",
      label: "Débito/Servicio"
    },
    DecimalColumn("amount", "Monto", a => a?.amount?.value)
  ],
};

const FundsTableSettings = {
  columns: [
    {
      id: "label",
      label: "Origen"
    },
    DecimalColumn("value", "Monto", getSafeValue)
  ],
};

const SummaryTableSettings = {
  columns: [
    {
      id: "label",
      label: "Dato"
    },
    DecimalColumn("value", "Monto", getSafeValue)
  ],
};

const ExpensesTableSettings = {
  columns: [
    {
      id: "label",
      label: "Gasto/Servicio"
    },
    DecimalColumn("value", "Monto ($)", getSafeValue)
  ],
};

const InvestmentsTableSettings = {
  columns: [
    {
      id: "symbol",
      label: "Activo"
    },
    DecimalColumn("averageReturn", "Rend. prom.", (v) => v.averageReturn),
    DecimalColumn("valued", "Valorado", (v) => v.valued)
  ],
};

const debitModulePesos = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
const debitModuleDollars = "03cc66c7-921c-4e05-810e-9764cd365c1d";

const debitModules = [
  debitModulePesos,
  debitModuleDollars
];

const debitBackgroundClasses = {};
debitBackgroundClasses[debitModulePesos] = "violet-bg";
debitBackgroundClasses[debitModuleDollars] = "orange-bg";

const debitTableNames = {};
debitTableNames[debitModulePesos] = "debit-pesos-table";
debitTableNames[debitModuleDollars] = "debit-dollars-table";

const debitTableTitles = {};
debitTableTitles[debitModulePesos] = "Débitos Pesos";
debitTableTitles[debitModuleDollars] = "Débitos Dólares";


const Dashboard = () => {
  const [creditCardData, setCreditCardData] = useState(null);
  const [fundsData, setFundsData] = useState(null);
  const [otherFundsData, setOtherFundsData] = useState(null);
  const [summaryGeneralData, setSummaryGeneralData] = useState(null);
  const [expensesData, setExpensesData] = useState(null);
  const [investmentsData, setInvestmentsData] = useState(null);
  const [debitsData, setDebitsData] = useState(null);

  useEffect(() => {
    let endpoints = debitModules.map(moduleId => `${urls.debits.latest}?AppModuleId=${moduleId}&IncludeDeactivated=false`)

    endpoints.push(urls.creditCards.get);
    endpoints.push(`${urls.summary.currentFunds}?DailyUse=true`);
    endpoints.push(urls.summary.totalExpenses);
    endpoints.push(urls.summary.currentInvestments);
    endpoints.push(`${urls.summary.general}?DailyUse=true`);
    endpoints.push(`${urls.summary.currentFunds}?DailyUse=false`);
    endpoints.push(`${urls.currencyExchangeRates.latestByShortName}/USDTC/latest`);

    const fetchData = async (fetchUrls) => {
      try {
        return await Promise.all(fetchUrls.map(url => fetch(url).then(response => response.json())));
      } catch (error) {
        console.error('Error fetching data:', error);
        return [];
      }
    };

    const getData = async () => {
      const data = await fetchData(endpoints);
      let debitsData = {};

      for (let i = 0; i < debitModules.length; i++) {
        debitsData[debitModules[i]] = data[i];
      }

      setDebitsData(debitsData);
      setCreditCardData(data[2]);
      setFundsData(data[3])
      setExpensesData(data[4])
      setInvestmentsData(data[5])
      setSummaryGeneralData(data[6])
      setOtherFundsData(data[7])

      var creditCardConversion = data[8];

      const dollarCalculator = function (r) { 
        console.log("dollarCalculator.r", r)
        return r.value * creditCardConversion.sellRate; };

      let columnIndex = 2;

      CreditCardTableSettings.columns[columnIndex].mapper = function (r) { return r ? dollarCalculator(r) : 0 };
      CreditCardTableSettings.columns[columnIndex].totals.reducer = function (acc, r) { return acc + CreditCardTableSettings.columns[columnIndex].mapper(r); };

      CreditCardTableSettings.columns[++columnIndex].mapper = function (r) { return r ? r.amount.value + dollarCalculator(r) : 0 };
      CreditCardTableSettings.columns[columnIndex].totals.reducer = function (acc, r) { return acc + CreditCardTableSettings.columns[columnIndex].mapper(r); };

      let totalsMapper = function (r) { 
        return r.amount.value + dollarCalculator(r);
       };

      CreditCardTableSettings.columns[++columnIndex].totals.reducer = function (acc, r) {
        return acc + r ? totalsMapper(r) * r.paymentNumber : 0;
      };

      CreditCardTableSettings.columns[++columnIndex].totals.reducer = function (acc, r) {
        return acc + r ? totalsMapper(r) * r.planSize : 0;
      };
    }

    getData();
  }, [
  ]);

  const tableClasses = ["table", "mt-2", "small", "table-sm"];

  const backgroundClasses = [
    "orange-bg",
    "blue-bg",
    "tomato-bg",
    "lime-bg"
  ];

  const CreditCardTable = ({ name, headerTitle, headerColor, url }) => {
    return (<FetchTable
      name={name}
      title={{
        text: headerTitle,
        class: `text-center ${headerColor}`
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
    />)
  }

  const DebitTable = ({ name, headerTitle, headerColor, data }) => {
    return (
      <FetchTable
        name={name}
        title={{
          text: headerTitle,
          class: `text-center ${headerColor}`
        }}
        data={data}
        columns={DebitTableSettings.columns}
        classes={tableClasses}
        hideIfEmpty={true}
        wrapper={
          {
            classes: ["w-auto", "overflow-hidden"]
          }
        }
      />);
  }

  return (
    <div className="container-fluid">
      <h5>BROKEN CONTENT</h5>
      <div className='row'>
        <div className="col-3 column flex-wrap">
          {summaryGeneralData && summaryGeneralData.items && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Summary`}
              title={{
                text: `Resúmen`,
                class: `text-center cornflowerblue-bg text-light`
              }}
              data={summaryGeneralData.items}
              columns={SummaryTableSettings.columns}
              classes={tableClasses}
              showTotals={false}
            />
          </div>
          }
          {fundsData && fundsData.items && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Funds`}
              title={{
                text: `Fondos`,
                class: `text-center coral-bg text-light`
              }}
              data={fundsData.items}
              columns={FundsTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
          {otherFundsData && otherFundsData.items && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`OtherFunds`}
              title={{
                text: `Otros Fondos`,
                class: `text-center coral-bg text-light`
              }}
              data={otherFundsData.items}
              columns={FundsTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
          {expensesData && expensesData.items && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Expenses`}
              title={{
                text: `Gastos`,
                class: `text-center red-bg text-light`
              }}
              data={expensesData.items}
              columns={ExpensesTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
          {debitModules && <div className="w-auto me-2 overflow-hidden">
            {debitsData && debitModules.map((appModuleId, index) => {
              const data = debitsData[appModuleId];
              const bgClass = debitBackgroundClasses[appModuleId];
              const tableName = debitTableNames[appModuleId];
              const title = debitTableTitles[appModuleId];

              return (<DebitTable
                key={appModuleId}
                name={`${tableName}`}
                headerTitle={`${title}`}
                headerColor={`${bgClass} text-light`}
                data={data}
              />);
            })}
          </div>
          }
        </div>
        <div className="col-6 column flex-wrap justify-content-center">
          {
            creditCardData && creditCardData.map((data, index) => {
              const url = `${urls.creditCardMovements.latest}?CreditCardId=${data.id}`;
              const bgClass = backgroundClasses[index < backgroundClasses.length ? index : backgroundClasses.length - index - 1];
              return (<CreditCardTable
                key={index}
                name={`${data.name}-${data.bank.name}-table`}
                headerTitle={`${data.name} ${data.bank.name}`}
                headerColor={`${bgClass} text-light`}
                url={url}
              />);
            })
          }
        </div>
        <div className="col-3 column flex-wrap justify-content-center">
          {investmentsData && investmentsData.items && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Investments`}
              title={{
                text: `Inversiones`,
                class: `text-center mediumpurple-bg text-light`
              }}
              data={investmentsData.items}
              columns={InvestmentsTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
