namespace MyLinter.Configuration;

using System.Text.Json;
using System.Text.RegularExpressions;

public static class ConfigLoader
{
    public static async Task<LintConfig> LoadAsync(string path = "config.jsonc")
    {
        var jsonc = await File.ReadAllTextAsync(path);

        // 環境変数の展開（${ENV_VAR}を実際の値に置換）
        jsonc = ExpandEnvironmentVariables(jsonc);

        // JSONCをパース（コメントを自動でスキップ）
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip, // 👈 コメント無視
            AllowTrailingCommas = true, // 👈 末尾カンマOK
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Deserialize<LintConfig>(jsonc, options)
            ?? throw new InvalidOperationException("Failed to load config");
    }

    private static string ExpandEnvironmentVariables(string content)
    {
        // ${ENV_VAR} パターンを環境変数の値に置換
        return Regex.Replace(content, @"\$\{(\w+)\}", match =>
        {
            var varName = match.Groups[1].Value;
            var value = Environment.GetEnvironmentVariable(varName);

            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"⚠️  Warning: Environment variable '{varName}' not found");
                return match.Value; // 見つからない場合はそのまま
            }

            return value;
        });
    }
}