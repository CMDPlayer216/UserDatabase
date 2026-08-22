namespace userdb.Models;

public class User
{
    public string name { get; set; } = "";
    public string userId { get; set; } = "";

    public List<string> additionalRoles { get; set; } = Array.Empty<string>().ToList();

    public int age { get; set; }
    public string fandom { get; set; } = "";
    public List<string> wantedRoles { get; set; } = Array.Empty<string>().ToList();

    public List<string> pronouns { get; set; } = Array.Empty<string>().ToList();

    public DateOnly dateRegistered { get; set; }
    public int streak { get; set; }
    public string status {get; set;} = "";
}