# Use official Microsoft .NET SDK image as base
# Supports Linux, macOS (via Docker Desktop), and Windows (WSL2/Hyper-V)
FROM mcr.microsoft.com/dotnet/sdk:10.0
# Install additional system tools
RUN apt-get update && apt-get install -y \
    git \
    curl \
    wget \
    ca-certificates \
    gnupg \
    lsb-release \
    apt-transport-https \
    && rm -rf /var/lib/apt/lists/*

# Install PowerShell as a .NET tool (accessible to all users)

RUN dotnet tool install --tool-path /usr/local/bin PowerShell

# Install GitHub CLI
RUN curl -fsSLo /usr/share/keyrings/githubcli-archive-keyring.gpg https://cli.github.com/packages/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt-get update && apt-get install -y gh \
    && rm -rf /var/lib/apt/lists/*
# Set environment variables for .NET CLI
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1
# Create non-root user (vscode) for development
RUN useradd -m -s /bin/bash vscode \
    && mkdir -p /home/vscode/.nuget/packages \
    && mkdir -p /home/vscode/.dotnet \
    && chown -R vscode:vscode /home/vscode
# Switch to non-root user for pre-caching
USER vscode
# Pre-cache common .NET packages by restoring a basic project
WORKDIR /tmp/dotnet-cache
RUN dotnet new console && dotnet restore && rm -rf /tmp/dotnet-cache

# Switch back to root so devcontainer tooling can remap vscode UID to match host user
USER root
WORKDIR /workspace
