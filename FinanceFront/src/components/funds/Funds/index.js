import React from 'react';
// import PropTypes from 'prop-types';
import styles from './styles.css';

import ImportButton from '../../utils/ImportButton';
import CreatePopUp from '../../utils/CreatePopUp';
import DeletePopUp from '../../utils/DeletePopUp';
import FundsGrid from '../FundsGrid';
import FundsTotal from '../FundsTotal';

import { UploadTypes } from '../../../utils/commons';

import { Outlet } from 'react-router-dom'

const FundsUploadType = UploadTypes.Funds;
const CreatePopUpSettings = [
    { id: "name", type: "TextInput", label: "Nombre" },
    { id: "lastname", type: "TextInput", label: "Apellido" }
];
const DeletePopUpSettings = [];

function Funds() {
    return (
        <div class="tab-content">
            <ImportButton UploadType={FundsUploadType} id="fundImport" />
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <CreatePopUp editorSettings={CreatePopUpSettings} formId="fundsForm" title="Alta de movimiento" />
                    <DeletePopUp editorSettings={DeletePopUpSettings} formId="fundsForm" title="Eliminar registros" />
                </div>
            </div>
            {/* <FondosChart /> */}
            <FundsGrid />
            <FundsTotal />
            <Outlet />
        </div>
    );
};

Funds.propTypes = {};

export default Funds;
