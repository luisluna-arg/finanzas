#!/bin/bash
# Script to stop funds services

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

echo "Stopping Funds project (project: $PROJECT_FUNDS)..."
docker compose -p "$PROJECT_FUNDS" -f "$FUNDS_COMPOSE" stop

echo ""
read -p "Stop shared services (backend, postgres, redis)? [y/N] " -r
if [[ $REPLY =~ ^[Yy]$ ]]; then
  echo "Stopping Shared project (project: $PROJECT_SHARED)..."
  docker compose -p "$PROJECT_SHARED" -f "$SHARED_COMPOSE" stop
  echo "All services stopped."
else
  echo "Funds frontend stopped. Shared services (backend, postgres, redis) are still running."
fi