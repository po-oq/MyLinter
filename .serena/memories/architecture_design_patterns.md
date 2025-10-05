# アーキテクチャとデザインパターン

## 全体アーキテクチャ
```
Program.cs (Entry Point)
    ↓
LintEngine (Main Orchestrator)
    ↓
┌─────────────────┬─────────────────┬─────────────────┐
│ DotNetFormat    │ Custom          │ AI              │
│ BatchEngine     │ BatchEngine     │ (未実装)        │
└─────────────────┴─────────────────┴─────────────────┘
    ↓
LintReport (Results)
```

## 主要コンポーネント

### 1. エントリーポイント (Program.cs)
- **Top-level statements**使用
- **System.CommandLine**でCLI引数パース
- **ローカル関数**でExecuteLint実装

### 2. Lint実行エンジン (LintEngine)
- Gitファイル情報を取得
- 各アナライザーを順次実行
- ルール適用条件の判定（applyTo、filePatterns）
- 結果を集約してLintReport生成

### 3. バッチエンジン
- **DotNetFormatBatchEngine**: Roslynアナライザー実行
- **CustomBatchEngine**: カスタムルール実行
- AI: 未実装（設計済み）

### 4. Git統合 (GitHelper)
- **LibGit2Sharp**を使用
- Staged/Committedファイルの差分取得
- ファイルステータス（New/Modified/Deleted）の判定

### 5. 設定管理 (ConfigLoader/LintConfig)
- **JSONC形式**対応（コメント・末尾カンマOK）
- **環境変数展開**: `${ENV_VAR}` → 実際の値
- 階層的な設定構造（analyzers → roslyn/custom/ai）

### 6. 結果管理 (LintReport/LintResult)
- Severity: Warning/Error
- ファイルパス、行番号、列番号、メッセージ
- Display()で整形出力
- GetExitCode()でプロセス終了コード決定

## デザインパターン

### Strategy Pattern
- **IRule**インターフェース
- カスタムルール（LineEndings等）を動的に適用
- 将来的な拡張が容易

### Factory/Builder Pattern
- ConfigLoader: 設定の生成・初期化
- LintEngine: 複数エンジンの組み立て

### Composition Pattern
- LintEngineが複数のBatchEngineを保持
- 各エンジンが独立して動作

## 拡張ポイント
1. **新規ルール追加**: `IRule`実装してRules/に配置
2. **新規アナライザー**: BatchEngine追加してLintEngineに統合
3. **AI統合**: AISettings使用してプロバイダー選択

## ファイル命名規則
- **機能別フォルダ**: Analyzers、Configuration、Git、Helpers、Rules、Results
- **単一責任**: 1ファイル1クラス（一部例外あり）
- **インターフェース**: IRule形式
