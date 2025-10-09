import { BackendClient } from "./BackendClient";

export const getBackendClient = async (accessToken: string) => {
  return new BackendClient(accessToken);
};
