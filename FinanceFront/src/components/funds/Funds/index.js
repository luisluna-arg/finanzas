import React, { useState } from 'react';

import ImportButton from '../../utils/ImportButton';
import CreatePopUp from '../../utils/CreatePopUp';
import EditPopUp from '../../utils/EditPopUp';
import DeletePopUp from '../../utils/DeletePopUp';
import FundsGrid from '../FundsGrid';
import FundsTotal from '../FundsTotal';

import { UploadTypes } from '../../../utils/commons';
import { Outlet } from 'react-router-dom'
import { useStateContext/*, Provider*/ } from '../../../context';

const FundsUploadType = UploadTypes.Funds;
const CreateEditPopUpSettings = [
    { id: "date", type: "DateInput", label: "Fecha" },
    { id: "concept1", type: "TextInput", label: "Concepto 1" },
    { id: "concept2", type: "TextInput", label: "Concepto 2" },
    { id: "movement", type: "DecimalInput", label: "Movimiento" },
    { id: "total", type: "DecimalInput", label: "Fondos" }
];
const DeletePopUpSettings = [];
const DeletePopUpActions = {
    confirmCallback: null
};

function Funds() {
    const [reloadGridOn, setReloadGridOn] = useState(false);

    const enableReload = function () { setReloadGridOn(true); }
    const disableReload = function () { setReloadGridOn(false); }

    const stateContext = useStateContext();

    DeletePopUpActions.confirmCallback = function () {
        enableReload();
    }

    return (
        <>
            <ImportButton UploadType={FundsUploadType} id="fundImport" />
            <div className="input-group mb-3">
                <div className="input-group-prepend">
                    <CreatePopUp editorSettings={CreateEditPopUpSettings} formId="fundsForm" title="Alta de movimiento" />
                    <EditPopUp editorSettings={CreateEditPopUpSettings} formId="fundsForm" title="Edición de movimiento" />
                    <DeletePopUp editorSettings={DeletePopUpSettings} actions={DeletePopUpActions} formId="fundsForm" title="Eliminar registros" />
                </div>
            </div>
            {/* <FondosChart /> */}
            <FundsGrid reloadGridOn={reloadGridOn} disableReload={disableReload} />
            <FundsTotal />
            <Outlet />
        </>
    );
};

Funds.propTypes = {};

export default Funds;
