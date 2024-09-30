import React from "react";
import { Form, Modal } from "react-bootstrap";
import ActionButton from "@/app/components/ui/utils/ActionButton";

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
        <Modal.Header closeButton>
          <Modal.Title>{title}</Modal.Title>
        </Modal.Header>
      )}
      <Modal.Body>
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
      </Modal.Body>
      <Modal.Footer>
        <ActionButton text="Aceptar" variant="success" action={handleAccept} />
        <ActionButton
          text="Cancelar"
          variant="secondary"
          action={handleCancel}
        />
      </Modal.Footer>
    </Modal>
  );
};

export default ConfirmationModal;
