import React, { useState, useEffect } from 'react';
import FetchTable from "../../utils/FetchTable";
import urls from "../../../routing/urls";
import "./styles.css";

const CreditCardTableSettings = {
  columns: [
    { label: "Concepto", id: "concept" },
    {
      label: "Monto", id: "amount", mapper: (r) => r ? r.value : 0
    },
    {
      label: "Dólares", id: "amountDollars", mapper: (r) => r ? r.value : 0
    },
    { label: "Cuota", id: "paymentNumber" },
    { label: "Total", id: "planSize" }
  ],
};

const DebitTableSettings = {
  columns: [
    { label: "Débito/Servicio", id: "origin" },
    {
      label: "Monto", id: "amount", mapper: (r) => r ? r.value : 0
    }
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
    }
  };

  useEffect(() => {
    fetchData(urls.creditCards.get, setCreditCardData);
  }, []);

  const tableClasses = ["table", "mt-2", "small"];

  const backgroundClasses = [
    'bg-primary',
    'bg-success',
    'bg-danger',
    'bg-warning',
    'bg-info',
    'bg-secondary',
  ];

  const CreditCardTable = ({ name, headerTitle, headerColor, url }) => {
    return (
      <div className="w-auto me-2 overflow-hidden">
        <FetchTable
          name={name}
          title={{
            text: headerTitle,
            class: `text-center ${headerColor}`
          }}
          url={url}
          columns={CreditCardTableSettings.columns}
          classes={tableClasses}
        />
      </div>)
  }

  const DebitTable = ({ name, headerTitle, headerColor, url }) => {
    return (
      <div className="w-auto me-2 overflow-hidden">
        <FetchTable
          name={name}
          title={{
            text: headerTitle,
            class: `text-center ${headerColor}`
          }}
          url={url}
          columns={DebitTableSettings.columns}
          classes={tableClasses}
        />
      </div>)
  }

  return (
    <div className="container-fluid">
      <div className='row'>
        <div className="col-8 row flex-wrap justify-content-center">
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
        <div className="col-4 row flex-wrap justify-content-center">
          {
            debitModules && debitModules.map((appModuleId, index) => {
              const url = `${urls.debits.latest}?AppModuleId=${appModuleId}`;
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
            })
          }
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
