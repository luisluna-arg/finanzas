# PowerShell script to check if Docker Desktop is running and start docker compose in Infra\Finances\Local
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$targetDir = Join-Path $PSScriptRoot "..\Infra\Finances\Local"
Write-Host "Changing directory to $targetDir"
Set-Location $targetDir

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "Error: .env file not found in $targetDir"
    Write-Host "Please ensure the .env file exists with all required environment variables."
    exit 1
}

Write-Host "Loading environment variables from .env file..."
Write-Host "Running: docker compose up --build -d"
docker compose up --build -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Services started successfully!"
    Write-Host "Frontend: http://localhost:5300"
    Write-Host "Backend API: http://localhost:5000"
    Write-Host "Redis: localhost:6379"
    Write-Host ""
    Write-Host "To stop services: docker compose down"
    Write-Host "To view logs: docker compose logs -f"
}
else {
    Write-Host "Error: Docker compose failed to start services."
    Write-Host "Check the logs with: docker compose logs"
    exit 1
}
