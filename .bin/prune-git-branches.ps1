#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Prune git branches by removing local branches that no longer exist on the remote.

.DESCRIPTION
    This script performs the following git cleanup operations:
    1. Fetches the latest changes from all remotes with pruning
    2. Removes local tracking branches that no longer exist on the remote
    3. Shows deleted branches and remaining local branches
    4. Optionally removes merged branches (with confirmation)

.PARAMETER PruneMerged
    Also remove local branches that have been merged into the default branch

.PARAMETER Force
    Skip confirmation prompts (use with caution)

.EXAMPLE
    .\prune-git-branches.ps1
    Basic pruning of remote tracking branches

.EXAMPLE
    .\prune-git-branches.ps1 -PruneMerged
    Prune remote branches and also remove merged local branches

.EXAMPLE
    .\prune-git-branches.ps1 -PruneMerged -Force
    Prune all branches without confirmation prompts
#>

param(
    [switch]$PruneMerged,
    [switch]$Force
)

# Use PowerShell's native color support instead of ANSI codes
Write-Host "Git Branch Pruning Script" -ForegroundColor Blue
Write-Host "========================`n"

# Check if we're in a git repository
if (-not (Test-Path ".git")) {
    Write-Host "ERROR: Not in a git repository" -ForegroundColor Red
    exit 1
}

# Get current branch
$currentBranch = git branch --show-current
Write-Host "Current branch: " -NoNewline -ForegroundColor Blue
Write-Host "$currentBranch" -ForegroundColor Green

# Fetch with prune to update remote tracking branches
Write-Host "`nFetching and pruning remote branches..." -ForegroundColor Yellow
git fetch --all --prune

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to fetch from remote" -ForegroundColor Red
    exit 1
}

# Remove local branches that track deleted remote branches
Write-Host "`nCleaning up local tracking branches..." -ForegroundColor Yellow
$goneBranches = git for-each-ref --format='%(refname:short) %(upstream:track)' refs/heads | Where-Object { $_ -match "\[gone\]" } | ForEach-Object { ($_ -split " ")[0] }

if ($goneBranches.Count -gt 0) {
    Write-Host "Branches tracking deleted remotes:" -ForegroundColor Red
    foreach ($branch in $goneBranches) {
        Write-Host "  - $branch" -ForegroundColor White
    }
    
    if ($Force -or (Read-Host "`nRemove these branches? [y/N]") -eq "y") {
        foreach ($branch in $goneBranches) {
            if ($branch -eq $currentBranch) {
                Write-Host "WARNING: Skipping current branch: $branch" -ForegroundColor Yellow
                continue
            }
            Write-Host "Removing: $branch" -ForegroundColor Red
            git branch -D $branch
        }
    }
} else {
    Write-Host "SUCCESS: No local branches tracking deleted remotes" -ForegroundColor Green
}

# Optional: Remove merged branches
if ($PruneMerged) {
    Write-Host "`nFinding merged branches..." -ForegroundColor Yellow
    
    # Get default branch (usually main or master)
    $defaultBranch = git symbolic-ref refs/remotes/origin/HEAD 2>$null
    if ($defaultBranch) {
        $defaultBranch = $defaultBranch -replace "refs/remotes/origin/", ""
    } else {
        $defaultBranch = "main"
        Write-Host "WARNING: Could not determine default branch, using 'main'" -ForegroundColor Yellow
    }
    
    # Find merged branches (excluding current and default branch)
    $mergedBranches = git branch --merged $defaultBranch | Where-Object { 
        $branch = $_.Trim()
        $branch -notmatch "^\*" -and 
        $branch -ne $defaultBranch -and 
        $branch -ne "master" -and 
        $branch -ne "main" -and
        $branch -ne $currentBranch
    } | ForEach-Object { $_.Trim() }
    
    if ($mergedBranches.Count -gt 0) {
        Write-Host "Merged branches (safe to delete):" -ForegroundColor Red
        foreach ($branch in $mergedBranches) {
            Write-Host "  - $branch" -ForegroundColor White
        }
        
        if ($Force -or (Read-Host "`nRemove these merged branches? [y/N]") -eq "y") {
            foreach ($branch in $mergedBranches) {
                Write-Host "Removing merged branch: $branch" -ForegroundColor Red
                git branch -d $branch
            }
        }
    } else {
        Write-Host "SUCCESS: No merged branches to remove" -ForegroundColor Green
    }
}

# Show remaining local branches
Write-Host "`nRemaining local branches:" -ForegroundColor Blue
git branch -vv

Write-Host "`nSUCCESS: Branch pruning complete!" -ForegroundColor Green