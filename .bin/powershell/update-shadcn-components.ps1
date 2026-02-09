#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Updates existing shadcn/ui components in FinanceApp

.DESCRIPTION
    This script scans the existing shadcn/ui components and updates only those
    that are already installed, rather than installing all possible components.

.EXAMPLE
    .\.bin\powershell\update-shadcn-components.ps1
#>

# Navigate to FinanceApp directory
$FinanceAppDir = Join-Path $PSScriptRoot ".." ".." "FinanceFrontEnd" "FinanceApp"
$ShadcnDir = Join-Path $FinanceAppDir "app" "components" "ui" "shadcn"

Push-Location $FinanceAppDir

try {
    Write-Host "Scanning existing shadcn/ui components..." -ForegroundColor Cyan
    
    # Get all .tsx files in the shadcn directory (excluding index files)
    $components = Get-ChildItem -Path $ShadcnDir -Filter "*.tsx" | 
        Where-Object { $_.Name -ne "index.tsx" } |
        ForEach-Object { $_.BaseName }
    
    $componentCount = $components.Count
    Write-Host "Found $componentCount components to update`n" -ForegroundColor Yellow
    
    $updated = 0
    $failed = 0
    
    foreach ($component in $components) {
        Write-Host "Updating $component..." -ForegroundColor Gray
        npx shadcn@latest add $component -y -o 2>&1 | Out-Null
        
        if ($LASTEXITCODE -eq 0) {
            $updated++
        } else {
            $failed++
            Write-Host "  Failed to update $component" -ForegroundColor Red
        }
    }
    
    Write-Host "`nUpdate complete:" -ForegroundColor Cyan
    Write-Host "  Updated: $updated" -ForegroundColor Green
    if ($failed -gt 0) {
        Write-Host "  Failed: $failed" -ForegroundColor Red
    }
    
} finally {
    Pop-Location
}
