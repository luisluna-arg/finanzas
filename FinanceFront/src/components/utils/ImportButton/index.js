import React from 'react';
import PropTypes from 'prop-types';
import { UploadUrls } from '../../../utils/commons'
import axios from 'axios';

const sendFile = function (uploadType, form, callback = null) {
    let uri = UploadUrls[uploadType] + "?dateKind=Local";
    let formData = new FormData(form);
    let config = { headers: { 'Content-Type': 'multipart/form-data', } };

    axios
        .post(uri, formData, config)
        .then(response => {
            if (callback != null) callback();
        })
        .catch((...errors) => {
            console.error("Couldn't upload file");
            console.error(errors);
        });
}

function ImportButton(props) {
    const UploadContent = () => {
        const form = document.querySelector("form");

        if (form != null) {
            sendFile(props.UploadType, form);
        }
        else {
            alert("Ha ocurrido un error inesperado");
        }
    };

    return (
        <div className="row">
            <div className="col-sm-3"></div>
            <div className="col-sm-5">
                <form action="/update-profile" method="post">
                    <input id="files" name={props.id + '-upload'} className="form-control" type="file" accept={props.acceptedFileTypes} />
                </form>
            </div>
            <div className="col-sm-1">
                <input id={props.id + '-button'} className="btn btn-primary" type="button" value={props.buttonText} onClick={UploadContent} />
            </div>
            <div className="col-sm-3"></div>
        </div>
    );
};

ImportButton.propTypes = {
    UploadType: PropTypes.symbol,
    id: PropTypes.string,
    buttonText: PropTypes.string
};

ImportButton.defaultProps = {
    UploadType: Symbol,
    id: "excel-file",
    buttonText: "Importar",
    acceptedFileTypes: ".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
};

export default ImportButton;
