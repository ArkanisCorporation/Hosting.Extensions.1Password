#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;


    internal static class OnePasswordHelper
    {
        /// <summary>
        ///     Internal factory for creating IOpCliInvoker instances. Used for testing.
        /// </summary>
        internal static Func<IOpCliInvoker>? InvokerFactory { get; set; }

        public static async Task Replace1PasswordConfigurationItems(Dictionary<string, IConfigurationSection> opSections, OnePasswordOptions options)
        {
            var parser = options.ResponseParser ?? new OnePasswordResponseParser(options);
            var opTemplate = string.Join(
                "\n",
                opSections.Values
                    .Select(section => KeyValuePair.Create(section.Path, section.Value ?? string.Empty))
                    .Select(parser.CreateResultTemplate)
            );

            var invoker = options.CliInvoker ?? InvokerFactory?.Invoke() ?? new OpCliInvoker();
            var result = await invoker.InvokeAsync(options.Account, opTemplate);

            if (!result.IsSuccess)
            {
                if (options.FailSilently)
                {
                    return;
                }

                throw new OnePasswordCliException(
                    "Failed to resolve secrets from 1Password. "
                    + "Ensure the 1Password CLI ('op') is installed and you are authenticated. "
                    + "Run 'op signin' to authenticate or check 'op --version' to verify installation."
                    + $"\nSTDERR:\n{result.StandardError}",
                    result.ExitCode,
                    result.StandardError
                );
            }

            foreach (var line in result.StandardOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!(parser.ParseResult(line) is { } pair))
                {
                    continue;
                }

                // Only update if the key exists in our sections dictionary
                if (opSections.TryGetValue(pair.Key, out var section))
                {
                    section.Value = pair.Value;
                }
            }
        }

        public static Dictionary<string, IConfigurationSection> GetOpConfigurationSections(
            IConfigurationManager configurationManager,
            OnePasswordOptions options
        )
        {
            var sections = new Queue<IConfigurationSection>(configurationManager.GetChildren());
            var opSections = new Dictionary<string, IConfigurationSection>();

            while (sections.TryDequeue(out var section))
            {
                if (section.Value is { } s && s.StartsWith(options.ConfigurationSectionItemSchema, StringComparison.OrdinalIgnoreCase))
                {
                    opSections.Add(section.Path, section);
                }
                else
                {
                    foreach (var child in section.GetChildren())
                    {
                        sections.Enqueue(child);
                    }
                }
            }

            return opSections;
        }
    }
}
