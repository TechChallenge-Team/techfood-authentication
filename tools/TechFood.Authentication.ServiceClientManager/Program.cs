using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Infra.Persistence.Contexts;


Console.WriteLine("=== TechFood Service Client Manager ===\n");

// Configuração do DbContext
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var options = new DbContextOptionsBuilder<AuthContext>()
    .UseSqlServer(configuration.GetConnectionString("DataBaseConection"))
    .Options;

using var context = new AuthContext(options);

var exit = false;

while (!exit)
{
    Console.WriteLine("\nEscolha uma opção:");
    Console.WriteLine("1 - Criar novo Service Client");
    Console.WriteLine("2 - Listar Service Clients");
    Console.WriteLine("3 - Desativar Service Client");
    Console.WriteLine("4 - Reativar Service Client");
    Console.WriteLine("5 - Rotacionar Secret");
    Console.WriteLine("0 - Sair");
    Console.Write("\nOpção: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            await CreateClient(context);
            break;
        case "2":
            await ListClients(context);
            break;
        case "3":
            await DeactivateClient(context);
            break;
        case "4":
            await ReactivateClient(context);
            break;
        case "5":
            await RotateSecret(context);
            break;
        case "0":
            exit = true;
            break;
        default:
            Console.WriteLine("Opção inválida!");
            break;
    }
}

static async Task CreateClient(AuthContext context)
{
    Console.WriteLine("\n=== Criar Novo Service Client ===");

    Console.Write("Nome do serviço (ex: Order Service): ");
    var name = Console.ReadLine();

    Console.Write("Client ID (deixe vazio para gerar automaticamente): ");
    var clientId = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(clientId))
    {
        clientId = GenerateClientId(name!);
    }

    // Verificar se já existe
    if (await context.ServiceClients.AnyAsync(c => c.ClientId == clientId))
    {
        Console.WriteLine($"\n❌ Client ID '{clientId}' já existe!");
        return;
    }

    Console.Write("Scopes (separados por vírgula, ex: orders.read,orders.write): ");
    var scopesInput = Console.ReadLine();
    var scopes = scopesInput?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .ToArray() ?? Array.Empty<string>();

    // Gerar secret
    var secret = GenerateClientSecret();
    var passwordHasher = new PasswordHasher<ServiceClient>();
    var secretHash = passwordHasher.HashPassword(null!, secret);

    var client = new ServiceClient
    {
        ClientId = clientId,
        ClientSecretHash = secretHash,
        Name = name!,
        Scopes = scopes,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    context.ServiceClients.Add(client);
    await context.SaveChangesAsync();

    Console.WriteLine("\n✅ Service Client criado com sucesso!\n");
    PrintClientInfo(client, secret);
}

static async Task ListClients(AuthContext context)
{
    Console.WriteLine("\n=== Service Clients ===\n");

    var clients = await context.ServiceClients
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync();

    if (!clients.Any())
    {
        Console.WriteLine("Nenhum client cadastrado.");
        return;
    }

    foreach (var client in clients)
    {
        var status = client.IsActive ? "✅ Ativo" : "❌ Inativo";
        var lastUsed = client.LastUsedAt.HasValue
            ? client.LastUsedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
            : "Nunca usado";

        Console.WriteLine($"ID: {client.Id}");
        Console.WriteLine($"Client ID: {client.ClientId}");
        Console.WriteLine($"Nome: {client.Name}");
        Console.WriteLine($"Status: {status}");
        Console.WriteLine($"Scopes: {string.Join(", ", client.Scopes)}");
        Console.WriteLine($"Criado em: {client.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Último uso: {lastUsed}");
        Console.WriteLine(new string('-', 50));
    }
}

static async Task DeactivateClient(AuthContext context)
{
    Console.Write("\nClient ID para desativar: ");
    var clientId = Console.ReadLine();

    var client = await context.ServiceClients
        .FirstOrDefaultAsync(c => c.ClientId == clientId);

    if (client == null)
    {
        Console.WriteLine("❌ Client não encontrado!");
        return;
    }

    if (!client.IsActive)
    {
        Console.WriteLine("⚠️ Client já está inativo!");
        return;
    }

    client.IsActive = false;
    await context.SaveChangesAsync();

    Console.WriteLine($"✅ Client '{client.Name}' desativado com sucesso!");
}

