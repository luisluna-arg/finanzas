/* eslint-disable react/prop-types */

type ToastProps = {
    show?: boolean;
    onClose?: () => void;
    delay?: number;
    autohide?: boolean;
    bg?: string;
    children?: React.ReactNode;
};

export const Toast: React.FC<ToastProps> = ({ children }) => {
    return <>{children}</>;
};

type ToastContainerProps = {
    position?: string;
    className?: string;
    children?: React.ReactNode;
};

export const ToastContainer: React.FC<ToastContainerProps> = ({ children }) => {
    return <>{children}</>;
};

type ToastHeaderProps = {
    children?: React.ReactNode;
};

export const ToastHeader: React.FC<ToastHeaderProps> = ({ children }) => {
    return <>{children}</>;
};

type ToastBodyProps = {
    children?: React.ReactNode;
};

export const ToastBody: React.FC<ToastBodyProps> = ({ children }) => {
    return <>{children}</>;
};
