# See https://nodejs.org/en/docs/guides/nodejs-docker-webapp/
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
COPY . .

ARG AUTH0_AUDIENCE
ARG AUTH0_CLIENT_ID
ARG AUTH0_CLIENT_SECRET
ARG AUTH0_DOMAIN
ARG AUTH0_SCOPES
ARG BASE_URL

ENV AUTH0_AUDIENCE=$AUTH0_AUDIENCE \
AUTH0_CLIENT_ID=$AUTH0_CLIENT_ID \
AUTH0_CLIENT_SECRET=$AUTH0_CLIENT_SECRET \
AUTH0_DOMAIN=$AUTH0_DOMAIN \
AUTH0_SCOPES=$AUTH0_SCOPES \
BASE_URL=$BASE_URL

RUN echo "Starting install..." && \
    npm install && \
    echo "Install done."

RUN echo "Starting build..." && \
    npm run build && \
    echo "Build done."

FROM node:20-alpine AS production
WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package*.json ./

# Production runtime only needs NODE_ENV
ENV NODE_ENV=production

EXPOSE 3000
CMD ["npm", "run", "preview", "--", "--port=3000", "--host"]
