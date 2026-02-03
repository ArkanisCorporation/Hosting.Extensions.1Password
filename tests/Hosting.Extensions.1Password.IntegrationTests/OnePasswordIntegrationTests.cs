namespace Arkanis.Hosting.Extensions._1Password.IntegrationTests;

using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Integration tests for <see cref="OnePasswordHostingExtension"/>.
/// </summary>
public class OnePasswordIntegrationTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnePasswordIntegrationTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper.</param>
    public OnePasswordIntegrationTests(ITestOutputHelper output)
        => _output = output;

    /// <summary>
    /// Tests that UseOnePassword loads password from configuration in development environment.
    /// </summary>
    /// <remarks>
    /// This test requires:
    /// - The 1Password CLI (`op`) to be installed and available in PATH
    /// - The 1Password app to be running and authenticated
    /// - A secret at `op://Private/DummyCredential/password` in your vault
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Category", "RequiresOnePassword")]
    public void UseOnePassword_InDevelopmentEnvironment_LoadsPasswordFromConfiguration()
    {
        // Arrange
        var builder = new HostApplicationBuilder();

        // Act
        builder.Use1Password("my.1password.com");
        var password = builder.Configuration["Password"];

        // Assert
        Assert.NotNull(password);
        _output.WriteLine("My Password is: {0}", password);
    }

    /// <summary>
    /// Tests that UseOnePasswordAsync loads password from configuration in development environment.
    /// </summary>
    /// <remarks>
    /// This test requires:
    /// - The 1Password CLI (`op`) to be installed and available in PATH
    /// - The 1Password app to be running and authenticated
    /// - A secret at `op://Private/DummyCredential/password` in your vault
    /// </remarks>
    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Category", "RequiresOnePassword")]
    public async Task UseOnePasswordAsync_InDevelopmentEnvironment_LoadsPasswordFromConfiguration()
    {
        // Arrange
        var builder = new HostApplicationBuilder();

        // Act
        await builder.Use1PasswordAsync("my.1password.com");
        var password = builder.Configuration["Password"];

        // Assert
        Assert.NotNull(password);
        _output.WriteLine("My Password is: {0}", password);
    }
}
