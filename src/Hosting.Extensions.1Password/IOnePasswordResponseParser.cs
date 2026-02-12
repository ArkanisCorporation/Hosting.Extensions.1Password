#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System.Collections.Generic;

    public interface IOnePasswordResponseParser
    {
        public string CreateResultTemplate(KeyValuePair<string, string> pair);

        public KeyValuePair<string, string>? ParseResult(string source);
    }
}
