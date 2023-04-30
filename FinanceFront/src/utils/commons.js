import moment from 'moment';

const UploadTypes = Object.freeze({
    Funds: Symbol("funds")
});

const APIs = Object.freeze({
    Movement: "movement"
});

const BASE_SERVICES_URL = "http://localhost:5015";

const UploadUrls = {};
UploadUrls[UploadTypes.Funds] = BASE_SERVICES_URL + "/fund/upload";

const ApiUrls = {};
ApiUrls[APIs.Movement] = {};
ApiUrls[APIs.Movement]['base'] = BASE_SERVICES_URL + "/movement";
ApiUrls[APIs.Movement]['all'] = ApiUrls[APIs.Movement].base + "/all";
ApiUrls[APIs.Movement]['single'] = ApiUrls[APIs.Movement].base + "/single/";
ApiUrls[APIs.Movement]['totals'] = ApiUrls[APIs.Movement].base + "/totals/";
ApiUrls[APIs.Movement]['create'] = ApiUrls[APIs.Movement].base + "/create/";

const dateHelpers = {
    format: (timeStamp) => moment(timeStamp).format("DD/MM/yyyy HH:mm"),
}

export {
    APIs,
    UploadTypes,
    BASE_SERVICES_URL,
    UploadUrls,
    ApiUrls,
    dateHelpers
}
