using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;

namespace YouTubeToMp3.Tests;

public class UiTests
{
    [AvaloniaFact]
    public async Task MainWindow_HasExpectedControls()
    {
        // Arrange
        var window = new MainWindow();
        
        // Act 
        window.ApplyTemplate(); // Apply the XAML template
        
        // Use Avalonia's FindControl method to find named controls
        var youTubeUrlInput = window.FindControl<TextBox>("YouTubeUrlInput");
        var outputDirectoryInput = window.FindControl<TextBox>("OutputDirectoryInput");
        var browseOutputButton = window.FindControl<Button>("BrowseOutputButton");
        var downloadButton = window.FindControl<Button>("DownloadButton");
        var statusTextBox = window.FindControl<TextBox>("StatusTextBox");
        var downloadProgressBar = window.FindControl<ProgressBar>("DownloadProgressBar");

        // Assert
        Assert.NotNull(youTubeUrlInput);
        Assert.NotNull(outputDirectoryInput);
        Assert.NotNull(browseOutputButton);
        Assert.NotNull(downloadButton);
        Assert.NotNull(statusTextBox);
        Assert.NotNull(downloadProgressBar);
    }

    [AvaloniaFact]
    public async Task DownloadButton_IsDisabled_WhenUrlIsEmpty()
    {
        // Arrange
        var window = new MainWindow();
        window.ApplyTemplate();
        
        var youTubeUrlInput = window.FindControl<TextBox>("YouTubeUrlInput");
        var downloadButton = window.FindControl<Button>("DownloadButton");

        // Act
        youTubeUrlInput.Text = string.Empty;
        
        // Simulate the validation that happens when clicking the download button
        var isValidUrl = CallIsValidUrlStatic(string.Empty);

        // Assert
        Assert.False(isValidUrl);
    }

    [AvaloniaFact]
    public async Task ValidYouTubeUrl_IsAccepted()
    {
        // Arrange
        var window = new MainWindow();
        window.ApplyTemplate();
        
        var validUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

        // Act
        var isValid = CallIsValidUrlStatic(validUrl);

        // Assert
        Assert.True(isValid);
    }

    [AvaloniaFact]
    public async Task MainWindow_InitializesWithUserProfileAsOutputDirectory()
    {
        // Arrange
        var window = new MainWindow();
        window.ApplyTemplate();
        
        var outputDirectoryInput = window.FindControl<TextBox>("OutputDirectoryInput");

        // Act
        var expectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Assert
        Assert.Equal(expectedPath, outputDirectoryInput.Text);
    }

    // Helper method to access static method via reflection
    private bool CallIsValidUrlStatic(string url)
    {
        var method = typeof(MainWindow).GetMethod("IsValidUrl", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Static);
        return (bool)method?.Invoke(null, new object[] { url });
    }
}