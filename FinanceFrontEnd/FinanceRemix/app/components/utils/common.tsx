const CommonUtils = {
    Params: (obj: Record<string, any>): string => Object.entries(obj)
        .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
        .join('&')
};

export default CommonUtils;
