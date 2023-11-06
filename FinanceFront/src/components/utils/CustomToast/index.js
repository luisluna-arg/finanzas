import React from "react";
// import Toast from "react-bootstrap/Toast";
// import ToastContainer from 'react-bootstrap/ToastContainer';

import { Toast, ToastContainer } from "react-bootstrap";

function CustomToast({
  brand,
  timestamp,
  text,
  variant,
  position = "bottom-center",
}) {
  return (
    <div
      aria-live="polite"
      aria-atomic="true"
      className="position-relative"
      style={{ minHeight: "30px" }}
    >
      <ToastContainer className="p-3" position={position} style={{ zIndex: 1 }}>
        <Toast bg={"success"}>
          {brand ||
            (timestamp && (
              <Toast.Header>
                <img
                  src="holder.js/20x20?text=%20"
                  className="rounded me-2"
                  alt=""
                />
                <strong className="me-auto">{brand}</strong>
                <small>{timestamp}</small>
              </Toast.Header>
            ))}
          <Toast.Body className={"text-white"}>{text}</Toast.Body>
        </Toast>
      </ToastContainer>
    </div>
  );
}

export default CustomToast;
