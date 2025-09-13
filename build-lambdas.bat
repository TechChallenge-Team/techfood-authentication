@echo off
echo Building AWS Lambda functions...

echo.
echo Building Authentication Lambda...
cd src\TechFood.Lambda.Authentication
dotnet publish -c Release -o bin\Release\publish
if %errorlevel% neq 0 (
    echo ERROR: Failed to build Authentication Lambda
    exit /b 1
)
cd ..\..

echo.
echo Building Customers Lambda...
cd src\TechFood.Lambda.Customers
dotnet publish -c Release -o bin\Release\publish
if %errorlevel% neq 0 (
    echo ERROR: Failed to build Customers Lambda
    exit /b 1
)
cd ..\..

echo.
echo Lambda functions built successfully!
echo.
echo To deploy using SAM CLI:
echo sam deploy --guided
echo.
echo Or use the deploy script:
echo deploy-lambdas.bat
