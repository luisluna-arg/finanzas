### Adding migrations
From root dir

- Add a migrations
- - `dotnet ef migrations add MigrationName --project ./Finance.Migrations --context FinanceDbContext`

- Remove latest migration
- - `dotnet ef migrations remove --project ./Finance.Migrations --context FinanceDbContext`