import React, { createContext, useContext, Fragment } from "react";
import { Dialog, Transition } from "@headlessui/react";

type ModalProps = {
    onHide?: () => void;
    show?: boolean;
    children?: React.ReactNode;
    size?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl' | '4xl';
};

const sizeClass: Record<NonNullable<ModalProps['size']>, string> = {
    sm: 'max-w-sm',
    md: 'max-w-md',
    lg: 'max-w-lg',
    xl: 'max-w-xl',
    '2xl': 'max-w-2xl',
    '3xl': 'max-w-3xl',
    '4xl': 'max-w-4xl',
};

const ModalContext = createContext<{ onHide?: () => void }>({});

export const Modal: React.FC<ModalProps> = ({
    children,
    show = false,
    onHide,
    size = 'md',
}) => {
    return (
        <ModalContext.Provider value={{ onHide }}>
            <Transition appear show={show} as={Fragment}>
                <Dialog
                    as="div"
                    className="relative z-50"
                    onClose={onHide ?? (() => {})}
                >
                    <Transition.Child
                        as={Fragment}
                        enter="ease-out duration-300"
                        enterFrom="opacity-0"
                        enterTo="opacity-100"
                        leave="ease-in duration-200"
                        leaveFrom="opacity-100"
                        leaveTo="opacity-0"
                    >
                        <div className="fixed inset-0 bg-black/40" />
                    </Transition.Child>

                    <div className="fixed inset-0 overflow-y-auto">
                        <div className="flex min-h-full items-center justify-center p-4 text-center">
                            <Transition.Child
                                as={Fragment}
                                enter="ease-out duration-300"
                                enterFrom="opacity-0 scale-95"
                                enterTo="opacity-100 scale-100"
                                leave="ease-in duration-200"
                                leaveFrom="opacity-100 scale-100"
                                leaveTo="opacity-0 scale-95"
                            >
                                <Dialog.Panel className={`w-full ${sizeClass[size]} transform overflow-hidden rounded-2xl bg-background text-foreground p-6 text-left align-middle shadow-xl transition-all border border-border`}>
                                    {children}
                                </Dialog.Panel>
                            </Transition.Child>
                        </div>
                    </div>
                </Dialog>
            </Transition>
        </ModalContext.Provider>
    );
};

type ModalHeaderProps = {
    children?: React.ReactNode;
    closeButton?: boolean;
};

export const ModalHeader: React.FC<ModalHeaderProps> = ({
    children,
    closeButton,
}) => {
    const { onHide } = useContext(ModalContext);
    return (
        <div className="flex items-start justify-between">
            <div className="flex-1">{children}</div>
            {closeButton ? (
                <button
                    aria-label="close"
                    onClick={onHide}
                    className="ml-4 text-muted-foreground hover:text-foreground"
                >
                    ×
                </button>
            ) : null}
        </div>
    );
};

type ModalTitleProps = {
    children?: React.ReactNode;
};

export const ModalTitle: React.FC<ModalTitleProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalBodyProps = {
    children?: React.ReactNode;
};

export const ModalBody: React.FC<ModalBodyProps> = ({ children }) => {
    return <>{children}</>;
};

type ModalFooterProps = {
    children?: React.ReactNode;
};

export const ModalFooter: React.FC<ModalFooterProps> = ({ children }) => {
    return <>{children}</>;
};
