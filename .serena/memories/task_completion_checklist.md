# タスク完了時のチェックリスト

## コード変更後に実行すべきこと

### 1. フォーマットチェック
```bash
dotnet format --verify-no-changes
```
- エラーがある場合は `dotnet format` で自動修正

### 2. ビルドチェック
```bash
dotnet build
```
- ビルドエラーがないことを確認
- 警告も可能な限り解消

### 3. コーディング規約の確認
- [ ] ファイルスコープnamespace使用（`namespace Xxx;`）
- [ ] 適切な命名規則（PascalCase/camelCase）
- [ ] Primary constructor使用（該当する場合）
- [ ] Auto property使用（該当する場合）
- [ ] CRLF改行コード使用
- [ ] 日本語コメントで意図を明記

### 4. 新規ファイル追加時
- [ ] `.csproj`に必要な参照を追加
- [ ] `GlobalUsings.cs`に共通usingを追加（必要に応じて）
- [ ] 適切なnamespaceを設定

### 5. 設定ファイル変更時
- [ ] `config.jsonc`の構文が正しいか確認
- [ ] 環境変数が正しく設定されているか確認

### 6. Git操作
```bash
git status              # 変更ファイル確認
git add .               # ステージング
git commit -m "message" # コミット
```

## テスト（現在未実装）
- テストプロジェクトは未実装のため、手動テストを実施
- 実行して期待通りの動作をするか確認

## 注意点
- 現在の環境は.NET SDK 8.0だが、プロジェクトは.NET 9.0対応が必要
- AIアナライザー使用時は環境変数設定が必須
