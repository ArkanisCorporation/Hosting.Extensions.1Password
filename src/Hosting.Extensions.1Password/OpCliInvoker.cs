#pragma warning disable CA1707
namespace Arkanis.Hosting.Extensions._1Password
#pragma warning restore CA1707
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CliWrap;
    using CliWrap.Buffered;

    internal sealed class OpCliInvoker : IOpCliInvoker
    {
        public async Task<BufferedCommandResult> InvokeAsync(string? account, string opTemplate)
        {
            var arguments = new List<string> { "inject" };
            if (account is { })
            {
                arguments.Add("--account");
                arguments.Add(account);
            }

            var result = await Cli.Wrap("op")
                .WithStandardInputPipe(PipeSource.FromString(opTemplate))
                .WithArguments(arguments, true)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            return result;
        }
    }
}
