@echo off
echo ========================================
echo  E-commerce API Test Runner
echo ========================================
echo.
echo Running all tests...
echo.
dotnet test Ecommerce.Tests/Ecommerce.Tests.csproj --verbosity normal
echo.
echo ========================================
echo  Test Results Summary
echo ========================================
pause








