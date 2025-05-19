# Script to remove the latest migration from the Finance.Migrations project

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

# Run the remove migration command
try {
    # Change to the solution directory where the EF Core commands should be run
    Push-Location -Path $solutionDir

    # Get all migrations sorted by date (newest first)
    $allMigrations = Get-ChildItem -Path "$migrationsDir" -Filter "*.cs" | 
        Where-Object { $_.Name -match "^\d{14}_(.*)\.cs$" -and $_.Name -notmatch "\.Designer\.cs$" } |
        Sort-Object Name -Descending
    
    # Get the latest migration
    $latestMigration = $allMigrations | Select-Object -First 1
    
    if ($latestMigration) {
        $migrationName = $latestMigration.Name -replace "^\d{14}_(.*)\.cs$", '$1'
        $migrationId = $latestMigration.Name -replace "\.cs$", ""
        
        # Get the previous migration for potential restore
        $previousMigration = $allMigrations | Select-Object -Skip 1 -First 1
        $previousMigrationName = if ($previousMigration) { $previousMigration.Name -replace "^\d{14}_(.*)\.cs$", '$1' } else { "0 (database empty)" }
        $previousMigrationId = if ($previousMigration) { $previousMigration.Name -replace "\.cs$", "" } else { "0" }
        
        # Ask if the migration has been applied to the database already
        Write-Host "The latest migration is: '$migrationName'" -ForegroundColor Cyan
        $migrationApplied = Read-Host -Prompt "Has this migration already been applied to the database? (y/n) [N]"
        if ([string]::IsNullOrWhiteSpace($migrationApplied)) {
            $migrationApplied = "n"  # Default to "no" if empty
        }
        
        # If migration has been applied, we need to revert the database first
        if ($migrationApplied.ToLower() -eq "y") {
            Write-Host "Before removing this migration, the database must be reverted to a previous migration." -ForegroundColor Yellow
            Write-Host "The previous migration is: '$previousMigrationName'" -ForegroundColor Cyan
            
            $restoreToOld = Read-Host -Prompt "Do you want to revert the database to the previous migration? (y/n) [Y]"
            if ([string]::IsNullOrWhiteSpace($restoreToOld)) {
                $restoreToOld = "y"  # Default to "yes" if empty
            }
            
            if ($restoreToOld.ToLower() -eq "y") {
                # Update database to previous migration
                Write-Host "Reverting database to previous migration '$previousMigrationName'..." -ForegroundColor Cyan
                $revertCommand = "dotnet ef database update $previousMigrationId --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
                Write-Host "Executing: $revertCommand" -ForegroundColor DarkGray
                $revertOutput = Invoke-Expression $revertCommand
                
                # Display command output
                $revertOutput
                
                if ($LASTEXITCODE -ne 0) {
                    Write-Host "Failed to revert database to previous migration. Cannot proceed with removing migration." -ForegroundColor Red
                    Pop-Location
                    exit 1
                }
                
                Write-Host "Database successfully reverted to previous migration." -ForegroundColor Green
            }
            else {
                Write-Host "Cannot remove a migration that has been applied without reverting the database first." -ForegroundColor Red
                Pop-Location
                exit 1
            }
        }
        
        # Ask for confirmation before removing
        $confirmation = Read-Host -Prompt "Are you sure you want to remove the latest migration: '$migrationName'? (y/n) [N]"
        if ([string]::IsNullOrWhiteSpace($confirmation)) {
            $confirmation = "n"  # Default to "no" if empty
        }
        
        if ($confirmation.ToLower() -eq "y") {
            Write-Host "Removing migration '$migrationName'..." -ForegroundColor Cyan
            # Run the EF Core command to remove the migration
            $command = "dotnet ef migrations remove --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
            Write-Host "Executing: $command" -ForegroundColor DarkGray
            $output = Invoke-Expression $command
            
            # Display command output
            $output

            # Check if removal was successful
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Migration '$migrationName' removed successfully!" -ForegroundColor Green
                
                # List available migrations for reference
                $migrations = Get-ChildItem -Path "$migrationsDir" -Filter "*.cs" | 
                    Where-Object { $_.Name -match "^\d{14}_(.*)\.cs$" -and $_.Name -notmatch "\.Designer\.cs$" } |
                    Sort-Object Name -Descending
                
                if ($migrations.Count -gt 0) {
                    Write-Host "`nAvailable migrations (from newest to oldest):" -ForegroundColor Cyan
                    $i = 1
                    foreach ($migration in $migrations) {
                        $migName = $migration.Name -replace "^\d{14}_(.*)\.cs$", '$1'
                        $migTime = $migration.Name -replace "^(\d{14})_.*\.cs$", '$1'
                        $formattedTime = $migTime -replace '(\d{4})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})', '$1-$2-$3 $4:$5:$6'
                        Write-Host "  $i. $migName ($formattedTime)" -ForegroundColor Gray
                        $i++
                    }
                }
                
                # Ask if the user wants to update to a specific migration
                $updateAfterRemove = Read-Host -Prompt "`nDo you want to update the database to a specific migration? (y/n) [N]"
                if ([string]::IsNullOrWhiteSpace($updateAfterRemove)) {
                    $updateAfterRemove = "n"  # Default to "no" if empty
                }
                
                if ($updateAfterRemove.ToLower() -eq "y") {
                    # Call the UpdateDatabase.ps1 script to handle the database update
                    $updateScriptPath = Join-Path -Path $currentDir -ChildPath "UpdateDatabase.ps1"
                    if (Test-Path $updateScriptPath) {
                        # Return to original directory to run the update script
                        Pop-Location
                        
                        # Call the update script
                        & $updateScriptPath
                        
                        # Go back to the solution directory for consistency
                        Push-Location -Path $solutionDir
                    } else {
                        Write-Host "Warning: UpdateDatabase.ps1 script not found. Cannot update to specific migration." -ForegroundColor Yellow
                    }
                }
            } else {
                Write-Host "Failed to remove migration. See error details above." -ForegroundColor Red
            }
        } else {
            Write-Host "Operation canceled. Migration was not removed." -ForegroundColor Yellow
        }
    } else {
        Write-Host "No migrations found in the project." -ForegroundColor Red
    }
} catch {
    Write-Host "An error occurred: $_" -ForegroundColor Red
} finally {
    # Return to the original directory
    Pop-Location
}
