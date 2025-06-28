# Format FinanceFunds project
Write-Host "Running ESLint fix on FinanceFunds project..." -ForegroundColor Cyan
Set-Location -Path c:\_dev\finanzas\FinanceFrontEnd\FinanceFunds
npm run lint:fix
Write-Host "Running Prettier format on FinanceFunds project..." -ForegroundColor Cyan
npm run format
Write-Host "Formatting complete!" -ForegroundColor Green
