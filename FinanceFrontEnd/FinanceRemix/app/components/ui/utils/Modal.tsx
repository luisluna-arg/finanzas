import React, { createContext, useContext, Fragment } from "react";
import { Dialog, Transition } from "@headlessui/react";

type ModalProps = {
    onHide?: () => void;
    show?: boolean;
    children?: React.ReactNode;
};

const ModalContext = createContext<{ onHide?: () => void }>({});

export const Modal: React.FC<ModalProps> = ({
    children,
    show = false,
    onHide,
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
                                <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
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
                    className="ml-4 text-gray-500 hover:text-gray-700"
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
