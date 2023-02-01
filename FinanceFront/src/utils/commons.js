const UploadTypes = Object.freeze({
    Funds: Symbol("funds")
});

const BASE_UPLOAD_URL = "http://localhost:5015";

const UploadUrls = {};
UploadUrls[UploadTypes.Funds] = BASE_UPLOAD_URL + "/fund/upload";


export {
    UploadTypes,
    BASE_UPLOAD_URL,
    UploadUrls
}
