#!/bin/bash
echo "Building AWS Lambda functions..."

echo ""
echo "Building Authentication Lambda..."
cd src/TechFood.Lambda.Authentication
dotnet publish -c Release -o bin/Release/publish
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to build Authentication Lambda"
    exit 1
fi
cd ../..

echo ""
echo "Building Customers Lambda..."
cd src/TechFood.Lambda.Customers
dotnet publish -c Release -o bin/Release/publish
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to build Customers Lambda"
    exit 1
fi
cd ../..

echo ""
echo "Lambda functions built successfully!"
echo ""
echo "To deploy using SAM CLI:"
echo "sam deploy --guided"
echo ""
echo "Or use the deploy script:"
echo "./deploy-lambdas.sh"
