import { LoaderFunctionArgs } from "@remix-run/node";
import { BackendClient } from "./BackendClient";

export const getBackendClient = async () => {
  const client = new BackendClient();
  return client;
};
