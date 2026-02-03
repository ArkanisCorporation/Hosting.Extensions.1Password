# Contributing to Hosting.Extensions.1Password

Thank you for your interest in contributing to Hosting.Extensions.1Password! We welcome contributions from the community.

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 10.0 or later](https://dotnet.microsoft.com/download)
- [1Password Desktop App](https://1password.com/downloads/) - for authentication with the 1Password CLI
- [1Password CLI (`op`)](https://developer.1password.com/docs/cli/get-started/) - for running integration tests
- Your favorite IDE (JetBrains Rider, Visual Studio, or VS Code)

### Setting Up Your Development Environment

1. **Fork and Clone**
   ```bash
   git clone https://github.com/YOUR-USERNAME/Hosting.Extensions.1Password.git
   cd Hosting.Extensions.1Password
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run Tests**
   ```bash
   # Run unit tests (no 1Password CLI required)
   dotnet test --filter Category!=Integration

   # Run all tests including integration tests (requires 1Password CLI and authentication)
   dotnet test
   ```

## 📋 Development Guidelines

### Code Standards

This project follows strict coding standards to ensure quality and consistency:

- **Target Framework**: .NET Standard 2.1
- **Language Version**: C# 8.0 (highest supported by netstandard2.1)
- **No Implicit Usings**: Always include explicit `using` statements
- **Nullable Reference Types**: Enabled - use nullable annotations appropriately
- **No C# 9+ Features**: Avoid records, init-only properties, top-level statements, etc.
- **Namespace Convention**: Use `Arkanis.Hosting.Extensions._1Password` (note the underscore)
- **Warnings as Errors**: The build treats all warnings as errors

### Coding Conventions

1. **XML Documentation**: All public APIs must have complete XML documentation with `<summary>`, `<param>`, and `<returns>` tags
2. **PublicAPI Attribute**: Mark public APIs with `[PublicAPI]` from JetBrains.Annotations
3. **Async/Await**: Follow async patterns with `Async` suffix on method names
4. **Interface-Based Design**: Use interfaces for external dependencies to ensure testability
5. **Error Handling**: Use the `OnePasswordException` for 1Password-related errors

### Example Code Style

```csharp
/// <summary>
/// Does something useful with configuration.
/// </summary>
/// <param name="builder">The host application builder.</param>
/// <param name="value">The value to process.</param>
/// <returns>The modified builder for chaining.</returns>
[PublicAPI]
public static IHostApplicationBuilder DoSomething(
    this IHostApplicationBuilder builder,
    string value)
{
    // Implementation here
    return builder;
}
```

## 🧪 Testing

### Writing Tests

- **Unit Tests**: Go in `tests/Hosting.Extensions.1Password.UnitTests/`
- **Integration Tests**: Go in `tests/Hosting.Extensions.1Password.IntegrationTests/`
- **Test Framework**: xUnit
- **Mocking**: Use NSubstitute for mocking (e.g., `IOpCliInvoker`)
- **Naming Convention**: `MethodName_Scenario_ExpectedBehavior`

### Example Unit Test

```csharp
[Fact]
public async Task Use1PasswordAsync_WithValidSecret_ResolvesCorrectly()
{
    // Arrange
    var config = new Dictionary<string, string?> { ["Secret"] = "op://vault/item/field" };
    var builder = new FakeHostApplicationBuilder(config);
    var mockInvoker = CreateMockInvoker("Secret=\"resolved-value\"");

    // Act
    await builder.Use1PasswordAsync(opCliInvoker: mockInvoker);

    // Assert
    Assert.Equal("resolved-value", builder.Configuration["Secret"]);
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter Category!=Integration

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📦 Package Management

This project uses [Central Package Management (CPM)](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management):

- Package references go in `.csproj` files **without version attributes**
- Versions are centralized in `Directory.Packages.props`
- Lock files (`packages.lock.json`) are committed to source control

### Adding a New Dependency

1. Add package reference to the appropriate `.csproj` (without version):
   ```xml
   <PackageReference Include="NewPackage" />
   ```

2. Add version to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="NewPackage" Version="1.0.0" />
   ```

3. Restore to update lock files:
   ```bash
   dotnet restore
   ```

## 🔄 Pull Request Process

1. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Your Changes**
   - Write code following the guidelines above
   - Add/update tests
   - Update documentation if needed

3. **Test Thoroughly**
   ```bash
   dotnet build
   dotnet test
   ```

4. **Commit Your Changes**
   - Use clear, descriptive commit messages
   - Follow [Conventional Commits](https://www.conventionalcommits.org/) format:
     ```
     feat: add support for custom CLI path
     fix: handle edge case with empty secret values
     docs: update README with new examples
     ```

5. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```
   - Open a Pull Request on GitHub
   - Fill out the PR template (if available)
   - Link any related issues

6. **Code Review**
   - Address any feedback from reviewers
   - Keep your branch up to date with main
   - Ensure CI/CD checks pass

## 🐛 Reporting Bugs

Please use GitHub Issues to report bugs. Include:

- Clear description of the issue
- Steps to reproduce
- Expected behavior vs actual behavior
- Your environment (OS, .NET version, 1Password CLI version)
- Any relevant logs or error messages

## 💡 Suggesting Features

We welcome feature suggestions! Please:

- Check if the feature has already been suggested
- Open a GitHub Issue with the `enhancement` label
- Describe the use case and why it would be valuable
- Consider if you'd like to implement it yourself

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

## 🙏 Thank You!

Your contributions help make this project better for everyone. We appreciate your time and effort!

---

**Questions?** Feel free to open a GitHub Discussion or reach out to the maintainers.
