namespace MyLinter.Analyzers;

using MyLinter.Results;
using MyLinter.Rules;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class CustomBatchEngine
{
    /// <summary>
    /// 複数ファイルを一括でdotnet formatでチェック（公式ツール使用）
    /// </summary>
    public async Task<IEnumerable<LintResult>> AnalyzeBatch(
        IEnumerable<string> filePaths,
        IEnumerable<string> ruleIds)
    {
        var results = new List<LintResult>();

        var linterEngine = new List<IRule>
        {
            new LineEndings()
        };

        foreach (var filePath in filePaths)
        {
            foreach (var ruleId in ruleIds)
            {
                var linter = linterEngine.Find(x => x.Id == ruleId);
                if (linter != null)
                {
                    var result = linter.Analyze(filePath);
                    results.Add(result);
                }
            }
        }

        return results;
    }
}