#!/bin/bash

echo "Starting setup for ProjectManager... Checking for missing dependencies...."

# If it's a Mac, then check if Homebrew is installed. Note: 'darwin' refers to mac btw
if [[ "$OSTYPE" == "darwin"* ]]; then
    if ! command -v brew &> /dev/null; then
        echo "Your mac is missing: Homebrew. Installing homebrew."
        /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    fi
fi

# Then we start looking for depdendencies. 
echo "Installing project dependencies: .NET SDK, Git, and Node.js..."
# If it's a mac, then attempt to install .NET SDK, Git, and Node.js via homebrew
if [[ "$OSTYPE" == "darwin"* ]]; then
    brew install dotnet git node
# If they are on linux, then use admin privleges to execute the command to install our three dependencies
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    sudo apt update && sudo apt install -y dotnet-sdk-7.0 git nodejs
fi

# For a final touch restore .NET dependencies
echo "Restoring .NET dependencies..."
dotnet restore

# Then finally, check for VS Code and open the project
if command -v code &> /dev/null; then
    echo "Opening project in VS Code..."
    code .
else
    echo "VS Code not found. Please install it from https://code.visualstudio.com/"
fi

echo "Setup complete. Run 'dotnet watch run' to start the project in vscode's terminal to open the project in chrome."
