import React from 'react';
// import PropTypes from 'prop-types';
import styles from './Funds.module.css';

import ImportButton from '../../utils/ImportButton/ImportButton';
import CRUDPopUp from '../../utils/CRUDPopUp/CRUDPopUp';
import FundsGrid from '../FundsGrid/FundsGrid';
import FundsTotal from '../FundsTotal/FundsTotal';

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
      <CRUDPopUp editorSettings={CRUDPopUpSettings} formId="fundsForm" />
      {/* <FondosChart /> */}
      <FundsGrid />
      <FundsTotal />

      <Outlet />
    </div>
  );
};

Funds.propTypes = {};

Funds.defaultProps = {};

export default Funds;
