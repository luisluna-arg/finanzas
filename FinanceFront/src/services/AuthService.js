import jwt_decode from "jwt-decode";

const StorageKeys = {
    USER: 'userDetails',
    SESSION_EXPIRE_DATE: 'logInExpirationDate'
}

function logoutTasks() {
    localStorage.removeItem(StorageKeys.USER);
    localStorage.removeItem(StorageKeys.SESSION_EXPIRE_DATE);
}

const SessionExpireDate = {
    create: () => new Date().getTime() + 5 * 60 * 1000,
    get: () => localStorage.getItem(StorageKeys.SESSION_EXPIRE_DATE),
    isExpired: () => {
        const logInExpirationDate = SessionExpireDate.get();
        return logInExpirationDate && new Date() > logInExpirationDate;
    }
}

const UserData = {
    get: () => localStorage.getItem(StorageKeys.USER),
    set: (codedData) => {
        localStorage.setItem(StorageKeys.USER, codedData);
        localStorage.setItem(StorageKeys.SESSION_EXPIRE_DATE, SessionExpireDate.create());
    },
    decode: () => jwt_decode(localStorage.getItem(StorageKeys.USER)),
    renewExpirationDate: () => localStorage.setItem(StorageKeys.SESSION_EXPIRE_DATE, SessionExpireDate.create())
}

export function logoutAction() {
    logoutTasks();
    window.location.reload();
}

export function checkLoggedIn() {
    return !!UserData.get();
}

export function loginSuccess(response) {
    if (response && response.credential) {
        UserData.set(response.credential);
        window.location.reload();
    }
}

export function checkAutoLogin() {
    if (!checkLoggedIn()) {
        logoutTasks();
        return;
    }

    if (SessionExpireDate.isExpired()) {
        logoutTasks();
        return;
    }
    
    UserData.renewExpirationDate();

    return true;
}
