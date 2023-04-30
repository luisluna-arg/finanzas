import React, { useState } from 'react';

import ImportButton from '../../utils/ImportButton';
import CreatePopUp from '../../utils/CreatePopUp';
import EditPopUp from '../../utils/EditPopUp';
import DeletePopUp from '../../utils/DeletePopUp';
import FundsGrid from '../FundsGrid';
import FundsTotal from '../FundsTotal';

import { UploadTypes } from '../../../utils/commons';
import { Outlet } from 'react-router-dom'

const FundsUploadType = UploadTypes.Funds;
const CreateEditPopUpSettings = [
    { id: "TimeStamp", type: "DateInput", label: "Fecha" },
    { id: "Concept1", type: "TextInput", label: "Concepto 1" },
    { id: "Concept2", type: "TextInput", label: "Concepto 2" },
    { id: "Amount", type: "DecimalInput", label: "Movimiento" },
    { id: "Total", type: "DecimalInput", label: "Fondos" }
];
const DeletePopUpSettings = [];
const DeletePopUpActions = {
    confirmCallback: null
};

function Funds() {
    const [reloadGridOn, setReloadGridOn] = useState(false);

    const enableReload = function () { setReloadGridOn(true); }
    const disableReload = function () { setReloadGridOn(false); }

    DeletePopUpActions.confirmCallback = function () {
        enableReload();
    }

    const create = (data) => {
        alert("Funciona");

        // movementsApi
        console.log("data", data);
    }

    const cancel = () => { }

    return (
        <div className='ps-3 pe-3'>
            <ImportButton UploadType={FundsUploadType} id="fundImport" />
            <FundsTotal />
            <div className="input-group mb-3">
                <div className="input-group-prepend">
                    <CreatePopUp editorSettings={CreateEditPopUpSettings} formId="fundsForm" title="Alta de movimiento" accept={create} cancel={cancel} />
                    <EditPopUp editorSettings={CreateEditPopUpSettings} formId="fundsForm" title="Edición de movimiento" />
                    <DeletePopUp editorSettings={DeletePopUpSettings} actions={DeletePopUpActions} formId="fundsForm" title="Eliminar registros" />
                </div>
            </div>
            {/* <FondosChart /> */}
            <FundsGrid reloadGridOn={reloadGridOn} disableReload={disableReload} />
            <Outlet />
        </div>
    );
};

Funds.propTypes = {};

export default Funds;
