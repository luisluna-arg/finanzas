# PowerShell script to stop and clean up finance services
$targetDir = Join-Path $PSScriptRoot "..\Infra\Finances\Local"
Write-Host "Changing directory to $targetDir"
Set-Location $targetDir

Write-Host "Stopping all finance services..."
docker compose down

Write-Host "Removing unused containers and networks..."
docker system prune -f

Write-Host "Finance services stopped and cleaned up."
