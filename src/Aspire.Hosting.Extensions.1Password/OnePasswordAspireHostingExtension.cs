namespace Arkanis.Aspire.Hosting.Extensions._1Password;

using System.Threading.Tasks;
using Arkanis.Hosting.Extensions._1Password;
using global::Aspire.Hosting;
using global::Aspire.Hosting.ApplicationModel;
using JetBrains.Annotations;

/// <summary>
/// Extension methods for integrating 1Password with IDistributedApplicationBuilder.
/// </summary>
public static class OnePasswordAspireHostingExtension
{
    /// <param name="builder">The distributed application builder.</param>
    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// Adds a parameter to the distributed application whose value is retrieved from 1Password using the specified 1Password key.
        /// </summary>
        /// <param name="key">The key of the parameter to add.</param>
        /// <param name="onePasswordKey">The 1Password key (e.g. "op://path/to/secret/field") to retrieve the value from.</param>
        /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
        /// <param name="failSilently">
        /// If true, CLI failures and malformed output will be silently ignored and the parameter value will be null.
        /// If false (default), throws <see cref="OnePasswordCliException"/> on CLI errors or malformed output.
        /// </param>
        /// <param name="secret">Optional flag indicating whether the parameter should be regarded as secret.</param>
        /// <param name="publishValueAsDefault">Indicates whether the value should be published to the manifest. This is not meant for sensitive data.</param>
        /// <exception cref="OnePasswordException">Thrown when the 1Password CLI fails or returns malformed output, and <paramref name="failSilently"/> is false.</exception>
        public IResourceBuilder<ParameterResource> Add1PasswordParameter(
            string key,
            string onePasswordKey,
            string? account = null,
            bool failSilently = false,
            bool secret = true,
            bool publishValueAsDefault = false
        )
            => builder.Add1PasswordParameter(
                key,
                onePasswordKey,
                publishValueAsDefault,
                secret,
                options =>
                {
                    options.Account = account;
                    options.FailSilently = failSilently;
                });

        /// <summary>
        /// Adds a parameter to the distributed application whose value is retrieved from 1Password using the specified 1Password key.
        /// </summary>
        /// <param name="key">The key of the parameter to add.</param>
        /// <param name="onePasswordKey">The 1Password key (e.g. "op://path/to/secret/field") to retrieve the value from.</param>
        /// <param name="configureOptions">An action to configure 1Password options such as account and error handling behavior.</param>
        /// <param name="secret">Optional flag indicating whether the parameter should be regarded as secret.</param>
        /// <param name="publishValueAsDefault">Indicates whether the value should be published to the manifest. This is not meant for sensitive data.</param>
        /// <exception cref="OnePasswordException">Thrown when the 1Password CLI fails or returns malformed output, and failSilently is false.</exception>
        public IResourceBuilder<ParameterResource> Add1PasswordParameter(
            string key,
            string onePasswordKey,
            bool secret = true,
            bool publishValueAsDefault = false,
            Action<OnePasswordOptions>? configureOptions = null
        )
        {
            var options = new OnePasswordOptions();
            configureOptions?.Invoke(options);

            return builder.AddParameter(key, ValueGetter, publishValueAsDefault, secret);

            string ValueGetter()
                => OnePasswordHelper.Resolve1PasswordItem(onePasswordKey, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Uses 1Password to inject secrets into the application's configuration.
        /// Automatically detects configuration entries that reference 1Password secrets
        /// using the "op://" URI scheme and replaces them with the actual secret values.
        /// </summary>
        /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
        /// <param name="failSilently">
        /// If true, CLI failures and malformed output will be silently ignored and op:// references remain unchanged.
        /// If false (default), throws <see cref="OnePasswordCliException"/> on CLI errors or malformed output.
        /// </param>
        /// <returns>The distributed application builder for chaining.</returns>
        /// <exception cref="OnePasswordException">
        /// Thrown when the 1Password CLI fails or returns malformed output, and <paramref name="failSilently"/> is false.
        /// </exception>
        [PublicAPI]
        public async Task<IDistributedApplicationBuilder> Use1PasswordAsync(string? account = null, bool failSilently = false)
            => await builder.Use1PasswordAsync(options =>
            {
                options.Account = account;
                options.FailSilently = failSilently;
            });

        /// <summary>
        /// Uses 1Password to inject secrets into the application's configuration.
        /// Automatically detects configuration entries that reference 1Password secrets
        /// using the "op://" URI scheme and replaces them with the actual secret values.
        /// </summary>
        /// <param name="configureOptions">An action to configure 1Password options such as account and error handling behavior.</param>
        /// <returns>The distributed application builder for chaining.</returns>
        /// <exception cref="OnePasswordException">Thrown when the 1Password CLI fails or returns malformed output, and failSilently is false.</exception>
        [PublicAPI]
        public async Task<IDistributedApplicationBuilder> Use1PasswordAsync(Action<OnePasswordOptions> configureOptions)
        {
            var options = new OnePasswordOptions();
            configureOptions(options);

            var opSections = OnePasswordHelper.GetOpConfigurationSections(builder.Configuration, options);
            if (opSections.Count == 0)
            {
                return builder;
            }

            await OnePasswordHelper.Replace1PasswordConfigurationItems(opSections, options);
            return builder;
        }

        /// <inheritdoc cref="Use1PasswordAsync(IDistributedApplicationBuilder,string,bool)" />
        [PublicAPI]
        public IDistributedApplicationBuilder Use1Password(string? account = null, bool failSilently = false)
            => builder.Use1PasswordAsync(account: account, failSilently: failSilently).GetAwaiter().GetResult();

        /// <inheritdoc cref="Use1PasswordAsync(IDistributedApplicationBuilder,Action{OnePasswordOptions})" />
        [PublicAPI]
        public IDistributedApplicationBuilder Use1Password(Action<OnePasswordOptions> configureOptions)
            => builder.Use1PasswordAsync(configureOptions: configureOptions).GetAwaiter().GetResult();
    }
}
