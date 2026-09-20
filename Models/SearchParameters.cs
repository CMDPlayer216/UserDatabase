namespace userdb.Models;

public class SearchParameters
{
    public string? Name { get; set; }
    public int Age { get; set; } = -1;
    public int Streak { get; set; } = -1;
    public string? Fandom { get; set; }
    public string? AdditionalRole { get; set; }
    public string? WantedRole { get; set; }
    public string? Id { get; set; }
    public string? Date { get; set; }
    public string? Pronoun { get; set; }
    public string? Status { get; set; }
    public int MinAge { get; set; } = -1;
    public int MaxAge { get; set; } = -1;
    public int MinStreak { get; set; } = -1;
    public int MaxStreak { get; set; } = -1;
    public string? MinDate { get; set; }
    public string? MaxDate { get; set; }
    public bool FastSearch { get; set; } = false;
    public bool InverseOrder { get; set; } = false;
    public bool RawModifier { get; set; } = false;
}
