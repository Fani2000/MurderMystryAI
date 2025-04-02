using Backend.Validators;

namespace Backend.Configuration;

public class AzureAIOptions
{
    public string Endpoint { get; private set; }
    public string ApiKey { get; private set; }
    public string Model { get; private set; }

    public AzureAIOptions(KeyVaultService keyVaultService)
    {
        if (keyVaultService == null)
        {
            throw new ArgumentNullException(nameof(keyVaultService), "KeyVaultService cannot be null");
        }

        Task.Run(async () =>
        {
            Endpoint = await keyVaultService.GetSecretAsync("AzureEndpoint");
            ApiKey = await keyVaultService.GetSecretAsync("AzureApiKey");
            Model = await keyVaultService.GetSecretAsync("AzureModel");
        }).GetAwaiter().GetResult();

        // Ensure secrets are loaded
        if (string.IsNullOrEmpty(Endpoint) || string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(Model))
        {
            throw new Exception("Failed to load one or more Azure AI configuration secrets from Key Vault.");
        }
    }
}