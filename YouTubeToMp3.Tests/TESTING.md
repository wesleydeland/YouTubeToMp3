# Testing Strategy for YouTube to MP3 Converter

## Overview
This document outlines the testing approach for the YouTube to MP3 Converter application, including unit tests, integration tests, and UI tests.

## Test Project Structure
- **YouTubeToMp3.Tests**: Main test project using xUnit framework
  - Unit tests for core functionality
  - UI tests using Avalonia.Headless.XUnit
  - Utility and helper method tests

## Types of Tests

### 1. Unit Tests (Non-UI)
Located in:
- `UnitTest1.cs`: URL validation, path checking, and regex pattern tests
- `ProcessTests.cs`: Process execution and argument generation tests  
- `UtilityTests.cs`: File system and path operations tests

These tests:
- Validate URL format and content
- Test the logic for finding yt-dlp executable
- Verify process start info generation
- Check file path operations

### 2. UI Tests
Located in:
- `UiTests.cs`: Visual element and UI interaction tests

These tests:
- Verify that necessary UI controls exist and are accessible
- Test initial UI state (e.g., default output directory)
- Test UI validation logic

### 3. UI Testing Capabilities

#### Available UI Tests:
1. **Control Existence Tests**:
   - Verify all named controls are present in MainWindow
   - Test TextBox controls (YouTube URL input, output directory)
   - Test Button controls (Download, Browse)
   - Test ProgressBar and Status TextBox

2. **State Validation Tests**:
   - Verify initial state of UI (default output directory set to user profile)
   - Test input validation logic
   - Verify UI response to valid/invalid URLs

3. **Visual Tree Tests**:
   - Ensure controls are properly arranged in the visual tree
   - Verify that FindControl can locate named elements

#### UI Testing Framework:
- Uses `Avalonia.Headless.XUnit` for UI testing without a display
- `AvaloniaFact` attribute for tests requiring Avalonia runtime

## Running Tests

### All Tests:
```bash
dotnet test
```

### Test with Detailed Output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Specific Test Category:
```bash
dotnet test --filter "FullyQualifiedName~UiTests"
```

## Test Coverage

### Currently Tested:
- URL validation logic (IsValidUrl method)
- Executable path detection (IsInPath, FindYtDlp methods)
- Process argument generation for yt-dlp
- UI control presence and initial state
- Regular expression patterns for progress parsing
- File system operations

### Potential Additional Tests:
- Integration tests for actual yt-dlp execution (would require mocking or test yt-dlp)
- File download validation tests
- Error handling in UI scenarios
- Progress update simulation tests

## UI Testing Best Practices

### Recommended UI Tests:
1. **Component Tests**: Verify UI controls exist and are accessible
2. **State Tests**: Test UI state changes based on input
3. **Integration Tests**: Test interaction between UI components and logic
4. **Layout Tests**: Verify UI layout and styling (limited without visual verification)

### Advanced UI Testing Options:
For more comprehensive UI testing, consider:
- **Avalonia Automation**: Using accessibility APIs for UI testing
- **Screen Recording Tools**: For visual regression testing
- **UI State Snapshots**: For verifying UI consistency
- **End-to-End Tests**: Using tools like Playwright for Avalonia if available

## Maintenance Notes

### Adding New Tests:
1. Add unit tests to existing files or create new test files as needed
2. Use `[Fact]` for synchronous tests and `[Fact]` with `async Task` for async tests
3. Use `[AvaloniaFact]` for tests requiring the Avalonia runtime
4. Follow Arrange-Act-Assert pattern for clear test structure

### Test Dependencies:
- The test project references the main application project
- Avalonia packages are included for UI testing
- xUnit framework provides the testing infrastructure