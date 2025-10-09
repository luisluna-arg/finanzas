type ModalProps = {
    onHide: () => void;
    children?: any | undefined;
};

export const Modal: React.FC<ModalProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalHeaderProps = {
    children?: any | undefined;
};

export const ModalHeader: React.FC<ModalHeaderProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalTitleProps = {
    children?: any | undefined;
};

export const ModalTitle: React.FC<ModalTitleProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalBodyProps = {
    children?: any | undefined;
};

export const ModalBody: React.FC<ModalBodyProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalFooterProps = {
    children?: any | undefined;
};

export const ModalFooter: React.FC<ModalFooterProps> = ({ children }) => {
    return <>{children}</>;
};
