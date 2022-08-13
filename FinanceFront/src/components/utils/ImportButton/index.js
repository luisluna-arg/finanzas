import React from 'react';
import PropTypes from 'prop-types';

// import { UploadUrls } from '../../../utils/commons'

const sendFile = function (uploadType, file) {
  console.log(`uploadType ${uploadType}`);
  console.log(`file ${file}`);
  // let uri = UploadUrls[uploadType];
  // axios.
  //     put(uri, file, { 
  //         headers: { 'Content-Type': file.type } 
  //         }).
  //     then(response => { 
  //         console.log('response:', response);
  //         });
}

function ImportButton(props) {
  const UploadContent = () => {
    let fileDom = document.getElementById("excelfile");

    if (fileDom != null) {
      for (let i = 0; i < fileDom.files.length; i++) {
        sendFile(props.UploadType, fileDom.files[i]);
      }
    }
    else {
      alert("Debe seleccionar un archivo válido");
    }
  };

  return (
    <div>
      <div className="row">
        <div className="col-sm-3"></div>
        <div className="col-sm-5">
          {/* <label for="formFile" className="form-label">Default file input example</label> */}
          <input id={props.id + '-upload'} className="form-control" type="file" accept={props.acceptedFileTypes} />
        </div>
        <div className="col-sm-1">
          <input id={props.id + '-button'} className="btn btn-primary" type="button" value={props.buttonText} onClick={UploadContent} />
        </div>
        <div className="col-sm-3"></div>
      </div>

      <br />
      <br />
      <table id={props.id + '-exceltable'}>
      </table>
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
