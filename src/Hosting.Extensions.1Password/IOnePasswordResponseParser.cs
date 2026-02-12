#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for parsing responses from the 1Password CLI.
    /// </summary>
    public interface IOnePasswordResponseParser
    {
        /// <summary>
        /// Creates a result template string from a key-value pair for use with the 1Password CLI.
        /// </summary>
        /// <param name="pair">The key-value pair to create a template for.</param>
        /// <returns>A formatted template string for the 1Password CLI.</returns>
        public string CreateResultTemplate(KeyValuePair<string, string> pair);

        /// <summary>
        /// Parses a result string from the 1Password CLI output.
        /// </summary>
        /// <param name="source">The output string from the 1Password CLI to parse.</param>
        /// <returns>A key-value pair parsed from the source, or null if parsing fails.</returns>
        /// <exception cref="OnePasswordResultException">
        /// Thrown when the source string cannot be parsed into a valid key-value pair and silent failure is not requested.
        /// </exception>
        public KeyValuePair<string, string>? ParseResult(string source);
    }
}
