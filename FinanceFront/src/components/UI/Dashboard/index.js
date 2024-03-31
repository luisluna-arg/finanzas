import React, { useState, useEffect } from 'react';
import FetchTable from "../../utils/FetchTable";
import urls from "../../../routing/urls";

const NumericColumn = (id, label) => {
  return {
    id: id,
    label: label,
    class: ["text-end"],
    headerClass: ["text-end"],
    mapper: (r) => r && r[id] ? r[id] : 0,
    formatter: (v) => v ? parseFloat(v.toFixed(2)) : 0,
    totals: {
      reducer: (r) => r && r[id] ? r[id] : 0
    }
  };
};

const CreditCardTableSettings = {
  columns: [
    {
      id: "concept",
      label: "Concepto"
    },
    {
      id: "amount",
      label: "Monto",
      class: ["text-end"],
      headerClass: ["text-end"],
      mapper: (r) => r ? r.amount.value : 0,
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.amount.value : 0
      }
    },
    {
      id: "amountDollars",
      label: "Dólares",
      class: ["text-end"],
      headerClass: ["text-end"],
      mapper: (r) => r ? r.amountDollars.value : 0,
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.amountDollars.value : 0
      }
    },
    {
      id: "totalAmount",
      label: "Total",
      class: ["text-end"],
      headerClass: ["text-end"],
      mapper: (r) => r ? r.amount.value : 0,
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.amount.value : 0
      }
    },
    {
      id: "paymentNumber",
      label: "Nro. Cuota",
      class: ["text-end"],
      headerClass: ["text-end"],
    },
    {
      id: "planSize",
      label: "Cuotas",
      class: ["text-end"],
      headerClass: ["text-end"],
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.amount.value * r.planSize : 0
      }
    }
  ],
};

const DebitTableSettings = {
  columns: [
    {
      id: "origin",
      label: "Débito/Servicio"
    },
    {
      id: "amount",
      label: "Monto",
      class: ["text-end"],
      headerClass: ["text-end"],
      mapper: (r) => r ? r.amount.value : 0,
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.amount.value : 0
      }
    }
  ],
};

const FundsTableSettings = {
  columns: [
    {
      id: "label",
      label: "Origen"
    },
    NumericColumn("value", "Monto")
  ],
};

const ExpensesTableSettings = {
  columns: [
    {
      id: "label",
      label: "Gasto/Servicio"
    },
    {
      id: "value",
      label: "Monto",
      class: ["text-end"],
      headerClass: ["text-end"],
      mapper: (r) => r ? r.value : 0,
      formatter: (r) => r ? parseFloat(r.toFixed(2)) : 0,
      totals: {
        reducer: (r) => r ? r.value : 0
      }
    }
  ],
};

const InvestmentsTableSettings = {
  columns: [
    {
      id: "symbol",
      label: "Activo"
    },
    NumericColumn("averageReturn", "Rend. prom."),
    NumericColumn("valued", "Valorado")
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
  const [expensesData, setExpensesData] = useState(null);
  const [investmentsData, setInvestmentsData] = useState(null);

  const fetchData = async (dataUrl, setter, onFetch) => {
    setter([]);
    try {
      if (dataUrl) {
        let fetchData = await fetch(dataUrl);
        let newData = await fetchData.json();
        setter(newData);
        onFetch && onFetch(newData);
      }
    } catch (error) {
      console.error("Error fetching data:", error);
    } finally {
      // Additional cleanup logic if needed
    }
  };

  useEffect(() => {
    fetchData(urls.creditCards.get, setCreditCardData);
    fetchData(urls.summary.currentFunds, setFundsData);
    fetchData(urls.summary.totalExpenses, setExpensesData);
    fetchData(urls.summary.currentInvestments, setInvestmentsData);
  }, []);

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

  const DebitTable = ({ name, headerTitle, headerColor, url }) => {
    return (
      <FetchTable
        name={name}
        title={{
          text: headerTitle,
          class: `text-center ${headerColor}`
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
  }

  return (
    <div className="container-fluid">
      <div className='row'>
        <div className="col-3 column flex-wrap">
          {fundsData && fundsData.funds && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Funds`}
              title={{
                text: `Fondos`,
                class: `text-center coral-bg text-light`
              }}
              data={fundsData.funds}
              columns={FundsTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
          {expensesData && expensesData.expenses && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Expenses`}
              title={{
                text: `Gastos`,
                class: `text-center red-bg text-light`
              }}
              data={expensesData.expenses}
              columns={ExpensesTableSettings.columns}
              classes={tableClasses}
            />
          </div>
          }
          {debitModules && <div className="w-auto me-2 overflow-hidden">
            {debitModules.map((appModuleId, index) => {
              const url = `${urls.debits.latest}?AppModuleId=${appModuleId}&IncludeDeactivated=false`;
              const bgClass = debitBackgroundClasses[appModuleId];
              const tableName = debitTableNames[appModuleId];
              const title = debitTableTitles[appModuleId];

              return (<DebitTable
                key={appModuleId}
                name={`${tableName}`}
                headerTitle={`${title}`}
                headerColor={`${bgClass} text-light`}
                url={url}
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
          {investmentsData && investmentsData.investments && <div className="w-auto me-2 overflow-hidden">
            <FetchTable
              name={`Investments`}
              title={{
                text: `Inversiones`,
                class: `text-center mediumpurple-bg text-light`
              }}
              data={investmentsData.investments}
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
