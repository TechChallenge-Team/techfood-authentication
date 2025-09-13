# TechFood - Implementação AWS Lambda

## 📋 Resumo da Implementação

Esta implementação converte os endpoints de **Customer** e **Authentication** do TechFood para **AWS Lambda Functions**, mantendo toda a lógica de negócios existente através do padrão MediatR.

## 🏗️ Arquitetura Implementada

```
┌─────────────────────────────────────────────────────────────┐
│                    AWS Cloud                                │
│                                                             │
│  ┌─────────────────┐    ┌─────────────────────────────────┐ │
│  │   API Gateway   │    │          Lambda Functions        │ │
│  │                 │    │                                 │ │
│  │ POST /signin    │────┤ TechFood.Lambda.Authentication  │ │
│  │ POST /customers │────┤ TechFood.Lambda.Customers       │ │
│  │ GET /customers  │    │                                 │ │
│  └─────────────────┘    └─────────────────────────────────┘ │
│                                       │                     │
│                                       ▼                     │
│                          ┌─────────────────────────────────┐ │
│                          │        Shared Layers           │ │
│                          │                                 │ │
│                          │ • TechFood.Application         │ │
│                          │ • TechFood.Domain              │ │
│                          │ • TechFood.Infra               │ │
│                          │ • TechFood.Contracts           │ │
│                          └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                                       │
                                       ▼
                          ┌─────────────────────────────────┐
                          │        Database                 │
                          │                                 │
                          │    SQL Server / RDS             │
                          └─────────────────────────────────┘
```

## 🚀 Funcionalidades Implementadas

### 1. Authentication Lambda

- **Endpoint**: `POST /v1/authentication/signin`
- **Funcionalidade**: Login de usuários
- **Handler**: `TechFood.Lambda.Authentication.Function`
- **Input**: `SignInRequest` (username, password)
- **Output**: JWT Token + User info

### 2. Customers Lambda

- **Endpoints**:
  - `POST /v1/customers` - Criar cliente
  - `GET /v1/customers/{document}` - Buscar cliente por documento
- **Funcionalidade**: Gestão de clientes
- **Handler**: `TechFood.Lambda.Customers.Function`
- **Roteamento**: Baseado no método HTTP (POST/GET)

## 📁 Estrutura de Arquivos Criados

```
fase1/
├── src/
│   ├── TechFood.Lambda.Authentication/        # 🆕 Lambda Authentication
│   │   ├── TechFood.Lambda.Authentication.csproj
│   │   ├── Function.cs
│   │   └── appsettings.json
│   └── TechFood.Lambda.Customers/             # 🆕 Lambda Customers
│       ├── TechFood.Lambda.Customers.csproj
│       ├── Function.cs
│       └── appsettings.json
├── template.yaml                              # 🆕 AWS SAM Template
├── samconfig.toml                             # 🆕 SAM Configuration
├── build-lambdas.bat                          # 🆕 Build Script (Windows)
├── build-lambdas.sh                           # 🆕 Build Script (Linux/Mac)
├── deploy-lambdas.bat                         # 🆕 Deploy Script (Windows)
├── deploy-lambdas.sh                          # 🆕 Deploy Script (Linux/Mac)
├── lambda-tests.http                          # 🆕 HTTP Test File
└── LAMBDA-README.md                           # 🆕 Documentação Completa
```

## 🛠️ Tecnologias Utilizadas

### AWS Services

- **AWS Lambda**: Compute serverless
- **API Gateway**: RESTful API management
- **CloudWatch**: Logs e monitoramento
- **CloudFormation**: Infrastructure as Code

### .NET Packages

- `Amazon.Lambda.Core`: Core Lambda functionality
- `Amazon.Lambda.APIGatewayEvents`: API Gateway integration
- `Amazon.Lambda.Serialization.SystemTextJson`: JSON serialization
- `Microsoft.Extensions.*`: Dependency injection e configuração

### Deployment Tools

- **AWS SAM CLI**: Serverless Application Model
- **AWS CLI**: Command line interface

## ⚡ Vantagens da Implementação

### 1. **Serverless Benefits**

- **Pay-per-use**: Pague apenas pelas execuções
- **Auto-scaling**: Escala automaticamente com a demanda
- **Zero server management**: AWS gerencia a infraestrutura
- **High availability**: Built-in redundancy

### 2. **Arquitetura Limpa Mantida**

- **Reutilização de código**: Mesma lógica de negócio (MediatR)
- **Separation of concerns**: Cada lambda tem responsabilidade específica
- **DI Container**: Mesma configuração de dependências
- **Testabilidade**: Pode ser testado localmente com SAM

### 3. **Flexibilidade de Deploy**

- **Multi-environment**: Fácil deploy para dev/staging/prod
- **Rollback**: Versioning automático das funções
- **Blue/Green**: Deploy sem downtime
- **Configuration**: Variáveis de ambiente por ambiente

## 🎯 Como Usar

### Pré-requisitos

1. **.NET 8 SDK**
2. **AWS CLI** configurado
3. **SAM CLI** instalado

### Deploy Rápido

```bash
# Windows
build-lambdas.bat
deploy-lambdas.bat

# Linux/Mac
./build-lambdas.sh
./deploy-lambdas.sh
```

### Teste Local

```bash
# Iniciar API local
sam local start-api

# Testar endpoints
curl -X POST http://localhost:3000/v1/authentication/signin \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "senha123"}'
```

## 🔄 Migração dos Controllers Originais

### Antes (Controllers)

```csharp
[ApiController]
[Route("v1/[controller]")]
public class AuthenticationController : ControllerBase
{
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(SignInRequest request)
    {
        var command = new SignInCommand(request.Username, request.Password);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
```

### Depois (Lambda)

```csharp
public async Task<APIGatewayProxyResponse> FunctionHandler(
    APIGatewayProxyRequest request, ILambdaContext context)
{
    var signInRequest = JsonSerializer.Deserialize<SignInRequest>(request.Body);
    var command = new SignInCommand(signInRequest.Username, signInRequest.Password);
    var result = await mediator.Send(command);

    return new APIGatewayProxyResponse
    {
        StatusCode = 200,
        Body = JsonSerializer.Serialize(result),
        Headers = GetCorsHeaders()
    };
}
```

## 🎉 Resultado Final

✅ **Endpoints funcionando como Lambda Functions**  
✅ **Lógica de negócio preservada**  
✅ **CORS configurado**  
✅ **Logs no CloudWatch**  
✅ **Deploy automatizado com SAM**  
✅ **Testes HTTP incluídos**  
✅ **Documentação completa**

Os endpoints de Customer e Authentication agora rodam de forma **serverless** na AWS, mantendo toda a funcionalidade original mas com os benefícios de escalabilidade automática e custo otimizado!
