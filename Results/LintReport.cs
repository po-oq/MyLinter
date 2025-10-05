namespace MyLinter.Results;

using System.Text.Json;

/// <summary>
/// Lint結果のレポート
/// </summary>
public class LintReport
{
    public List<LintResult> Results { get; set; } = new();
    public DateTime ExecutedAt { get; set; } = DateTime.Now;
    public TimeSpan ExecutionTime { get; set; }
    public int FilesChecked { get; set; }

    public LintReport() { }

    public LintReport(List<LintResult> results)
    {
        Results = results;
    }

    /// <summary>
    /// エラーが1つでもあるか
    /// </summary>
    public bool HasErrors => Results.Any(r => r.Severity == LintSeverity.Error);

    /// <summary>
    /// 警告が1つでもあるか
    /// </summary>
    public bool HasWarnings => Results.Any(r => r.Severity == LintSeverity.Warning);

    /// <summary>
    /// 重要度別の件数
    /// </summary>
    public Dictionary<LintSeverity, int> GetCountBySeverity()
    {
        return Results
            .GroupBy(r => r.Severity)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// ファイル別の結果取得
    /// </summary>
    public Dictionary<string, List<LintResult>> GroupByFile()
    {
        return Results
            .GroupBy(r => r.FilePath)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// コンソールに表示
    /// </summary>
    public void Display(string? basePath = null)
    {
        basePath ??= Directory.GetCurrentDirectory();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine($"Lint Results ({FilesChecked} files checked)");
        Console.WriteLine("========================================");
        Console.WriteLine();

        if (!Results.Any())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✨ No issues found!");
            Console.ResetColor();
            Console.WriteLine();
            return;
        }

        // ファイル別にグループ化して表示
        var groupedByFile = GroupByFile();

        foreach (var (filePath, results) in groupedByFile.OrderBy(x => x.Key))
        {
            var relativePath = Path.GetRelativePath(basePath, filePath);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"📁 {relativePath}");
            Console.ResetColor();

            foreach (var result in results.OrderBy(r => r.Line))
            {
                Console.ForegroundColor = result.Severity.ToConsoleColor();
                var location = result.Column.HasValue
                    ? $"Line {result.Line}:{result.Column}"
                    : $"Line {result.Line}";
                Console.Write($"  {result.Severity.ToDisplayString()} {location}");
                Console.ResetColor();
                Console.WriteLine($": [{result.RuleId}] {result.Message}");
            }

            Console.WriteLine();
        }

        // サマリー表示
        DisplaySummary();
    }

    /// <summary>
    /// サマリー表示
    /// </summary>
    private void DisplaySummary()
    {
        Console.WriteLine("========================================");

        var counts = GetCountBySeverity();
        var errorCount = counts.GetValueOrDefault(LintSeverity.Error, 0);
        var warningCount = counts.GetValueOrDefault(LintSeverity.Warning, 0);
        var suggestionCount = counts.GetValueOrDefault(LintSeverity.Suggestion, 0);

        var parts = new List<string>();
        if (errorCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            parts.Add($"{errorCount} error{(errorCount > 1 ? "s" : "")}");
        }
        if (warningCount > 0)
        {
            if (parts.Any()) Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            parts.Add($"{warningCount} warning{(warningCount > 1 ? "s" : "")}");
        }
        if (suggestionCount > 0)
        {
            if (parts.Any()) Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            parts.Add($"{suggestionCount} suggestion{(suggestionCount > 1 ? "s" : "")}");
        }

        Console.Write("Summary: ");
        Console.WriteLine(string.Join(", ", parts));
        Console.ResetColor();

        if (ExecutionTime.TotalSeconds > 0)
        {
            Console.WriteLine($"Execution time: {ExecutionTime.TotalSeconds:F2}s");
        }

        Console.WriteLine("========================================");
        Console.WriteLine();
    }

    /// <summary>
    /// JSON形式で保存
    /// </summary>
    public async Task SaveAsJson(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(this, options);
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// GitHub Actions形式で出力
    /// </summary>
    public void DisplayGitHubActions()
    {
        foreach (var result in Results)
        {
            var level = result.Severity switch
            {
                LintSeverity.Error => "error",
                LintSeverity.Warning => "warning",
                _ => "notice"
            };

            var file = result.FilePath;
            var line = result.Line;
            var message = $"[{result.RuleId}] {result.Message}";

            Console.WriteLine($"::{level} file={file},line={line}::{message}");
        }
    }

    /// <summary>
    /// 終了コード取得（CI/CD用）
    /// </summary>
    public int GetExitCode()
    {
        return HasErrors ? 1 : 0;
    }
}