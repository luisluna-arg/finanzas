import create from 'zustand';
import apiCall from '../../api';

const DEFAULTS = {
    all: [],
    single: {},
    isLoading: false,
}

const useStore = create((set, get) => ({
    getAll: async (url) => {
        try {
            set({ isLoading: DEFAULTS.isLoading, errorMessage: "", hasError: false });
            set({ all: await apiCall({ url: url }) });
        }
        catch (error) {
            console.error(error);
            set({ all: DEFAULTS.all, hasError: true, errorMessage: "Algo ha pasado, verifica tu conexión" });
        }
        finally {
            set({ isLoading: DEFAULTS.isLoading });
        }
    },
    all: [],
    clearAll: async () => {
        set({ all: DEFAULTS.all });
    },
    getSingle: async (url, id) => {
        if (!id) return;

        try {
            set({ isLoading: DEFAULTS.isLoading, errorMessage: "", hasError: false });
            set({ single: await apiCall({ url: url + `${id}` }) });
        }
        catch (error) {
            console.error(error);
            set({ single: DEFAULTS.single, hasError: true, errorMessage: "Algo ha pasado, verifica tu conexión" });
        }
        finally {
            set({ isLoading: DEFAULTS.isLoading });
        }
    },
    single: DEFAULTS.single,
    clearSingle: async () => {
        set({ single: DEFAULTS.single });
    },
    isLoading: DEFAULTS.isLoading,
    errorMessage: "",
    hasError: false
}));

export default useStore;
