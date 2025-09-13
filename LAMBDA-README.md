# TechFood AWS Lambda Functions

Este projeto contém as AWS Lambda Functions para os endpoints de Customer e Authentication do TechFood.

## Estrutura

```
├── src/
│   ├── TechFood.Lambda.Authentication/     # Lambda para autenticação
│   └── TechFood.Lambda.Customers/          # Lambda para customers
├── template.yaml                           # AWS SAM template
├── build-lambdas.bat/sh                   # Scripts de build
└── deploy-lambdas.bat/sh                  # Scripts de deploy
```

## Pré-requisitos

### Ferramentas Necessárias

1. **.NET 8 SDK**

   ```bash
   dotnet --version
   ```

2. **AWS CLI**

   ```bash
   aws --version
   ```

3. **SAM CLI**
   ```bash
   sam --version
   ```

### Configuração AWS

Configure suas credenciais AWS:

```bash
aws configure
```

## Build e Deploy

### 1. Build Local

**Windows:**

```cmd
build-lambdas.bat
```

**Linux/Mac:**

```bash
chmod +x build-lambdas.sh
./build-lambdas.sh
```

### 2. Deploy para AWS

**Windows:**

```cmd
deploy-lambdas.bat
```

**Linux/Mac:**

```bash
chmod +x deploy-lambdas.sh
./deploy-lambdas.sh
```

### 3. Deploy Manual com SAM

```bash
# Build
sam build

# Deploy (primeira vez)
sam deploy --guided

# Deploy subsequentes
sam deploy
```

## Endpoints

Após o deploy, os seguintes endpoints estarão disponíveis:

### Authentication

- **POST** `/v1/authentication/signin`
  ```json
  {
    "username": "admin",
    "password": "senha123"
  }
  ```

### Customers

- **POST** `/v1/customers`

  ```json
  {
    "cpf": "12345678901",
    "name": "João Silva",
    "email": "joao@email.com"
  }
  ```

- **GET** `/v1/customers/{document}`
  - Exemplo: `/v1/customers/12345678901`

## Configuração

### Variáveis de Ambiente

As seguintes variáveis podem ser configuradas:

- `ConnectionStrings__DataBaseConection`: String de conexão do banco
- `Jwt__Key`: Chave para assinatura JWT
- `Jwt__Issuer`: Issuer do JWT
- `Jwt__Audience`: Audience do JWT
- `Jwt__ExpireMinutes`: Tempo de expiração em minutos

### Configuração no template.yaml

Edite o arquivo `template.yaml` para ajustar:

- Connection strings
- Configurações JWT
- Timeout e memória das funções
- Configurações de CORS

## Testes

### Teste Local com SAM

```bash
# Iniciar API localmente
sam local start-api

# Testar endpoints
curl -X POST http://localhost:3000/v1/authentication/signin \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "senha123"}'
```

### Teste na AWS

Use o URL da API Gateway retornado após o deploy:

```bash
curl -X POST https://your-api-id.execute-api.region.amazonaws.com/Prod/v1/authentication/signin \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "senha123"}'
```

## Monitoramento

### CloudWatch Logs

```bash
# Ver logs da função Authentication
sam logs -n AuthenticationFunction --tail

# Ver logs da função Customers
sam logs -n CustomersFunction --tail
```

### Métricas

As métricas estão disponíveis no CloudWatch:

- Invocations
- Duration
- Errors
- Throttles

## Troubleshooting

### Problemas Comuns

1. **Erro de build**: Verifique se o .NET 8 SDK está instalado
2. **Erro de deploy**: Verifique se as credenciais AWS estão configuradas
3. **Timeout**: Aumente o timeout no template.yaml
4. **Memória**: Aumente a memória no template.yaml

### Logs de Debug

Para habilitar logs detalhados, configure:

```yaml
Environment:
  Variables:
    DOTNET_ENVIRONMENT: Development
```

## Limpeza

Para remover os recursos da AWS:

```bash
sam delete
```

## Desenvolvimento

### Estrutura das Funções

Cada função Lambda segue o padrão:

1. **Configuração DI**: Setup do container de dependências
2. **Handler**: Processamento da requisição API Gateway
3. **Roteamento**: Direcionamento baseado no método HTTP
4. **Mediator**: Uso do MediatR para commands/queries
5. **Response**: Formatação da resposta com CORS

### Adicionando Novos Endpoints

1. Crie uma nova função Lambda
2. Adicione ao template.yaml
3. Configure os eventos da API Gateway
4. Implemente o handler seguindo o padrão existente
