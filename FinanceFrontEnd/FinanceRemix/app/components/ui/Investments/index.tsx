import { useEffect, useState } from "react";
import urls from "@/utils/urls";
import Uploader from "@/components/ui/utils/Uploader";
import PaginatedTable, { Column } from "@/components/ui/utils/PaginatedTable";
import { InputType } from "@/components/ui/utils/InputType";

type InvestmentField = {
  symbol: string;
  description: string;
};

function Movements() {
  const [reloadData, setReloadData] = useState<boolean>(false);

  const AppModuleTypeEnumFundsId = 3;

  const IOLInvestmentsUploadEndpoint = `${urls.iolInvestments.upload}?DateKind=Local&AppModuleId=${AppModuleTypeEnumFundsId}`;
  const iolInvestmentsEndpoint = `${urls.iolInvestments.paginated}`;

  const onFetchInvestmentsTable = (data: any) => {
  };

  useEffect(() => {
  }, [AppModuleTypeEnumFundsId]);

  const numericColumn = (columnId: string, label: string): Column => ({
    id: columnId,
    label: label,
    placeholder,
    header: {
      classes: ["text-end"]
    },
    class: "text-end",
    editable: true,
    mapper: (field: number) => parseFloat(field.toFixed(2)),
    conditionalClass: [
      {
        class: "text-success fw-bold",
        eval: (field: number) => field > 0,
      },
      {
        class: "text-danger fw-bold",
        eval: (field: number) => field < 0,
      },
    ],
  });

  const placeholder = "Ingrese un valor";

  const investmentsTableColumns: Column[] = [
    {
      id: "timeStamp",
      label: "Fecha",
      placeholder,
      type: InputType.DateTime,
      editable: true,
      datetime: {
        timeIntervals: 15,
        dateFormat: "DD/MM/yyyy",
        timeFormat: "HH:mm",
        placeholder: "Seleccionar fecha",
      },
      header: {
        style: {
          width: "160px",
        },
      },
    },
    {
      id: "asset",
      label: "Activo",
      placeholder,
      editable: true,
      mapper: (field: InvestmentField) => `${field.symbol} - ${field.description}`,
    },
    numericColumn("alarms", "Alarmas"),
    numericColumn("quantity", "Cantidad"),
    numericColumn("assets", "Activos comp."),
    numericColumn("dailyVariation", "Variación diaria"),
    numericColumn("lastPrice", "Último precio"),
    numericColumn("averageBuyPrice", "Precio prom. compra"),
    numericColumn("averageReturnPercent", "Rendimiento (%)"),
    numericColumn("averageReturn", "Rendimiento prom."),
    numericColumn("valued", "Valorado"),
  ];

  let enabled = Boolean(iolInvestmentsEndpoint);

  return (
    <>
      <div className="container pt-3 pb-3">
        {!enabled && <div>Cargando...</div>}
        {enabled && (
          <div>
            <hr className="py-1" />
            <Uploader
              url={IOLInvestmentsUploadEndpoint}
              extensions={[".xls", ".xlsx"]}
              onSuccess={() => {
                setReloadData(true);
              }}
            />
            <hr className="py-1" />
            <PaginatedTable
              name={"investments-table"}
              url={iolInvestmentsEndpoint}
              rowCount={20}
              admin={{
                endpoint: urls.iolInvestments.endpoint,
                addEnabled: false,
              }}
              onFetch={onFetchInvestmentsTable}
              columns={investmentsTableColumns}
              reloadData={reloadData}
            />
          </div>
        )}
      </div>
    </>
  );
}

export default Movements;
