# YouTube to MP3 Converter

A cross-platform desktop application to convert YouTube videos to MP3 audio files using yt-dlp.

## Features

- Paste YouTube URL and convert to MP3
- Cross-platform (Windows, macOS, Linux)
- Simple and intuitive UI
- Progress tracking and status updates
- Custom output directory selection

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) installed and available in your PATH

### Installing yt-dlp

#### On macOS:
```bash
brew install yt-dlp
```

#### On Windows:
```cmd
winget install yt-dlp.yt-dlp
```

#### On Linux:
```bash
sudo curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp
sudo chmod a+rx /usr/local/bin/yt-dlp
```

## Building and Running

1. Clone or download this repository
2. Navigate to the project directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## Building for Distribution

To build a self-contained executable for your current platform:

```bash
dotnet publish -c Release -r [runtime-identifier] --self-contained true -p:PublishSingleFile=true
```

Where `[runtime-identifier]` is:
- `win-x64` for 64-bit Windows
- `osx-x64` for 64-bit macOS
- `osx-arm64` for Apple Silicon macOS
- `linux-x64` for 64-bit Linux

## Usage

1. Paste a YouTube URL into the input field
2. Select an output directory (defaults to user profile directory)
3. Click "Download MP3"
4. Wait for the conversion to complete
5. Check the output directory for your MP3 file

## License

This project is licensed under the MIT License - see the LICENSE file for details.