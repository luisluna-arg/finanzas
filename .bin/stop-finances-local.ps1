# PowerShell script to stop finance services
$financesCompose = Join-Path $PSScriptRoot "..\Infra\Local\Finances\docker-compose.yaml"
$sharedCompose = Join-Path $PSScriptRoot "..\Infra\Local\Shared\docker-compose.yaml"

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

Write-Host "Stopping Finances project (project: $projectFinances)..."
docker compose -p $projectFinances -f $financesCompose stop

Write-Host ""
$stopShared = Read-Host "Stop shared services (backend, postgres, redis)? [y/N]"
if ($stopShared -eq 'y' -or $stopShared -eq 'Y') {
    Write-Host "Stopping Shared project (project: $projectShared)..."
    docker compose -p $projectShared -f $sharedCompose stop
    Write-Host "All services stopped."
}
else {
    Write-Host "Finances frontend stopped. Shared services (backend, postgres, redis) are still running."
}
