import React, { FC } from "react";
import {
  Modal,
  ModalBody,
  ModalFooter,
  ModalHeader,
  ModalTitle,
} from "@/components/ui/utils/Modal";
import { Form } from "@/components/ui/utils/Form";
import ActionButton, {
  ButtonType,
} from "@/components/ui/utils/ActionButton";
import CustomToast from "@/components/ui/utils/CustomToast";
import Input from "@/components/ui/utils/Input";
import { InputType } from "@/components/ui/utils/InputType";
import { VARIANT } from "@/components/ui/utils/Bootstrap/ColorVariant";

type EditorSettings = {
  id: string;
  type: InputType;
  label: string;
  placeholder?: string;
  visible?: false;
};

type FormModalProps = {
  title: string;
  formId: string;
  error?: string;
  editorSettings?: EditorSettings[];
  form: any;
  show: boolean;
  handleCancel?: any;
  handleAccept?: any;
};

const DEFAULTS: FormModalProps = {
  formId: `form-${new Date().getTime()}`,
  title: "DefaultTitle",
  editorSettings: [],
  form: {},
  show: true,
};

const isNullOrUndefined = (instance?: any) =>
  typeof instance === "undefined" || instance === null;

const setPropsDefaults = (originalProps: FormModalProps) => {
  let fullProps = Object.assign({}, DEFAULTS, originalProps);

  if (isNullOrUndefined(originalProps.editorSettings)) {
    fullProps.editorSettings = DEFAULTS.editorSettings;
  }

  if (isNullOrUndefined(originalProps.form)) {
    fullProps.form = DEFAULTS.form;
  }

  if (isNullOrUndefined(originalProps.title)) {
    fullProps.title = DEFAULTS.title;
  }

  for (let i = 0; i < fullProps.editorSettings!.length; i++) {
    const fieldSettings = fullProps.editorSettings![i];
    if (
      !(fieldSettings.id in fullProps.form) ||
      isNullOrUndefined(fullProps.form[fieldSettings.id])
    ) {
      fullProps.form[fieldSettings.id] = "";
    }
  }

  return fullProps;
};

const FormModal: React.FC<FormModalProps> = (props) => {
  let { title, formId, editorSettings, form } = setPropsDefaults(props);

  return (
    <Modal show={props.show} onHide={props.handleCancel}>
      <ModalHeader closeButton>
        <ModalTitle>{title}</ModalTitle>
      </ModalHeader>
      <ModalBody>
        {props.error && (
          <CustomToast variant={VARIANT.DANGER} text={props.error} />
        )}
        <Form
          id={formId}
          onSubmit={(event) => {
            event.preventDefault();
          }}
          onReset={(event) => {
            event.preventDefault();
          }}
        >
          {editorSettings &&
            editorSettings.map((fieldSettings, index) => (
              <Input
                settings={fieldSettings}
                value={form[fieldSettings.id]}
                key={fieldSettings.id + "-" + index}
              />
            ))}
        </Form>
      </ModalBody>
      <ModalFooter>
        <ActionButton
          text={"Aceptar"}
          variant={VARIANT.SUCCESS}
          type={ButtonType.None}
          action={props.handleAccept}
        />
        <ActionButton
          text="Cancelar"
          variant={VARIANT.SECONDARY}
          type={ButtonType.None}
          action={props.handleCancel}
        />
      </ModalFooter>
    </Modal>
  );
};

export default FormModal;
