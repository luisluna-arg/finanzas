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
}

Write-Host ""
Write-Host "Starting Finances project (project: $projectFinances)..."
docker compose -p $projectFinances -f $financesCompose up --build -d
$exitFinances = $LASTEXITCODE
if ($exitFinances -ne 0) {
    Write-Host "Finances failed with exit code: $exitFinances"
}

Write-Host ""
if ($exitFinances -eq 0 -and $exitShared -eq 0) {
    Write-Host "All services started successfully!"
    Write-Host "Frontend: http://localhost:5100"
    Write-Host "Backend API: http://localhost:5000/swagger/index.html"
    Write-Host "Redis: localhost:6379"
    Write-Host ""
    Write-Host "To stop: docker compose -p $projectFinances stop; docker compose -p $projectShared stop"
}
else {
    Write-Host "Error: One or more services failed to start."
    Write-Host "Finances exit code: $exitFinances"
    Write-Host "Shared exit code: $exitShared"
    exit 1
}
