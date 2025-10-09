import urls from "@/utils/urls";
import { useLoaderData } from "@remix-run/react";
import Table, { TableColumn } from "@/components/ui/utils/Table";
import { toNumber, ValueLike } from "@/utils/common";

interface EndpointsCollection {
    pesoDebits: unknown;
    dollarDebits: unknown;
    pesosModuleId: string;
    dollarsModuleId: string;
}

const TableColumns: Array<TableColumn> = [
    {
        id: "origin",
        label: "Origen",
        mapper: (record: unknown) => {
            if (typeof record === "object" && record !== null) {
                const r = record as Record<string, unknown>;
                const origin = r.origin as Record<string, unknown> | undefined;
                return (
                    (origin && (origin.name as string)) ??
                    (r.name as string) ??
                    "-"
                );
            }
            return "-";
        },
    },
    {
        id: "amount_value",
        label: "Monto",
        className: "text-right",
        headerClassName: "text-right",
        mapper: (d: unknown) =>
            toNumber(
                (d && typeof d === "object"
                    ? (d as Record<string, unknown>).amount
                    : d) as ValueLike,
                0
            ),
    },
];

function MonthlyDebits() {
    // Use the loader data
    const { pesoDebits, dollarDebits, pesosModuleId, dollarsModuleId } =
        useLoaderData<EndpointsCollection>();

    const pesosTableName = "pesos-debit-table";
    const dollarsTableName = "dollars-debit-table";

    interface ColumnProps {
        title: string;
        tableName?: string;
        data: unknown;
        adminSettings?: unknown;
        columns: TableColumn[];
    }

    const Column = ({ title, data, columns }: ColumnProps) => {
        return (
            <div className="col w-1/2 px-10 py-5">
                <div className="row">
                    <div className="col text-center">
                        <span className="fs-3">{title}</span>
                    </div>
                </div>
                <hr className="mt-1 mb-1" />
                <Table
                    columns={columns}
                    data={(data as { items?: unknown[] })?.items ?? []}
                />

                {/* <Table
          name={tableName}
          admin={adminSettings}
          url={url}
          columns={columns}
        /> */}
            </div>
        );
    };

    return (
        <div>
            <div className="row flex">
                <Column
                    title={"Monto"}
                    tableName={pesosTableName}
                    data={pesoDebits}
                    adminSettings={{
                        endpoint: urls.debits.monthly.endpoint,
                        key: {
                            id: "AppModuleId",
                            value: pesosModuleId,
                        },
                    }}
                    columns={TableColumns}
                />
                <Column
                    title={"Dólares"}
                    tableName={dollarsTableName}
                    data={dollarDebits}
                    adminSettings={{
                        endpoint: urls.debits.monthly.endpoint,
                        key: {
                            id: "AppModuleId",
                            value: dollarsModuleId,
                        },
                    }}
                    columns={TableColumns}
                />
            </div>
        </div>
    );
}

export default MonthlyDebits;
