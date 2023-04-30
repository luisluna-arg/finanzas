import React, { useState } from 'react'

import PropTypes from 'prop-types'

import Button from 'react-bootstrap/Button'
import Modal from 'react-bootstrap/Modal'
import Form from 'react-bootstrap/Form'
import CreatePopUpInput from '../PopUpInput'
import movementsApi from '../../../api/movementsApi'

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
    const handleShow = (e) => {
        e.preventDefault();
        setShow(true);
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

    const accept = (event) => {
        event.preventDefault();

        let data = {};

        editorSettings.forEach((item) => {
            data[item.id] = event.target[item.id].value;
        });

        props.accept(data);

        console.log(movementsApi);

        movementsApi.create(data, (result) => {
            console.log("Result", result);
        });
    }

    return (
        <div className="d-inline pb-1 pe-1">
            <Button id="show-btn" onClick={handleShow} className="btn btn-primary p-1 me-1">
                Registrar
            </Button>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header className={["text-light", "bg-success"]} closeButton>
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
                        <div className={['mt-3']}>
                            <Button className="btn btn-primary me-2" type="submit" variant="primary">
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
    accept: PropTypes.any,
    cancel: PropTypes.any,
}

export default CreatePopUp
