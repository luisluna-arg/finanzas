# PowerShell script to apply pending EF Core migrations to the Shared stack's Postgres container
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
if (-not $dockerProcess) {
    Write-Host "Docker Desktop is not running. Please start Docker Desktop."
    exit 1
}

$repoRoot = Join-Path $PSScriptRoot "..\.."
$sharedCompose = Join-Path $repoRoot ".infra\local\shared\docker-compose.yaml"

if (-not (Test-Path $sharedCompose)) {
    Write-Host "Error: Shared compose file not found: $sharedCompose"
    exit 1
}

$sharedEnv = Join-Path (Split-Path -Parent $sharedCompose) '.env'

if (-not (Test-Path $sharedEnv)) {
    Write-Host "Error: Shared env file not found: $sharedEnv"
    exit 1
}

$projectShared = (Get-Content $sharedEnv | Where-Object { $_ -match '^COMPOSE_PROJECT_NAME=' } | ForEach-Object { $_.Split('=')[1].Trim('"') })
if (-not $projectShared) { $projectShared = "shared" }

# The backend container talks to Postgres via host.docker.internal, which only resolves inside
# Docker's network. From this script (running on the host), the same Postgres is reachable on
# localhost instead, since it's the native/host-installed instance (see .env for the container's
# own connection string).
$connectionStringLine = Get-Content $sharedEnv | Where-Object { $_ -match '^ConnectionStrings__PostgresDb=' } | Select-Object -First 1
if (-not $connectionStringLine) {
    Write-Host "Error: ConnectionStrings__PostgresDb not found in $sharedEnv"
    exit 1
}

$containerConnectionString = ($connectionStringLine -split '=', 2)[1].Trim('"')
$localConnectionString = $containerConnectionString -replace 'host\.docker\.internal', 'localhost'

Write-Host "Applying migrations (project: $projectShared)..."
Write-Host "  Target: $($localConnectionString -replace 'Password=[^;]*', 'Password=***')"

$migrationsProject = Join-Path $repoRoot "FinanceBackEnd\src\Finance.Migrations"
if (-not (Test-Path $migrationsProject)) {
    Write-Host "Error: Migrations project not found: $migrationsProject"
    exit 1
}

$env:ConnectionStrings__PostgresDb = $localConnectionString
try {
    dotnet ef database update --project $migrationsProject --startup-project $migrationsProject
    $exitCode = $LASTEXITCODE
} finally {
    Remove-Item Env:\ConnectionStrings__PostgresDb -ErrorAction SilentlyContinue
}

if ($exitCode -ne 0) {
    Write-Host "Migration failed with exit code: $exitCode"
    exit 1
}

Write-Host ""
Write-Host "Migrations applied successfully."
