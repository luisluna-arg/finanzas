# PowerShell script to check if Docker Desktop is running and start docker compose for Funds
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$fundsCompose = Join-Path $PSScriptRoot "..\Infra\Local\Funds\docker-compose.yaml"
$sharedCompose = Join-Path $PSScriptRoot "..\Infra\Local\Shared\docker-compose.yaml"

Write-Host "Checking compose files:"
Write-Host "  Funds: $fundsCompose"
Write-Host "  Shared: $sharedCompose"

if (-not (Test-Path $fundsCompose)) {
    Write-Host "Error: Funds compose file not found: $fundsCompose"
    exit 1
}
if (-not (Test-Path $sharedCompose)) {
    Write-Host "Error: Shared compose file not found: $sharedCompose"
    exit 1
}

# Get project names from .env files or use defaults
$fundsEnv = Join-Path (Split-Path -Parent $fundsCompose) '.env'
$sharedEnv = Join-Path (Split-Path -Parent $sharedCompose) '.env'

$projectFunds = if (Test-Path $fundsEnv) { 
    $envValue = Get-Content $fundsEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "funds" }
} else { "funds" }

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
}

Write-Host ""
Write-Host "Starting Funds project (project: $projectFunds)..."
docker compose -p $projectFunds -f $fundsCompose up --build -d
$exitFunds = $LASTEXITCODE
if ($exitFunds -ne 0) {
    Write-Host "Funds failed with exit code: $exitFunds"
}

Write-Host ""
if ($exitFunds -eq 0 -and $exitShared -eq 0) {
    Write-Host "All services started successfully!"
    Write-Host "Frontend: http://localhost:5200"
    Write-Host "Backend API: http://localhost:5000/swagger/index.html"
    Write-Host "Redis: localhost:6380"
    Write-Host ""
    Write-Host "To stop: docker compose -p $projectFunds stop; docker compose -p $projectShared stop"
}
else {
    Write-Host "Error: One or more services failed to start."
    Write-Host "Funds exit code: $exitFunds"
    Write-Host "Shared exit code: $exitShared"
    exit 1
}
