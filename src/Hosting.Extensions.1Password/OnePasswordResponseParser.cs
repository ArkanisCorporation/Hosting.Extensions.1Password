namespace Arkanis.Hosting.Extensions._1Password
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Default implementation of <see cref="IOnePasswordResponseParser"/> for parsing 1Password CLI responses.
    /// </summary>
    public class OnePasswordResponseParser : IOnePasswordResponseParser
    {
        private readonly OnePasswordOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordResponseParser"/> class.
        /// </summary>
        /// <param name="options">The options to use for parsing.</param>
        public OnePasswordResponseParser(OnePasswordOptions options)
            => _options = options;

        /// <summary>
        /// Gets the separator character used between keys and values in the result format.
        /// </summary>
        protected const char Separator = '=';

        /// <summary>
        /// Creates a result template string from a key-value pair for use with the 1Password CLI.
        /// </summary>
        /// <param name="pair">The key-value pair to create a template for.</param>
        /// <returns>A formatted template string (format: "key="value"").</returns>
        public virtual string CreateResultTemplate(KeyValuePair<string, string> pair)
            => $"{pair.Key}{Separator}\"{pair.Value}\"";

        /// <summary>
        /// Parses a result string from the 1Password CLI output.
        /// </summary>
        /// <param name="source">The output string from the 1Password CLI to parse.</param>
        /// <returns>A key-value pair parsed from the source, or null if parsing fails silently.</returns>
        /// <exception cref="OnePasswordResultException">Thrown when the output format is invalid and silent failure is disabled.</exception>
        public KeyValuePair<string, string>? ParseResult(string source)
        {
            // Split into max 2 parts to handle values containing "="
            var sourceSpan = source.AsSpan();
            var separatorIndex = sourceSpan.IndexOf(Separator);
            if (separatorIndex == -1)
            {
                if (_options.FailSilently)
                {
                    // Skip malformed lines when silent failure is enabled
                    return null;
                }

                var expectedFormat = CreateResultTemplate(KeyValuePair.Create<string, string>("key", "value"));
                throw new OnePasswordResultException($"Received malformed output from 1Password CLI. Expected '{expectedFormat}' format but got: {source}");
            }

            var key = sourceSpan[..separatorIndex];
            var value = sourceSpan[(separatorIndex + 1)..];

            // Remove surrounding quotes if present
            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            {
                value = value[1..^1];
            }

            return KeyValuePair.Create(key.ToString(), value.ToString());
        }
    }
}
