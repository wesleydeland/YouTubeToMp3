# YouTube to MP3 Converter - Unit Tests

This project includes unit tests to verify the functionality of the YouTube to MP3 Converter application.

## Test Categories

### MainWindow Tests
- **URL Validation**: Tests for the `IsValidUrl` method to ensure it correctly identifies valid YouTube URLs
- **Path Resolution**: Tests for the `IsInPath` method to verify executable path checking functionality
- **Progress Parsing**: Tests that verify the regex patterns used to parse progress from yt-dlp output

### Process Tests
- **Process Creation**: Tests that verify correct ProcessStartInfo creation for yt-dlp execution
- **Command Line Arguments**: Tests for proper argument formatting when executing yt-dlp

### Utility Tests
- **Directory Validation**: Tests for directory existence checking functionality
- **Path Operations**: Tests for path combination and validation

## Running Tests

To run the tests, use the following command:

```bash
dotnet test
```

To run tests with detailed output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Coverage

The tests provide coverage for:
- Input validation logic
- URL parsing and validation
- Process execution preparation
- Basic file system operations
- Regular expression patterns for progress parsing

## Known Limitations

- UI-specific functionality (like updating progress bars) is tested indirectly via regex patterns
- Integration with actual yt-dlp executable is not tested in unit tests (would require integration tests)
- File system operations are tested using temporary/known paths