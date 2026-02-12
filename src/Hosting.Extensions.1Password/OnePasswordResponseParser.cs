#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System.Collections.Generic;

    public class OnePasswordResponseParser : IOnePasswordResponseParser
    {
        private readonly OnePasswordOptions _options;

        public OnePasswordResponseParser(OnePasswordOptions options)
            => _options = options;

        protected const char Separator = '=';

        public virtual string CreateResultTemplate(KeyValuePair<string, string> pair)
            => $"{pair.Key}{Separator}\"{pair.Value}\"";

        public KeyValuePair<string, string>? ParseResult(string source)
        {
            // Split into max 2 parts to handle values containing "="
            var parts = source.Split(Separator, 2);
            if (parts.Length != 2)
            {
                if (_options.FailSilently)
                {
                    // Skip malformed lines when silent failure is enabled
                    return null;
                }

                var expectedFormat = CreateResultTemplate(KeyValuePair.Create<string, string>("key", "value"));
                throw new OnePasswordResultException($"Received malformed output from 1Password CLI. Expected '{expectedFormat}' format but got: {source}");
            }

            var key = parts[0];
            var value = parts[1];

            // Remove surrounding quotes if present
            if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            {
                value = value[1..^1];
            }

            return KeyValuePair.Create(key, value);
        }
    }
}
