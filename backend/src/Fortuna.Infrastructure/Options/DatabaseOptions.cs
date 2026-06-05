namespace Fortuna.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string WriteConnectionString { get; set; } = string.Empty;
    public string ReadConnectionString { get; set; } = string.Empty;
}
