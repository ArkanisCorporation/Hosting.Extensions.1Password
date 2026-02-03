# 🔐 Arkanis.Hosting.Extensions.1Password

**Seamlessly integrate 1Password secrets into your .NET applications during development.**

[![NuGet](https://img.shields.io/nuget/v/Arkanis.Hosting.Extensions.1Password.svg)](https://www.nuget.org/packages/Arkanis.Hosting.Extensions.1Password/)
[![License](https://img.shields.io/github/license/ArkanisCorporation/Hosting.Extensions.1Password.svg)](LICENSE.md)

---

## ✨ Why This Library?

If you love 1Password (and who doesn't?) and want to bring that same level of security and convenience to your .NET development workflow, this library is for you.

**The Problem:** Managing secrets during development is painful. You don't want to commit API keys, connection strings, or passwords to source control, but you also don't want to juggle multiple files, environment variables, or complicated secret management systems just to run your app locally.

**The Solution:** This library bridges the gap between 1Password and your .NET application's configuration system. Just reference your 1Password secrets using the `op://` URI scheme in your existing configuration files, and let the library handle the rest. No changes to your existing configuration infrastructure required!

## 🚀 Features

- **🎯 Zero Configuration Changes Required** - Works seamlessly with your existing `IConfiguration` setup
- **🔄 Universal Compatibility** - Supports `appsettings.json`, environment variables, or any configuration source
- **⚡ Async-First Design** - Built for modern .NET with full async/await support
- **🧪 Fully Testable** - Comes with a mockable interface for unit testing
- **🌐 Multi-Account Support** - Specify which 1Password account to use
- **🎨 Developer-Friendly** - Simple API with just one method call to set up
- **📦 Lightweight** - Built on .NET Standard 2.1 for broad compatibility

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Arkanis.Hosting.Extensions.1Password
```

Or via Package Manager Console:

```powershell
Install-Package Arkanis.Hosting.Extensions.1Password
```

## 🎯 Quick Start

### 1. Store your secrets in 1Password

Create a secret in 1Password (e.g., an API key in the "Private" vault under "MyApp/ApiKey").

### 2. Reference it in your configuration

Add a reference to your secret in `appsettings.json` using the `op://` URI scheme:

```json
{
  "ApiKey": "op://Private/MyApp/ApiKey",
  "Database": {
    "ConnectionString": "op://Private/MyApp/database-connection"
  }
}
```

### 3. Enable 1Password integration

Add one line to your application startup:

```csharp
using Hosting.Extensions._1Password;

var builder = Host.CreateApplicationBuilder(args);

// That's it! 🎉
await builder.Use1PasswordAsync("my.1password.com");
// or builder.Use1Password("my.1password.com");

var host = builder.Build();
await host.RunAsync();
```

### 4. Access your secrets normally

Your secrets are now available through the standard `IConfiguration` interface.
They are only resolved in-memory at runtime, so your configuration files remain clean.

```csharp
var apiKey = builder.Configuration["ApiKey"];
var connectionString = builder.Configuration["Database:ConnectionString"];

// Use them as you normally would!
```

## 📚 Usage Examples

### Basic Usage

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Resolve secrets from your default 1Password account
await builder.Use1PasswordAsync();

var host = builder.Build();
await host.RunAsync();
```

### Specify a 1Password Account

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Use a specific 1Password account or sign-in address
await builder.Use1PasswordAsync("my.1password.com");

var host = builder.Build();
await host.RunAsync();
```

### Synchronous API

If you prefer synchronous code:

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Synchronous version (blocks until secrets are resolved)
builder.Use1Password("my.1password.com");

var host = builder.Build();
host.Run();
```

### Use in ASP.NET Core

Works perfectly with ASP.NET Core applications:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Inject 1Password secrets before registering services
await builder.Use1PasswordAsync("my.1password.com");

builder.Services.AddControllers();
// ... register other services

var app = builder.Build();
app.Run();
```

### Environment-Specific Configuration

Only use 1Password in development:

```csharp
var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    await builder.Use1PasswordAsync("my.1password.com");
}

var host = builder.Build();
await host.RunAsync();
```

### Complex Configuration Structures

The library automatically traverses nested configuration:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "op://Private/OAuth/Google/client-id",
      "ClientSecret": "op://Private/OAuth/Google/client-secret"
    },
    "GitHub": {
      "ClientId": "op://Private/OAuth/GitHub/client-id",
      "ClientSecret": "op://Private/OAuth/GitHub/client-secret"
    }
  },
  "Database": {
    "Primary": "op://Private/Database/primary-connection",
    "Readonly": "op://Private/Database/readonly-connection"
  }
}
```

All `op://` references are automatically detected and resolved, no matter how deeply nested!

## 🧪 Testing

The library is designed with testability in mind. Use the `IOpCliInvoker` interface to mock 1Password CLI calls in your tests:

```csharp
using NSubstitute;

// Create a mock invoker
var mockInvoker = Substitute.For<IOpCliInvoker>();
mockInvoker.InvokeAsync(Arg.Any<string?>(), Arg.Any<string>())
    .Returns(new BufferedCommandResult(
        exitCode: 0,
        standardOutput: "ApiKey=\"test-api-key-123\"",
        standardError: ""
    ));

// Use it in your tests
var builder = Host.CreateApplicationBuilder(args);
await builder.Use1PasswordAsync(
    account: "my.1password.com",
    opCliInvoker: mockInvoker
);

// Verify the configuration was populated
Assert.Equal("test-api-key-123", builder.Configuration["ApiKey"]);
```

## 📋 Requirements

### Runtime Requirements
- **.NET Runtime**: .NET Standard 2.1 or higher (supports .NET Core 3.0+, .NET 5+, .NET 6+, etc.)
- **1Password CLI**: The `op` CLI must be installed and available in your PATH
- **Authentication**: You must be signed in to 1Password (the CLI will use your existing session)

### Installation
To install the 1Password CLI:

```bash
# macOS (via Homebrew)
brew install 1password-cli

# Windows (via WinGet)
winget install 1password-cli

# Or download from: https://developer.1password.com/docs/cli/get-started/
```

## 🔧 How It Works

1. **Detection**: The library scans your `IConfiguration` for any values starting with `op://`
2. **Templating**: It builds a template with all detected secret references
3. **Resolution**: It invokes the 1Password CLI (`op inject`) to resolve all secrets in one call
4. **Injection**: The resolved values are written back into your configuration in-memory

All of this happens transparently during application startup, before your services are registered or initialized.

## 🎯 Use Cases

This library is perfect for:

- **✅ Local development** - Keep secrets secure while developing locally
- **✅ Team collaboration** - Share secret references (not secrets themselves) in source control
- **✅ Rapid prototyping** - Get started quickly without complex secret management
- **✅ Multi-environment setups** - Different secrets per environment, same configuration structure

**Not recommended for:**

- **❌ Production deployments** - Use your platform's native secret management (Azure Key Vault, AWS Secrets Manager, etc.)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Building from Source

```bash
# Clone the repository
git clone https://github.com/ArkanisCorporation/Hosting.Extensions.1Password.git
cd Hosting.Extensions.1Password

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built with ❤️ for the .NET and 1Password communities
- Powered by [CliWrap](https://github.com/Tyrrrz/CliWrap) for reliable CLI invocation
- Inspired by the amazing developer experience that 1Password provides

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/ArkanisCorporation/Hosting.Extensions.1Password/issues)
- **Discussions**: [GitHub Discussions](https://github.com/ArkanisCorporation/Hosting.Extensions.1Password/discussions)

---

**Made with 🔐 by developers who love 1Password**
