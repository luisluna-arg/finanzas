# Database Migration Scripts

This folder contains PowerShell scripts for managing Entity Framework Core migrations in the Finance project.

## Available Scripts

### AddMigration.ps1
Creates a new migration with a user-provided name.
```powershell
.\AddMigration.ps1
```
- Prompts for migration name
- Validates PascalCase naming
- Option for verbose output
- Option to apply the migration immediately

### RemoveMigration.ps1
Removes the latest migration from the project.
```powershell
.\RemoveMigration.ps1
```
- Shows the migration that will be removed
- Option for verbose output
- Shows remaining migrations after removal
- Option to update to a specific migration afterward

### UpdateDatabase.ps1
Updates the database to the latest migration or a specific migration.
```powershell
.\UpdateDatabase.ps1
```
- Option for verbose output
- Support for targeting a specific migration (by name or full identifier)
- Detailed output messages

### DropDatabase.ps1
Drops the entire database (use with caution).
```powershell
.\DropDatabase.ps1
```
- Requires double confirmation for safety
- Option for verbose output
- Option to recreate a fresh database after dropping

## Usage Notes

1. These scripts should be run from the Scripts directory
2. All scripts support verbose output option
3. The scripts are interconnected - UpdateDatabase.ps1 is used by other scripts when needed
