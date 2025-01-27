import React from "react";
import { Form } from "@/components/ui/utils/Form";
import {
  Modal,
  ModalHeader,
  ModalBody,
  ModalTitle,
  ModalFooter,
} from "@/components/ui/utils/Modal";
import ActionButton from "@/components/ui/utils/ActionButton";

interface ConfirmationModalProps {
  text: React.ReactNode;
  title?: string;
  show: boolean;
  handleAccept: () => void;
  handleCancel: () => void;
}

const ConfirmationModal: React.FC<ConfirmationModalProps> = ({
  text,
  title,
  show,
  handleAccept,
  handleCancel,
}) => {
  return (
    <Modal show={show} onHide={handleCancel}>
      {title && (
        <ModalHeader closeButton>
          <ModalTitle>{title}</ModalTitle>
        </ModalHeader>
      )}
      <ModalBody>
        <Form
          onSubmit={(event) => {
            event.preventDefault();
          }}
          onReset={(event) => {
            event.preventDefault();
          }}
        >
          {text}
        </Form>
      </ModalBody>
      <ModalFooter>
        <ActionButton text="Aceptar" variant="success" action={handleAccept} />
        <ActionButton
          text="Cancelar"
          variant="secondary"
          action={handleCancel}
        />
      </ModalFooter>
    </Modal>
  );
};

export default ConfirmationModal;
