const UploadTypes = Object.freeze({
    Funds:   Symbol("funds")
});

const BASE_UPLOAD_URL = "localhost:8080";

const UploadUrls = {};
UploadUrls[UploadTypes.Funds] = BASE_UPLOAD_URL + "/funds";


export {
    UploadTypes,
    BASE_UPLOAD_URL,
    UploadUrls
}