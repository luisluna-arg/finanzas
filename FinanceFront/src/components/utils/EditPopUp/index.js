import React, { useState } from 'react';

import PropTypes from 'prop-types'

import Button from 'react-bootstrap/Button'
import PopUpModal from '../PopUpModal'
import { useStateContext/*, Provider*/ } from '../../../context';

const EditPopUp = (props) => {

    const stateContext = useStateContext();

    const [visible, setVisible] = useState(false)
    const [, setMovementId] = useState(null)

    const handleVisible = () => {
        if (!stateContext.context.selectedIds || stateContext.context.selectedIds.length === 0) {
            alert("Debe seleccionar un elemento a editar");
        }
        else if (!!stateContext.context.selectedIds && stateContext.context.selectedIds.length !== 1) {
            alert("Debe seleccionar sólo un elemento a editar");
        }
        else {
            setMovementId(stateContext.context.selectedIds[0]);
            setVisible(true);
        }
    }

    return (
        <div className="d-inline pb-1 pe-1">
            <Button id="show-btn" onClick={handleVisible} className="btn btn-primary p-1 me-1">
                Editar
            </Button>
            <PopUpModal visible={visible} />
        </div>
    )
}

EditPopUp.propTypes = {
    formId: PropTypes.string,
    editorSettings: PropTypes.any,
    form: PropTypes.any,
    title: PropTypes.string,
}

export default EditPopUp
