using Arkanis.Aspire.Hosting.Extensions._1Password;

var builder = DistributedApplication.CreateBuilder(args);

// bulk in-place secret resolution
await builder.Use1PasswordAsync("my.1password.com");
builder.AddParameter("ConnectionString", secret: true);
// or
builder.Add1PasswordParameter("ConnectionString2", "op://Private/DummyCredential/password", "my.1password.com");

await builder.Use1PasswordAsync(options =>
{
    options.ConfigurationSectionItemSchema = "op-company://";
    options.Account = "my.1password.com"; // replace with company-specific account if needed
});
builder.AddParameter("ConnectionString-Company", secret: true);
// or
builder.Add1PasswordParameter("ConnectionString2-company", "op-company://Private/DummyCredential/password", configureOptions: options =>
{
    options.ConfigurationSectionItemSchema = "op-company://";
    options.Account = "my.1password.com"; // replace with company-specific account if needed
});

builder.Build().Run();
