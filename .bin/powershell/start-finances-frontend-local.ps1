# PowerShell script to check if Docker Desktop is running and start docker compose in .infra\local\finances
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$financesCompose = Join-Path $PSScriptRoot "..\..\.infra\local\finances\docker-compose.yaml"

Write-Host "Checking compose files:"
Write-Host "  Finances: $financesCompose"

if (-not (Test-Path $financesCompose)) {
    Write-Host "Error: Finances compose file not found: $financesCompose"
    exit 1
}

# Get project names from .env files or use defaults
$financesEnv = Join-Path (Split-Path -Parent $financesCompose) '.env'

$projectFinances = if (Test-Path $financesEnv) { 
    $envValue = Get-Content $financesEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') }
    if ($envValue) { $envValue } else { "finances" }
} else { "finances" }

Write-Host ""
Write-Host "Starting Finances project (project: $projectFinances)..."
docker compose -p $projectFinances -f $financesCompose up --build -d
$exitFinances = $LASTEXITCODE
if ($exitFinances -ne 0) {
    Write-Host "Finances failed with exit code: $exitFinances"
}

Write-Host ""
if ($exitFinances -eq 0) {
    Write-Host "Waiting for finances frontend to be healthy (max 60s)..."
    $timeout = 60
    $elapsed = 0
    while ($elapsed -lt $timeout) {
        $health = docker inspect --format='{{.State.Health.Status}}' "$projectFinances-frontend-1" 2>$null
        if ($health -eq "healthy") {
            Write-Host "Frontend is healthy!"
            break
        }
        if ($health -eq "unhealthy") {
            Write-Host "Frontend is unhealthy. Showing logs..."
            docker compose -p $projectFinances -f $financesCompose logs frontend --tail=50
            break
        }
        Write-Host "Frontend status: $health - waiting... ($elapsed/$timeout seconds)"
        Start-Sleep -Seconds 5
        $elapsed += 5
    }
    if ($elapsed -ge $timeout) {
        Write-Host "WARNING: Frontend health check timed out. Showing logs..."
        docker compose -p $projectFinances -f $financesCompose logs frontend --tail=50
    }

    Write-Host ""
    Write-Host "All services started successfully!"
    Write-Host "Frontend: http://localhost:5100"
    Write-Host ""
    Write-Host "To stop: docker compose -p $projectFinances stop"
}
else {
    Write-Host "Error: One or more services failed to start."
    Write-Host "Finances exit code: $exitFinances"
    exit 1
}
