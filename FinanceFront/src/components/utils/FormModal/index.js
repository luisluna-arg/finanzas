import React from "react";
import { Form, Modal } from "react-bootstrap";
import Button from "../Button";
import CustomToast from "../CustomToast";
import Input from "../FormInput";

const DEFAULTS = {
  formId: `form-${new Date().getTime()}`,
  title: "DefaultTitle",
  editorSettings: [],
  form: {},
};

const setPropsDefaults = (originalProps) => {
  let fullProps = Object.assign({}, DEFAULTS, originalProps);

  if (
    typeof originalProps.editorSettings === "undefined" ||
    originalProps.editorSettings === null
  ) {
    fullProps.editorSettings = DEFAULTS.editorSettings;
  }

  if (typeof originalProps.form === "undefined" || originalProps.form === null) {
    fullProps.form = DEFAULTS.form;
  }

  if (
    typeof originalProps.title === "undefined" ||
    originalProps.title === null
  ) {
    fullProps.title = DEFAULTS.title;
  }

  for (let i = 0; i < fullProps.editorSettings.length; i++) {
    const fieldSettings = fullProps.editorSettings[i];

    if (
      !(fieldSettings.id in fullProps.form) ||
      (typeof fullProps.form[fieldSettings.id] === "undefined" &&
        fullProps.form[fieldSettings.id] === null)
    )
      fullProps.form[fieldSettings.id] = "";
  }

  return fullProps;
};

const FormModal = (props) => {
  let { title, formId, editorSettings, form } = setPropsDefaults(props);

  return (
    <Modal show={props.show} onHide={props.handleCancel}>
      <Modal.Header closeButton>
        <Modal.Title>{title}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {props.error && <CustomToast variant="danger" text={props.error} />}
        <Form
          id={formId}
          onSubmit={(event) => {
            event.preventDefault();
          }}
          onReset={(event) => {
            event.preventDefault();
          }}
        >
          {editorSettings.map((fieldSettings, index) => (
            <Input
              settings={fieldSettings}
              value={form[fieldSettings.id]}
              key={fieldSettings.id + "-" + index}
            />
          ))}
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button
          text="Aceptar"
          variant="success"
          onClickAction={props.handleAccept}
        />
        <Button
          text="Cancelar"
          variant="secondary"
          onClickAction={props.handleCancel}
        />
      </Modal.Footer>
    </Modal>
  );
};

export default FormModal;
