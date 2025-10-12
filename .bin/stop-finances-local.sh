#!/bin/bash
# Script to stop finance services

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
FINANCES_COMPOSE="$SCRIPT_DIR/../Infra/Local/Finances/docker-compose.yaml"
SHARED_COMPOSE="$SCRIPT_DIR/../Infra/Local/Shared/docker-compose.yaml"

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

echo "Stopping Finances project (project: $PROJECT_FINANCES)..."
docker compose -p "$PROJECT_FINANCES" -f "$FINANCES_COMPOSE" stop

echo ""
read -p "Stop shared services (backend, postgres, redis)? [y/N] " -r
if [[ $REPLY =~ ^[Yy]$ ]]; then
  echo "Stopping Shared project (project: $PROJECT_SHARED)..."
  docker compose -p "$PROJECT_SHARED" -f "$SHARED_COMPOSE" stop
  echo "All services stopped."
else
  echo "Finances frontend stopped. Shared services (backend, postgres, redis) are still running."
fi
