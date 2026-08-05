namespace Arkanis.Hosting.Extensions._1Password.UnitTests;

using Arkanis.Aspire.Hosting.Extensions._1Password;
using global::Aspire.Hosting;
using global::Aspire.Hosting.ApplicationModel;

/// <summary>
/// Unit tests for <see cref="OnePasswordAspireHostingExtension" />.
/// </summary>
public class OnePasswordAspireHostingExtensionTests
{
    /// <summary>
    /// Tests that source metadata is normalized before it is attached to a parameter resource.
    /// </summary>
    [Fact]
    public void OnePasswordParameterReferenceAnnotation_WithWhitespace_StoresTrimmedMetadata()
    {
        // Arrange
        var reference = " " + OnePasswordSecretReference.Prefix + "Shared/service/password ";
        const string schema = " op-company:// ";

        // Act
        var annotation = new OnePasswordParameterReferenceAnnotation(reference, schema);

        // Assert
        Assert.Equal(OnePasswordSecretReference.Prefix + "Shared/service/password", annotation.Reference);
        Assert.Equal("op-company://", annotation.ConfigurationSectionItemSchema);
    }

    /// <summary>
    /// Tests that source metadata rejects blank required values.
    /// </summary>
    [Theory]
    [InlineData("", "op-company://")]
    [InlineData("reference", " ")]
    public void OnePasswordParameterReferenceAnnotation_WithBlankRequiredValue_ThrowsArgumentException(
        string reference,
        string schema
    )
    {
        // Arrange
        var action = () => new OnePasswordParameterReferenceAnnotation(reference, schema);

        // Act
        var exception = Record.Exception(action);

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    /// <summary>
    /// Tests that an Aspire parameter records its configured 1Password source metadata.
    /// </summary>
    [Fact]
    public void Add1PasswordParameter_CapturesReferenceAndConfiguredSchema()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var reference = OnePasswordSecretReference.Prefix + "Shared/service/password";
        const string configuredSchema = "op-company://";

        // Act
        var parameter = builder.Add1PasswordParameter(
            "service-password",
            reference,
            configureOptions: options => options.ConfigurationSectionItemSchema = configuredSchema
        );

        // Assert
        var annotation = Assert.Single(parameter.Resource.Annotations.OfType<OnePasswordParameterReferenceAnnotation>());
        Assert.Equal(reference, annotation.Reference);
        Assert.Equal(configuredSchema, annotation.ConfigurationSectionItemSchema);
    }
}
