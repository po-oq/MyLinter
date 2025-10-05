namespace MyLinter.Configuration;

public class LintConfig
{
    public string GitMode { get; set; } = "staged";
    public List<string> TargetExtensions { get; set; } = new();
    public AnalyzerSettings Analyzers { get; set; } = new();
}

public class AnalyzerSettings
{
    public RoslynSettings Roslyn { get; set; } = new();
    public CustomSettings Custom { get; set; } = new();
    public AISettings AI { get; set; } = new();
}

public class RoslynSettings
{
    public bool Enabled { get; set; }
    public List<Rule> Rules { get; set; } = new();
}

public class Rule
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Severity { get; set; } = "warning";
    public List<string> ApplyTo { get; set; } = new();
    public List<string> FilePatterns { get; set; } = new();
}

public class CustomSettings
{
    public bool Enabled { get; set; }
    public List<Rule> Rules { get; set; } = new();
}

public class AISettings
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "claude";
    public string RulesPath { get; set; } = "";
    public Dictionary<string, AIProviderConfig> ApiConfig { get; set; } = new();
}

public class AIProviderConfig
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "";
    public string Endpoint { get; set; } = "";
}