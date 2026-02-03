# Integration Tests

These tests verify the actual integration with 1Password CLI and are **excluded from CI** by default.

## Requirements

To run these integration tests locally, you need:

1. **1Password CLI (`op`)** installed and available in your PATH
   - Download from: https://developer.1password.com/docs/cli/get-started/
   - Verify installation: `op --version`

2. **1Password Desktop App** running and authenticated
   - You must be signed in to your 1Password account
   - The app must be running in the background

3. **Test Secret** in your 1Password vault
   - Create a vault item at: `op://Private/DummyCredential/password`
   - Or update `appsettings.json` to point to an existing secret

## Running Integration Tests

### From Command Line

Run only integration tests:
```bash
dotnet test --filter "Category=Integration"
```

Run all tests including integration tests:
```bash
dotnet test --filter "Category!=Integration"  # This is what CI runs
dotnet test  # Run everything (will fail if 1Password not set up)
```

### From IDE

Most IDEs support filtering by xUnit traits:
- **Visual Studio**: Use Test Explorer and filter by `Category:Integration`
- **Rider**: Right-click on the test and select "Run" or use the test runner filter
- **VS Code**: Use the .NET Test Explorer extension with trait filters

## Modifying Test Secrets

The test secrets are configured in `appsettings.json`. To use different secrets:

1. Update the configuration paths in `appsettings.json`
2. Ensure those secrets exist in your 1Password vault
3. Run the tests

## CI/CD

Integration tests are **automatically excluded** from CI pipelines to avoid requiring 1Password setup on build agents.

See `.github/workflows/_test.yaml` for the test filter configuration:
```yaml
run: dotnet test --filter "Category!=Integration"
```

## Troubleshooting

### "op: command not found"
- Install the 1Password CLI from the link above
- Ensure it's in your PATH

### "Authentication required"
- Make sure the 1Password desktop app is running
- Sign in to your account in the app
- Try running `op whoami` to verify authentication

### "Item not found"
- Verify the secret path in `appsettings.json` exists in your vault
- Check the vault name and item title match exactly (case-sensitive), or use the UUIDs instead
- Use `op item list` to see available items
