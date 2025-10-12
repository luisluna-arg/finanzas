#!/bin/bash
# Script to check if Docker is running and start docker compose for Funds + Shared

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FUNDS_COMPOSE="$SCRIPT_DIR/../Infra/Local/Funds/docker-compose.yaml"
SHARED_COMPOSE="$SCRIPT_DIR/../Infra/Local/Shared/docker-compose.yaml"

echo "Checking compose files:"
echo "  Funds: $FUNDS_COMPOSE"
echo "  Shared: $SHARED_COMPOSE"

if [ ! -f "$FUNDS_COMPOSE" ]; then
  echo "Error: Funds compose file not found: $FUNDS_COMPOSE"
  exit 1
fi
if [ ! -f "$SHARED_COMPOSE" ]; then
  echo "Error: Shared compose file not found: $SHARED_COMPOSE"
  exit 1
fi

# Get project names from .env files or use defaults
FUNDS_ENV="$(dirname "$FUNDS_COMPOSE")/.env"
SHARED_ENV="$(dirname "$SHARED_COMPOSE")/.env"

if [ -f "$FUNDS_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$FUNDS_ENV"; then
  PROJECT_FUNDS=$(grep "^COMPOSE_PROJECT_NAME=" "$FUNDS_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_FUNDS="funds"
fi

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
fi

echo ""
echo "Starting Funds project (project: $PROJECT_FUNDS)..."
docker compose -p "$PROJECT_FUNDS" -f "$FUNDS_COMPOSE" up --build -d
EXIT_FUNDS=$?
if [ $EXIT_FUNDS -ne 0 ]; then
  echo "Funds failed with exit code: $EXIT_FUNDS"
fi

echo ""
if [ $EXIT_FUNDS -eq 0 ] && [ $EXIT_SHARED -eq 0 ]; then
  echo "All services started successfully!"
  echo "Frontend: http://localhost:5200"
  echo "Backend API: http://localhost:5000/swagger/index.html"
  echo "Redis: localhost:6380"
  echo ""
  echo "To stop: docker compose -p $PROJECT_FUNDS stop; docker compose -p $PROJECT_SHARED stop"
else
  echo "Error: One or more services failed to start."
  echo "Funds exit code: $EXIT_FUNDS"
  echo "Shared exit code: $EXIT_SHARED"
  exit 1
fi
