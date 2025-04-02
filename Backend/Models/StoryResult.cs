namespace Backend.GraphQL;

public class StoryResult
{
    public string? FullStory { get; set; }
    public string? Killer { get; set; }
    public Dictionary<string, string>? UserStories { get; set; }
}
