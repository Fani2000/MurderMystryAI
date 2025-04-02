using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Backend.Validators;

public class KeyVaultService
{
    private readonly SecretClient _secretClient;

    public KeyVaultService(string keyVaultUrl)
    {
        if (string.IsNullOrEmpty(keyVaultUrl))
            throw new ArgumentException("Key Vault URL cannot be null or empty", nameof(keyVaultUrl));

        _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        if (string.IsNullOrEmpty(secretName))
            throw new ArgumentException("Secret name cannot be null or empty", nameof(secretName));

        KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value;
    }
}