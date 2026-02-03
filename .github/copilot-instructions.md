# Copilot Instructions for Hosting.Extensions.1Password

## Project Overview
This is a .NET library that provides integration between Microsoft's Hosting extensions and 1Password CLI (`op`). It allows applications to resolve secret references (using `op://` URI scheme) during development by automatically invoking the 1Password CLI.

## Technology Stack
- **Target Framework**: .NET Standard 2.1 (main library) / .NET 10.0 (test projects)
- **Language Version**: C# 8.0 for library (highest supported by netstandard2.1), preview for tests
- **SDK Version**: .NET 10.0.100 with `latestFeature` roll-forward policy (from `global.json`)
- **Package Management**: Central Package Management (CPM) via `Directory.Packages.props`
- **Testing Frameworks**: xUnit, NSubstitute (for mocking), coverlet (for coverage)
- **Key Dependencies**:
  - CliWrap 3.10.0 (for CLI invocation)
  - Microsoft.Extensions.Hosting 10.0.1 (for hosting integration)
  - JetBrains.Annotations 2025.2.4 (for code annotations)

## Build System Configuration

### Directory.Build.props Settings
The project uses shared MSBuild properties that MUST be respected:

1. **Language Settings**:
   - `LangVersion`: `preview` (globally), but library uses `8` (C# 8.0 only)
   - `AnalysisLevel`: `preview` - use latest code analysis
   - `TreatWarningsAsErrors`: `true` - all warnings must be fixed
   - `NoWarn`: CA1707 (Identifiers should not contain underscores) - suppressed globally

2. **Documentation**:
   - `GenerateDocumentationFile`: `true` - XML documentation is REQUIRED for all public APIs

3. **Build Artifacts**:
   - `UseArtifactsOutput`: `true` - all build output goes to `artifacts/` folder
   - `DisableImplicitNuGetFallbackFolder`: `true` - no implicit NuGet fallback
   - `RestorePackagesWithLockFile`: `true` - ALWAYS use package lock files

4. **Metadata**:
   - Publisher: Arkanis Corporation
   - Authors: FatalMerlin, TheKronnY and contributors
   - Copyright: Auto-generated with current year

5. **Visibility**:
   - InternalsVisibleTo is enabled for UnitTests and IntegrationTests

### Directory.Packages.props (Central Package Management)
- **CRITICAL**: All package versions are centralized in `Directory.Packages.props`
- When adding a package reference to a `.csproj`, do NOT include `Version` attribute
- Only add/modify versions in `Directory.Packages.props` in the `<PackageVersion>` section
- After package changes, restore packages to update `packages.lock.json`

### Package Lock Files
- All projects use `packages.lock.json` files for deterministic builds
- These files MUST be kept in sync when dependencies change
- Run `dotnet restore` after any package changes to update lock files

## Coding Standards & Conventions

### General Guidelines
1. **Explicit Usings**:
   - Main library: `ImplicitUsings` is DISABLED - always include explicit `using` statements
   - Test projects: `ImplicitUsings` is ENABLED - but xUnit is explicitly included via `<Using>`

2. **Nullable Reference Types**:
   - ENABLED across all projects - use nullable annotations appropriately
   - Use `?` for nullable reference types
   - Use `{ }` pattern for null checks (e.g., `if (account is { })`)

3. **Language Features**:
   - Main library: LIMITED to C# 8.0 features only (no C# 9+ features like records, init-only properties, top-level statements, etc.)
   - Test projects: Can use preview C# features

4. **Namespace Convention**:
   - Use `Arkanis.Hosting.Extensions._1Password` (note: underscore before `1Password`)
   - Wrap namespace declarations with `#pragma warning disable CA1707` and `#pragma warning restore CA1707`

5. **Root Namespace**:
   - Library: `Arkanis.Hosting.Extensions._1Password`
   - Tests: `Arkanis.Hosting.Extensions._1Password.UnitTests` or `.IntegrationTests`

### Code Style from .editorconfig

#### File Settings
- **Charset**: UTF-8
- **Indent Style**: Spaces (4 spaces for C#, 2 for YAML/JSON)
- **End of Line**: LF (Unix-style)
- **Insert Final Newline**: true
- **Trim Trailing Whitespace**: true

#### C# Formatting Rules
1. **Var Usage**: Use `var` everywhere (enforced as warning):
   - For built-in types: `var count = 5;`
   - When type is apparent: `var builder = new StringBuilder();`
   - Elsewhere: `var result = GetResult();`

2. **Expression-Bodied Members**: Prefer expression bodies for:
   - Methods: `public int GetAge() => age;`
   - Constructors, operators, properties, indexers, accessors
   - Lambdas and local functions

3. **New Lines**:
   - Open braces on new line (Allman style)
   - `else`, `catch`, `finally` on new lines
   - Members in object/anonymous initializers on new lines

4. **Modifiers**:
   - Always specify accessibility modifiers (warning level)
   - Order: `public, private, protected, internal, static, extern, new, virtual, abstract, sealed, override, readonly, unsafe, volatile, async`

5. **Spacing**:
   - After keywords in control flow
   - Around binary operators (before and after)
   - After commas, semicolons in for statements
   - Before colons in inheritance clauses

6. **Pattern Matching**: Prefer modern patterns:
   - Switch expressions over switch statements
   - Pattern matching over `is` with cast check
   - Pattern matching over `as` with null check
   - `not` pattern over negation (when using C# 9+, not in library)

7. **Preferences**:
   - Use `throw` expressions where appropriate
   - Use conditional delegate calls (`?.Invoke()`)
   - Use object/collection initializers
   - Use auto-properties
   - Prefer simplified boolean expressions
   - Prefer compound assignments
   - Prefer braces for all control structures

8. **Line Length**:
   - ReSharper max line length: 160 characters
   - Wrap if long with proper chopping rules

9. **Namespace Declarations**:
   - Library must use traditional braced namespaces (C# 8.0)
   - Tests can use file-scoped namespaces (C# 10+)

#### Naming Conventions
- **Namespaces, Classes, Enums, Structs, Delegates, Events, Methods, Properties**: PascalCase
- **Interfaces**: PascalCase with `I` prefix (e.g., `IOpCliInvoker`)
- **Generic Type Parameters**: PascalCase with `T` prefix
- **Parameters**: camelCase
- **Public/Protected Constant Fields**: PascalCase
- **Public/Protected Static Readonly Fields**: PascalCase
- **Other Public/Protected Fields**: DISALLOWED (error level)

### XML Documentation
All public APIs MUST have XML documentation with:
- `<summary>` tag describing the purpose
- `<param>` tags for each parameter
- `<returns>` tag if applicable
- `<exception>` tags for documented exceptions
- `<inheritdoc />` or `<inheritdoc cref="..." />` for overloads that share documentation

Mark all public APIs with `[PublicAPI]` attribute from JetBrains.Annotations.

### Async Patterns
- All secret resolution is asynchronous
- Use `Async` suffix on method names
- Provide both async and sync overloads (sync calls `.GetAwaiter().GetResult()`)
- Use `Task<T>` return types for async methods

### Testing
- **Unit Tests**: `tests/Hosting.Extensions.1Password.UnitTests/`
  - Target: .NET 10.0
  - Use NSubstitute for mocking (e.g., `IOpCliInvoker`)
  - Naming: `MethodName_Scenario_ExpectedBehavior`
  - Use Arrange-Act-Assert pattern

- **Integration Tests**: `tests/Hosting.Extensions.1Password.IntegrationTests/`
  - Target: .NET 10.0
  - Test actual CLI integration
  - Use `appsettings.json` (copied to output)
  - Require actual `op` CLI to be installed

- Both use xUnit test framework with:
  - `Microsoft.NET.Test.Sdk`
  - `xunit` and `xunit.runner.visualstudio`
  - `coverlet.collector` for coverage

## Project Structure
```
Hosting.Extensions.1Password/
├── .github/                    # GitHub specific files (workflows, copilot instructions)
├── artifacts/                  # Build outputs (bin/ and obj/) - DO NOT COMMIT
├── scripts/                    # Build and release automation (Bash scripts)
├── src/
│   ├── Demo/                   # Demo application (net10.0)
│   └── Hosting.Extensions.1Password/  # Main library (netstandard2.1)
├── test-package/               # Local test packages
├── tests/
│   ├── Hosting.Extensions.1Password.IntegrationTests/
│   └── Hosting.Extensions.1Password.UnitTests/
├── Directory.Build.props       # Shared MSBuild properties
├── Directory.Build.targets     # Shared MSBuild targets (custom clean)
├── Directory.Packages.props    # Central Package Management
├── global.json                 # SDK version lock
├── nuget.config                # NuGet configuration
├── .editorconfig               # Code style rules
└── *.slnx                      # Solution file
```

## Key Features to Maintain

1. **Secret Resolution**:
   - Detects `op://` references in `IConfiguration`
   - Resolves them via 1Password CLI using `op inject`
   - In-memory replacement only (never modifies files)

2. **Account Support**:
   - Optional `account` parameter for multi-account scenarios
   - Passed as `--account` flag to `op` CLI

3. **Error Handling**:
   - `failSilently` parameter controls exception behavior
   - When false (default): throws `OnePasswordException` with exit code and stderr
   - When true: silently returns without resolving on errors

4. **Testability**:
   - Uses `IOpCliInvoker` interface to allow mocking of CLI calls
   - Internal `InvokerFactory` property for test injection

5. **Async Operations**:
   - All secret resolution is asynchronous
   - Sync wrapper provided for convenience

## Common Patterns in This Codebase

### Namespace Declaration with CA1707 Suppression
```csharp
#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System;
    using System.Threading.Tasks;
    // ... code
}
```

### Public API with Documentation and Annotations
```csharp
/// <summary>
/// Uses 1Password to inject secrets into the application's configuration.
/// </summary>
/// <param name="builder">The host application builder.</param>
/// <param name="account">The 1Password account or sign-in address.</param>
/// <param name="failSilently">If true, failures will not throw exceptions.</param>
/// <returns>The host application builder for chaining.</returns>
/// <exception cref="OnePasswordException">Thrown when the 1Password CLI fails.</exception>
[PublicAPI]
public static async Task<IHostApplicationBuilder> Use1PasswordAsync(
    this IHostApplicationBuilder builder,
    string? account = null,
    bool failSilently = false)
{
    // Implementation
}
```

### CLI Invocation Pattern
```csharp
var invoker = InvokerFactory?.Invoke() ?? new OpCliInvoker();
var result = await invoker.InvokeAsync(account, opTemplate);

if (result.ExitCode != 0)
{
    if (failSilently)
    {
        return builder;
    }

    throw new OnePasswordException(
        "Failed to resolve secrets from 1Password. " +
        "Ensure the 1Password CLI ('op') is installed and you are authenticated.",
        result.ExitCode,
        result.StandardError);
}
```

### Configuration Access Pattern
```csharp
// Detect op:// prefixed values
if (section.Value?.StartsWith("op://", StringComparison.Ordinal) == true)
{
    // Mark for resolution
}

// Update configuration after resolution
section.Value = resolvedValue;
```

### Null Check Pattern (C# 8.0 compatible)
```csharp
// Use property pattern for null checks
if (account is { })
{
    arguments.Add("--account");
    arguments.Add(account);
}

// Or traditional null check
if (value != null && value.Length >= 2)
{
    // Process value
}
```

## When Making Changes

### Adding New Dependencies
1. Add `<PackageReference Include="PackageName" />` to `.csproj` WITHOUT `Version` attribute
2. Add `<PackageVersion Include="PackageName" Version="x.y.z" />` to `Directory.Packages.props`
3. Run `dotnet restore` to update `packages.lock.json`
4. Commit both `.csproj`, `Directory.Packages.props`, and `packages.lock.json`

### Adding New Features
1. Consider testability - use interfaces for external dependencies
2. Add unit tests for logic in `tests/Hosting.Extensions.1Password.UnitTests/`
3. Add integration tests if CLI interaction is involved
4. Update XML documentation for all public APIs
5. Ensure compatibility with .NET Standard 2.1 and C# 8.0 for library code
6. Mark public APIs with `[PublicAPI]` attribute
7. Update README.md with usage examples if applicable

### Editing Files
1. For library code: Ensure explicit `using` statements (no implicit usings)
2. For test code: Can use implicit usings, but verify what's available
3. Use C# 8.0 features only in library code
4. Follow .editorconfig rules (especially var usage, expression bodies)
5. Wrap namespaces with CA1707 pragma suppressions
6. Add XML documentation to public APIs

### Code Reviews
Before submitting changes, verify:
- [ ] No C# 9+ features in library code (records, init, top-level statements, `with` expressions, etc.)
- [ ] Nullable annotations are correct and complete
- [ ] XML documentation is complete for all public APIs
- [ ] Explicit `using` statements in library code
- [ ] Tests are added/updated appropriately
- [ ] `packages.lock.json` updated if dependencies changed
- [ ] Code follows .editorconfig rules (warnings treated as errors)
- [ ] Public APIs marked with `[PublicAPI]` attribute
- [ ] Line length under 160 characters where reasonable
- [ ] All warnings fixed (TreatWarningsAsErrors is enabled)

## Package Information
- **Package ID**: `Arkanis.Hosting.Extensions.1Password`
- **Assembly Name**: `Arkanis.Hosting.Extensions.1Password`
- **Description**: Allows you to use secrets from 1Password during development. Supports secrets set via `appsettings.json`, environment variable, or any other method compatible with `IConfiguration`. Automatically injects secrets from 1Password by finding all `IConfiguration` entries that contain a 1Password secret reference and resolves them by invoking the `op` CLI.
- **Repository**: https://github.com/ArkanisCorporation/Hosting.Extensions.1Password
- **License**: See LICENSE.md
- **Tags**: 1Password, Hosting, Extensions, Secrets
- **Icon**: icon.png (included in package)
- **README**: README.md (included in package)

## Important Notes

### Development Environment
- This library is designed for **development scenarios** where developers have 1Password CLI installed
- Production deployments should use appropriate secret management for the target environment
- The `op` CLI must be installed and authenticated for the library to function

### CLI Requirements
- Requires 1Password CLI (`op`) to be installed
- User must be authenticated (via `op signin`)
- Library invokes `op inject` with template containing all `op://` references

### Malformed Output Handling
- Currently, malformed output from `op` CLI is silently ignored (lines that don't parse correctly are skipped)
- Consider making this configurable or logging warnings in future improvements

### .NET Standard 2.1 Limitations
- Cannot use implicit usings
- Cannot use C# 9+ features (records, init-only properties, with expressions)
- Cannot use C# 10+ features (file-scoped namespaces, global usings)
- Cannot use C# 11+ features (raw string literals, required members)
- Highest supported LangVersion is 8.0

### Custom Build Targets
`Directory.Build.targets` includes:
- Custom clean logic that properly cleans `artifacts/` folder
- Workaround for rebuild to avoid race conditions
- EF Core migrations support (if needed in future)

## Common Issues and Solutions

### Issue: CA1707 Warning About Underscores
**Solution**: Wrap namespace with pragma directives:
```csharp
#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
```

### Issue: Package Version Conflicts
**Solution**:
1. Check `Directory.Packages.props` for centralized versions
2. Do NOT add `Version` attribute to `<PackageReference>` in `.csproj`
3. Run `dotnet restore` to update lock files

### Issue: Build Output in Wrong Location
**Solution**: Build system uses `UseArtifactsOutput=true`, all output goes to `artifacts/` folder, not `bin/obj` in project folder

### Issue: Using C# 9+ Feature in Library
**Solution**: Library targets netstandard2.1 with C# 8.0 maximum. Remove the feature and use C# 8.0 alternatives:
- Records → regular classes
- Init-only properties → constructor initialization or regular properties
- Top-level statements → traditional Program class with Main
- File-scoped namespaces → traditional braced namespaces
- `with` expressions → manual copying
- Target-typed `new` → explicit type specification

### Issue: Missing XML Documentation Warning
**Solution**: Add complete XML documentation with `<summary>`, `<param>`, `<returns>`, and `<exception>` tags as needed. All warnings are treated as errors.

## Scripts and Automation

The `scripts/` directory contains Bash scripts for release automation:
- `common.sh` - Shared functions
- `common-verify.sh` - Common verification steps
- `common-publish.sh` - Common publishing logic
- `release-10-verify.sh` - Pre-release verification
- `release-11-verify-nuget.sh` - NuGet package verification
- `release-20-prepare.sh` - Prepare release
- `release-30-publish.sh` - Publish release
- `release-31-publish-nuget.sh` - Publish to NuGet

These scripts are for maintainers and should not be modified without understanding the release process

