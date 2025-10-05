namespace MyLinter.Helpers;

using System.Diagnostics;

public static class EnvironmentChecker
{
    public static async Task<bool> CheckDotNetFormat()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "format --version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null) return false;

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"✅ dotnet format: {output.Trim()}\n");
                return true;
            }
        }
        catch
        {
            // コマンドが見つからない
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ dotnet format not found!\n");
        Console.ResetColor();
        Console.WriteLine("Please install .NET SDK with dotnet format:");
        Console.WriteLine("  https://dotnet.microsoft.com/download\n");

        return false;
    }
}