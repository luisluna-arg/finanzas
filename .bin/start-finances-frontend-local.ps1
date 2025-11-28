# PowerShell script to check if Docker Desktop is running and start docker compose in Infra\Local\Finances
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$financesCompose = Join-Path $PSScriptRoot "..\Infra\Local\Finances\docker-compose.yaml"

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
    Write-Host "All services started successfully!"
    Write-Host "Frontend: http://localhost:5100"
    Write-Host "Redis: localhost:6379"
    Write-Host ""
    Write-Host "To stop: docker compose -p $projectFinances stop"
}
else {
    Write-Host "Error: One or more services failed to start."
    Write-Host "Finances exit code: $exitFinances"
    exit 1
}
