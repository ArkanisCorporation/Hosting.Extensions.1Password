namespace Arkanis.Aspire.Hosting.Extensions._1Password;

using System;
using Arkanis.Hosting.Extensions._1Password;
using global::Aspire.Hosting.ApplicationModel;

/// <summary>
///     Stores the configured 1Password reference and schema for an Aspire parameter resource.
/// </summary>
/// <remarks>
///     The annotation lets publishing integrations inspect the parameter's configured source without
///     resolving the secret value during model construction.
/// </remarks>
public sealed class OnePasswordParameterReferenceAnnotation : IResourceAnnotation
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OnePasswordParameterReferenceAnnotation" /> class.
    /// </summary>
    /// <param name="reference">The configured 1Password secret reference.</param>
    /// <param name="configurationSectionItemSchema">The schema used to recognize 1Password references.</param>
    /// <exception cref="ArgumentException">
    ///     <paramref name="reference" /> or <paramref name="configurationSectionItemSchema" /> is empty or whitespace.
    /// </exception>
    public OnePasswordParameterReferenceAnnotation(string reference, string configurationSectionItemSchema)
    {
        Reference = Required(reference, nameof(reference));
        ConfigurationSectionItemSchema = Required(configurationSectionItemSchema, nameof(configurationSectionItemSchema));
    }

    /// <summary>Gets the trimmed configured 1Password secret reference.</summary>
    public string Reference { get; }

    /// <summary>Gets the trimmed schema used to recognize 1Password references.</summary>
    public string ConfigurationSectionItemSchema { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
