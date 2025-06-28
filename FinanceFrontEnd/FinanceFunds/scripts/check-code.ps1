# Format and Lint Script for FinanceFunds
Write-Host "🧹 Running formatter and linter..." -ForegroundColor Cyan

# Format code with Prettier
Write-Host "`n📝 Formatting code with Prettier..." -ForegroundColor Green
npm run format

# Run ESLint checks
Write-Host "`n🔍 Running ESLint..." -ForegroundColor Green
npm run lint

# Check if there are TypeScript errors
Write-Host "`n⚙️ Checking TypeScript compilation..." -ForegroundColor Green
npx tsc --noEmit

Write-Host "`n✅ All checks completed!" -ForegroundColor Cyan
