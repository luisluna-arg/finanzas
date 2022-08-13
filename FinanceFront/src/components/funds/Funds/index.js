import React from 'react';
// import PropTypes from 'prop-types';
import styles from './styles.css';

import ImportButton from '../../utils/ImportButton';
import CRUDPopUp from '../../utils/CRUDPopUp';
import FundsGrid from '../FundsGrid';
import FundsTotal from '../FundsTotal';

import { UploadTypes } from '../../../utils/commons';

import { Outlet } from 'react-router-dom'

// import { URLs } from "./../../../router/urls";

const FundsUploadType = UploadTypes.Funds;
const CRUDPopUpSettings = [
  { id: "name", type: "TextInput", label: "Nombre" },
  { id: "lastname", type: "TextInput", label: "Apellido" }
];

function Funds() {
  return (
    <div className={"page " + styles.Funds}>
      <ImportButton UploadType={FundsUploadType} id="fundImport" />
      <CRUDPopUp editorSettings={CRUDPopUpSettings} formId="fundsForm" title="Alta de movimiento" />
      {/* <FondosChart /> */}
      <FundsGrid />
      <FundsTotal />

      <Outlet />
    </div>
  );
};

Funds.propTypes = {};

export default Funds;
