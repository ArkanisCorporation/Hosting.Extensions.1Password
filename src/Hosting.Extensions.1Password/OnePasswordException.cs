namespace Arkanis.Hosting.Extensions._1Password
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Exception thrown when 1Password CLI operations fail.
    /// </summary>
    [PublicAPI]
    public class OnePasswordException : Exception
    {
        /// <summary>
        /// Gets the exit code returned by the 1Password CLI.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// Gets the standard error output from the 1Password CLI.
        /// </summary>
        public string StandardError { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exitCode">The exit code from the 1Password CLI.</param>
        /// <param name="standardError">The standard error output from the CLI.</param>
        public OnePasswordException(string message, int exitCode, string standardError)
            : base(message)
        {
            ExitCode = exitCode;
            StandardError = standardError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exitCode">The exit code from the 1Password CLI.</param>
        /// <param name="standardError">The standard error output from the CLI.</param>
        /// <param name="innerException">The inner exception.</param>
        public OnePasswordException(string message, int exitCode, string standardError, Exception innerException)
            : base(message, innerException)
        {
            ExitCode = exitCode;
            StandardError = standardError;
        }
    }
}
