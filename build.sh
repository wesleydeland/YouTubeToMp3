#!/bin/bash

# Build script for YouTube to MP3 Converter

echo "YouTube to MP3 Converter - Build Script"
echo "========================================"

# Check if .NET SDK is available
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed or not in PATH"
    echo "Please install .NET SDK 8.0 or later from https://dotnet.microsoft.com/download"
    exit 1
fi

# Get the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Change to the project directory
cd "$SCRIPT_DIR/YouTubeToMp3/YouTubeToMp3"

echo "Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "Error: Failed to restore packages"
    exit 1
fi

echo "Building project..."
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo "Error: Build failed"
    exit 1
fi

echo ""
echo "Available platforms for publishing:"
echo "1) macOS x64     - osx-x64"
echo "2) macOS ARM64   - osx-arm64  (Apple Silicon)"
echo "3) Windows x64   - win-x64"
echo "4) Linux x64     - linux-x64"
echo ""

read -p "Enter platform (or press Enter for current OS): " PLATFORM

case $PLATFORM in
    1) PLATFORM="osx-x64" ;;
    2) PLATFORM="osx-arm64" ;;
    3) PLATFORM="win-x64" ;;
    4) PLATFORM="linux-x64" ;;
    "") PLATFORM="" ;;
    *) echo "Invalid selection. Using auto-detection." ;;
esac

if [ -z "$PLATFORM" ]; then
    echo "Building for current OS..."
    dotnet publish -c Release --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
else
    echo "Building for $PLATFORM..."
    dotnet publish -c Release -r $PLATFORM --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
fi

if [ $? -eq 0 ]; then
    echo ""
    echo "Build completed successfully!"
    if [ -z "$PLATFORM" ]; then
        echo "Output: bin/Release/net8.0/publish/"
    else
        echo "Output: bin/Release/net8.0/$PLATFORM/publish/"
    fi
    echo ""
    echo "The executable can be found in the publish directory."
else
    echo "Build failed!"
    exit 1
fi