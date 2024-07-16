import React from "react";

function Table({ columns, items }) {
  return (
    <table className="table">
      <thead>
        <tr>
          {columns.map((column, index) => (
            <th scope="col" key={index}>
              {column}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {items.map((item, index) => {
          return (
            <tr key={item.rowId} id={item.rowId}>
              {item.values.map((value, index) => (
                <td>{value}</td>
              ))}
            </tr>
          );
        })}
      </tbody>
    </table>
  );
}

export default Table;
