import React, { useState, useEffect } from 'react';
import FetchTable from "../../utils/FetchTable";
import urls from "../../../routing/urls";

const TableSettings = {
  columns: [
    { label: "Concepto", id: "concept" },
    {
      label: "Monto", id: "amount", mapper: (r) => {
        return r.value;
      }
    },
    { label: "Dólares", id: "amountDollars", mapper: (r) => r.value },
    { label: "Cuota", id: "paymentNumber" },
    { label: "Total", id: "planSize" }
  ],
};

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

  const ContentTable = ({ name, headerTitle, headerColor, url }) => {
    return (
      <div className="w-auto me-2 overflow-hidden">
        <FetchTable
          name={name}
          title={{
            text: headerTitle,
            class: `${headerColor} text-center`
          }}
          url={url}
          columns={TableSettings.columns}
          classes={tableClasses}
        />
      </div>)
  }

  return (
    <div className="container-fluid">
      <div className="row flex-wrap  justify-content-center">
        {
          creditCardData && creditCardData.map((data, index) => {
            const url = `${urls.creditCardMovements.get}?CreditCardId=${data.id}`;
            const bgClass = backgroundClasses[index < backgroundClasses.length ? index : backgroundClasses.length - index - 1];
            return (<ContentTable
              key={index}
              name={`${data.name}-${data.bank.name}-table`}
              headerTitle={`${data.name} ${data.bank.name}`}
              headerColor={`${bgClass} text-light`}
              url={url}
            />);
          })
        }
      </div>
    </div>
  );
}

export default Dashboard;
