import React/*, { useState }*/ from 'react';
// import PropTypes from 'prop-types';
import styles from './styles.css';
// import ReactDataGrid from '@inovua/reactdatagrid-community'
// import '@inovua/reactdatagrid-community/index.css'

const FundsGrid = () => {

  // const columns = [
  //   { name: "Id", header: "Id" },
  //   { name: "Fecha", header: "Fecha" },
  //   { name: "Concepto", header: "Concepto" },
  //   { name: "Concepto2", header: "Concepto2" },
  //   { name: "Movimiento", header: "Movimiento" },
  //   { name: "Fondos", header: "Fondos" },
  //   { name: "Ingresos", header: "Ingresos" }
  // ];

  // const gridStyle = { minHeight: 550 }

  // const dataSource = [
  //   {
  //     Id: 1,
  //     Fecha: "Fecha",
  //     Concepto: "Concepto",
  //     Concepto2: "Concepto2",
  //     Movimiento: "Movimiento",
  //     Fondos: "Fondos",
  //     Ingresos: "Ingresos"
  //   }
  // ];


  return (
    <div className={styles.FundsGrid}>
      {/* <ReactDataGrid
        idProperty="id"
        columns={columns}
        dataSource={dataSource}
        style={gridStyle}
      /> */}
    </div >
  )
};

FundsGrid.propTypes = {};

export default FundsGrid;
