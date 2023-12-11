import React, { useState } from "react";
import CustomToast from "../../utils/CustomToast";

const Uploader = ({ url, extensions }) => {
    const [errorMessage, setErrorMessage] = useState(null);

    const uploadFile = () => {
        const fileInput = document.getElementById('fileInput');
        const file = fileInput.files[0];

        if (file) {
            const formData = new FormData();
            formData.append('file', file);

            fetch(url, {
                method: 'POST',
                body: formData
            })
                .then(data => {
                    setErrorMessage(null);
                })
                .catch(error => {
                    const message = `Error uploading file: ${error}`;
                    setErrorMessage(message);
                    console.log(message);
                });
        } else {
            setErrorMessage('No file selected.');
        }
    }

    return (
        <>
            {errorMessage && <CustomToast text={"Cargando..."}></CustomToast>}
            <div className="container">
                <form className="row">
                    <div className="input-group">
                        <input id="fileInput" type="file" className="form-control" aria-describedby="inputGroupFileAddon04" aria-label="Subir" accept={extensions} />
                        <button id="fileUploadButton" className="btn btn-outline-primary" type="button" onClick={uploadFile}>Subir</button>
                    </div>
                </form>
            </div>
        </>
    );
};

export default Uploader;
