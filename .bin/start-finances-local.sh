#!/bin/bash
# Script to check if Docker is running and start docker compose for Finances + Shared

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FINANCES_COMPOSE="$SCRIPT_DIR/../Infra/Local/Finances/docker-compose.yaml"
SHARED_COMPOSE="$SCRIPT_DIR/../Infra/Local/Shared/docker-compose.yaml"

echo "Checking compose files:"
echo "  Finances: $FINANCES_COMPOSE"
echo "  Shared:   $SHARED_COMPOSE"

if [ ! -f "$FINANCES_COMPOSE" ]; then
  echo "Error: Finances compose file not found: $FINANCES_COMPOSE"
  exit 1
fi
if [ ! -f "$SHARED_COMPOSE" ]; then
  echo "Error: Shared compose file not found: $SHARED_COMPOSE"
  exit 1
fi

# Get project names from .env files or use defaults
FINANCES_ENV="$(dirname "$FINANCES_COMPOSE")/.env"
SHARED_ENV="$(dirname "$SHARED_COMPOSE")/.env"

if [ -f "$FINANCES_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$FINANCES_ENV"; then
  PROJECT_FINANCES=$(grep "^COMPOSE_PROJECT_NAME=" "$FINANCES_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_FINANCES="finances"
fi

if [ -f "$SHARED_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV"; then
  PROJECT_SHARED=$(grep "^COMPOSE_PROJECT_NAME=" "$SHARED_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_SHARED="shared"
fi

echo ""
echo "Starting Finances project (project: $PROJECT_FINANCES)..."
docker compose -p "$PROJECT_FINANCES" -f "$FINANCES_COMPOSE" up --build -d
EXIT_FINANCES=$?
if [ $EXIT_FINANCES -ne 0 ]; then
  echo "Finances failed with exit code: $EXIT_FINANCES"
fi

echo ""
echo "Starting Shared project (project: $PROJECT_SHARED)..."
docker compose -p "$PROJECT_SHARED" -f "$SHARED_COMPOSE" up --build -d
EXIT_SHARED=$?
if [ $EXIT_SHARED -ne 0 ]; then
  echo "Shared failed with exit code: $EXIT_SHARED"
fi

echo ""
if [ $EXIT_FINANCES -eq 0 ] && [ $EXIT_SHARED -eq 0 ]; then
  echo "All services started successfully!"
  echo "Frontend: http://localhost:5100"
  echo "Backend API: http://localhost:5000/swagger/index.html"
  echo "Redis: localhost:6379"
  echo ""
  echo "To stop: docker compose -p $PROJECT_FINANCES stop; docker compose -p $PROJECT_SHARED stop"
else
  echo "Error: One or more services failed to start."
  echo "Finances exit code: $EXIT_FINANCES"
  echo "Shared exit code: $EXIT_SHARED"
  exit 1
fi
