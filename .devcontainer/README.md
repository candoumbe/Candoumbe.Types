# Dev Container - Candoumbe.Types

Complete and unified development environment for **Candoumbe.Types** that works on Windows, Linux, and macOS.

## 🚀 Quick Start

### Option 1: VS Code (Recommended)

1. Install the **Remote - Containers** extension in VS Code
2. Open the Candoumbe.Types project
3. A notification appears: click **"Reopen in Container"**
4. Wait for the build (5-10 minutes the first time)

### Option 2: Docker Compose

```bash
cd .devcontainer
docker-compose up -d
docker-compose exec dev-container bash
cd /workspace && dotnet build
```

## 📋 Prerequisites

- **VS Code** : https://code.visualstudio.com/
- **Docker Desktop** :
  - Windows : https://www.docker.com/products/docker-desktop (WSL2)
  - macOS : https://www.docker.com/products/docker-desktop
  - Linux : Docker Engine + Docker Compose

## 📦 Contents

- ✅ .NET SDK 10.0 (netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0)
- ✅ Tools: Nuke, GitVersion, CodeCov, Stryker
- ✅ VS Code Extensions: C# Dev Kit, EditorConfig, GitLens
- ✅ CLI: Git, GitHub CLI, PowerShell
- ✅ Optimized Caches: NuGet, .NET

## 🔧 Common Commands

```bash
dotnet restore        # Restore dependencies
dotnet build          # Build
dotnet test           # Run tests
dotnet clean          # Clean artifacts
dotnet tool restore   # Global tools
```

## 🛠️ Troubleshooting

| Issue | Solution |
|-------|----------|
| Docker not found | Install Docker Desktop |
| Permissions (Linux) | `sudo usermod -aG docker $USER` |
| NuGet slow | Normal on first run (persistent cache afterward) |
| VS Code extensions | Wait for sync, reload window |

## 🌍 Supported Platforms

✅ Windows (WSL2)  
✅ macOS (Intel & Apple Silicon)  
✅ Linux

## 📚 Files

- `devcontainer.json` : VS Code Remote Containers configuration
- `Dockerfile` : Custom .NET 10.0 Docker image
- `docker-compose.yml` : Docker orchestration
- `README.md` : This documentation

## ℹ️ Notes

- Named volumes optimize performance on macOS/Windows
- `DOTNET_*` environment variables disable telemetry
- Allocate minimum 4 CPU, 8 GB RAM, 10 GB disk
- Apple Silicon (M1/M2/M3): Natively supported via ARM64 image

## 📖 Resources

- [Dev Containers Documentation](https://containers.dev/)
- [Microsoft Dev Containers for .NET](https://github.com/devcontainers/images/tree/main/src/dotnet)
- [Candoumbe.Types Repository](https://github.com/candoumbe/Candoumbe.Types)