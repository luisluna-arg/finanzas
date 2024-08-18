import moment from "moment";
import "moment-timezone";

const Format = {
  Date: "DD/MM/yyyy",
  DateTime: "DD/MM/yyyy HH:mm",
};

const fromInputToRequest = (value: string): string =>
  moment(value.replaceAll(" / ", "/"), "DD/MM/YYYY hh:mm A").format();

const toDisplay = (timeStamp: string): string => moment(timeStamp).format(Format.DateTime);

const toUtcDisplay = (timeStamp: string): string => moment.utc(timeStamp).format(Format.Date);

const toRequest = (timeStamp?: string): string =>
  (timeStamp ? moment(timeStamp) : moment()).format();

const tryGet = (dateToCheck: string): moment.Moment | null => {
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

  let result: moment.Moment | null = null;

  formatsToCheck.forEach((format) => {
    let localResult = moment(dateToCheck, format, true);
    if (localResult.isValid()) {
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
