using LibGit2Sharp;
using MyLinter.Results;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyLinter.Rules;

public class LineEndings : IRule
{
    public string Id => "CRLF";

    public LintResult Analyze(string filePath)
    {
        var content = File.ReadAllText(filePath);
        // CRLFを消す
        var withoutCRLF = content.Replace("\r\n", "");

        // 残ったCRとLFをチェック
        bool hasCR = withoutCRLF.Contains('\r');
        bool hasLF = withoutCRLF.Contains('\n');

        var hasLineEndings = hasCR || hasLF;

        return new LintResult
        {
            FilePath = filePath,
            Line = 0,
            Column = 0,
            Message = "改行コードがCRLFではありません",
            Severity = LintSeverity.Warning,
            Source = "LineEndings"
        };
    }

    public static void NormalizeToCRLF(string filePath)
    {
        var content = File.ReadAllText(filePath);

        // CR、LF、CRLFすべてをCRLFに置換
        content = Regex.Replace(content, @"\r\n|\r|\n", "\r\n");

        File.WriteAllText(filePath, content);
    }
}
