import React, { useState } from 'react';

import ImportButton from '../../utils/ImportButton';
import CreatePopUp from '../../utils/CreatePopUp';
import EditPopUp from '../../utils/EditPopUp';
import DeletePopUp from '../../utils/DeletePopUp';
import FundsGrid from '../FundsGrid';
import FundsTotal from '../FundsTotal';

import { UploadTypes } from '../../../utils/commons';
import { Outlet } from 'react-router-dom'
import dateFormat from './../../../utils/dates.js';
import MovementsService from '../../../services/movementsService';

const FundsUploadType = UploadTypes.Funds;
const CreateEditPopUpSettings = [
    { id: "timeStamp", type: "DateInput", label: "Fecha" },
    { id: "concept1", type: "TextInput", label: "Concepto 1" },
    { id: "concept2", type: "TextInput", label: "Concepto 2" },
    { id: "amount", type: "DecimalInput", label: "Movimiento" },
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

    DeletePopUpActions.confirmCallback = function () {
        enableReload();
    }

    const create = (data) => {
        data.timeStamp = dateFormat.fromInputToRequest(data.timeStamp);
        console.log("data", data);
        MovementsService.create(data);
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
