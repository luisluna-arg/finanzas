#!/bin/bash
# Script to rebuild and restart only the backend service in the Shared stack

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SHARED_COMPOSE="$SCRIPT_DIR/../.infra/local/shared/docker-compose.yaml"

if [ ! -f "$SHARED_COMPOSE" ]; then
  echo "Error: Shared compose file not found: $SHARED_COMPOSE"
  exit 1
fi

SHARED_ENV="$(dirname "$SHARED_COMPOSE")/.env"

if [ -f "$SHARED_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV"; then
  PROJECT_SHARED=$(grep "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_SHARED="shared"
fi

echo "Rebuilding backend service (project: $PROJECT_SHARED)..."
docker compose -p "$PROJECT_SHARED" -f "$SHARED_COMPOSE" up --build -d backend
EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
  echo "Backend rebuild failed with exit code: $EXIT_CODE"
  exit 1
fi

echo ""
echo "Backend service rebuilt and restarted successfully."
