import { useState, useRef } from 'react';
import type { BackendUrl } from '@/utils/BackendUrl';
import { Toast, ToastContainer, ToastHeader, ToastBody } from '@/components/ui/utils/Toast';
import ActionButton, { ButtonType } from '@/components/ui/utils/ActionButton';
import SafeLogger from '../../../utils/SafeLogger';

interface UploaderProps {
  url: BackendUrl | string;
  extensions: string[];
  onSuccess?: () => void;
  onError?: () => void;
}

const Uploader = ({ url, extensions, onSuccess, onError }: UploaderProps) => {
  const [showToast, setShowToast] = useState<boolean>(false);
  const [toastMessage, setToastMessage] = useState<string>('');
  const [toastType, setToastType] = useState<'success' | 'error'>('success');
  const [uploading, setUploading] = useState<boolean>(false);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const uploadFile = async () => {
    const fileInput = fileInputRef.current;
    const file = fileInput?.files?.[0];

    if (!file) {
      setToastMessage('No file selected.');
      setToastType('error');
      setShowToast(true);
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    SafeLogger.log('URL', url);

    setUploading(true);
    try {
      const response = await fetch(String(url), {
        method: 'POST',
        body: formData,
      });

      if (response.ok) {
        if (fileInputRef.current) fileInputRef.current.value = '';
        setToastMessage('File uploaded successfully!');
        setToastType('success');
        setShowToast(true);
        if (onSuccess) onSuccess();
      } else {
        const errorText = await response.text().catch(() => 'File upload failed.');
        setToastMessage(errorText || 'File upload failed.');
        setToastType('error');
        setShowToast(true);
        if (onError) onError();
      }
    } catch (error) {
      SafeLogger.error('Upload error:', error);
      setToastMessage('File upload failed.');
      setToastType('error');
      setShowToast(true);
      if (onError) onError();
    } finally {
      setUploading(false);
    }
  };

  return (
    <>
      <div className="container">
        <form className="row">
          <div className="input-group">
            <input
              ref={fileInputRef}
              type="file"
              className="form-control"
              accept={extensions.join(',')}
              aria-describedby="inputGroupFileAddon04"
              aria-label="Subir"
            />
            <ActionButton
              text={uploading ? 'Subiendo...' : 'Subir'}
              type={ButtonType.None}
              action={uploadFile}
              disabled={uploading}
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
          bg={toastType === 'success' ? 'success' : 'danger'} // Change color based on type
        >
          <ToastHeader>
            <strong className="me-auto">{toastType === 'success' ? 'Success' : 'Error'}</strong>
            <small>Just now</small>
          </ToastHeader>
          <ToastBody>{toastMessage}</ToastBody>
        </Toast>
      </ToastContainer>
    </>
  );
};

export default Uploader;
