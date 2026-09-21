# PowerShell script to run an arbitrary command, an inline SQL statement, or a
# SQL file against the shared Postgres container, on the funds prod server
# over SSH.
#
# On first use in a terminal session, prompts for and caches SSH_HOST/SSH_USER/
# SSH_PORT as process-scoped env vars, so subsequent calls in the same terminal
# don't need them passed again. These are NOT persisted beyond the terminal.
#
# Usage:
#   .\run-prod-ssh-command.ps1 -Command "docker compose ps"
#   .\run-prod-ssh-command.ps1 -Sql 'SELECT "Id", "Name" FROM "Bank";'
#   .\run-prod-ssh-command.ps1 -SqlFile .\scratch\seed-banks.sql
#   .\run-prod-ssh-command.ps1 -Sql 'SELECT 1;' -RemoteDir "/var/www/finance-funds/.infra/prod/shared"

param(
    [string]$Command,
    [string]$Sql,
    [string]$SqlFile,
    [string]$RemoteDir = "/var/www/finance-funds/.infra/prod/shared"
)

$modeCount = @($Command, $Sql, $SqlFile) | Where-Object { $_ } | Measure-Object | Select-Object -ExpandProperty Count

if ($modeCount -eq 0) {
    Write-Host "Error: pass one of -Command <remote shell command>, -Sql <sql statement>, or -SqlFile <path>."
    exit 1
}

if ($modeCount -gt 1) {
    Write-Host "Error: pass only one of -Command, -Sql, or -SqlFile, not more than one."
    exit 1
}

if ($SqlFile) {
    if (-not (Test-Path $SqlFile)) {
        Write-Host "Error: SQL file not found: $SqlFile"
        exit 1
    }
    $Sql = Get-Content -Raw $SqlFile
}

function Get-OrPromptEnvVar {
    param(
        [string]$Name,
        [string]$Prompt,
        [string]$Default
    )

    $current = [System.Environment]::GetEnvironmentVariable($Name)
    if ($current) {
        return $current
    }

    $promptText = if ($Default) { "$Prompt [$Default]" } else { $Prompt }
    $value = Read-Host $promptText
    if (-not $value -and $Default) {
        $value = $Default
    }
    if (-not $value) {
        Write-Host "Error: $Name is required."
        exit 1
    }

    Set-Item -Path "Env:$Name" -Value $value
    return $value
}

$sshHost = Get-OrPromptEnvVar -Name "SSH_HOST" -Prompt "SSH host (IP or domain)"
$sshUser = Get-OrPromptEnvVar -Name "SSH_USER" -Prompt "SSH user"
$sshPort = Get-OrPromptEnvVar -Name "SSH_PORT" -Prompt "SSH port" -Default "22"

if ($Sql) {
    # -T disables pseudo-tty allocation so psql reads the query from stdin,
    # which ssh forwards from this script's own stdin pipe below. $POSTGRES_USER
    # / $POSTGRES_DB must resolve inside the container (set via docker-compose
    # environment), not on the remote host — so they're passed through a single-
    # quoted `sh -c` to stop the remote host's shell from expanding (and blanking)
    # them before docker even runs.
    $remoteCommand = "cd $RemoteDir && docker compose exec -T postgres sh -c 'psql -U `"`$POSTGRES_USER`" -d `"`$POSTGRES_DB`"'"
    Write-Host "Running SQL against postgres on ${sshUser}@${sshHost}:${sshPort} ..."
    $Sql | ssh -p $sshPort "$sshUser@$sshHost" $remoteCommand
} else {
    Write-Host "Running command on ${sshUser}@${sshHost}:${sshPort} ..."
    ssh -p $sshPort "$sshUser@$sshHost" $Command
}

$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Host "Command failed with exit code: $exitCode"
    exit $exitCode
}
