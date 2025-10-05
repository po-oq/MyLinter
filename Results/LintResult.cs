namespace MyLinter.Results;

/// <summary>
/// 個別のLint結果
/// </summary>
public class LintResult
{
    /// <summary>ルールID（例: IDE0290）</summary>
    public string RuleId { get; set; } = "";

    /// <summary>ルール名</summary>
    public string RuleName { get; set; } = "";

    /// <summary>ファイルパス</summary>
    public string FilePath { get; set; } = "";

    /// <summary>行番号（1始まり）</summary>
    public int Line { get; set; }

    /// <summary>列番号（1始まり、オプション）</summary>
    public int? Column { get; set; }

    /// <summary>メッセージ</summary>
    public string Message { get; set; } = "";

    /// <summary>重要度</summary>
    public LintSeverity Severity { get; set; } = LintSeverity.Warning;

    /// <summary>Lintエンジンの種類（roslyn, custom, ai）</summary>
    public string Source { get; set; } = "";

    /// <summary>
    /// 相対パス取得（表示用）
    /// </summary>
    public string GetRelativePath(string basePath)
    {
        return Path.GetRelativePath(basePath, FilePath);
    }

    /// <summary>
    /// 1行で表示
    /// </summary>
    public override string ToString()
    {
        var location = Column.HasValue
            ? $"Line {Line}, Col {Column}"
            : $"Line {Line}";
        return $"{Severity.ToDisplayString()} [{RuleId}] {location}: {Message}";
    }
}