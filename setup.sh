#!/bin/bash

echo "Starting setup for ProjectManager... Checking for missing dependencies...."

# If it's a Mac, then check if Homebrew is installed. Note: 'darwin' refers to mac
if [[ "$OSTYPE" == "darwin"* ]]; then
    if ! command -v brew &> /dev/null; then
        echo "Your mac is missing: Homebrew. Installing homebrew."
        /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    fi
fi

# Then we start looking for dependencies.
echo "Installing project dependencies: .NET SDK, Git, and Node.js..."
if [[ "$OSTYPE" == "darwin"* ]]; then
    brew install dotnet git node
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    sudo apt update && sudo apt install -y dotnet-sdk-7.0 git nodejs
fi

# Add Entity Framework Core SQLite provider
echo "Adding EF Core SQLite package..."
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Restore any missing .NET dependencies
echo "Restoring .NET dependencies..."
dotnet restore

# Open the project in VS Code if installed
if command -v code &> /dev/null; then
    echo "Opening project in VS Code..."
    code .
else
    echo "VS Code not found. Please install it from https://code.visualstudio.com/"
fi

echo "Setup complete. Run 'dotnet watch run' in the vscode terminal to launch the app."
