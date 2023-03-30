// import styles from './styles.module.css';

import React, { useState } from 'react'

import PropTypes from 'prop-types'

import Button from 'react-bootstrap/Button'
import Modal from 'react-bootstrap/Modal'
import Form from 'react-bootstrap/Form'
import CreatePopUpInput from '../CreatePopUpInput'

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

const CreatePopUp = (props) => {
    let { title, formId, editorSettings, form } = setPropsDefaults(props)

    const [show, setShow] = useState(false)

    const handleClose = () => setShow(false)
    const handleShow = () => setShow(true)

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
        <div className="d-inline pb-1 pr-1">
            <Button id="show-btn" onClick={handleShow} className="btn btn-primary p-1 mr-1">
                Registrar
            </Button>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header className='bg-dark' closeButton>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body className='bg-dark'>
                    <Form id={formId} onSubmit={accept} onReset={cancel}>
                        {editorSettings.map((fieldSettings, index) => (
                            <CreatePopUpInput
                                settings={fieldSettings}
                                value={form[fieldSettings.id]}
                                key={fieldSettings.id + '-' + index}
                            />
                        ))}
                        <div class='mt-1'>
                            <Button className="btn btn-primary mr-1" type="submit" variant="primary">
                                Aceptar
                            </Button>
                            <Button className="btn btn-danger" type="reset" variant="danger">
                                Cancelar
                            </Button>
                        </div>
                    </Form>
                </Modal.Body>
            </Modal>
        </div>
    )
}

CreatePopUp.propTypes = {
    formId: PropTypes.string,
    editorSettings: PropTypes.any,
    form: PropTypes.any,
    title: PropTypes.string,
}

export default CreatePopUp
