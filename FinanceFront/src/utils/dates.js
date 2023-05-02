import 'moment-timezone';
import moment from 'moment';

const Format = {
    Date: "DD/MM/yyyy",
    DateTime: "dd/MM/yyyy, HH:mm"
}


const fromInputToRequest = (value)  => moment(value.replaceAll(" / ", "/"), 'DD/MM/YYYY hh:mm A').format();

const toDisplay = (timeStamp) => moment(timeStamp).format(Format.DateTime);

const toUtcDisplay = (timeStamp) => moment.utc(timeStamp).format(Format.Date)

const toRequest = (timeStamp) => (timeStamp ? moment(timeStamp) : moment()).format();

const dateFormat = {
    fromInputToRequest,
    toDisplay,
    toUtcDisplay,
    toRequest
}

export default dateFormat;