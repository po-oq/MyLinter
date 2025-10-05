# コーディングスタイルと規約

## 命名規則
- **Namespace**: PascalCase、ファイルスコープ宣言を使用（例: `namespace MyLinter.Analyzers;`）
- **クラス名**: PascalCase（例: `LintEngine`, `GitHelper`）
- **メソッド名**: PascalCase（例: `CheckDotNetFormat`, `LoadAsync`）
- **フィールド**: camelCase（例: `formatEngine`, `repoPath`）
- **プロパティ**: PascalCase（例: `FilePath`, `IsNew`）

## ファイル構成
- **ファイルスコープnamespace**: `namespace MyLinter.Xxx;` 形式を使用
- **GlobalUsings.cs**: 共通using宣言を集約
  ```csharp
  global using System;
  global using System.Collections.Generic;
  global using System.IO;
  global using System.Linq;
  global using System.Text.Json;
  global using System.Threading.Tasks;
  ```

## C#言語機能の使用
- **Top-level statements**: Program.csでクラスやMainメソッド不要
- **File-scoped namespace**: ネストを減らす
- **Nullable有効**: `<Nullable>enable</Nullable>`
- **Implicit usings**: `<ImplicitUsings>enable</ImplicitUsings>`
- **Primary constructor**: 推奨（IDE0290ルール）
- **Auto property**: 推奨（IDE0032ルール）
- **ローカル関数**: Program.cs内で`ExecuteLint`のように使用

## コメント
- **日本語コメント**: コード内で日本語コメントを積極的に使用
- **絵文字**: Console出力で使用（例: 🔍, 📝, ⚙️）
- **説明的コメント**: ビジネスロジックの意図を明確に記述

## その他
- **改行コード**: CRLF（\r\n）を標準とする
- **JSON設定**: JSONC形式（コメント・末尾カンマ対応）
- **環境変数**: `${ENV_VAR}` 形式で設定ファイルに記述