static async Task ReactivateClient(AuthContext context)
{
    Console.Write("\nClient ID para reativar: ");
    var clientId = Console.ReadLine();

    var client = await context.ServiceClients
        .FirstOrDefaultAsync(c => c.ClientId == clientId);

    if (client == null)
    {
        Console.WriteLine("❌ Client não encontrado!");
        return;
    }

    if (client.IsActive)
    {
        Console.WriteLine("⚠️ Client já está ativo!");
        return;
    }

    client.IsActive = true;
    await context.SaveChangesAsync();

    Console.WriteLine($"✅ Client '{client.Name}' reativado com sucesso!");
}

static async Task RotateSecret(AuthContext context)
{
    Console.WriteLine("\n=== Rotacionar Secret ===");
    Console.Write("Client ID: ");
    var clientId = Console.ReadLine();

    var client = await context.ServiceClients
        .FirstOrDefaultAsync(c => c.ClientId == clientId);

    if (client == null)
    {
        Console.WriteLine("❌ Client não encontrado!");
        return;
    }

    Console.WriteLine($"\n⚠️ Você está prestes a rotacionar o secret do client '{client.Name}'");
    Console.WriteLine("O secret atual será invalidado imediatamente!");
    Console.Write("Confirmar? (s/n): ");

    if (Console.ReadLine()?.ToLower() != "s")
    {
        Console.WriteLine("Operação cancelada.");
        return;
    }

    // Gerar novo secret
    var newSecret = GenerateClientSecret();
    var passwordHasher = new PasswordHasher<ServiceClient>();
    var newSecretHash = passwordHasher.HashPassword(null!, newSecret);

    client.ClientSecretHash = newSecretHash;
    await context.SaveChangesAsync();

    Console.WriteLine("\n✅ Secret rotacionado com sucesso!\n");
    Console.WriteLine("=== Novo Secret ===");
    Console.WriteLine($"Client ID: {client.ClientId}");
    Console.WriteLine($"Client Secret: {newSecret}");
    Console.WriteLine("\n⚠️ IMPORTANTE: Armazene este secret em um local seguro!");
    Console.WriteLine("Ele não poderá ser recuperado novamente.\n");
}

static string GenerateClientSecret(int length = 32)
{
    var bytes = new byte[length];
    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes);
}

static string GenerateClientId(string serviceName)
{
    return serviceName
        .ToLowerInvariant()
        .Replace(" ", "-")
        .Replace("_", "-");
}

static void PrintClientInfo(ServiceClient client, string secret)
{
    Console.WriteLine("=== Informações do Client ===");
    Console.WriteLine($"Service Name: {client.Name}");
    Console.WriteLine($"Client ID: {client.ClientId}");
    Console.WriteLine($"Client Secret: {secret}");
    Console.WriteLine($"Scopes: {string.Join(", ", client.Scopes)}");
    Console.WriteLine($"Created At: {client.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
    Console.WriteLine("\n⚠️ IMPORTANTE: Armazene o Client Secret em um local seguro!");
    Console.WriteLine("Ele não poderá ser recuperado novamente.\n");
    Console.WriteLine("=== Exemplo de Configuração ===");
    Console.WriteLine($@"{{
  ""Authentication"": {{
    ""ClientId"": ""{client.ClientId}"",
    ""ClientSecret"": ""{secret}""
  }}
}}");
    Console.WriteLine("\n=== Exemplo cURL ===");
    Console.WriteLine($@"curl -X POST https://auth.techfood.com/v1/token \
  -H 'Content-Type: application/json' \
  -d '{{
    ""client_id"": ""{client.ClientId}"",
    ""client_secret"": ""{secret}"",
    ""grant_type"": ""client_credentials""
  }}'");
    Console.WriteLine();
}
