namespace MyLinter.Analyzers;

using Microsoft.CodeAnalysis.Diagnostics;
using MyLinter.Configuration;
using MyLinter.Git;
using MyLinter.Results;
using MyLinter.Rules;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class LintEngine(LintConfig config, GitHelper git)
{
    private readonly DotNetFormatBatchEngine formatEngine = new();

    public async Task<LintReport> Execute()
    {
        var stopwatch = Stopwatch.StartNew();

        // Gitファイル情報取得
        var gitFiles = config.GitMode == "staged"
            ? git.GetStagedFilesWithStatus()
            : git.GetCommittedFilesWithStatus();

        var targetFiles = gitFiles
            .Where(f => IsTargetFile(f.FilePath));

        var results = new List<LintResult>();

        // Roslynアナライザー（dotnet format使用）
        if (config.Analyzers.Roslyn.Enabled)
        {
            var applicableFiles = new List<string>();
            var ruleIds = new List<string>();

            // ファイルとルールのフィルタリング
            foreach (var gitFile in targetFiles)
            {
                var fileRules = config.Analyzers.Roslyn.Rules
                    .Where(rule => ShouldApplyRule(rule, gitFile))
                    .Select(r => r.Id);

                if (fileRules.Any())
                {
                    applicableFiles.Add(gitFile.FilePath);
                    ruleIds.AddRange(fileRules);
                }
            }

            if (applicableFiles.Any())
            {
                // dotnet formatで一括解析！
                var formatResults = await formatEngine.AnalyzeBatch(
                    applicableFiles.Distinct(),
                    ruleIds.Distinct()
                );
                results.AddRange(formatResults);
            }
        }

        // カスタムアナライザー
        if (config.Analyzers.Custom.Enabled)
        {
            var applicableFiles = new List<string>();
            var ruleIds = new List<string>();

            // ファイルとルールのフィルタリング
            foreach (var gitFile in targetFiles)
            {
                var fileRules = config.Analyzers.Custom.Rules
                    .Where(rule => ShouldApplyRule(rule, gitFile))
                    .Select(r => r.Id);

                if (fileRules.Any())
                {
                    applicableFiles.Add(gitFile.FilePath);
                    ruleIds.AddRange(fileRules);
                }
            }

            if (applicableFiles.Any())
            {
                var customEngine = new CustomBatchEngine();
                var customResults = await customEngine.AnalyzeBatch(
                    applicableFiles.Distinct(),
                    ruleIds.Distinct()
                );
                results.AddRange(customResults);
            }
        }

        // AIルールも同様に...

        return new LintReport(results)
        {
            ExecutionTime = stopwatch.Elapsed,
            FilesChecked = targetFiles.Count()
        };

    }

    private bool ShouldApplyRule(Rule rule, GitFileInfo gitFile)
    {
        // ファイルパターンチェック
        if (rule.FilePatterns?.Any() == true)
        {
            if (!rule.FilePatterns.Any(pattern =>
                IsMatch(gitFile.FilePath, pattern)))
            {
                return false;
            }
        }

        // applyTo フィルタ
        if (rule.ApplyTo == null || rule.ApplyTo.Contains("all"))
        {
            return true;
        }

        if (rule.ApplyTo.Contains("new") && gitFile.IsNew)
        {
            return true;
        }

        if (rule.ApplyTo.Contains("modified") && gitFile.IsModified)
        {
            return true;
        }

        return false;
    }

    private bool IsMatch(string filePath, string pattern)
    {
        // ワイルドカードパターンマッチング
        var regex = new Regex(
            "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$",
            RegexOptions.IgnoreCase
        );
        return regex.IsMatch(Path.GetFileName(filePath));
    }

    // ファイルが解析対象かどうか判定するメソッドを追加
    private bool IsTargetFile(string filePath)
    {
        if (config.TargetExtensions?.Any() == true)
        {
             return config.TargetExtensions.Any(ext =>
                filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
        return true;
    }
}