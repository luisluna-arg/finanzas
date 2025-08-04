# See https://nodejs.org/en/docs/guides/nodejs-docker-webapp/
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
COPY . .

ARG VITE_AUTH0_DOMAIN
ARG VITE_AUTH0_CLIENT_ID
ARG VITE_AUTH0_REDIRECT_URI
ARG VITE_API_URL
ARG VITE_AUTH0_AUDIENCE

ENV VITE_AUTH0_DOMAIN=$VITE_AUTH0_DOMAIN \
    VITE_AUTH0_CLIENT_ID=$VITE_AUTH0_CLIENT_ID \
    VITE_AUTH0_REDIRECT_URI=$VITE_AUTH0_REDIRECT_URI \
    VITE_API_URL=$VITE_API_URL \
    VITE_AUTH0_AUDIENCE=$VITE_AUTH0_AUDIENCE

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
