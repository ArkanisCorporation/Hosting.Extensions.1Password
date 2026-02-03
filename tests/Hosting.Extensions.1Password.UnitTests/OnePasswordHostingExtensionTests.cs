namespace Arkanis.Hosting.Extensions._1Password.UnitTests;

using System;

/// <summary>
/// Unit tests for <see cref="OnePasswordHostingExtension"/>.
/// </summary>
[Collection("OnePasswordTests")]
public class OnePasswordHostingExtensionTests : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnePasswordHostingExtensionTests"/> class.
    /// </summary>
    public OnePasswordHostingExtensionTests()
        // Clean state before each test
        => OnePasswordHostingExtension.InvokerFactory = null;

    /// <summary>
    /// Cleans up after each test by resetting the invoker factory.
    /// </summary>
    public void Dispose()
    {
        OnePasswordHostingExtension.InvokerFactory = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that UseOnePasswordAsync replaces op:// values with CLI output.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_ReplacesOpValues_WithCliOutput()
    {
        // Arrange
        var config = new Dictionary<string, string?>
        {
            ["SecretA"] = "op://itemA",
            ["SecretB"] = "op://itemB",
        };

        var builder = new FakeHostApplicationBuilder(config);

        var fakeInvoker = new FakeOpCliInvoker("SecretA=\"valueA\"\nSecretB=\"valueB\"");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"valueA", builder.Configuration.GetSection("SecretA").Value);
        Assert.Equal((string?)"valueB", builder.Configuration.GetSection("SecretB").Value);
    }

    /// <summary>
    /// Tests that UseOnePasswordAsync does nothing when there are no op:// entries.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_NoOpEntries_DoesNothing()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["Normal"] = "plain" };

        var builder = new FakeHostApplicationBuilder(config);
        var fakeInvoker = new FakeOpCliInvoker(string.Empty);
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"plain", builder.Configuration.GetSection("Normal").Value);
    }

    /// <summary>
    /// Tests that the account parameter is correctly passed to the invoker.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_Account_IsPassedToInvoker()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["SecretX"] = "op://x" };

        var builder = new FakeHostApplicationBuilder(config);
        var captured = new FakeOpCliInvoker(string.Empty);
        OnePasswordHostingExtension.InvokerFactory = () => captured;

        // Act
        await builder.Use1PasswordAsync(account: "acct-1");

        // Assert
        Assert.Equal((string?)"acct-1", captured.CapturedAccount);
    }

    /// <summary>
    /// Tests that quoted values are properly unquoted.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_QuotedValue_IsUnquoted()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["SecretQ"] = "op://q" };

        var builder = new FakeHostApplicationBuilder(config);
        var fakeInvoker = new FakeOpCliInvoker("SecretQ=\"quoted\"");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"quoted", builder.Configuration.GetSection("SecretQ").Value);
    }

    /// <summary>
    /// Tests that malformed output throws an exception by default.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_MalformedOutput_ThrowsException()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["SecretM"] = "op://m" };

        var builder = new FakeHostApplicationBuilder(config);
        var fakeInvoker = new FakeOpCliInvoker("malformed-line-without-equals");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnePasswordException>(
            async () => await builder.Use1PasswordAsync()
        );

        Assert.Contains("malformed output", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("malformed-line-without-equals", exception.Message);
    }

    /// <summary>
    /// Tests that malformed output is silently skipped when failSilently is true.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_MalformedOutputWithFailSilently_SkipsSilently()
    {
        // Arrange
        var config = new Dictionary<string, string?>
        {
            ["SecretM"] = "op://m",
            ["SecretN"] = "op://n",
        };

        var builder = new FakeHostApplicationBuilder(config);
        // Mix valid and malformed output
        var fakeInvoker = new FakeOpCliInvoker("SecretM=\"validValue\"\nmalformed-line\nSecretN=\"anotherValid\"");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act
        await builder.Use1PasswordAsync(failSilently: true);

        // Assert - only valid lines should be processed
        Assert.Equal((string?)"validValue", builder.Configuration.GetSection("SecretM").Value);
        Assert.Equal((string?)"anotherValid", builder.Configuration.GetSection("SecretN").Value);
    }

    /// <summary>
    /// Tests that CLI errors throw an exception by default.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_CliError_ThrowsException()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["SecretE"] = "op://e" };

        var builder = new FakeHostApplicationBuilder(config);
        var fakeInvoker = new FakeOpCliInvokerWithError(exitCode: 1, stderr: "CLI error message");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnePasswordException>(
            async () => await builder.Use1PasswordAsync()
        );

        Assert.Equal(1, exception.ExitCode);
        Assert.Equal((string?)"CLI error message", exception.StandardError);
    }

    /// <summary>
    /// Tests that CLI errors are silently ignored when failSilently is true.
    /// </summary>
    [Fact]
    public async Task UseOnePasswordAsync_CliErrorWithFailSilently_DoesNotThrow()
    {
        // Arrange
        var config = new Dictionary<string, string?> { ["SecretE"] = "op://e" };

        var builder = new FakeHostApplicationBuilder(config);
        var fakeInvoker = new FakeOpCliInvokerWithError(exitCode: 1, stderr: "CLI error message");
        OnePasswordHostingExtension.InvokerFactory = () => fakeInvoker;

        // Act
        await builder.Use1PasswordAsync(failSilently: true);

        // Assert - op:// reference should remain unchanged
        Assert.Equal((string?)"op://e", builder.Configuration.GetSection("SecretE").Value);
    }
}
