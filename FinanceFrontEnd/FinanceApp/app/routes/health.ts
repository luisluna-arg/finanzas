/**
 * Simple health check endpoint for Docker and load balancers.
 * Does NOT touch Redis or Auth0 — must stay dependency-free so it responds
 * immediately even while the app is still warming up.
 */
export const loader = () =>
  new Response('OK', {
    status: 200,
    headers: { 'Content-Type': 'text/plain' },
  });
