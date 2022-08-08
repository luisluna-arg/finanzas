import React, { useState } from 'react';

import PropTypes from 'prop-types';

import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import Form from 'react-bootstrap/Form';
import CRUDPopUpInput from '../CRUDPopUpInput/CRUDPopUpInput'

const CRUDPopUp = (props) => {
  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  const sendForm = function (formValues, callback) {
    if (typeof callback == "function") {
      callback();
    }
  }

  const resetForm = () => {
    document.getElementById(props.formId).reset();
    for (let field in props.form) {
      delete props.form[field];
    }
  };

  const showModal = (isVisible) => {
    setShow(!!isVisible);
  };

  const cancel = () => {
    showModal(false);
    resetForm();
  };

  const accept = () => {
    let showModalCallback = showModal;
    let resetFormCallback = resetForm;
    sendForm(props.form, function () {
      showModalCallback(false);
      resetFormCallback();
    });
  }

  console.log(this);

  return (<div>
    <Button id="show-btn" onClick={handleShow}>Registrar</Button>

    <Modal show={show} onHide={handleClose} onSubmit={accept} onReset={cancel}>
      <Modal.Header closeButton>
        <Modal.Title>Using Component Methods</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form id={props.formId} onSubmit="accept" onReset="cancel">
          {props.editorSettings.map((fieldSettings, index) => (
            <CRUDPopUpInput settings={fieldSettings} value={props.form[fieldSettings.id]} key={fieldSettings.id + '-' + index} />
          ))}
          <Button type="submit" variant="primary">Aceptar</Button>
          <Button type="reset" variant="danger">Cancelar</Button>
        </Form>
      </Modal.Body>
    </Modal >
  </div >)
};

CRUDPopUp.propTypes = {
  formId: PropTypes.string,
  editorSettings: PropTypes.array,
  form: PropTypes.any
};

CRUDPopUp.defaultProps = {
  formId: 'DefaultForm',
  editorSettings: [],
  form: {}
};

export default CRUDPopUp;
