using Azure.AI.OpenAI;
using Backend.Configuration;
using Backend.GraphQL;
using Backend.Models;
using OpenAI.Chat;

namespace Backend.GraphQL.Mutations;
public class Mutation(AzureAIOptions azureAiOptions)
{
    public async Task<StoryResult> GenerateMurderMysteryAsync(List<UserInput> users)
    {
        // Validate the minimum number of participants
        if (users == null || users.Count < 3)
        {
            throw new Exception("At least three users are required for the game. One killer, one detective, and at least one other participant.");
        }

        // Randomly assign the 'killer' role
        Random random = new();
        var killerIndex = random.Next(users.Count);
        var killer = users[killerIndex].Name;

        // Randomly assign the 'detective' role (not the same as the killer)
        int detectiveIndex;
        do
        {
            detectiveIndex = random.Next(users.Count);
        } while (detectiveIndex == killerIndex);

        var detective = users[detectiveIndex].Name;

        // Build the story prompt
        var userNames = string.Join(", ", users.Select(u => u.Name));
        var storyPrompt = $@"
            Create a detailed murder mystery story involving these participants: {userNames}. 
            One person is the 'killer', specifically {killer}, who tries to mislead others. 
            Another person is the 'detective', specifically {detective}, who works to uncover the truth 
            by finding relevant clues and suspect behavior. Make this mystery suspenseful and creative.";

        // Set up Azure OpenAI client using AzureAIOptions
        AzureOpenAIClient openAIClient = new(
            new Uri(azureAiOptions.Endpoint),
            new Azure.AzureKeyCredential(azureAiOptions.ApiKey)
        );
        ChatClient chatClient = openAIClient.GetChatClient(azureAiOptions.Model);

        string fullStory;
        List<string> detectiveClues;

        try
        {
            // Request the murder mystery story
            var storyResponse = await chatClient.CompleteChatAsync(
                messages: 
                [
                    new SystemChatMessage("You are a creative story-writing AI."),
                    new UserChatMessage(storyPrompt)
                ]
            );

            fullStory = storyResponse?.Value?.Content?.FirstOrDefault()?.Text ??
                        throw new Exception("No story content received from Azure AI.");

            // Build a clue prompt specifically for the detective
            var cluePrompt = $@"
                From the following story: {fullStory}, 
                generate exactly 3 cryptic, engaging, and creative clues for the detective ({detective})
                to help them connect the dots and determine that the killer is {killer}. 
                Ensure the clues are coherent, drawn from the events in the story, 
                and guide the detective toward solving the crime without being overly obvious.";

            // Request detective clues from Azure OpenAI
            var clueResponse = await chatClient.CompleteChatAsync(
                messages: 
                [
                    new SystemChatMessage("You are an expert in generating hints and logical clues."),
                    new UserChatMessage(cluePrompt)
                ]
            );

            detectiveClues = clueResponse?.Value?.Content?
                .FirstOrDefault()
                ?.Text?
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)?.ToList()
                ?? throw new Exception("No detective clues received from Azure AI.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating story or clues from Azure OpenAI: {ex.Message}", ex);
        }

        // Create unique user-specific stories
        var userStories = new Dictionary<string, string>();
        foreach (var user in users)
        {
            if (user.Name == detective)
            {
                userStories[user.Name] = $@"
                    Detective {user.Name}'s Role:
                    - You arrive at the crime scene, determined to uncover the killer's identity. 
                    - Clues to guide you:
                      1. {detectiveClues.ElementAtOrDefault(0) ?? "No clue available"}
                      2. {detectiveClues.ElementAtOrDefault(1) ?? "No clue available"}
                      3. {detectiveClues.ElementAtOrDefault(2) ?? "No clue available"}
                    - Think critically and use your intuition to solve the case!";
            }
            else if (user.Name == killer)
            {
                userStories[user.Name] = $@"
                    {user.Name}'s Role as Killer:
                    - You are the killer. Your goal is to mislead others and avoid suspicion.
                    - Be careful not to reveal too much about your motives or actions in the story.
                    - Remember: deception is your greatest weapon.";
            }
            else
            {
                userStories[user.Name] = $@"
                    {user.Name}'s Role as Participant:
                    - You are a key figure in this mystery. Work with others to uncover the truth.
                    - Watch for suspicious behaviors and try to identify clues that point to the killer.
                ";
            }
        }

        // Compile and return the result
        return new StoryResult
        {
            FullStory = fullStory,
            Killer = killer,
            Detective = detective,
            DetectiveClues = detectiveClues,
            UserStories = userStories
        };
    }
}
