import create from 'zustand';
import apiCall from '../../api';

const useStore = create((set, get) => ({
    getAll: async (url) => {
        try {
            set({ isLoading: false, errorMessage: "", hasError: false });
            set({ results: await apiCall({ url: url }) });
        }
        catch (error) {
            console.error(error);
            set({ results: [], hasError: true, errorMessage: "Algo ha pasado, verifica tu conexión" });
        }
        finally {
            set({ isLoading: false });
        }
    },
    results: [],
    getSingle: async (url, id) => {
        if (!id) return;

        try {
            set({ isLoading: false, errorMessage: "", hasError: false });
            set({ result: await apiCall({ url: url + `${id}` }) });
        }
        catch (error) {
            console.error(error);
            set({ hasError: true, errorMessage: "Algo ha pasado, verifica tu conexión", single: {} });
        }
        finally {
            set({ isLoading: false });
        }
    },
    single: [],
    isLoading: false,
    errorMessage: "",
    hasError: false
}));

export default useStore;
