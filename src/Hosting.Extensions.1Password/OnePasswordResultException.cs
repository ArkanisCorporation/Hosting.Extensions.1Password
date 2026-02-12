#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Exception thrown when the 1Password CLI returns a malformed result.
    /// </summary>
    [PublicAPI]
    public class OnePasswordResultException : OnePasswordException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordResultException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OnePasswordResultException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePasswordResultException"/> class with the specified message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public OnePasswordResultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
