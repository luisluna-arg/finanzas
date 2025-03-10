import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function Debits() {
  const [pesosEndpoint, setPesosEndpoint] = useState(``);
  const [dollarsEndpoint, setDollarsEndpoint] = useState(``);

  const pesosModuleId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
  const dollarsModuleId = "03cc66c7-921c-4e05-810e-9764cd365c1d";

  const pesosTableName = "pesos-debit-table";
  const dollarsTableName = "dollars-debit-table";

  const refreshEndpoints = () => {
    setPesosEndpoint(`${urls.debits.paginated}?AppModuleId=${pesosModuleId}&Frequency=monthly`);
    setDollarsEndpoint(`${urls.debits.paginated}?AppModuleId=${dollarsModuleId}&Frequency=monthly`);
  }

  const onFetchMovementsTable = (data) => {
  }

  const valueConditionalClass = {
    class: "text-success fw-bold",
    eval: (field) => field != null && field.value > 0
  };

  const valueMapper = (field) => field != null ? field.value : null;

  const numericHeader = {
    classes: "text-end",
    style: {
      width: "120px"
    }
  };

  const TableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder: "Fecha",
      type: InputControlTypes.DateTime,
      editable: false,
      datetime: {
        timeFormat: "HH:mm",
        timeIntervals: 15,
        dateFormat: "DD/MM/YYYY HH:mm",
        placeholder: "Seleccionar fecha",
      },
      header: {
        style: {
          width: "160px"
        }
      }
    },
    {
      id: "origin",
      label: "Gasto/Servicio",
      placeholder: "Gasto/Servicio",
      editable: true
    },
    {
      id: "amount",
      label: "Monto",
      placeholder: "Monto",
      type: InputControlTypes.Decimal,
      min: 0.0,
      header: numericHeader,
      class: "text-end",
      editable: true,
      mapper: valueMapper,
      conditionalClass: valueConditionalClass
    },
  ];

  useEffect(() => {
    refreshEndpoints();
  }, []);

  const Column = ({
    title,
    tableName,
    url,
    adminSettings,
    onFetch,
    columns
  }) => {

    return (<div className="col">
      <div className="row">
        <div className="col text-center">
          <span className="fs-5">{title}</span>
        </div>
      </div>
      <hr className="mt-1 mb-1" />
      <PaginatedTable
        name={tableName}
        admin={adminSettings}
        url={url}
        onFetch={onFetch}
        columns={columns} />
    </div>)
  };

  return (
    <div className="container pt-3 pb-3">
      <div className="row">
        <Column
          title={"Pesos"}
          tableName={pesosTableName}
          url={pesosEndpoint}
          adminSettings={{
            endpoint: urls.debits.endpoint,
            key: {
              id: "AppModuleId",
              value: pesosModuleId
            }
          }}
          onFetch={onFetchMovementsTable}
          columns={TableColumns}
        />
        <Column
          title={"Dólares"}
          tableName={dollarsTableName}
          url={dollarsEndpoint}
          adminSettings={{
            endpoint: urls.debits.endpoint,
            key: {
              id: "AppModuleId",
              value: dollarsModuleId
            }
          }}
          onFetch={onFetchMovementsTable}
          columns={TableColumns}
        />
      </div>

    </div>
  );
}

export default Debits;
