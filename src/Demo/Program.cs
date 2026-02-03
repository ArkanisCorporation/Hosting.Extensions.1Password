using Arkanis.Hosting.Extensions._1Password;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Create a host builder
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add your services here
// builder.Services.AddSingleton<IMyService, MyService>();

// Use 1Password to inject secrets - ONLY in Development environment
// This is the recommended pattern to ensure secrets are properly managed in production
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Running in Development environment - using 1Password for secrets...");
    await builder.Use1PasswordAsync();
}
else
{
    Console.WriteLine($"Running in {builder.Environment.EnvironmentName} environment - using environment-specific secret management");
}

var app = builder.Build();

// Display configuration values (for demonstration purposes)
var config = app.Services.GetRequiredService<IConfiguration>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("=== Configuration Demo ===");
logger.LogInformation("App Name: {AppName}", config["AppName"]);
logger.LogInformation("Database Connection: {DbConnection}", config["ConnectionStrings:Database"]);
logger.LogInformation("API Key: {ApiKey}", MaskSecret(config["Secrets:ApiKey"]));
logger.LogInformation("API Secret: {ApiSecret}", MaskSecret(config["Secrets:ApiSecret"]));
logger.LogInformation("========================");

// Run your application
await app.RunAsync();

// Helper method to mask secrets in logs
static string? MaskSecret(string? secret)
{
    if (string.IsNullOrEmpty(secret))
        return secret;

    if (secret.Length <= 4)
        return "****";

    return secret[..4] + new string('*', secret.Length - 4);
}

