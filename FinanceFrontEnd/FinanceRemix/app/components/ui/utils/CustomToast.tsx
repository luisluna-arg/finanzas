/* eslint-disable react/prop-types */
import React, { useState } from "react";
import {
    Toast,
    ToastContainer,
    ToastHeader,
    ToastBody,
} from "@/components/ui/utils/Toast";

type CustomToastProps = {
    header?: string;
    // variant: COLOR_VARIANT;
    text: string;
};

const CustomToast: React.FC<CustomToastProps> = ({ header, text }) => {
    const [show, setShow] = useState(true);

    return (
        <ToastContainer position="top-end" className="p-3">
            <Toast
                show={show}
                onClose={() => setShow(false)}
                delay={3000}
                autohide
            >
                {header && (
                    <ToastHeader>
                        <strong className="me-auto">{header}</strong>
                    </ToastHeader>
                )}
                <ToastBody>{text}</ToastBody>
            </Toast>
        </ToastContainer>
    );
};

export default CustomToast;
