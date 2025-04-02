namespace Backend.Models;

public class UserInput
{
    public string Name { get; set; } // Name of the user
    public string? Role { get; set; } // Optional: To specify a role in the game later (e.g., detective, killer, etc.)
}