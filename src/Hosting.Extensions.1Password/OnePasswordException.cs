namespace Arkanis.Hosting.Extensions._1Password
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Exception thrown when an operation with 1Password fails.
    /// </summary>
    /// <remarks>
    /// This is a general base exception for all errors related to 1Password operations.
    /// You can catch this exception to handle any 1Password-related errors in a generic way.
    /// </remarks>
    [PublicAPI]
    public class OnePasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OnePasswordException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordException"/> class with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public OnePasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
