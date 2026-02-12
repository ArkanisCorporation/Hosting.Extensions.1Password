#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System;
    using JetBrains.Annotations;

    [PublicAPI]
    public class OnePasswordResultException : OnePasswordException
    {
        public OnePasswordResultException(string message)
            : base(message)
        {
        }

        public OnePasswordResultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
