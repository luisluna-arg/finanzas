import axios from 'axios';
import { ApiUrls, APIs } from './../utils/commons';

const methods = {
    get: "GET",
    post: "POST",
    del: "DELETE",
    put: "PUT"
}

const makeCall = (method = methods.get, url, successCallback, catchCallback, finallyCallback) => {
    axios({
        method: method,
        url: url
      })
      .then(successCallback)
      .catch(catchCallback)
      .finally(finallyCallback);
}

const totals = (successCallback, catchCallback, finallyCallback) => {
    makeCall(methods.get, ApiUrls[APIs.Movement].totals, successCallback, catchCallback, finallyCallback);
}

const create = (data, successCallback, catchCallback, finallyCallback) => {
    makeCall(methods.put, ApiUrls[APIs.Movement].create, successCallback, catchCallback, finallyCallback);
}

const movementsApi = {
    totals,
    create
};

export default movementsApi;