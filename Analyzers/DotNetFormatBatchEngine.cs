namespace MyLinter.Analyzers;

using MyLinter.Results;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class DotNetFormatBatchEngine
{
    /// <summary>
    /// 複数ファイルを一括でdotnet formatでチェック（公式ツール使用）
    /// </summary>
    public async Task<IEnumerable<LintResult>> AnalyzeBatch(
        IEnumerable<string> filePaths,
        IEnumerable<string> ruleIds)
    {
        // 1. .editorconfigを一時生成（ルール指定）
        var editorConfigDir = await CreateTempEditorConfig(ruleIds);

        try
        {
            // 2. dotnet format を1回だけ実行（全ファイル一括）
            return await RunDotNetFormatBatch(filePaths, editorConfigDir, ruleIds);
        }
        finally
        {
            // 3. 一時ファイル削除
            if (Directory.Exists(editorConfigDir))
                Directory.Delete(editorConfigDir, true);
        }
    }

    private async Task<string> CreateTempEditorConfig(IEnumerable<string> ruleIds)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"mylinter_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var editorConfigPath = Path.Combine(tempDir, ".editorconfig");
        var sb = new StringBuilder();

        sb.AppendLine("root = true");
        sb.AppendLine();
        sb.AppendLine("[*.cs]");

        // 各ルールを有効化
        foreach (var ruleId in ruleIds)
        {
            sb.AppendLine($"dotnet_diagnostic.{ruleId}.severity = warning");

            // IDE0290用の追加設定
            if (ruleId == "IDE0290")
            {
                sb.AppendLine("csharp_style_prefer_primary_constructors = true");
            }
            // IDE0032用の追加設定
            else if (ruleId == "IDE0032")
            {
                sb.AppendLine("dotnet_style_prefer_auto_properties = true");
            }
            // IDE0032用の追加設定
            else if (ruleId == "IDE0009")
            {
                sb.AppendLine("dotnet_style_qualification_for_field = true");
                sb.AppendLine("dotnet_style_qualification_for_property = true");
                sb.AppendLine("dotnet_style_qualification_for_method = true");
                sb.AppendLine("dotnet_style_qualification_for_event = true");
            }
            // 他のIDE系ルールも同様に...
        }

        await File.WriteAllTextAsync(editorConfigPath, sb.ToString());
        return tempDir;
    }

    private async Task<IEnumerable<LintResult>> RunDotNetFormatBatch(
        IEnumerable<string> filePaths,
        string editorConfigDir,
        IEnumerable<string> ruleIds)
    {
        // 一時プロジェクトファイル作成（重要！）
        var projectFile = await CreateTempProject(filePaths, editorConfigDir);

        // dotnet format --verify-no-changes で診断のみ実行
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"format \"{projectFile}\" --verify-no-changes --severity info --verbosity diagnostic",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = Process.Start(startInfo);
        if (process == null)
            throw new InvalidOperationException("Failed to start dotnet format");

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // 出力をパースして結果取得
        return ParseDotNetFormatOutput(output + error, filePaths, ruleIds);
    }

    private async Task<string> CreateTempProject(IEnumerable<string> filePaths, string tempDir)
    {
        // 一時的な.csprojファイルを作成
        var projectPath = Path.Combine(tempDir, "temp.csproj");

        var sb = new StringBuilder();
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
        sb.AppendLine("    <LangVersion>13.0</LangVersion>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");

        // 対象ファイルをCompile項目として追加
        foreach (var file in filePaths)
        {
            sb.AppendLine($"    <Compile Include=\"{file}\" />");
        }

        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");

        await File.WriteAllTextAsync(projectPath, sb.ToString());
        return projectPath;
    }

    private IEnumerable<LintResult> ParseDotNetFormatOutput(string output, IEnumerable<string> filePaths, IEnumerable<string> ruleIds)
    {
        var results = new List<LintResult>();

        // dotnet format の出力パターン:
        // "  Program.cs(5,13): warning IDE0290: Use primary constructor"
        var pattern = @"([^(]+)\((\d+),(\d+)\):\s+(info|warning|error)\s+([A-Z]+\d+):\s+(.+)";
        var regex = new Regex(pattern);

        foreach (Match match in regex.Matches(output))
        {
            var fileName = match.Groups[1].Value.Trim();
            var line = int.Parse(match.Groups[2].Value);
            var column = int.Parse(match.Groups[3].Value);
            var severityText = match.Groups[4].Value;
            var ruleId = match.Groups[5].Value;
            var message = match.Groups[6].Value;

            if (!ruleIds.Contains(ruleId))
                continue; // 指定されたルールID以外は無視

            // ファイルパスを完全パスに復元
            var fullPath = filePaths.FirstOrDefault(f =>
                f.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (fullPath != null)
            {
                results.Add(new LintResult
                {
                    RuleId = ruleId,
                    FilePath = fullPath,
                    Line = line,
                    Column = column,
                    Message = message,
                    Severity = severityText == "error"
                        ? LintSeverity.Error
                        : LintSeverity.Warning,
                    Source = "dotnet-format"
                });
            }
        }

        return results;
    }
}