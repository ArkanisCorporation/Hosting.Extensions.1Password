# 1Password Integration Demo

This demo project showcases how to use the `Arkanis.Hosting.Extensions.1Password` library in a .NET application.

## Overview

This minimal example demonstrates:
- How to integrate 1Password secret resolution into your application
- The recommended pattern: only use 1Password in Development environment
- How to configure secrets using the `op://` URI scheme
- How to access resolved secrets through `IConfiguration`

## Prerequisites

1. **1Password CLI (`op`)** must be installed and available in your PATH
2. You must be signed in to 1Password CLI: `op signin`
3. You need to have secrets stored in your 1Password vault

## Setup

### 1. Install the NuGet Package

```bash
dotnet add package Arkanis.Hosting.Extensions.1Password
```

### 2. Configure Your Secrets

In `appsettings.Development.json`, reference your 1Password secrets using the `op://` URI scheme:

```json
{
  "ConnectionStrings": {
    "Database": "op://Dev/MyApp Database/connection string"
  },
  "Secrets": {
    "ApiKey": "op://Dev/MyApp API/api key",
    "ApiSecret": "op://Dev/MyApp API/api secret"
  }
}
```

The format is: `op://[vault]/[item]/[field]`

### 3. Use 1Password in Your Application

In your `Program.cs`, add the 1Password integration **only for Development**:

```csharp
using Arkanis.Hosting.Extensions._1Password;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Use 1Password ONLY in Development environment
if (builder.Environment.IsDevelopment())
{
    await builder.Use1PasswordAsync();
}

var app = builder.Build();
await app.RunAsync();
```

### 4. Access Your Secrets

Secrets are automatically resolved and available through `IConfiguration`:

```csharp
var config = app.Services.GetRequiredService<IConfiguration>();
var apiKey = config["Secrets:ApiKey"];
var dbConnection = config["ConnectionStrings:Database"];
```

## Running the Demo

1. Ensure you're signed in to 1Password CLI:
   ```bash
   op signin
   ```

2. Set the environment to Development (this is usually the default):
   ```bash
   $env:ASPNETCORE_ENVIRONMENT = "Development"  # PowerShell
   ```

3. Run the demo:
   ```bash
   dotnet run
   ```

## Expected Output

```
Running in Development environment - using 1Password for secrets...
info: Program[0]
      === Configuration Demo ===
info: Program[0]
      App Name: 1Password Integration Demo
info: Program[0]
      Database Connection: [your actual database connection string from 1Password]
info: Program[0]
      API Key: [masked API key]
info: Program[0]
      API Secret: [masked API secret]
info: Program[0]
      ========================
```

## Multi-Account Support

If you have multiple 1Password accounts, you can specify which account to use:

```csharp
if (builder.Environment.IsDevelopment())
{
    await builder.Use1PasswordAsync(account: "my-team.1password.com");
}
```

## Production Deployment

In production, the library should **not** be used. Instead:
- Use environment variables
- Use Azure Key Vault, AWS Secrets Manager, or similar
- Use your platform's secret management system

The demo automatically skips 1Password integration when not in Development environment, falling back to the base configuration in `appsettings.json`.

## Copy-Paste Quick Start

Here's the minimal code you need:

```csharp
// Program.cs
using Arkanis.Hosting.Extensions._1Password;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    await builder.Use1PasswordAsync();
}

var app = builder.Build();
await app.RunAsync();
```

```jsonc
// appsettings.Development.json
{
  "MySecret": "op://Vault/Item/field"
}
```

That's it! Your secrets will be automatically resolved from 1Password when running in Development.
