namespace Arkanis.Hosting.Extensions._1Password.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap.Buffered;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Shared test helper classes for unit tests.
/// </summary>
internal static class TestHelpers
{
    /// <summary>
    /// Creates a fake host application builder for testing.
    /// </summary>
    /// <param name="values">The configuration values to initialize with.</param>
    /// <returns>A fake host application builder.</returns>
    public static FakeHostApplicationBuilder CreateHostApplicationBuilder(IDictionary<string, string?> values)
        => new FakeHostApplicationBuilder(values);
}

/// <summary>
/// Fake implementation of <see cref="IHostApplicationBuilder"/> for testing.
/// </summary>
internal sealed class FakeHostApplicationBuilder : IHostApplicationBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeHostApplicationBuilder"/> class.
    /// </summary>
    /// <param name="values">The configuration values to initialize with.</param>
    public FakeHostApplicationBuilder(IDictionary<string, string?> values)
    {
        var builder = new ConfigurationManager();
        builder.AddInMemoryCollection(values);

        Configuration = builder;
        Properties = new Dictionary<object, object>();

        Services = new ServiceCollection();

        var loggingBuilder = new FakeLoggingBuilder();
        Logging = loggingBuilder;

        var metricsBuilder = new FakeMetricsBuilder();
        Metrics = metricsBuilder;
    }

    /// <inheritdoc />
    public IMetricsBuilder Metrics { get; }

    /// <inheritdoc />
    public ILoggingBuilder Logging { get; }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public IHostEnvironment Environment
        => throw new NotImplementedException();

    /// <inheritdoc />
    public IDictionary<object, object> Properties { get; }

    /// <inheritdoc />
    public IConfigurationManager Configuration { get; }

    /// <inheritdoc />
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null)
        where TContainerBuilder : notnull
        => throw new NotImplementedException();
}

/// <summary>
/// Fake implementation of <see cref="ILoggingBuilder"/> for testing.
/// </summary>
internal sealed class FakeLoggingBuilder : ILoggingBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; } = new ServiceCollection();
}

/// <summary>
/// Fake implementation of <see cref="IMetricsBuilder"/> for testing.
/// </summary>
internal sealed class FakeMetricsBuilder : IMetricsBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; } = new ServiceCollection();
}

/// <summary>
/// Fake implementation of <see cref="IOpCliInvoker"/> for testing.
/// </summary>
internal sealed class FakeOpCliInvoker : IOpCliInvoker
{
    private readonly string _stdout;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeOpCliInvoker"/> class.
    /// </summary>
    /// <param name="stdout">The standard output to return.</param>
    public FakeOpCliInvoker(string stdout)
        => _stdout = stdout;

    /// <summary>
    /// Gets the account parameter that was passed to the last invocation.
    /// </summary>
    public string? CapturedAccount { get; private set; }

    /// <inheritdoc />
    public Task<BufferedCommandResult> InvokeAsync(string? account, string opTemplate)
    {
        CapturedAccount = account;

        var result = new BufferedCommandResult(
            0, // exitCode
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            _stdout,
            string.Empty
        );

        return Task.FromResult(result);
    }
}

/// <summary>
/// Fake implementation of <see cref="IOpCliInvoker"/> that returns an error.
/// </summary>
internal sealed class FakeOpCliInvokerWithError : IOpCliInvoker
{
    private readonly int _exitCode;
    private readonly string _stderr;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeOpCliInvokerWithError"/> class.
    /// </summary>
    /// <param name="exitCode">The exit code to return.</param>
    /// <param name="stderr">The standard error to return.</param>
    public FakeOpCliInvokerWithError(int exitCode, string stderr)
    {
        _exitCode = exitCode;
        _stderr = stderr;
    }

    /// <inheritdoc />
    public Task<BufferedCommandResult> InvokeAsync(string? account, string opTemplate)
    {
        var result = new BufferedCommandResult(
            _exitCode,
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            string.Empty,
            _stderr
        );

        return Task.FromResult(result);
    }
}

/// <summary>
/// Fake implementation of <see cref="IOpCliInvoker"/> that counts invocations.
/// </summary>
internal sealed class FakeOpCliInvokerWithCounter : IOpCliInvoker
{
    private readonly string _stdout;
    private readonly Action _onInvoke;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeOpCliInvokerWithCounter"/> class.
    /// </summary>
    /// <param name="stdout">The standard output to return.</param>
    /// <param name="onInvoke">Action to invoke when the CLI is invoked.</param>
    public FakeOpCliInvokerWithCounter(string stdout, Action onInvoke)
    {
        _stdout = stdout;
        _onInvoke = onInvoke;
    }

    /// <inheritdoc />
    public Task<BufferedCommandResult> InvokeAsync(string? account, string opTemplate)
    {
        _onInvoke();

        var result = new BufferedCommandResult(
            0,
            DateTimeOffset.Now,
            DateTimeOffset.Now,
            _stdout,
            string.Empty
        );

        return Task.FromResult(result);
    }
}
