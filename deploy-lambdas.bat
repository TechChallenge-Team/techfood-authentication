@echo off
echo Deploying TechFood Lambda functions to AWS...

REM Check if SAM CLI is installed
sam --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: SAM CLI is not installed. Please install it first:
    echo https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html
    exit /b 1
)

REM Build the functions first
echo Building functions...
call build-lambdas.bat
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    exit /b 1
)

echo.
echo Deploying with SAM...

REM Deploy with SAM
sam deploy --guided

echo.
echo Deployment completed!
echo.
echo To test the endpoints:
echo 1. Get the API Gateway URL from the CloudFormation outputs
echo 2. Use the endpoints:
echo    POST /v1/authentication/signin
echo    POST /v1/customers
echo    GET /v1/customers/{document}
