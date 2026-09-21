# See https://nodejs.org/en/docs/guides/nodejs-docker-webapp/
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
COPY . .

# All app config (AUTH0_*, BASE_URL, API_URL, REDIS_URL, secrets, ...) is read via
# process.env at request time by the SSR server, never at build time — none of it
# needs to be a build ARG. NODE_ENV is intentionally not set to "production" here
# either: that would make npm skip devDependencies (typescript, vite), which the
# build step needs.

RUN echo "Starting install..." && \
    npm install --ignore-scripts && \
    echo "Install done."

RUN echo "Starting build..." && \
    npm run build && \
    echo "Build done."

FROM node:20-alpine AS production
WORKDIR /app
COPY --from=build /app/build ./build
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package*.json ./
COPY --from=build /app/server.js ./server.js

# Production runtime only needs NODE_ENV
ENV NODE_ENV=production

# Use PORT environment variable with fallback to 5200
ENV PORT=${PORT:-5200}
EXPOSE $PORT
CMD ["node", "server.js"]
