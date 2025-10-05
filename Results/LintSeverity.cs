namespace MyLinter.Results;

/// <summary>
/// Lint結果の重要度
/// </summary>
public enum LintSeverity
{
    /// <summary>無効（チェックしない）</summary>
    None,

    /// <summary>提案（緑の波線）</summary>
    Suggestion,

    /// <summary>情報</summary>
    Info,

    /// <summary>警告（黄色）</summary>
    Warning,

    /// <summary>エラー（赤）</summary>
    Error
}

public static class LintSeverityExtensions
{
    /// <summary>
    /// 文字列からLintSeverityに変換
    /// </summary>
    public static LintSeverity Parse(string severity) => severity.ToLower() switch
    {
        "none" => LintSeverity.None,
        "suggestion" => LintSeverity.Suggestion,
        "info" => LintSeverity.Info,
        "warning" => LintSeverity.Warning,
        "error" => LintSeverity.Error,
        _ => LintSeverity.Warning
    };

    /// <summary>
    /// 絵文字付きの文字列取得
    /// </summary>
    public static string ToDisplayString(this LintSeverity severity) => severity switch
    {
        LintSeverity.Error => "❌",
        LintSeverity.Warning => "⚠️",
        LintSeverity.Suggestion => "💡",
        LintSeverity.Info => "ℹ️",
        LintSeverity.None => "⚪",
        _ => "?"
    };

    /// <summary>
    /// コンソールカラー取得
    /// </summary>
    public static ConsoleColor ToConsoleColor(this LintSeverity severity) => severity switch
    {
        LintSeverity.Error => ConsoleColor.Red,
        LintSeverity.Warning => ConsoleColor.Yellow,
        LintSeverity.Suggestion => ConsoleColor.Cyan,
        LintSeverity.Info => ConsoleColor.Blue,
        _ => ConsoleColor.Gray
    };
}