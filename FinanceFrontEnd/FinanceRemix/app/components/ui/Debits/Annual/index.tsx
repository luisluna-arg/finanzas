import urls from "@/utils/urls";
import { useLoaderData } from "@remix-run/react";
import { toNumber } from "@/utils/common";
import { InputType } from "@/components/ui/utils/InputType";
import PaginatedTable from "@/components/ui/utils/PaginatedTable";

interface EndpointsCollection {
    pesosEndpoint?: string,
    dollarsEndpoint?: string,
    pesosModuleId?: string,
    dollarsModuleId?: string
  }

function AnnualDebits() {
  // Use the loader data
  const { pesosEndpoint, dollarsEndpoint, pesosModuleId, dollarsModuleId } =
    useLoaderData<EndpointsCollection>();

  const pesosTableName = "pesos-debit-table";
  const dollarsTableName = "dollars-debit-table";

  const onFetchMovementsTable = (data: any) => {};

  const valueConditionalClass = {
    class: "text-success fw-bold",
    eval: (field: any) => field != null && toNumber(field) > 0,
  };

  const valueMapper = (field: any) => (field != null ? toNumber(field) : null);

  const numericHeader = {
    classes: "text-end",
    style: {
      width: "120px",
    },
  };

  const TableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder: "Fecha",
      type: InputType.DateTime,
      editable: false,
      datetime: {
        timeFormat: "HH:mm",
        timeIntervals: 15,
        dateFormat: "DD/MM/YYYY",
        placeholder: "Seleccionar fecha",
      },
      header: {
        style: {
          width: "160px",
        },
      },
    },
    {
      id: "origin",
      label: "Gasto/Servicio",
      placeholder: "Gasto/Servicio",
      editable: true,
    },
    {
      id: "amount",
      label: "Monto",
      placeholder: "Monto",
      type: InputType.Decimal,
      min: 0.0,
      header: numericHeader,
      class: "text-end",
      editable: true,
      mapper: valueMapper,
      conditionalClass: valueConditionalClass,
    },
  ];

  const Column = ({
    title,
    tableName,
    url,
    adminSettings,
    onFetch,
    columns,
  }: any) => {
    return (
      <div className="col">
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
          columns={columns}
        />
      </div>
    );
  };

  return (
    <div className="container pt-3 pb-3">
      <div className="row">
        <Column
          title={"Monto"}
          tableName={pesosTableName}
          url={pesosEndpoint}
          adminSettings={{
            endpoint: urls.debits.monthly.endpoint,
            key: {
              id: "AppModuleId",
              value: pesosModuleId,
            },
          }}
          onFetch={onFetchMovementsTable}
          columns={TableColumns}
        />
        <Column
          title={"Dólares"}
          tableName={dollarsTableName}
          url={dollarsEndpoint}
          adminSettings={{
            endpoint: urls.debits.monthly.endpoint,
            key: {
              id: "AppModuleId",
              value: dollarsModuleId,
            },
          }}
          onFetch={onFetchMovementsTable}
          columns={TableColumns}
        />
      </div>
    </div>
  );
}

export default AnnualDebits;
