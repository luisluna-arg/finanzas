import moment from "moment";
import "moment-timezone";

const Format = {
  Date: "DD/MM/yyyy",
  DateTime: "DD/MM/yyyy HH:mm",
};

const fromInputToRequest = (value) =>
  moment(value.replaceAll(" / ", "/"), "DD/MM/YYYY hh:mm A").format();

const toDisplay = (timeStamp) => moment(timeStamp).format(Format.DateTime);

const toUtcDisplay = (timeStamp) => moment.utc(timeStamp).format(Format.Date);

const toRequest = (timeStamp) =>
  (timeStamp ? moment(timeStamp) : moment()).format();

const tryGet = (dateToCheck) => {
  const formatsToCheck = [
    'YYYY-MM-DD', 
    'YYYY/MM/DD',
    'DD-MM-YYYY',
    'DD/MM/YYYY',
    'YYYY-MM-DDTHH:mm', 
    'YYYY-MM-DDTHH:mm:ss', 
    'YYYY-MM-DD HH:mm', 
    'YYYY-MM-DD HH:mm:ss', 
    'YYYY/MM/DD HH:mm',
    'YYYY/MM/DD HH:mm:ss',
    'DD-MM-YYYY HH:mm',
    'DD-MM-YYYY HH:mm:ss',
    'DD/MM/YYYY HH:mm',
    'DD/MM/YYYY HH:mm:ss',
  ];

  let result = null;
  let isValid = false;

  formatsToCheck.forEach((format) => {
    let localResult = moment(dateToCheck, format, true);
    console.log(`Date: ${dateToCheck}`);
    console.log(localResult);
    if (localResult.isValid()) {
      isValid = true;
      result = localResult;
      return false; // exit the loop if a valid format is found
    }
  });

  return result;
}

const dateFormat = {
  fromInputToRequest,
  toDisplay,
  toUtcDisplay,
  toRequest,
  tryGet
};

export default dateFormat;
