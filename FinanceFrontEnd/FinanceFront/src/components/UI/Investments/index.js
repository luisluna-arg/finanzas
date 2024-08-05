import React, { useEffect, useState } from "react";
import urls from "../../../routing/urls";
import Uploader from "../../utils/Uploader";
import PaginatedTable from "../../utils/PaginatedTable";
import { InputControlTypes } from "../../utils/InputControl";

function Movements() {
  const [reloadData, setReloadData] = useState(false);

  const AppModuleTypeEnumFundsId = 3;

  const IOLInvestmentsUploadEndpoint = `${urls.iolInvestments.upload}?DateKind=Local&AppModuleId=${AppModuleTypeEnumFundsId}`;
  const iolInvestmentsEndpoint = `${urls.iolInvestments.paginated}`;

  const onFetchInvestmentsTable = (data) => {

  }

  useEffect(() => {
  }, [
    AppModuleTypeEnumFundsId,
  ]);

  const numericColumn = (columnId, label) => ({
    id: columnId,
    label: label,
    placeholder,
    headerClass: "text-end",
    class: "text-end",
    editable: true,
    mapper: (field) => parseFloat(field.toFixed(2)),
    conditionalClass: [
      {
        class: "text-success fw-bold",
        eval: (field) => field > 0
      },
      {
        class: "text-danger fw-bold",
        eval: (field) => field < 0
      }
    ]
  });

  const placeholder = "Ingrese un valor";
  const investmentsTableColumns = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder,
      type: InputControlTypes.DateTime,
      editable: true,
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
      id: "asset",
      label: "Activo",
      placeholder,
      editable: true,
      mapper: (field) => `${field.symbol} - ${field.description}`,
    },
    numericColumn("alarms", "Alarmas"),
    numericColumn("quantity", "Cantidad"),
    numericColumn("assets", "Activos comp."),
    numericColumn("dailyVariation", "Variación diaria"),
    numericColumn("lastPrice", "Último precio"),
    numericColumn("averageBuyPrice", "Precio prom. compra"),
    numericColumn("averageReturnPercent", "Rendimiento (%)"),
    numericColumn("averageReturn", "Rendimiento prom."),
    numericColumn("valued", "Valorado")
  ];

  let enabled = iolInvestmentsEndpoint;

  return (
    <>
      <div className="container pt-3 pb-3">
        {!enabled && <div>Cargando...</div>}
        {enabled && <div>
          <hr className="py-1" />
          <Uploader
            url={IOLInvestmentsUploadEndpoint}
            extensions={[".xls", ".xlsx"]}
            onSuccess={() => { setReloadData(true); }}
          />
          <hr className="py-1" />
          <PaginatedTable
            name={"investments-table"}
            url={iolInvestmentsEndpoint}
            rowCount={20}
            admin={{
              endpoint: urls.iolInvestments.endpoint,
              addEnabled: false
            }}
            onFetch={onFetchInvestmentsTable}
            columns={investmentsTableColumns} 
            reloadData={reloadData}
            />
        </div>
        }
      </div>
    </>
  );
}

export default Movements;
