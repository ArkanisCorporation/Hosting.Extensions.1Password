namespace Arkanis.Hosting.Extensions._1Password
{
    using JetBrains.Annotations;

    /// <summary>
    /// Configuration options for 1Password integration.
    /// </summary>
    public class OnePasswordOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordOptions"/> class with default values.
        /// </summary>
        public OnePasswordOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordOptions"/> class by copying values from another instance.
        /// </summary>
        /// <param name="options">The options instance to copy from.</param>
        [PublicAPI]
        public OnePasswordOptions(OnePasswordOptions options)
        {
            CliInvoker = options.CliInvoker;
            ResponseParser = options.ResponseParser;
            ConfigurationSectionItemSchema = options.ConfigurationSectionItemSchema;
            Account = options.Account;
            FailSilently = options.FailSilently;
        }

        /// <summary>
        /// Gets or sets the CLI invoker to use for executing 1Password CLI commands.
        /// </summary>
        public IOpCliInvoker? CliInvoker { get; set; }

        /// <summary>
        /// Gets or sets the response parser to use for parsing 1Password CLI output.
        /// </summary>
        public IOnePasswordResponseParser? ResponseParser { get; set; }

        /// <summary>
        /// Gets or sets the schema prefix used to identify 1Password secret references in configuration (default: "op://").
        /// </summary>
        public string ConfigurationSectionItemSchema { get; set; } = "op://";

        /// <summary>
        /// Gets or sets the 1Password account or sign-in address to use.
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether failures should be silently ignored.
        /// </summary>
        public bool FailSilently { get; set; }
    }
}
