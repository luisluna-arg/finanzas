type FormProps = {
    id?: string;
    onSubmit?: (event: any) => void;
    onReset?: (event: any) => void;
    children?: any;
};

export const Form: React.FC<FormProps> = ({ children }) => {
    return <>{children}</>;
};

type FormCheckProps = {
    id: string;
    checked: boolean | undefined;
    onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
    children?: any | undefined;
};

export const FormCheck: React.FC<FormCheckProps> = ({ children }) => {
    return <>{children}</>;
};
