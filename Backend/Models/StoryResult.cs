namespace Backend.GraphQL;

public class StoryResult
{
    public string? FullStory { get; set; }
    public string? Killer { get; set; }
    public string? Detective { get; set; } // New Property for assigned detective
    public Dictionary<string, string>? UserStories { get; set; } // Story segments for all users
    public List<string>? DetectiveClues { get; set; } // Exclusive clues for the detective
}