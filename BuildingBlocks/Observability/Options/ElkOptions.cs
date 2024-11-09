namespace Observability.Options;

public class ElkOptions
{
    public const string SectionName = "ElkSettings";

    public bool Enabled { get; set; }
    public string ElasticSearchUrl { get; set; } = default!;
}