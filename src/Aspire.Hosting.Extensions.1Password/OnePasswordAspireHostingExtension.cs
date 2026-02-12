#pragma warning disable CA1707
namespace Arkanis.Aspire.Hosting.Extensions._1Password;
#pragma warning restore CA1707

using System.Threading.Tasks;
using Arkanis.Hosting.Extensions._1Password;
using global::Aspire.Hosting;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for integrating 1Password with IHostApplicationBuilder.
/// </summary>
public static class OnePasswordAspireHostingExtension
{
    /// <summary>
    /// Uses 1Password to inject secrets into the application's configuration.
    /// Automatically detects configuration entries that reference 1Password secrets
    /// using the "op://" URI scheme and replaces them with the actual secret values.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
    /// <param name="failSilently">
    /// If true, CLI failures and malformed output will be silently ignored and op:// references remain unchanged.
    /// If false (default), throws <see cref="OnePasswordCliException"/> on CLI errors or malformed output.
    /// </param>
    /// <returns>The distributed application builder for chaining.</returns>
    /// <exception cref="OnePasswordCliException">Thrown when the 1Password CLI fails or returns malformed output, and <paramref name="failSilently"/> is false.</exception>
    [PublicAPI]
    public static async Task<IDistributedApplicationBuilder> Use1PasswordAsync(this IDistributedApplicationBuilder builder, string? account = null, bool failSilently = false)
    {
        var options = new OnePasswordOptions
        {
            Account = account,
            FailSilently = failSilently,
        };

        return await builder.Use1PasswordAsync(options);
    }

    /// <summary>
    /// Uses 1Password to inject secrets into the application's configuration.
    /// Automatically detects configuration entries that reference 1Password secrets
    /// using the "op://" URI scheme and replaces them with the actual secret values.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="options">The options for configuring 1Password integration, including account and failure behaviour.</param>
    /// <returns>The distributed application builder for chaining.</returns>
    [PublicAPI]
    public static async Task<IDistributedApplicationBuilder> Use1PasswordAsync(this IDistributedApplicationBuilder builder, OnePasswordOptions options)
    {
        var opSections = OnePasswordHelper.GetOpConfigurationSections(builder.Configuration, options);
        if (opSections.Count == 0)
        {
            return builder;
        }

        await OnePasswordHelper.Replace1PasswordConfigurationItems(opSections, options);
        return builder;
    }

    /// <summary>
    /// Uses 1Password to inject secrets into the application's configuration.
    /// Automatically detects configuration entries that reference 1Password secrets
    /// using the "op://" URI scheme and replaces them with the actual secret values.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
    /// <param name="failSilently">
    /// If true, CLI failures and malformed output will be silently ignored and op:// references remain unchanged.
    /// If false (default), throws <see cref="OnePasswordCliException"/> on CLI errors or malformed output.
    /// </param>
    /// <returns>The distributed application builder for chaining.</returns>
    /// <exception cref="OnePasswordCliException">Thrown when the 1Password CLI fails or returns malformed output, and <paramref name="failSilently"/> is false.</exception>
    [PublicAPI]
    public static IHostApplicationBuilder Use1Password(this IHostApplicationBuilder builder, string? account = null, bool failSilently = false)
        => builder.Use1PasswordAsync(account: account, failSilently: failSilently).GetAwaiter().GetResult();
}
