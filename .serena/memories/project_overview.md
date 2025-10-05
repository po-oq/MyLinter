# MyLinter プロジェクト概要

## プロジェクトの目的
Gitリポジトリ内のファイルに対して、複数のLint検査を実行するコマンドラインツール。
Roslyn、カスタムルール、AI（Claude/OpenAI/Gemini）ベースの3種類のアナライザーをサポート。

## 技術スタック
- **言語**: C# (.NET 9.0)
- **主要パッケージ**:
  - Microsoft.CodeAnalysis.CSharp 4.14.0（Roslynアナライザー）
  - LibGit2Sharp 0.31.0（Git操作）
  - System.CommandLine 2.0.0-rc.1（CLIパーサー）
- **実行環境**: .NET 9.0 SDK（現在の環境は8.0.119）

## プロジェクト構造
```
MyLinter/
├── Program.cs              # エントリーポイント（トップレベルステートメント）
├── GlobalUsings.cs         # グローバルusing宣言
├── config.jsonc            # Lint設定ファイル（JSONC形式）
├── Analyzers/              # Lintエンジン
│   ├── LintEngine.cs
│   ├── DotNetFormatBatchEngine.cs
│   └── CustomBatchEngine.cs
├── Configuration/          # 設定関連
│   ├── ConfigLoader.cs
│   └── LintConfig.cs
├── Git/                    # Git操作
│   ├── GitHelper.cs
│   └── GitFileInfo.cs
├── Helpers/                # ユーティリティ
│   └── EnvironmentChecker.cs
├── Results/                # 結果データ構造
│   ├── LintReport.cs
│   ├── LintResult.cs
│   └── LintSeverity.cs
└── Rules/                  # カスタムルール
    ├── IRule.cs
    └── LineEndings.cs
```

## 主要機能
1. **Git統合**: staged/committedファイルを対象にLint実行
2. **3種類のアナライザー**:
   - Roslyn: IDE0290、IDE0032などのルール
   - カスタム: CRLF改行チェックなど
   - AI: Claude/OpenAI/Geminiを使用したコード分析
3. **ルール適用条件**:
   - `applyTo`: new（新規）、modified（変更）で適用範囲を制御
   - `filePatterns`: ファイルパターンマッチング

## 実行方法
```bash
dotnet run --gitpath <リポジトリパス> [--output <出力ファイルパス>]
```

## 設定ファイル (config.jsonc)
- **gitMode**: "staged" または "committed"
- **targetExtensions**: 対象拡張子（.cs, .js, .ts など）
- **analyzers**: roslyn、custom、aiの各設定
- 環境変数展開: `${CLAUDE_API_KEY}` などをサポート
