# PowerShell script to fetch logs for a docker compose service on the
# funds prod server over SSH.
#
# On first use in a terminal session, prompts for and caches SSH_HOST/SSH_USER/
# SSH_PORT as process-scoped env vars, so subsequent calls in the same terminal
# don't need them passed again. These are NOT persisted beyond the terminal.
#
# Usage:
#   .\get-prod-container-logs.ps1 -Service backend
#   .\get-prod-container-logs.ps1 -Service postgres -Tail 500
#   .\get-prod-container-logs.ps1 -Service backend -Follow
#   .\get-prod-container-logs.ps1 -Service backend -Since 30m
#   .\get-prod-container-logs.ps1 -Service frontend -RemoteDir "/var/www/finance-funds/.infra/prod/funds"

param(
    [Parameter(Mandatory = $true)]
    [string]$Service,
    [int]$Tail = 200,
    [switch]$Follow,
    [string]$Since,
    [string]$RemoteDir = "/var/www/finance-funds/.infra/prod/shared"
)

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

$logsArgs = @("--tail=$Tail", "--timestamps")
if ($Follow) {
    $logsArgs += "-f"
}
if ($Since) {
    $logsArgs += "--since=$Since"
}
$logsArgs += $Service

$remoteCommand = "cd $RemoteDir && docker compose logs $($logsArgs -join ' ')"

Write-Host "Fetching logs for '$Service' on ${sshUser}@${sshHost}:${sshPort} ..."

if ($Follow) {
    # -t allocates a pty so Ctrl+C locally stops the remote `docker compose
    # logs -f` instead of leaving it running detached on the server.
    ssh -t -p $sshPort "$sshUser@$sshHost" $remoteCommand
} else {
    ssh -p $sshPort "$sshUser@$sshHost" $remoteCommand
}

$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Host "Command failed with exit code: $exitCode"
    exit $exitCode
}
