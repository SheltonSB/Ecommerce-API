@echo off
cls
echo ========================================
echo  E-commerce API Startup Script
echo ========================================
echo.
echo Building and starting the API...
echo.
cd Ecommerce.Api
echo Running: dotnet run
echo.
echo API will be available at http://localhost:5154
echo Swagger UI: http://localhost:5154
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.
dotnet run

