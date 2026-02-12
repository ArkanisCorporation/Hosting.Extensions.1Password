using Arkanis.Aspire.Hosting.Extensions._1Password;

var builder = DistributedApplication.CreateBuilder(args);

await builder.Use1PasswordAsync("my.1password.com");
builder.AddParameter("ConnectionString", secret: true);

builder.Build().Run();
