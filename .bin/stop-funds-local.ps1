# PowerShell script to stop funds services
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

Write-Host "Stopping Funds project (project: $projectFunds)..."
docker compose -p $projectFunds -f $fundsCompose stop

Write-Host ""
$stopShared = Read-Host "Stop shared services (backend, postgres, redis)? [y/N]"
if ($stopShared -eq 'y' -or $stopShared -eq 'Y') {
    Write-Host "Stopping Shared project (project: $projectShared)..."
    docker compose -p $projectShared -f $sharedCompose stop
    Write-Host "All services stopped."
}
else {
    Write-Host "Funds frontend stopped. Shared services (backend, postgres, redis) are still running."
}