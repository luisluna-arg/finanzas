import { Agent } from 'https';

export function buildAxiosConfig(httpsAgent: Agent, accessToken: string, params?: object) {
  return {
    params: params ?? {},
    httpsAgent,
    timeout: 10_000,
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  };
}
