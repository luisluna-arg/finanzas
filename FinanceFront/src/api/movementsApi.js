import axios from 'axios';
import { ApiUrls, APIs } from './../utils/commons';

const methods = {
    get: "GET",
    post: "POST",
    del: "DELETE",
    put: "PUT"
}

const makeCall = (method = methods.get, url, data, successCallback, catchCallback, finallyCallback) => {
    const requestSettings = {
        method: method,
        url: url
    };

    if (data) requestSettings["data"] = data;

    axios(requestSettings)
        .then(successCallback)
        .catch(catchCallback)
        .finally(finallyCallback);
}

const totals = (successCallback, catchCallback, finallyCallback) => {
    makeCall(methods.get, ApiUrls[APIs.Movement].totals, null, successCallback, catchCallback, finallyCallback);
}

const create = (data, successCallback, catchCallback, finallyCallback) => {
    makeCall(methods.post, ApiUrls[APIs.Movement].base, data, successCallback, catchCallback, finallyCallback);
}

const movementsApi = {
    totals,
    create
};

export default movementsApi;