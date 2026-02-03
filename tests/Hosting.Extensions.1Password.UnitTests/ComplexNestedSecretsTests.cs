namespace Arkanis.Hosting.Extensions._1Password.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Tests for resolving secrets in complex configuration structures with multiple nesting levels.
/// </summary>
[Collection("OnePasswordTests")]
public class ComplexNestedSecretsTests : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexNestedSecretsTests"/> class.
    /// </summary>
    public ComplexNestedSecretsTests()
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
    /// Tests that secrets in deeply nested configuration sections are resolved correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_DeeplyNestedSecrets_ResolvesAllLevels()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["Level1:Level2:Level3:Level4:Secret"] = "op://vault/item/field1",
            ["Level1:Level2:Level3:OtherValue"] = "plain-value",
            ["Level1:Level2:AnotherSecret"] = "op://vault/item/field2",
            ["Level1:NonSecret"] = "plain-value-2"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "Secret=\"resolved-secret-1\"\n" +
            "AnotherSecret=\"resolved-secret-2\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"resolved-secret-1", builder.Configuration.GetSection("Level1:Level2:Level3:Level4:Secret").Value);
        Assert.Equal((string?)"plain-value", builder.Configuration.GetSection("Level1:Level2:Level3:OtherValue").Value);
        Assert.Equal((string?)"resolved-secret-2", builder.Configuration.GetSection("Level1:Level2:AnotherSecret").Value);
        Assert.Equal((string?)"plain-value-2", builder.Configuration.GetSection("Level1:NonSecret").Value);
    }

    /// <summary>
    /// Tests that secrets in array configuration sections are resolved correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_ArrayWithSecrets_ResolvesAllElements()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Primary"] = "op://vault/db1/connection",
            ["ConnectionStrings:Backup"] = "plain-connection-string",
            ["ConnectionStrings:Secondary"] = "op://vault/db2/connection",
            ["ConnectionStrings:Tertiary"] = "op://vault/db3/connection"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "Primary=\"Server=db1;User=user1\"\n" +
            "Secondary=\"Server=db2;User=user2\"\n" +
            "Tertiary=\"Server=db3;User=user3\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"Server=db1;User=user1", builder.Configuration.GetSection("ConnectionStrings:Primary").Value);
        Assert.Equal((string?)"plain-connection-string", builder.Configuration.GetSection("ConnectionStrings:Backup").Value);
        Assert.Equal((string?)"Server=db2;User=user2", builder.Configuration.GetSection("ConnectionStrings:Secondary").Value);
        Assert.Equal((string?)"Server=db3;User=user3", builder.Configuration.GetSection("ConnectionStrings:Tertiary").Value);
    }

    /// <summary>
    /// Tests that secrets in nested objects with mixed content are resolved correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_NestedObjectsWithMixedContent_ResolvesSecretsOnly()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["Services:Database:Host"] = "localhost",
            ["Services:Database:Port"] = "5432",
            ["Services:Database:Username"] = "op://vault/db/username",
            ["Services:Database:Password"] = "op://vault/db/password",
            ["Services:Api:BaseUrl"] = "https://api.example.com",
            ["Services:Api:ApiKey"] = "op://vault/api/key",
            ["Services:Cache:Type"] = "Redis",
            ["Services:Cache:ConnectionString"] = "op://vault/redis/connection"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "Username=\"dbuser\"\n" +
            "Password=\"dbpass123\"\n" +
            "ApiKey=\"apikey456\"\n" +
            "ConnectionString=\"redis://localhost:6379\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"localhost", builder.Configuration.GetSection("Services:Database:Host").Value);
        Assert.Equal((string?)"5432", builder.Configuration.GetSection("Services:Database:Port").Value);
        Assert.Equal((string?)"dbuser", builder.Configuration.GetSection("Services:Database:Username").Value);
        Assert.Equal((string?)"dbpass123", builder.Configuration.GetSection("Services:Database:Password").Value);
        Assert.Equal((string?)"https://api.example.com", builder.Configuration.GetSection("Services:Api:BaseUrl").Value);
        Assert.Equal((string?)"apikey456", builder.Configuration.GetSection("Services:Api:ApiKey").Value);
        Assert.Equal((string?)"Redis", builder.Configuration.GetSection("Services:Cache:Type").Value);
        Assert.Equal((string?)"redis://localhost:6379", builder.Configuration.GetSection("Services:Cache:ConnectionString").Value);
    }

    /// <summary>
    /// Tests that secrets in complex nested arrays with objects are resolved correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_NestedArraysWithObjects_ResolvesAllSecrets()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["Endpoints:0:Name"] = "Service1",
            ["Endpoints:0:Url"] = "https://service1.example.com",
            ["Endpoints:0:ApiKey"] = "op://vault/service1/apikey",
            ["Endpoints:1:Name"] = "Service2",
            ["Endpoints:1:Url"] = "https://service2.example.com",
            ["Endpoints:1:Token"] = "op://vault/service2/token",
            ["Endpoints:1:Secret"] = "op://vault/service2/secret"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "ApiKey=\"key123\"\n" +
            "Token=\"token456\"\n" +
            "Secret=\"secret789\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"Service1", builder.Configuration.GetSection("Endpoints:0:Name").Value);
        Assert.Equal((string?)"https://service1.example.com", builder.Configuration.GetSection("Endpoints:0:Url").Value);
        Assert.Equal((string?)"key123", builder.Configuration.GetSection("Endpoints:0:ApiKey").Value);
        Assert.Equal((string?)"Service2", builder.Configuration.GetSection("Endpoints:1:Name").Value);
        Assert.Equal((string?)"https://service2.example.com", builder.Configuration.GetSection("Endpoints:1:Url").Value);
        Assert.Equal((string?)"token456", builder.Configuration.GetSection("Endpoints:1:Token").Value);
        Assert.Equal((string?)"secret789", builder.Configuration.GetSection("Endpoints:1:Secret").Value);
    }

    /// <summary>
    /// Tests that secrets with underscores in nested paths are resolved correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_NestedPathsWithUnderscores_ResolvesCorrectly()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["AppSettings:Feature_Alpha:AlphaKey"] = "op://vault/alpha/key",
            ["AppSettings:Feature_Beta:BetaSecret"] = "op://vault/beta/secret",
            ["AppSettings:Feature_Beta:NestedValue"] = "plain-value"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "AlphaKey=\"alphakey123\"\n" +
            "BetaSecret=\"betasecret456\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"alphakey123", builder.Configuration.GetSection("AppSettings:Feature_Alpha:AlphaKey").Value);
        Assert.Equal((string?)"betasecret456", builder.Configuration.GetSection("AppSettings:Feature_Beta:BetaSecret").Value);
        Assert.Equal((string?)"plain-value", builder.Configuration.GetSection("AppSettings:Feature_Beta:NestedValue").Value);
    }

    /// <summary>
    /// Tests that configuration with extreme nesting depth is handled correctly.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_ExtremeNestingDepth_ResolvesSuccessfully()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["A:B:C:D:E:F:G:H:I:J:DeepSecret"] = "op://vault/deep/secret",
            ["A:B:C:D:E:F:G:H:I:J:Plain"] = "plain-value"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var mockInvoker = new FakeOpCliInvoker(
            "DeepSecret=\"deep-secret-value\"");
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal((string?)"deep-secret-value", builder.Configuration.GetSection("A:B:C:D:E:F:G:H:I:J:DeepSecret").Value);
        Assert.Equal((string?)"plain-value", builder.Configuration.GetSection("A:B:C:D:E:F:G:H:I:J:Plain").Value);
    }

    /// <summary>
    /// Tests that multiple secrets across different nested sections are all resolved in a single call.
    /// </summary>
    [Fact]
    public async Task Use1PasswordAsync_MultipleNestedSections_SingleInvocation()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>
        {
            ["Section1:Nested:Secret1"] = "op://vault/item1/field1",
            ["Section2:Deep:Nested:Secret2"] = "op://vault/item2/field2",
            ["Section3:Secret3"] = "op://vault/item3/field3"
        };

        var builder = CreateHostApplicationBuilder(configuration);

        var invocationCount = 0;
        var mockInvoker = new FakeOpCliInvokerWithCounter(
            "Secret1=\"value1\"\n" +
            "Secret2=\"value2\"\n" +
            "Secret3=\"value3\"",
            () => invocationCount++);
        OnePasswordHostingExtension.InvokerFactory = () => mockInvoker;

        // Act
        await builder.Use1PasswordAsync();

        // Assert
        Assert.Equal(1, invocationCount);
        Assert.Equal((string?)"value1", builder.Configuration.GetSection("Section1:Nested:Secret1").Value);
        Assert.Equal((string?)"value2", builder.Configuration.GetSection("Section2:Deep:Nested:Secret2").Value);
        Assert.Equal((string?)"value3", builder.Configuration.GetSection("Section3:Secret3").Value);
    }

    private static FakeHostApplicationBuilder CreateHostApplicationBuilder(IDictionary<string, string?> values)
        => TestHelpers.CreateHostApplicationBuilder(values);
}
