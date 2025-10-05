// クラスもMainメソッドも不要！直接コード書けちゃう
using MyLinter.Analyzers;
using MyLinter.Configuration;
using MyLinter.Git;
using MyLinter.Helpers;
using System.CommandLine;

try
{
    Console.WriteLine("🔍 MyLinter Starting...\n");

    // dotnet format チェック
    if (!await EnvironmentChecker.CheckDotNetFormat())
    {
        Environment.Exit(1);
    }

    // 設定ファイル読み込み
    var config = await ConfigLoader.LoadAsync();

    Console.WriteLine($"🔍 Git Mode: {config.GitMode}");
    Console.WriteLine($"📝 Target Extensions: {string.Join(", ", config.TargetExtensions)}");
    Console.WriteLine($"⚙️  Roslyn Enabled: {config.Analyzers.Roslyn.Enabled}");

    Option<DirectoryInfo> gitOption = new("--gitpath")
    {
        Description = "Git repository path.",
        Required = true,
    };

    Option<string?> outputOption = new("--output")
    {
        Description = "Output file path (optional).",
    };

    var rootCommand = new RootCommand("Git Lint Tool");
    rootCommand.Options.Add(gitOption.AcceptExistingOnly());
    rootCommand.Options.Add(outputOption);

    rootCommand.SetAction(async parseResult =>
    {
        var gitPath = parseResult.GetValue(gitOption);
        var output = parseResult.GetValue(outputOption);

        if (gitPath != null)
        {
            await ExecuteLint(config, gitPath.FullName, output);
        }
    });

    ParseResult parseResult = rootCommand.Parse(args);
    parseResult.Invoke();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}




// ローカル関数として実装できる！
async Task ExecuteLint(LintConfig config, string gitPath, string? outputPath)
{
    Console.WriteLine($"🔍 args gitPath: {gitPath}, output: {outputPath}\n");
    Console.WriteLine($"🔍 Linting files...\n");

    // Git操作
    var git = new GitHelper(gitPath);

    // Lintエンジン実行
    var engine = new LintEngine(config, git);
    var report = await engine.Execute();

    // 結果表示
    report.Display();

    // 終了コード
    Environment.Exit(report.GetExitCode());

    //var config = await LintConfig.LoadAsync("config.json");
    //var git = new GitHelper(Directory.GetCurrentDirectory());
    //var engine = new LintEngine(config, git);

    //var report = await engine.Execute();

    //// 結果表示
    //report.Display();

    //// JSON出力（オプション）
    //if (outputPath != null)
    //{
    //    await report.SaveAsJson(outputPath);
    //    Console.WriteLine($"\n📄 Results saved to {outputPath}");
    //}

    //// エラーがあったら終了コード1で終了
    //Environment.Exit(report.HasErrors ? 1 : 0);
}