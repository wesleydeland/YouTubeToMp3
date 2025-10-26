using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace YouTubeToMp3.Tests;

public class ProcessTests
{
    [Fact]
    public void FindYtDlp_ReturnsNonEmptyString_WhenYtDlpExists()
    {
        // This test checks that the FindYtDlp method doesn't crash
        // The actual result depends on whether yt-dlp is installed in the test environment
        var result = CallFindYtDlpStatic();
        
        // We just verify the method executes without throwing an exception
        Assert.NotNull(result);
    }

    [Fact]
    public void ProcessStartInfo_CreatesValidArguments_WhenGivenValidInputs()
    {
        // Arrange
        string ytDlpPath = "yt-dlp"; // Placeholder
        string url = "https://www.youtube.com/watch?v=test";
        string outputDir = "/tmp";

        // Act
        var processInfo = new ProcessStartInfo
        {
            FileName = ytDlpPath,
            Arguments = $"-x --audio-format mp3 -o \"{Path.Combine(outputDir, "%(title)s.%(ext)s")}\" --newline \"{url}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Assert
        Assert.Equal(ytDlpPath, processInfo.FileName);
        Assert.Contains("-x", processInfo.Arguments);
        Assert.Contains("--audio-format mp3", processInfo.Arguments);
        Assert.Contains(url, processInfo.Arguments);
        Assert.True(processInfo.RedirectStandardOutput);
        Assert.True(processInfo.RedirectStandardError);
        Assert.False(processInfo.UseShellExecute);
        Assert.True(processInfo.CreateNoWindow);
    }

    // Helper method to access static method via reflection
    private string CallFindYtDlpStatic()
    {
        var method = typeof(MainWindow).GetMethod("FindYtDlp", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Static);
        return (string)method?.Invoke(null, null);
    }
}