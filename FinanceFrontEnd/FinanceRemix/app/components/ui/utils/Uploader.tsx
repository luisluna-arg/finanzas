import { useState, useRef } from "react";
import { useFetcher } from "@remix-run/react";
import {
  Toast,
  ToastContainer,
  ToastHeader,
  ToastBody,
} from "@/components/ui/utils/Toast";
import { OUTLINE_VARIANT } from "@/components/ui/utils/Bootstrap/ColorVariant";
import ActionButton, {
  ButtonType,
} from "@/components/ui/utils/ActionButton";

interface UploaderProps {
  url: string;
  extensions: string[];
  onSuccess?: () => void;
  onError?: () => void;
}

const Uploader = ({ url, extensions, onSuccess, onError }: UploaderProps) => {
  const [showToast, setShowToast] = useState<boolean>(false); // State for controlling toast visibility
  const [toastMessage, setToastMessage] = useState<string>(""); // Message for the toast
  const [toastType, setToastType] = useState<"success" | "error">("success"); // Toast type
  const fetcher = useFetcher<any>();
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const uploadFile = () => {
    const fileInput = fileInputRef.current;
    const file = fileInput?.files?.[0];

    if (file) {
      const formData = new FormData();
      formData.append("file", file);

      console.log("URL", url);

      fetcher.submit(formData, {
        method: "post",
        action: url, // URL for the upload action
        encType: "multipart/form-data",
      });
    } else {
      setToastMessage("No file selected.");
      setToastMessage("No file selected.");
      setToastType("error");
      setShowToast(true); // Show the toast
    }
  };

  if (fetcher.state === "submitting") {
    setToastMessage("");
  }

  if (fetcher.data && fetcher.data.success) {
    fileInputRef.current!.value = "";
    if (onSuccess) onSuccess();
    setToastMessage("File uploaded successfully!");
    setToastType("success");
    setShowToast(true); // Show success toast
  } else if (fetcher.data && fetcher.data.error) {
    setToastMessage(fetcher.data.error);
    if (onError) onError();
    setToastMessage(fetcher.data.error || "File upload failed.");
    setToastType("error");
    setShowToast(true); // Show error toast
  }

  return (
    <>
      <div className="container">
        <form className="row">
          <div className="input-group">
            <input
              ref={fileInputRef}
              type="file"
              className="form-control"
              accept={extensions.join(",")}
              aria-describedby="inputGroupFileAddon04"
              aria-label="Subir"
            />
            <ActionButton
              text={"Subir"}
              variant={OUTLINE_VARIANT.WARNING}
              type={ButtonType.None}
              //classes={["btn", "btn-outline-secondary"]}
              action={uploadFile}
              disabled={false}
            />
          </div>
        </form>
      </div>

      {/* Toast Component */}
      <ToastContainer position="top-end" className="p-3">
        <Toast
          onClose={() => setShowToast(false)}
          show={showToast}
          delay={3000}
          autohide
          bg={toastType === "success" ? "success" : "danger"} // Change color based on type
        >
          <ToastHeader>
            <strong className="me-auto">
              {toastType === "success" ? "Success" : "Error"}
            </strong>
            <small>Just now</small>
          </ToastHeader>
          <ToastBody>{toastMessage}</ToastBody>
        </Toast>
      </ToastContainer>
    </>
  );
};

export default Uploader;
