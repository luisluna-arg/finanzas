import { BackendClient } from "./BackendClient";

export const getBackendClient = async () => {
  return new BackendClient();
};
