type ToastProps = {
  show: boolean;
  onClose: () => void;
  delay: number;
  autohide: boolean;
  bg?: string;
  children: any;
};

export const Toast: React.FC<ToastProps> = ({
  children,
}) => {
  return <>{children}</>;
};

type ToastContainerProps = {
  position: string | undefined;
  className: string | undefined;
  children: any;
};

export const ToastContainer: React.FC<ToastContainerProps> = ({
  children,
}) => {
  return <>{children}</>;
};

type ToastHeaderProps = {
  children: any;
};

export const ToastHeader: React.FC<ToastHeaderProps> = ({ children }) => {
  return <>{children}</>;
};

type ToastBodyProps = {
  children: any;
};

export const ToastBody: React.FC<ToastBodyProps> = ({ children }) => {
  return <>{children}</>;
};
