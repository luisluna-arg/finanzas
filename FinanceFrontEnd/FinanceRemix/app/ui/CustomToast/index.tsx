import React, { CSSProperties, FunctionComponent } from "react";
import { Toast, ToastContainer } from "react-bootstrap";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";

const floatStyles: CSSProperties = {
  position: "absolute",
  top: "10px",
  right: "10px",
  zIndex: 1,
};

interface CustomToastProps {
  brand: string,
  timestamp: string,
  text: string,
  variant: string,
  closeButton: boolean,
  float: boolean,
  position: ToastPosition,
}

const CustomToast : FunctionComponent<CustomToastProps> = ({
  brand,
  timestamp,
  text,
  variant,
  closeButton,
  float,
  position = "bottom-center",
}) => {
  return (
    <div
      aria-live="polite"
      aria-atomic="true"
      className="position-relative"
      style={{ minHeight: "30px" }}
    >
      <ToastContainer
        className="p-3"
        position={position}
        style={float ? floatStyles : { zIndex: 1 }}
      >
        <Toast bg={variant ?? "success"}>
          {brand ||
            (timestamp && (
              <Toast.Header closeButton={closeButton}>
                <img
                  src="holder.js/20x20?text=%20"
                  className="rounded me-2"
                  alt=""
                />
                <strong className="me-auto">{brand}</strong>
                <small>{timestamp}</small>
              </Toast.Header>
            ))}
          <div className={"d-flex flex-row"}>
            <Toast.Body className={"text-white"}>{text}</Toast.Body>
            {!timestamp && closeButton && (
              <button
                type="button"
                className="btn-close btn-close-white me-2 m-auto"
                data-bs-dismiss="toast"
                aria-label="Close"
              ></button>
            )}
          </div>
        </Toast>
      </ToastContainer>
    </div>
  );
}

export default CustomToast;
