# See https://nodejs.org/en/docs/guides/nodejs-docker-webapp/
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./

RUN echo "Starting install..." && \
    npm install && \
    echo "Install done."

COPY . .

ARG AUTH0_AUDIENCE
ARG AUTH0_CLIENT_ID
ARG AUTH0_DOMAIN
ARG AUTH0_SCOPES
ARG BASE_URL
ARG API_URL

# NOTE: Do NOT bake secrets into the image using ARG/ENV (for example AUTH0_CLIENT_SECRET).
# Secrets must be injected at runtime by your deployment platform (CI secrets, Docker secrets,
# Kubernetes Secrets, or environment variables set by the runtime). Keep secret values out of
# build-time ARGs to avoid leakage in image history.

# Set non-secret build-time values via ARG/ENV if needed (BASE_URL, audience, client id may be okay
# if they are not sensitive). Keep sensitive values out of the image.
ENV AUTH0_AUDIENCE=$AUTH0_AUDIENCE \
    AUTH0_CLIENT_ID=$AUTH0_CLIENT_ID \
    AUTH0_DOMAIN=$AUTH0_DOMAIN \
    AUTH0_SCOPES=$AUTH0_SCOPES \
    BASE_URL=$BASE_URL \
    API_URL=$API_URL

RUN echo "Starting build..." && \
    npm run build && \
    echo "Build done."

FROM node:20-alpine AS production
WORKDIR /app
COPY --from=build /app/build ./build
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package*.json ./

# Production runtime only needs NODE_ENV
ENV NODE_ENV=production

# Use PORT environment variable with fallback to 5100
ENV PORT=${PORT:-5100}
EXPOSE $PORT
CMD ["npx", "remix-serve", "./build/server/index.js"]
