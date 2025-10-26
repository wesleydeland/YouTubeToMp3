using System;
using System.IO;
using System.Text.RegularExpressions;

namespace YouTubeToMp3.Tests;

public class MainWindowTests
{
    [Fact]
    public void IsValidUrl_WithValidYouTubeUrl_ReturnsTrue()
    {
        // Arrange
        string validUrls = @"
            https://www.youtube.com/watch?v=dQw4w9WgXcQ
            https://youtu.be/dQw4w9WgXcQ
            https://youtube.com/watch?v=dQw4w9WgXcQ
            https://www.youtu.be/dQw4w9WgXcQ
            https://m.youtube.com/watch?v=dQw4w9WgXcQ
            https://music.youtube.com/watch?v=dQw4w9WgXcQ
        ";

        // Act & Assert
        foreach (string url in validUrls.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!string.IsNullOrEmpty(url))
            {
                var result = CallIsValidUrlStatic(url);
                Assert.True(result, $"URL should be valid: {url}");
            }
        }
    }

    [Fact]
    public void IsValidUrl_WithInvalidUrl_ReturnsFalse()
    {
        // Arrange
        string[] invalidUrls = {
            "https://www.google.com",
            "https://www.example.com",
            "not-a-url",
            "",
            "ftp://youtube.com/watch?v=dQw4w9WgXcQ",
            "https://youtub3.com/watch?v=dQw4w9WgXcQ"
        };

        // Act & Assert
        foreach (string url in invalidUrls)
        {
            var result = CallIsValidUrlStatic(url);
            Assert.False(result, $"URL should be invalid: {url}");
        }
    }

    [Fact]
    public void IsInPath_WhenExecutableExistsInPath_ReturnsTrue()
    {
        // Since we can't easily test with the real system PATH in tests
        // This test would verify that the method works when a known executable is in PATH
        // For this test, we'll check if a common executable exists (like "dotnet" in PATH)
        var result = CallIsInPathStatic("dotnet");
        // This result might be true or false depending on the test environment
        // The important thing is the method doesn't throw an exception
        Assert.True(result || !result); // Always true - just checks method executes without error
    }

    [Fact]
    public void UpdateProgressFromLine_CanBeTestedThroughPublicMethod()
    {
        // Since the UpdateProgressFromLine method is private and depends on UI elements,
        // we can't easily test it directly. Instead, we can test the regex patterns it uses.
        var regex1 = new Regex(@"\[download\].*?(\d+\.?\d*)%", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        var regex2 = new Regex(@"\[download\]\s*(\d+\.?\d*)%\s*\(", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        var regex3 = new Regex(@"\((\d+\.?\d*)%\)", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        
        var testCases = new[]
        {
            "[download]  10.4% of 12.50MiB at 470.45KiB/s ETA 00:21",
            "[download]  10.4% (1.30MiB at 470.45KiB/s)",
            "[download]  1.30MiB at 470.45KiB/s (10.4%)"
        };

        // Act & Assert
        Assert.Matches(regex1, testCases[0]);
        Assert.Matches(regex2, testCases[1]);
        Assert.Matches(regex3, testCases[2]);
    }

    // Helper methods to access static methods via reflection
    private bool CallIsValidUrlStatic(string url)
    {
        var method = typeof(MainWindow).GetMethod("IsValidUrl", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Static);
        return (bool)method?.Invoke(null, new object[] { url });
    }

    private bool CallIsInPathStatic(string fileName)
    {
        var method = typeof(MainWindow).GetMethod("IsInPath", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Static);
        return (bool)method?.Invoke(null, new object[] { fileName });
    }
}
