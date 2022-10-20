namespace Bot.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; init; }
}