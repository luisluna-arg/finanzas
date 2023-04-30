import React, { useState } from 'react'

import PropTypes from 'prop-types'

import Button from 'react-bootstrap/Button'
import Modal from 'react-bootstrap/Modal'
import Form from 'react-bootstrap/Form'

const DEFAULTS = {
    formId: 'DefaultForm',
    title: 'DefaultTitle',
    editorSettings: [],
    form: {},
    setReloadGrid: null
}

const setPropsDefaults = (originalProps) => {
    let fullProps = Object.assign({}, DEFAULTS, originalProps);

    if (typeof originalProps.editorSettings == 'undefined' ||
        originalProps.editorSettings == null) {
        fullProps.editorSettings = DEFAULTS.editorSettings;
    }

    if (typeof originalProps.form == 'undefined' || originalProps.form == null) {
        fullProps.form = DEFAULTS.form;
    }

    if (typeof originalProps.title == 'undefined' ||
        originalProps.title == null) {
        fullProps.title = DEFAULTS.title;
    }

    if (typeof originalProps.setReloadGrid == 'undefined' ||
        originalProps.setReloadGrid == null) {
        fullProps.setReloadGrid = DEFAULTS.setReloadGrid;
    }

    for (let i = 0; i < fullProps.editorSettings.length; i++) {
        const fieldSettings = fullProps.editorSettings[i];

        if (!(fieldSettings.id in fullProps.form) ||
            (typeof fullProps.form[fieldSettings.id] == 'undefined' &&
                fullProps.form[fieldSettings.id] == null))
            fullProps.form[fieldSettings.id] = '';
    }

    return fullProps;
}

const DeletePopUp = (props) => {
    let { title, formId, /*editorSettings,*/ form, actions } = setPropsDefaults(props)

    const [show, setShow] = useState(false);

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

    const sendForm = function (formValues, callback) {
        if (typeof callback == 'function') {
            callback();
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

    const cancel = () => {
        showModal(false)
        resetForm()
    }

    const accept = (e) => {
        e.preventDefault();
        let showModalCallback = showModal
        let resetFormCallback = resetForm
        sendForm(form, function () {
            showModalCallback(false);
            resetFormCallback();
            actions.confirmCallback();
        })
    }

    return (
        <div className="d-inline pb-1 pe-1">
            <Button id="show-btn" onClick={handleShow} className="btn btn-danger p-1 me-1">
                Eliminar
            </Button>

            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton className="bg-dark">
                    <Modal.Title className="fs-5">{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body className="bg-dark">
                    <Form id={formId} onSubmit={accept} onReset={cancel} >
                        <div>¿Desea eliminar los elementos seleccionados?</div>
                        <Button type="submit" variant="primary" className='mt-2 me-2'>
                            Aceptar
                        </Button>
                        <Button type="reset" variant="danger" className='mt-2'>
                            Cancelar
                        </Button>
                    </Form>
                </Modal.Body>
            </Modal>
        </div>
    )
}

DeletePopUp.propTypes = {
    formId: PropTypes.string,
    editorSettings: PropTypes.any,
    form: PropTypes.any,
    title: PropTypes.string,
    actions: PropTypes.any,
}

export default DeletePopUp
