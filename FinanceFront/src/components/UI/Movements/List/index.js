// Movements.js
import React, { useState } from "react";
import { Table } from "react-bootstrap";
import urls from "../../../../routing/urls";
import usePaginatedQuery from "../../../../stores/PaginatedQuery";
import dateFormat from "../../../../utils/dates";
import CustomToast from "../../../utils/CustomToast";

const fetchMovements = async (page, pageSize) => {
  let url = `${urls.movements.paginated}?page=${page}&pageSize=${pageSize}`;
  const response = await fetch(url);
  return await response.json();
};

function MovementsList() {
  const [pageSize, setPageSize] = useState(10); // State for the number of items per page
  const [currentPage, setCurrentPage] = useState(0); // State for the current page

  // Use the usePaginatedQuery hook to fetch paginated data
  const { data, fetchNextPage, isFetchingNextPage, hasNextPage, isFetching } =
    usePaginatedQuery(["movements", currentPage, pageSize], fetchMovements);

  if (!data)
    return (
      <CustomToast text={"Cargando..."} position="top-center"></CustomToast>
    );

  return (
    <div className="container pt-3 pb-3">
      <Table className="table">
        <thead>
          <tr>
            <th scope="col">Fecha</th>
            <th>Fecha</th>
            <th>Concepto 1</th>
            <th>Concepto 2</th>
            <th>Monto</th>
            <th>Total</th>
          </tr>
        </thead>
        <tbody>
          {data.pages.map((page) =>
            page.items.map((movement) => (
              <tr key={movement.id}>
                <td>{dateFormat.toDisplay(movement.timeStamp)}</td>
                <td>{movement.timeStamp}</td>
                <td>{movement.concept1}</td>
                <td>{movement.concept2}</td>
                <td>{movement.amount.value}</td>
                <td>{movement.total.value}</td>
              </tr>
            ))
          )}
        </tbody>
      </Table>

      {isFetching && !isFetchingNextPage && (
        <CustomToast text={"Cargando..."}></CustomToast>
      )}
      {isFetchingNextPage && (
        <CustomToast text={"Cargando más..."}></CustomToast>
      )}
      {hasNextPage && (
        <button
          type="button"
          className="btn btn-outline-success"
          onClick={() => fetchNextPage()}
        >
          Load More
        </button>
      )}
    </div>
  );
}

export default MovementsList;
