import { useState } from 'react';
import { Toast, ToastContainer } from 'react-bootstrap';
import { COLOR_VARIANT } from "@/app/components/ui/utils/Bootstrap/ColorVariant";

type CustomToastProps = {
  header?: string;
  variant: COLOR_VARIANT,
  text: string
};

const CustomToast: React.FC<CustomToastProps> = ({
  header,
  text,
  variant
}) => {
  const [show, setShow] = useState(true);

  return (
    <ToastContainer position="top-end" className="p-3">
      <Toast show={show} onClose={() => setShow(false)} delay={3000} autohide>
        {header && <Toast.Header>
          <strong className="me-auto">{header}</strong>
        </Toast.Header>}
        <Toast.Body>{text}</Toast.Body>
      </Toast>
    </ToastContainer>
  );
}

export default CustomToast;