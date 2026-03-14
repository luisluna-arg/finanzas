# PowerShell script to rebuild and restart only the backend service in the Shared stack
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$sharedCompose = Join-Path $PSScriptRoot "..\..\.infra\local\shared\docker-compose.yaml"

if (-not (Test-Path $sharedCompose)) {
    Write-Host "Error: Shared compose file not found: $sharedCompose"
    exit 1
}

$sharedEnv = Join-Path (Split-Path -Parent $sharedCompose) '.env'

$projectShared = if (Test-Path $sharedEnv) {
    $envValue = Get-Content $sharedEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "shared" }
} else { "shared" }

Write-Host "Rebuilding backend service (project: $projectShared)..."
docker compose -p $projectShared -f $sharedCompose up --build --force-recreate -d backend
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Host "Backend rebuild failed with exit code: $exitCode"
    exit 1
}

Write-Host ""
Write-Host "Waiting for backend to be healthy (max 120s)..."
$timeout = 120
$elapsed = 0
while ($elapsed -lt $timeout) {
    $health = docker inspect --format='{{.State.Health.Status}}' "$projectShared-backend-1" 2>$null
    if ($health -eq "healthy") {
        Write-Host "Backend is healthy!"
        break
    }
    if ($health -eq "unhealthy") {
        Write-Host "Backend is unhealthy. Showing logs..."
        docker compose -p $projectShared -f $sharedCompose logs backend --tail=50
        Write-Host ""
        Write-Host "To see live logs: docker compose -p $projectShared -f $sharedCompose logs -f backend"
        exit 1
    }
    Write-Host "Backend status: $health - waiting... ($elapsed/$timeout seconds)"
    Start-Sleep -Seconds 5
    $elapsed += 5
}

if ($elapsed -ge $timeout) {
    Write-Host "WARNING: Backend health check timed out. Showing logs..."
    docker compose -p $projectShared -f $sharedCompose logs backend --tail=50
    exit 1
}

Write-Host ""
Write-Host "Backend service rebuilt and is healthy."
