# PowerShell script to check if Docker Desktop is running and start docker compose in Infra\Local\Finances
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$financesCompose = Join-Path $PSScriptRoot "..\Infra\Local\Finances\docker-compose.yaml"
$sharedCompose = Join-Path $PSScriptRoot "..\Infra\Local\Shared\docker-compose.yaml"

Write-Host "Checking compose files:"
Write-Host "  Finances: $financesCompose"
Write-Host "  Shared:   $sharedCompose"

if (-not (Test-Path $financesCompose)) {
    Write-Host "Error: Finances compose file not found: $financesCompose"
    exit 1
}
if (-not (Test-Path $sharedCompose)) {
    Write-Host "Error: Shared compose file not found: $sharedCompose"
    exit 1
}

# Get project names from .env files or use defaults
$financesEnv = Join-Path (Split-Path -Parent $financesCompose) '.env'
$sharedEnv = Join-Path (Split-Path -Parent $sharedCompose) '.env'

$projectFinances = if (Test-Path $financesEnv) { 
    $envValue = Get-Content $financesEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "finances" }
} else { "finances" }

$projectShared = if (Test-Path $sharedEnv) { 
    $envValue = Get-Content $sharedEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "shared" }
} else { "shared" }

Write-Host ""
Write-Host "Starting Shared project first (project: $projectShared)..."
docker compose -p $projectShared -f $sharedCompose up --build -d
$exitShared = $LASTEXITCODE
if ($exitShared -ne 0) {
    Write-Host "Shared failed with exit code: $exitShared"
    Write-Host ""
    Write-Host "=== SHARED PROJECT LOGS ==="
    docker compose -p $projectShared -f $sharedCompose logs --tail=50
}

Write-Host ""
Write-Host "Waiting for backend to be healthy (max 60s)..."
$timeout = 60
$elapsed = 0
while ($elapsed -lt $timeout) {
    $health = docker inspect --format='{{.State.Health.Status}}' "$projectShared-backend-1" 2>$null
    if ($health -eq "healthy") {
        Write-Host "Backend is healthy!"
        break
    }
    if ($health -eq "unhealthy") {
        Write-Host "Backend is unhealthy. Showing logs..."
        docker compose -p $projectShared -f $sharedCompose logs backend --tail=100
        Write-Host ""
        Write-Host "To see live logs: docker compose -p $projectShared -f $sharedCompose logs -f backend"
        break
    }
    Write-Host "Backend status: $health - waiting... ($elapsed/$timeout seconds)"
    Start-Sleep -Seconds 5
    $elapsed += 5
}

if ($elapsed -ge $timeout) {
    Write-Host "WARNING: Backend health check timed out. Showing logs..."
    docker compose -p $projectShared -f $sharedCompose logs backend --tail=100
}

Write-Host ""
Write-Host "Starting Finances project (project: $projectFinances)..."
docker compose -p $projectFinances -f $financesCompose up --build -d
$exitFinances = $LASTEXITCODE
if ($exitFinances -ne 0) {
    Write-Host "Finances failed with exit code: $exitFinances"
    Write-Host ""
    Write-Host "=== FINANCES PROJECT LOGS ==="
    docker compose -p $projectFinances -f $financesCompose logs --tail=50
}

Write-Host ""
if ($exitFinances -eq 0 -and $exitShared -eq 0) {
    Write-Host "All services started successfully!"
    Write-Host ""
    Write-Host "=== SERVICE STATUS ==="
    docker compose -p $projectShared -f $sharedCompose ps
    docker compose -p $projectFinances -f $financesCompose ps
    Write-Host ""
    Write-Host "=== ACCESS URLS ==="
    Write-Host "Frontend: http://localhost:5100"
    Write-Host "Backend API: http://localhost:5000/swagger/index.html"
    Write-Host "Redis: localhost:6379"
    Write-Host "Postgres: localhost:5432"
    Write-Host ""
    Write-Host "=== USEFUL COMMANDS ==="
    Write-Host "View logs (backend): docker compose -p $projectShared -f $sharedCompose logs -f backend"
    Write-Host "View logs (frontend): docker compose -p $projectFinances -f $financesCompose logs -f frontend"
    Write-Host "View all logs: docker compose -p $projectShared -f $sharedCompose logs -f; docker compose -p $projectFinances -f $financesCompose logs -f"
    Write-Host "Stop services: docker compose -p $projectFinances -f $financesCompose stop; docker compose -p $projectShared -f $sharedCompose stop"
    Write-Host "Remove services: docker compose -p $projectFinances -f $financesCompose down; docker compose -p $projectShared -f $sharedCompose down"
}
else {
    Write-Host "Error: One or more services failed to start."
    Write-Host "Finances exit code: $exitFinances"
    Write-Host "Shared exit code: $exitShared"
    Write-Host ""
    Write-Host "Run these commands to troubleshoot:"
    Write-Host "  docker compose -p $projectShared -f $sharedCompose logs"
    Write-Host "  docker compose -p $projectFinances -f $financesCompose logs"
    exit 1
}
