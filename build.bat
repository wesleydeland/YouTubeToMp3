@echo off
setlocal

echo YouTube to MP3 Converter - Build Script
echo ========================================

REM Check if .NET SDK is available
where dotnet >nul 2>nul
if errorlevel 1 (
    echo Error: .NET SDK is not installed or not in PATH
    echo Please install .NET SDK 8.0 or later from https://dotnet.microsoft.com/download
    exit /b 1
)

REM Change to the project directory
cd /d "%~dp0\YouTubeToMp3\YouTubeToMp3"

echo Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo Error: Failed to restore packages
    exit /b 1
)

echo Building project...
dotnet build -c Release
if errorlevel 1 (
    echo Error: Build failed
    exit /b 1
)

echo.
echo Available platforms for publishing:
echo 1) Windows x64   - win-x64
echo 2) Windows x86   - win-x86
echo 3) Windows Arm64 - win-arm64
echo 4) Current OS    - (Press Enter)
echo.

set /p PLATFORM="Enter platform choice (or press Enter for current OS): "
if "%PLATFORM%"=="1" set PLATFORM=win-x64
if "%PLATFORM%"=="2" set PLATFORM=win-x86
if "%PLATFORM%"=="3" set PLATFORM=win-arm64
if "%PLATFORM%"=="4" set PLATFORM=""

if "%PLATFORM%"=="" (
    echo Building for current OS...
    dotnet publish -c Release --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
) else (
    echo Building for %PLATFORM%...
    dotnet publish -c Release -r %PLATFORM% --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
)

if errorlevel 0 (
    echo.
    echo Build completed successfully!
    if "%PLATFORM%"=="" (
        echo Output: bin\Release\net8.0\publish\
    ) else (
        echo Output: bin\Release\net8.0\%PLATFORM%\publish\
    )
    echo.
    echo The executable can be found in the publish directory.
) else (
    echo Build failed!
    exit /b 1
)

pause