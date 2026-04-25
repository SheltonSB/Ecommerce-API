@echo off
echo ========================================
echo  E-commerce API Build Script
echo ========================================
echo.
echo Cleaning previous builds...
dotnet clean
echo.
echo Building API project...
dotnet build Ecommerce.Api/Ecommerce.Api.csproj
echo.
echo Building Test project...
dotnet build Ecommerce.Tests/Ecommerce.Tests.csproj
echo.
echo ========================================
echo  Build Complete!
echo ========================================
pause








