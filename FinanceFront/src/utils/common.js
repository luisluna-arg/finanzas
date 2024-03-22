const CommonUtils = {
    Params: (obj) => Object.entries(obj)
        .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
        .join('&')
};

export default CommonUtils;
