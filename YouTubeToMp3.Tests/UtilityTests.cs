using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YouTubeToMp3.Tests;

public class UtilityTests
{
    [Fact]
    public void IsValidDirectory_ReturnsFalse_WhenDirectoryDoesNotExist()
    {
        // Arrange
        string nonExistentPath = "/this/path/does/not/exist";

        // Act
        bool result = Directory.Exists(nonExistentPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidDirectory_ReturnsTrue_WhenDirectoryExists()
    {
        // Arrange
        string tempPath = Path.GetTempPath();

        // Act
        bool result = Directory.Exists(tempPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PathCombination_CreatesValidPath()
    {
        // Arrange
        string directory = "/tmp";
        string fileName = "%(title)s.%(ext)s";

        // Act
        string combinedPath = Path.Combine(directory, fileName);

        // Assert
        Assert.Equal($"{directory}{Path.DirectorySeparatorChar}{fileName}", combinedPath);
    }
}