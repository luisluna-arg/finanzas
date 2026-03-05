# Script to update the database with the current migrations
# This script updates the database to the latest migration or a specific migration point

# Get the current directory
$currentDir = $PSScriptRoot
$migrationsDir = Split-Path -Parent $currentDir
$solutionDir = Split-Path -Parent $migrationsDir

# Ask if verbose output is desired
$verboseOption = Read-Host -Prompt "Do you want verbose output? (y/n) [N]"
if ([string]::IsNullOrWhiteSpace($verboseOption)) {
    $verboseOption = "n"  # Default to "no" if empty
}
$verboseFlag = if ($verboseOption.ToLower() -eq "y") { "--verbose" } else { "" }

# Check if a specific migration should be targeted
$specificMigrationPrompt = Read-Host -Prompt "Update to a specific migration? Leave empty for latest (format: 20230915060135_Debits or MigrationName)"
$migrationParam = ""

# Process the migration input - support both migration names and full identifiers
if (-not [string]::IsNullOrWhiteSpace($specificMigrationPrompt)) {
    # Check if the input is just a name (not a timestamp_name format)
    if (-not ($specificMigrationPrompt -match "^\d{14}_")) {
        # Try to find the migration by name (case insensitive)
        $migrations = Get-ChildItem -Path "$migrationsDir" -Filter "*.cs" | 
            Where-Object { $_.Name -match "^\d{14}_(.*)\.cs$" -and $_.Name -notmatch "\.Designer\.cs$" }

        $matchingMigration = $migrations | Where-Object { 
            $migName = $_.Name -replace "^\d{14}_(.*)\.cs$", '$1'
            $migName -eq $specificMigrationPrompt
        } | Select-Object -First 1

        if ($matchingMigration) {
            $specificMigration = $matchingMigration.Name -replace "\.cs$", ""
            Write-Host "Found matching migration: $specificMigration" -ForegroundColor Green
        } else {
            Write-Host "Warning: Could not find a migration with name '$specificMigrationPrompt'. Using as provided." -ForegroundColor Yellow
            $specificMigration = $specificMigrationPrompt
        }
    } else {
        $specificMigration = $specificMigrationPrompt
    }

    $migrationParam = " $specificMigration"
}

# Run the database update command
try {
    # Change to the solution directory
    Push-Location -Path $solutionDir

    # Show what we're about to do
    $targetMessage = if ([string]::IsNullOrWhiteSpace($specificMigration)) { "to latest migration" } else { "to migration '$specificMigration'" }
    Write-Host "Updating database $targetMessage..." -ForegroundColor Cyan

    # Run the EF Core command to update the database
    Write-Host "Run from '$solutionDir'" -ForegroundColor Blue
    $command = "dotnet ef database update$migrationParam --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
    Write-Host "Executing: $command" -ForegroundColor DarkGray
    $output = Invoke-Expression $command

    # Display command output
    $output

    # Check if update was successful
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database updated successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to update database. See error details above." -ForegroundColor Red
    }
} catch {
    Write-Host "An error occurred: $_" -ForegroundColor Red
} finally {
    # Return to the original directory
    Pop-Location
}
