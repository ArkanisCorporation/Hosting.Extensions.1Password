namespace Arkanis.Hosting.Extensions._1Password
{
    using System.Threading.Tasks;
    using CliWrap.Buffered;

    /// <summary>
    /// Interface for invoking the 1Password CLI.
    /// </summary>
    public interface IOpCliInvoker
    {
        /// <summary>
        /// Invokes the 1Password CLI to inject secrets into the provided template.
        /// </summary>
        /// <param name="account">The 1Password account or sign-in address to pass to the `op` CLI.</param>
        /// <param name="opTemplate">The template containing 1Password secret references to inject.</param>
        /// <returns>The buffered result from executing the CLI command.</returns>
        public Task<BufferedCommandResult> InvokeAsync(string? account, string opTemplate);
    }
}
