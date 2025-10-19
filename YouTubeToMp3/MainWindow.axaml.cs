using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeToMp3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OutputDirectoryInput.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string youtubeUrl = YouTubeUrlInput.Text?.Trim();
            
            if (string.IsNullOrEmpty(youtubeUrl))
            {
                StatusTextBox.Text = "Please enter a YouTube URL";
                return;
            }

            if (!IsValidUrl(youtubeUrl))
            {
                StatusTextBox.Text = "Please enter a valid YouTube URL";
                return;
            }

            string outputDir = OutputDirectoryInput.Text?.Trim();
            if (string.IsNullOrEmpty(outputDir) || !Directory.Exists(outputDir))
            {
                StatusTextBox.Text = "Please select a valid output directory";
                return;
            }

            await DownloadMp3(youtubeUrl, outputDir);
        }

        private async Task DownloadMp3(string url, string outputDir)
        {
            try
            {
                StatusTextBox.Text = "Starting download...\n";
                DownloadProgressBar.IsVisible = true;
                DownloadProgressBar.Value = 0;
                DownloadButton.IsEnabled = false;

                // Find yt-dlp executable
                string ytDlpPath = FindYtDlp();
                if (string.IsNullOrEmpty(ytDlpPath))
                {
                    StatusTextBox.Text += "Error: yt-dlp not found. Please install yt-dlp.\n";
                    return;
                }

                // Create the output path with template
                string outputTemplate = Path.Combine(outputDir, "%(title)s.%(ext)s");
                
                // Prepare the process with progress callback
                var processInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = $"-x --audio-format mp3 -o \"{outputTemplate}\" --newline \"{url}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processInfo))
                {
                    if (process == null)
                    {
                        StatusTextBox.Text += "Failed to start yt-dlp process\n";
                        return;
                    }

                    // Read output and error streams asynchronously
                    var outputTask = ReadOutputStream(process.StandardOutput);
                    var errorTask = ReadErrorStream(process.StandardError);

                    await process.WaitForExitAsync();

                    // Wait for both streams to finish processing
                    await Task.WhenAll(outputTask, errorTask);

                    if (process.ExitCode == 0)
                    {
                        StatusTextBox.Text += "\nDownload completed successfully!\n";
                        StatusTextBox.Text += $"Output directory: {outputDir}\n";
                    }
                    else
                    {
                        StatusTextBox.Text += $"\nError: yt-dlp exited with code {process.ExitCode}\n";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusTextBox.Text += $"Error: {ex.Message}\n";
            }
            finally
            {
                DownloadProgressBar.IsVisible = false;
                DownloadButton.IsEnabled = true;
            }
        }

        private async Task ReadOutputStream(StreamReader reader)
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Update UI on the main thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Update progress if line contains progress information
                    UpdateProgressFromLine(line);
                    
                    // Append the line to status
                    StatusTextBox.Text += line + "\n";
                    // Auto-scroll to the bottom
                    StatusTextBox.CaretIndex = StatusTextBox.Text.Length;
                });
            }
        }

        private void UpdateProgressFromLine(string line)
        {
            // Handle different types of progress lines from yt-dlp
            // Format 1: [download]  10.4% of 12.50MiB at 470.45KiB/s ETA 00:21
            var match = Regex.Match(line, @"\[download\].*?(\d+\.?\d*)%");
            if (match.Success)
            {
                if (double.TryParse(match.Groups[1].Value, out double progressValue1))
                {
                    DownloadProgressBar.Value = progressValue1;
                }
                return;
            }

            // Format 2: [download]  10.4% (1.30MiB at 470.45KiB/s)
            match = Regex.Match(line, @"\[download\]\s*(\d+\.?\d*)%\s*\(");
            if (match.Success)
            {
                if (double.TryParse(match.Groups[1].Value, out double progressValue2))
                {
                    DownloadProgressBar.Value = progressValue2;
                }
                return;
            }

            // Format 3: [download]  1.30MiB at 470.45KiB/s (10.4%)
            match = Regex.Match(line, @"\((\d+\.?\d*)%\)");
            if (match.Success)
            {
                if (double.TryParse(match.Groups[1].Value, out double progressValue3))
                {
                    DownloadProgressBar.Value = progressValue3;
                }
            }
        }

        private async Task ReadErrorStream(StreamReader reader)
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Update UI on the main thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // This is usually for errors, just append to status
                    StatusTextBox.Text += line + "\n";
                    // Auto-scroll to the bottom
                    StatusTextBox.CaretIndex = StatusTextBox.Text.Length;
                });
            }
        }

        private string FindYtDlp()
        {
            // Check common locations for yt-dlp
            string[] possiblePaths = {
                "/opt/homebrew/bin/yt-dlp",  // macOS homebrew on Apple Silicon
                "/usr/local/bin/yt-dlp",  // macOS homebrew
                "/usr/bin/yt-dlp",  // Linux
                "C:\\yt-dlp\\yt-dlp.exe",  // Windows typical installation
                "./yt-dlp",  // Local directory
                "yt-dlp",  // In PATH (macOS/Linux)
                "yt-dlp.exe",  // Windows
                "./yt-dlp.exe"  // Local directory on Windows
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path) || IsInPath(Path.GetFileNameWithoutExtension(path)))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        private bool IsInPath(string fileName)
        {
            string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? new string[0];
            
            foreach (string path in paths)
            {
                try
                {
                    string fullPath = Path.Combine(path, fileName);
                    string exePath = fullPath + ".exe"; // For Windows
                    
                    if (File.Exists(fullPath) || File.Exists(exePath))
                    {
                        return true;
                    }
                }
                catch
                {
                    // Ignore invalid paths
                }
            }
            
            return false;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) &&
                   (uriResult.Host.Contains("youtube.com") || uriResult.Host.Contains("youtu.be"));
        }

        private async void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog
            {
                Title = "Select Output Directory"
            };
            
            string result = await openFolderDialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                OutputDirectoryInput.Text = result;
            }
        }
    }
}