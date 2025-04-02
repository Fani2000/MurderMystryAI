using Backend.Models;
using Azure.AI.OpenAI;
using Backend.Validators;
using OpenAI.Chat;

namespace Backend.GraphQL.Mutations;

public class Mutation
{
    private readonly KeyVaultService _keyVaultService;

    public Mutation(KeyVaultService keyVaultService)
    {
        _keyVaultService = keyVaultService;
    }

    public async Task<StoryResult> GenerateMurderMysteryAsync(List<UserInput> users)
    {
        if (users == null || users.Count < 2)
        {
            throw new Exception("At least two users are required for the game.");
        }

        // Randomly assign a 'killer'
        Random random = new();
        var killer = users[random.Next(users.Count)].Name;

        // Build the story prompt
        var userNames = string.Join(", ", users.Select(u => u.Name));
        var storyPrompt =
            $"Create a murder mystery involving these participants: {userNames}. Randomly assign one as the killer and make it suspenseful.";

        // Retrieve secrets from KeyVaultService
        var endpoint = await _keyVaultService.GetSecretAsync("AzureEndpoint");
        var apiKey = await _keyVaultService.GetSecretAsync("AzureApiKey");
        var model = await _keyVaultService.GetSecretAsync("AzureModel");

        // Set up Azure OpenAI client
        AzureOpenAIClient openAIClient = new(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
        ChatClient chatClient = openAIClient.GetChatClient(model);

        // Generate the murder mystery story
        string fullStory;
        try
        {
            var response = await chatClient.CompleteChatAsync(
                messages: [
                    new SystemChatMessage("You are an AI tasked with writing murder mystery stories."),
                    new UserChatMessage(storyPrompt)
                ]
            );

            fullStory = response.Value.Content.FirstOrDefault()?.Text ??
                        throw new Exception("No content received from Azure AI.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating story from Azure OpenAI: {ex.Message}", ex);
        }

        // Create user-specific story segments
        var userStories = users.ToDictionary(
            u => u.Name,
            u => $"{u.Name}'s part in the mystery: ..."
        );

        // Return the result
        return new StoryResult
        {
            FullStory = fullStory,
            Killer = killer,
            UserStories = userStories
        };
    }
}