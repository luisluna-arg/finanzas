// import styles from './styles.module.css';

import React, { useEffect, useState } from 'react';

import PropTypes from 'prop-types'

import Button from 'react-bootstrap/Button'
import Modal from 'react-bootstrap/Modal'
import Form from 'react-bootstrap/Form'
import PopUpInput from '../PopUpInput'
import { ApiUrls, APIs } from '../../../utils/commons';
import { useStateContext/*, Provider*/ } from '../../../context';
import { shallow } from 'zustand/shallow';
import useMovementsStore from "../../../zustand/stores/generic";

const DEFAULTS = {
    formId: 'DefaultForm',
    title: 'DefaultTitle',
    editorSettings: [],
    form: {},
}

const setPropsDefaults = (originalProps) => {
    let fullProps = Object.assign({}, DEFAULTS, originalProps)

    if (
        typeof originalProps.editorSettings == 'undefined' ||
        originalProps.editorSettings == null
    ) {
        fullProps.editorSettings = DEFAULTS.editorSettings
    }

    if (typeof originalProps.form == 'undefined' || originalProps.form == null) {
        fullProps.form = DEFAULTS.form
    }

    if (
        typeof originalProps.title == 'undefined' ||
        originalProps.title == null
    ) {
        fullProps.title = DEFAULTS.title
    }

    for (let i = 0; i < fullProps.editorSettings.length; i++) {
        const fieldSettings = fullProps.editorSettings[i]

        if (
            !(fieldSettings.id in fullProps.form) ||
            (typeof fullProps.form[fieldSettings.id] == 'undefined' &&
                fullProps.form[fieldSettings.id] == null)
        )
            fullProps.form[fieldSettings.id] = ''
    }

    return fullProps
}

const PopUpModal = (props) => {
    let { title, formId, editorSettings, form } = setPropsDefaults(props)

    const stateContext = useStateContext();

    const [show, setShow] = useState(props.visible)
    const [reloadSingle, setReloadSingle] = useState(false)
    const [movementId, setMovementId] = useState(null)

    const handleClose = () => {
        setShow(false);
        clearSingle();
    }

    const { getSingle, single, clearSingle, isLoading } = useMovementsStore(state => ({
        getSingle: state.getSingle,
        single: state.single,
        isLoading: state.isLoading,
        clearSingle: state.clearSingle
    }), shallow); // Using zustand


    const sendForm = function (formValues, callback) {
        if (typeof callback == 'function') {
            callback()
        }
    }

    const resetForm = () => {
        document.getElementById(formId).reset()
        for (let field in form) {
            delete form[field]
        }
    }

    const showModal = (isVisible) => {
        setShow(!!isVisible)
    }

    const getFieldValue = (formValueId, record) => {
        // console.log("formValueId", formValueId);
        // console.log("record", record);
        switch (formValueId) {
            case "date": return record.timeStamp;
            case "concept1": return record.concept1;
            case "concept2": return record.concept2;
            case "movement": return record.ammount;
            default: return null;
        }
    }

    const cancel = () => {
        showModal(false)
        resetForm()
    }

    const accept = () => {
        let showModalCallback = showModal
        let resetFormCallback = resetForm
        sendForm(form, function () {
            showModalCallback(false)
            resetFormCallback()
        })
    }

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header className={["text-light", "bg-success"]} closeButton>
                <Modal.Title>{title}</Modal.Title>
            </Modal.Header>
            <Modal.Body className='bg-dark'>
                <Form id={formId} onSubmit={accept} onReset={cancel}>
                    {editorSettings.map((fieldSettings, index) => (
                        <PopUpInput
                            settings={fieldSettings}
                            value={getFieldValue(fieldSettings.id, single)}
                            key={fieldSettings.id + '-' + index}
                        />
                    ))}
                    <div className={['mt-3']}>
                        <Button className="btn btn-primary mr-2" type="submit" variant="primary">
                            Aceptar
                        </Button>
                        <Button className="btn btn-danger" type="reset" variant="danger">
                            Cancelar
                        </Button>
                    </div>
                </Form>
            </Modal.Body>
        </Modal>
    )
}

PopUpModal.propTypes = {
    formId: PropTypes.string,
    editorSettings: PropTypes.any,
    form: PropTypes.any,
    title: PropTypes.string,
    visible: PropTypes.bool
}

export default PopUpModal;
