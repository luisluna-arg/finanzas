#!/bin/bash
# Script to check if Docker is running and start docker compose for the Shared stack

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SHARED_COMPOSE="$SCRIPT_DIR/../Infra/Local/Shared/docker-compose.yaml"

echo "Checking Shared compose file:"
echo "  Shared: $SHARED_COMPOSE"

if [ ! -f "$SHARED_COMPOSE" ]; then
  echo "Error: Shared compose file not found: $SHARED_COMPOSE"
  exit 1
fi

# Get project name from .env file or use default
SHARED_ENV="$(dirname "$SHARED_COMPOSE")/.env"

if [ -f "$SHARED_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV"; then
  PROJECT_SHARED=$(grep "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_SHARED="shared"
fi

echo ""
echo "Starting Shared project (project: $PROJECT_SHARED)..."
docker compose -p "$PROJECT_SHARED" -f "$SHARED_COMPOSE" up --build -d
EXIT_SHARED=$?
if [ $EXIT_SHARED -ne 0 ]; then
  echo "Shared failed with exit code: $EXIT_SHARED"
  exit 1
fi

echo ""
echo "Shared services started successfully."
echo "To stop: docker compose -p $PROJECT_SHARED stop"
