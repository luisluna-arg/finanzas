#!/bin/bash
# Script to check if Docker is running and start docker compose in Infra/Finances/Local

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
  echo "Docker is not running. Please start Docker Desktop."
  exit 1
fi

# Navigate to the target directory
cd "$(dirname "$0")/../Infra/Finances/Local" || exit 1

# Check if .env file exists
if [ ! -f ".env" ]; then
  echo "Error: .env file not found in $(pwd)"
  echo "Please ensure the .env file exists with all required environment variables."
  exit 1
fi

echo "Loading environment variables from .env file..."
echo "Running: docker compose up --build -d"
docker compose up --build -d

if [ $? -eq 0 ]; then
  echo ""
  echo "Services started successfully!"
  echo "Frontend: http://localhost:5300"
  echo "Backend API: http://localhost:5000" 
  echo "Redis: localhost:6379"
  echo ""
  echo "To stop services: docker compose down"
  echo "To view logs: docker compose logs -f"
else
  echo "Error: Docker compose failed to start services."
  echo "Check the logs with: docker compose logs"
  exit 1
fi
