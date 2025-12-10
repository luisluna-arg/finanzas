/* eslint-disable react/prop-types */

type FormProps = {
    id?: string;
    onSubmit?: (event: React.FormEvent<HTMLFormElement>) => void;
    onReset?: (event: React.FormEvent<HTMLFormElement>) => void;
    children?: React.ReactNode;
};

export const Form: React.FC<FormProps> = ({ children }) => {
    return <>{children}</>;
};

type FormCheckProps = {
    id: string;
    checked: boolean | undefined;
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    children?: React.ReactNode | undefined;
};

export const FormCheck: React.FC<FormCheckProps> = ({ children }) => {
    return <>{children}</>;
};
