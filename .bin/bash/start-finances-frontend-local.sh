#!/bin/bash
# Script to check if Docker is running and start docker compose for Finances

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FINANCES_COMPOSE="$SCRIPT_DIR/../.infra/local/finances/docker-compose.yaml"

echo "Checking compose files:"
echo "  Finances: $FINANCES_COMPOSE"

if [ ! -f "$FINANCES_COMPOSE" ]; then
  echo "Error: Finances compose file not found: $FINANCES_COMPOSE"
  exit 1
fi

# Get project names from .env files or use defaults
FINANCES_ENV="$(dirname "$FINANCES_COMPOSE")/.env"

if [ -f "$FINANCES_ENV" ] && grep -q "^COMPOSE_PROJECT_NAME=" "$FINANCES_ENV"; then
  PROJECT_FINANCES=$(grep "^COMPOSE_PROJECT_NAME=" "$FINANCES_ENV" | cut -d'=' -f2 | tr -d '"')
else
  PROJECT_FINANCES="finances"
fi

echo ""
echo "Starting Finances project (project: $PROJECT_FINANCES)..."
docker compose -p "$PROJECT_FINANCES" -f "$FINANCES_COMPOSE" up --build -d
EXIT_FINANCES=$?
if [ $EXIT_FINANCES -ne 0 ]; then
  echo "Finances failed with exit code: $EXIT_FINANCES"
fi

echo ""
if [ $EXIT_FINANCES -eq 0 ] then
  echo "All services started successfully!"
  echo "Frontend: http://localhost:5100"
  echo ""
  echo "To stop: docker compose -p $PROJECT_FINANCES stop"
else
  echo "Error: One or more services failed to start."
  echo "Finances exit code: $EXIT_FINANCES"
  exit 1
fi
