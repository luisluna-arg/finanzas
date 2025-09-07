# PowerShell script to check if Docker Desktop is running and start docker compose in Infra\Finances\Local
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$targetDir = Join-Path $PSScriptRoot "..\Infra\Finances\Local"
Write-Host "Changing directory to $targetDir"
Set-Location $targetDir

Write-Host "Running: docker compose up --build -d"
docker compose up --build -d
