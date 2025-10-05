# 開発コマンド

## 環境要件
- **.NET SDK**: 9.0以上（現在の環境は8.0.119で不足）
- **dotnet format**: 8.1.631901+（インストール済み）

## ビルド・実行
```bash
# ビルド
dotnet build

# 実行
dotnet run --gitpath <リポジトリパス> [--output <出力ファイルパス>]

# 例
dotnet run --gitpath /path/to/repo --output results.json
```

## フォーマット・Lint
```bash
# dotnet formatによるフォーマットチェック
dotnet format --verify-no-changes

# dotnet formatによる自動修正
dotnet format
```

## プロジェクト管理
```bash
# 依存関係の復元
dotnet restore

# クリーンビルド
dotnet clean
dotnet build

# パッケージ追加
dotnet add package <パッケージ名>
```

## Git操作（Linuxシステム）
```bash
# 基本コマンド
ls          # ファイル一覧
cd          # ディレクトリ移動
grep        # テキスト検索
find        # ファイル検索
git status  # Git状態確認
git add     # ステージング
git commit  # コミット
```

## 注意事項
- 現在の環境では.NET 9.0が必要だがSDK 8.0のためビルドエラーが発生
- `dotnet format`は別途インストール済みで使用可能
- 環境変数（CLAUDE_API_KEY等）を設定してからAIアナライザーを使用
