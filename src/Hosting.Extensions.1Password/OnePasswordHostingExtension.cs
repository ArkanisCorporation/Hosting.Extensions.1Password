#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension methods for integrating 1Password with IHostApplicationBuilder.
    /// </summary>
    public static class OnePasswordHostingExtension
    {
        private static readonly char[] EqualsSeparator = { '=' };

        /// <summary>
        /// Internal factory for creating IOpCliInvoker instances. Used for testing.
        /// </summary>
        internal static Func<IOpCliInvoker>? InvokerFactory { get; set; }

        /// <summary>
        /// Uses 1Password to inject secrets into the application's configuration.
        /// Automatically detects configuration entries that reference 1Password secrets
        /// using the "op://" URI scheme and replaces them with the actual secret values.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
        /// <param name="failSilently">If true, CLI failures and malformed output will be silently ignored and op:// references remain unchanged. If false (default), throws <see cref="OnePasswordException"/> on CLI errors or malformed output.</param>
        /// <returns>The host application builder for chaining.</returns>
        /// <exception cref="OnePasswordException">Thrown when the 1Password CLI fails or returns malformed output, and <paramref name="failSilently"/> is false.</exception>
        [PublicAPI]
        public static async Task<IHostApplicationBuilder> Use1PasswordAsync(this IHostApplicationBuilder builder, string? account = null, bool failSilently = false)
        {
            var opSections = GetOpConfigurationSections(builder);
            if (opSections.Count == 0)
            {
                return builder;
            }

            var opTemplate = string.Join(
                "\n",
                opSections.Values
                    .Select(section => $"{section.Key}=\"{section.Value}\"")
            );

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
                    "Ensure the 1Password CLI ('op') is installed and you are authenticated. " +
                    "Run 'op signin' to authenticate or check 'op --version' to verify installation.",
                    result.ExitCode,
                    result.StandardError);
            }

            foreach (var line in result.StandardOutput.Split("\n").Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                // Split into max 2 parts to handle values containing "="
                var parts = line.Split(EqualsSeparator, 2);
                if (parts.Length != 2)
                {
                    if (failSilently)
                    {
                        // Skip malformed lines when silent failure is enabled
                        continue;
                    }

                    throw new OnePasswordException(
                        $"Received malformed output from 1Password CLI. Expected 'key=\"value\"' format but got: {line}",
                        result.ExitCode,
                        result.StandardError);
                }

                var key = parts[0];
                var value = parts[1];

                // Remove surrounding quotes if present
                if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
                {
                    value = value[1..^1];
                }

                // Only update if the key exists in our sections dictionary
                if (opSections.TryGetValue(key, out var section))
                {
                    section.Value = value;
                }
            }

            return builder;
        }

        /// <inheritdoc cref="Use1PasswordAsync" />
        [PublicAPI]
        public static IHostApplicationBuilder Use1Password(this IHostApplicationBuilder builder, string? account = null, bool failSilently = false)
            => builder.Use1PasswordAsync(account: account, failSilently: failSilently).GetAwaiter().GetResult();

        private static Dictionary<string, IConfigurationSection> GetOpConfigurationSections(
            IHostApplicationBuilder builder
        )
        {
            var sections = new Queue<IConfigurationSection>(builder.Configuration.GetChildren());
            var opSections = new Dictionary<string, IConfigurationSection>();

            while (sections.TryDequeue(out var section))
            {
                if (section.Value is { } s && s.StartsWith("op://", StringComparison.OrdinalIgnoreCase))
                {
                    opSections.Add(section.Key, section);
                }
                else
                {
                    foreach (var child in section.GetChildren())
                    {
                        sections.Enqueue(child);
                    }
                }
            }

            return opSections;
        }
    }
}
