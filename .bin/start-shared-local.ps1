# PowerShell script to check if Docker Desktop is running and start docker compose for the Shared stack
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$sharedCompose = Join-Path $PSScriptRoot "..\Infra\Local\Shared\docker-compose.yaml"

Write-Host "Checking Shared compose file:"
Write-Host "  Shared: $sharedCompose"

if (-not (Test-Path $sharedCompose)) {
    Write-Host "Error: Shared compose file not found: $sharedCompose"
    exit 1
}

# Get project name from .env file or use default
$sharedEnv = Join-Path (Split-Path -Parent $sharedCompose) '.env'

$projectShared = if (Test-Path $sharedEnv) { 
    $envValue = Get-Content $sharedEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "shared" }
} else { "shared" }

Write-Host ""
Write-Host "Starting Shared project (project: $projectShared)..."
docker compose -p $projectShared -f $sharedCompose up --build -d
$exitShared = $LASTEXITCODE
if ($exitShared -ne 0) {
    Write-Host "Shared failed with exit code: $exitShared"
    exit 1
}

Write-Host ""
Write-Host "Shared services started successfully."
Write-Host "To stop: docker compose -p $projectShared stop"
