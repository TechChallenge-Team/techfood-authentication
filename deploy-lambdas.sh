#!/bin/bash
echo "Deploying TechFood Lambda functions to AWS..."

# Check if SAM CLI is installed
if ! command -v sam &> /dev/null; then
    echo "ERROR: SAM CLI is not installed. Please install it first:"
    echo "https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install.html"
    exit 1
fi

# Build the functions first
echo "Building functions..."
./build-lambdas.sh
if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "Deploying with SAM..."

# Deploy with SAM
sam deploy --guided

echo ""
echo "Deployment completed!"
echo ""
echo "To test the endpoints:"
echo "1. Get the API Gateway URL from the CloudFormation outputs"
echo "2. Use the endpoints:"
echo "   POST /v1/authentication/signin"
echo "   POST /v1/customers"
echo "   GET /v1/customers/{document}"
