$scriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Resolve-Path (Join-Path $scriptDir "..\..\FinanceFrontEnd\FinanceApp")

if (-not (Test-Path (Join-Path $projectDir "package.json"))) {
    Write-Host "ERROR: Could not find package.json at $projectDir" -ForegroundColor Red
    exit 1
}

Push-Location $projectDir

Write-Host "Script directory : $scriptDir" -ForegroundColor Cyan
Write-Host "npm project      : $projectDir" -ForegroundColor Cyan
Write-Host "Fetching open Dependabot alerts..." -ForegroundColor Cyan
$alerts = gh api "repos/luisluna-arg/finanzas/dependabot/alerts?state=open&per_page=100" | ConvertFrom-Json
Write-Host "Found $($alerts.Count) open alerts.`n" -ForegroundColor Cyan

$results = $alerts | ForEach-Object {
    $pkg = $_.security_vulnerability.package.name
    $num = $_.number
    $severity = $_.security_vulnerability.severity
    Write-Host "  Checking alert #$num  [$severity]  $pkg ..." -NoNewline
    $installedLines = npm ls $pkg --all --prefix $projectDir 2>&1 | Where-Object { $_ -match "@\d" -and $_ -notmatch "deduped" }
    $status = if ($installedLines) { "INSTALLED" } else { "NOT FOUND - can dismiss" }
    $color  = if ($installedLines) { "Yellow" } else { "Green" }
    Write-Host "  $status" -ForegroundColor $color
    [PSCustomObject]@{
        alert    = $num
        severity = $severity
        package  = $pkg
        status   = $status
        versions = ($installedLines -replace ".*($pkg@[\d\.]+).*", '$1' | Select-Object -Unique) -join ", "
    }
}

Write-Host "`n--- SUMMARY ---" -ForegroundColor Cyan
$results | Sort-Object status, severity | Format-Table -AutoSize | Tee-Object -FilePath (Join-Path $scriptDir "check-unused-main-app-dependabot-alerts-results.log.log")
Write-Host "Results saved to $scriptDir\check-unused-main-app-dependabot-alerts-results.log.log" -ForegroundColor Cyan

$dismissable = $results | Where-Object { $_.status -eq "NOT FOUND - can dismiss" }
if ($dismissable.Count -eq 0) {
    Write-Host "`nNo alerts to dismiss." -ForegroundColor Yellow
} else {
    Write-Host "`n$($dismissable.Count) alert(s) can be dismissed (package not found in dependency tree):" -ForegroundColor Green
    $dismissable | ForEach-Object { Write-Host "  #$($_.alert) [$($_.severity)] $($_.package)" }

    $confirm = Read-Host "`nDismiss all $($dismissable.Count) alert(s)? (y/N)"
    if ($confirm -eq 'y' -or $confirm -eq 'Y') {
        $dismissable | ForEach-Object {
            $num = $_.alert
            $pkg = $_.package
            Write-Host "  Dismissing alert #$num ($pkg) ..." -NoNewline
            gh api -X PATCH "repos/luisluna-arg/finanzas/dependabot/alerts/$num" `
                --field state=dismissed `
                --field dismissed_reason=not_used `
                --field "dismissed_comment=Package '$pkg' was not found in the dependency tree. Verified with 'npm ls $pkg --all' returning empty. Dismissing as not used." | Out-Null
            Write-Host " Done" -ForegroundColor Green
        }
        Write-Host "`nAll dismissable alerts have been closed." -ForegroundColor Green
    } else {
        Write-Host "No alerts dismissed." -ForegroundColor Yellow
    }
}

Pop-Location
