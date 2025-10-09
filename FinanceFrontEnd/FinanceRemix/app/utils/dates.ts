import dayjs from "dayjs";
import utc from "dayjs/plugin/utc";
import timezone from "dayjs/plugin/timezone";
import customParseFormat from "dayjs/plugin/customParseFormat";

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(customParseFormat);

const Format = {
    Date: "DD/MM/YYYY",
    DateTime: "DD/MM/YYYY HH:mm",
};

const fromInputToRequest = (value: string): string =>
    dayjs(value.replaceAll(" / ", "/"), "DD/MM/YYYY hh:mm A").format();

const toDisplay = (timeStamp: string): string =>
    dayjs(timeStamp).format(Format.DateTime);

const toUtcDisplay = (timeStamp: string): string =>
    dayjs.utc(timeStamp).format(Format.Date);

const toRequest = (timeStamp?: string): string =>
    (timeStamp ? dayjs(timeStamp) : dayjs()).format();

const tryGet = (dateToCheck: string): dayjs.Dayjs | null => {
    const formatsToCheck = [
        "YYYY-MM-DD",
        "YYYY/MM/DD",
        "DD-MM-YYYY",
        "DD/MM/YYYY",
        "YYYY-MM-DDTHH:mm",
        "YYYY-MM-DDTHH:mm:ss",
        "YYYY-MM-DD HH:mm",
        "YYYY-MM-DD HH:mm:ss",
        "YYYY/MM/DD HH:mm",
        "YYYY/MM/DD HH:mm:ss",
        "DD-MM-YYYY HH:mm",
        "DD-MM-YYYY HH:mm:ss",
        "DD/MM/YYYY HH:mm",
        "DD/MM/YYYY HH:mm:ss",
    ];

    for (const format of formatsToCheck) {
        const parsed = dayjs(dateToCheck, format, true);
        if (parsed.isValid()) {
            return parsed;
        }
    }

    return null;
};

const dateFormat = {
    fromInputToRequest,
    toDisplay,
    toUtcDisplay,
    toRequest,
    tryGet,
};

export default dateFormat;
