using System.Text.Json.Serialization;

namespace _userdatabase.UserDatabase;

public class User
{
    public string name { get; set; } = "";
    public string userId { get; set; } = "";

    [JsonPropertyName("aditionalRoles")]
    public string[] additionalRoles { get; set; } = Array.Empty<string>();

    public int age { get; set; }
    public string fandom { get; set; } = "";
    public string[] lookedCharacters { get; set; } = Array.Empty<string>();

    [JsonPropertyName("pronauns")]
    public string[] pronouns { get; set; } = Array.Empty<string>();

    public DateOnly dateRegistered { get; set; }
    public int streak { get; set; }
}