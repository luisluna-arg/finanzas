# Script to drop the database for the Finance project
# This script completely removes the database, so use with caution

# Get the current directory
$currentDir = $PSScriptRoot
$migrationsDir = Split-Path -Parent $currentDir
$solutionDir = Split-Path -Parent $migrationsDir

# Ask for confirmation with double check since this is a destructive operation
Write-Host "WARNING: This script will completely delete the database!" -ForegroundColor Red
Write-Host "All data will be permanently lost." -ForegroundColor Red
$confirmation = Read-Host -Prompt "Are you sure you want to DROP the database? Type 'yes' to confirm"

if ($confirmation.ToLower() -ne "yes") {
    Write-Host "Operation canceled. The database was not dropped." -ForegroundColor Green
    exit 0
}

# Double-check confirmation
$doubleCheck = Read-Host -Prompt "Type the word 'drop' to confirm you understand this will DELETE ALL DATA"

if ($doubleCheck.ToLower() -ne "drop") {
    Write-Host "Operation canceled. The database was not dropped." -ForegroundColor Green
    exit 0
}

# Ask if verbose output is desired
$verboseOption = Read-Host -Prompt "Do you want verbose output? (y/n) [N]"
if ([string]::IsNullOrWhiteSpace($verboseOption)) {
    $verboseOption = "n"  # Default to "no" if empty
}
$verboseFlag = if ($verboseOption.ToLower() -eq "y") { "--verbose" } else { "" }

# Run the database drop command
try {
    # Change to the solution directory
    Push-Location -Path $solutionDir

    # Show what we're about to do
    Write-Host "Dropping the database..." -ForegroundColor Cyan

    # Run the EF Core command to drop the database
    Write-Host "Run from '$solutionDir'" -ForegroundColor Blue
    $command = "dotnet ef database drop --force --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
    Write-Host "Executing: $command" -ForegroundColor DarkGray
    $output = Invoke-Expression $command

    # Display command output
    $output

    # Check if drop was successful
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database dropped successfully!" -ForegroundColor Green

        # Ask if the user wants to recreate a fresh database
        $recreateDb = Read-Host -Prompt "Do you want to recreate a fresh database with migrations? (y/n) [N]"
        if ([string]::IsNullOrWhiteSpace($recreateDb)) {
            $recreateDb = "n"  # Default to "no" if empty
        }

        if ($recreateDb.ToLower() -eq "y") {
            Write-Host "Recreating database with migrations..." -ForegroundColor Cyan

            # Return to original directory to run the update script
            Pop-Location

            # Call the UpdateDatabase.ps1 script to handle the database creation
            $updateScriptPath = Join-Path -Path $currentDir -ChildPath "UpdateDatabase.ps1"
            if (Test-Path $updateScriptPath) {
                & $updateScriptPath
            } else {
                Write-Host "Warning: UpdateDatabase.ps1 script not found. Using direct command..." -ForegroundColor Yellow
    
                # Change back to solution directory
                Push-Location -Path $solutionDir
    
                # Run the database update command directly as fallback
                $updateCommand = "dotnet ef database update --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
                Write-Host "Executing: $updateCommand" -ForegroundColor DarkGray
                $updateOutput = Invoke-Expression $updateCommand
    
                # Display command output
                $updateOutput
    
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Fresh database created successfully!" -ForegroundColor Green
                } else {
                    Write-Host "Failed to create fresh database. See error details above." -ForegroundColor Red
                }
            }

            # Return to solution directory for consistency
            Push-Location -Path $solutionDir
        }
    } else {
        Write-Host "Failed to drop database. See error details above." -ForegroundColor Red
    }
} catch {
    Write-Host "An error occurred: $_" -ForegroundColor Red
} finally {
    # Return to the original directory
    Pop-Location
}
