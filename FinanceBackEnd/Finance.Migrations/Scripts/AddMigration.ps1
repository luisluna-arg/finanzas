# Script to add a new migration to the Finance.Migrations project
# This script prompts the user for a migration name and runs the dotnet ef migrations add command

# Get the current directory
$currentDir = $PSScriptRoot
$migrationsDir = Split-Path -Parent $currentDir
$solutionDir = Split-Path -Parent $migrationsDir

# Prompt the user for the migration name
$migrationName = Read-Host -Prompt "Enter the name of the migration (PascalCase)"

# Validate migration name
if ([string]::IsNullOrWhiteSpace($migrationName)) {
    Write-Host "Migration name cannot be empty. Exiting..." -ForegroundColor Red
    exit 1
}

# Format migration name to PascalCase if not already (simple check)
if (-not ($migrationName -cmatch '^[A-Z][a-zA-Z0-9]*$')) {
    Write-Host "Warning: Migration name should be in PascalCase format (e.g., AddUserTable)" -ForegroundColor Yellow
    $response = Read-Host -Prompt "Do you want to continue anyway? (y/n)"
    if ($response.ToLower() -ne "y") {
        Write-Host "Operation canceled. Exiting..." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Adding migration '$migrationName' to project..." -ForegroundColor Cyan

# Ask if verbose output is desired
$verboseOption = Read-Host -Prompt "Do you want verbose output? (y/n) [N]"
if ([string]::IsNullOrWhiteSpace($verboseOption)) {
    $verboseOption = "n"  # Default to "no" if empty
}
$verboseFlag = if ($verboseOption.ToLower() -eq "y") { "--verbose" } else { "" }

# Run the migration command
try {
    # Change to the solution directory where DbContext exists
    Push-Location -Path $solutionDir

    # Run the EF Core command to add a migration
    Write-Host "Run from '$solutionDir'" -ForegroundColor Blue
    $command = "dotnet ef migrations add $migrationName --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
    Write-Host "Executing: $command" -ForegroundColor DarkGray
    $output = Invoke-Expression $command

    # Display command output
    $output

    # Check if migration was successful
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration '$migrationName' added successfully!" -ForegroundColor Green
    }
    else {
        Write-Host "Failed to add migration. See error details above." -ForegroundColor Red
    }
}
catch {
    Write-Host "An error occurred: $_" -ForegroundColor Red
}
finally {
    # Return to the original directory
    Pop-Location
}

# Ask if the user wants to apply the migration
$applyMigration = Read-Host -Prompt "Do you want to apply the migration to the database? (y/n) [N]"
if ([string]::IsNullOrWhiteSpace($applyMigration)) {
    $applyMigration = "n"  # Default to "no" if empty
}
if ($applyMigration.ToLower() -eq "y") {
    Write-Host "Applying migration to the database..." -ForegroundColor Cyan

    # Call the UpdateDatabase.ps1 script to handle the database update
    $updateScriptPath = Join-Path -Path $currentDir -ChildPath "UpdateDatabase.ps1"
    if (Test-Path $updateScriptPath) {
        # Call the update script
        & $updateScriptPath
    } else {
        Write-Host "Warning: UpdateDatabase.ps1 script not found. Using direct command..." -ForegroundColor Yellow

        try {
            # Change to the solution directory
            Push-Location -Path $solutionDir

            # Run the database update command directly as fallback
            $command = "dotnet ef database update --project ./Finance.Migrations --context FinanceDbContext $verboseFlag"
            Write-Host "Executing: $command" -ForegroundColor DarkGray
            $output = Invoke-Expression $command

            # Display command output
            $output

            # Check if update was successful
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Database updated successfully!" -ForegroundColor Green
            }
            else {
                Write-Host "Failed to update database. See error details above." -ForegroundColor Red
            }
        }
        catch {
            Write-Host "An error occurred during database update: $_" -ForegroundColor Red
        }
        finally {
            # Return to the original directory
            Pop-Location
        }
    }
}
