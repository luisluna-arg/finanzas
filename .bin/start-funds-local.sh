#!/bin/bash
# Script to check if Docker is running and start docker compose in Infra/Funds/Local

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

# Navigate to the target directory
cd "$(dirname "$0")/../Infra/Funds/Local" || exit 1

echo "Running: docker compose up --build -d"
docker compose up --build -d
