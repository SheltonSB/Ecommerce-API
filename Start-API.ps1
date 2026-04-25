# E-commerce API Startup Script
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " E-commerce API - Starting..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to the API directory
Set-Location -Path "Ecommerce.Api"

Write-Host "Building the API..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Build successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Starting the API server..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " API will be available at:" -ForegroundColor Green
    Write-Host " 🌐 http://localhost:5154" -ForegroundColor White
    Write-Host " 📚 Swagger UI: http://localhost:5154" -ForegroundColor White
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
    Write-Host ""
    
    dotnet run --no-build
} else {
    Write-Host ""
    Write-Host "❌ Build failed. Please check the errors above." -ForegroundColor Red
    Write-Host ""
    pause
}








