namespace userdb.Models;

public class ModifyingUser
{
    public string source { get; set; } = "";
    public string name { get; set; } = "";
    public string userId { get; set; } = "";
    public List<string> additionalRolesToAdd { get; set; } = Array.Empty<string>().ToList();
    public List<string> additionalRolesToRemove { get; set; } = Array.Empty<string>().ToList();
    public int age { get; set; }
    public string fandom { get; set; } = "";
    public List<string> wantedRolesToAdd { get; set; } = Array.Empty<string>().ToList();
    public List<string> wantedRolesToRemove { get; set; } = Array.Empty<string>().ToList();
    public List<string> pronounsToAdd { get; set; } = Array.Empty<string>().ToList();
    public List<string> pronounsToRemove { get; set; } = Array.Empty<string>().ToList();
    public int streak { get; set; } = -1;
    public string status {get; set;} = "";
}