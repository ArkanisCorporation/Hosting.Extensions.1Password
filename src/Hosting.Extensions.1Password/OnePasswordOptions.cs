#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    public class OnePasswordOptions
    {
        public static readonly OnePasswordOptions Default = new OnePasswordOptions();

        public OnePasswordOptions()
        {
        }

        public OnePasswordOptions(OnePasswordOptions options)
        {
            CliInvoker = options.CliInvoker;
            ResponseParser = options.ResponseParser;
            ConfigurationSectionItemSchema = options.ConfigurationSectionItemSchema;
            Account = options.Account;
            FailSilently = options.FailSilently;
        }

        public IOpCliInvoker? CliInvoker { get; set; }
        public IOnePasswordResponseParser? ResponseParser { get; set; }

        public string ConfigurationSectionItemSchema { get; set; } = "op://";
        public string? Account { get; set; }
        public bool FailSilently { get; set; }
    }
}
