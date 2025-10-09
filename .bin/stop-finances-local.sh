#!/bin/bash
# Script to stop and clean up finance services

# Navigate to the target directory
cd "$(dirname "$0")/../Infra/Finances/Local" || exit 1

echo "Stopping all finance services..."
docker compose down

echo "Removing unused containers and networks..."
docker system prune -f

echo "Finance services stopped and cleaned up."
