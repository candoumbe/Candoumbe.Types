# Dev Container Architecture
## Overview
This Dev Container configuration provides a complete, reproducible development environment for Candoumbe.Types that works identically across Windows, macOS, and Linux.
## Architecture Diagram
```
┌─────────────────────────────────────────────────────────┐
│                   Host Machine                          │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │            Docker Desktop / Engine               │  │
│  │  ┌────────────────────────────────────────────┐ │  │
│  │  │        Dev Container Image                 │ │  │
│  │  │  ┌──────────────────────────────────────┐ │ │  │
│  │  │  │  Debian 12 (Jammy)                  │ │ │  │
│  │  │  │  - .NET SDK 10.0                    │ │ │  │
│  │  │  │  - Git / GitHub CLI                 │ │ │  │
│  │  │  │  - PowerShell Core                  │ │ │  │
│  │  │  │  - Build Tools                      │ │ │  │
│  │  │  └──────────────────────────────────────┘ │ │  │
│  │  │                                            │ │  │
│  │  │  ┌──────────────────────────────────────┐ │ │  │
│  │  │  │      Volumes (Named Volumes)         │ │ │  │
│  │  │  │  - candoumbe-types-nuget-cache      │ │ │  │
│  │  │  │  - candoumbe-types-dotnet-cache     │ │ │  │
│  │  │  └──────────────────────────────────────┘ │ │  │
│  │  │                                            │ │  │
│  │  │  ┌──────────────────────────────────────┐ │ │  │
│  │  │  │      Mount Points                    │ │ │  │
│  │  │  │  /workspace → Project Root           │ │ │  │
│  │  │  │  ~/.nuget → NuGet Cache             │ │ │  │
│  │  │  │  ~/.dotnet → .NET Tools             │ │ │  │
│  │  │  └──────────────────────────────────────┘ │ │  │
│  │  └────────────────────────────────────────────┘ │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │        VS Code (Host Machine)                    │  │
│  │  Remote - Containers Extension                  │  │
│  │  Connects to Dev Container via Docker socket    │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```
## Components
### 1. Base Image
- **Image**: `mcr.microsoft.com/devcontainers/dotnet:1-10.0-jammy`
- **OS**: Debian 12 (Jammy) - lightweight and stable
- **SDK**: .NET SDK 10.0.103+ (supports net8.0, net9.0, net10.0, netstandard2.0, netstandard2.1)
- **Architecture**: Supports Linux x86_64, ARM64 (Apple Silicon), and WSL2
### 2. Docker Customization (Dockerfile)
Additional tools installed:
- **Git**: Version control
- **GitHub CLI (gh)**: GitHub operations
- **PowerShell Core**: Cross-platform scripting
- **cURL, wget**: Network utilities
- **ca-certificates, gnupg**: Security/PKI
**User**: Creates non-root `vscode` user for security
### 3. VS Code Integration (devcontainer.json)
#### Extensions Auto-installed
- `ms-dotnettools.csharp` - C# Dev Kit (primary IDE support)
- `ms-dotnettools.vscode-dotnet-runtime` - .NET runtime support
- `EditorConfig.EditorConfig` - EditorConfig support
- `eamodio.gitlens` - Git visualization
- `GitHub.copilot` - AI code assistant
- `ms-vscode.makefile-tools` - Makefile support
- `ms-vscode-remote.remote-containers` - Remote container support
#### Auto Configuration
- Formatter: C# Dev Kit (format on save)
- Roslyn Analyzers: Enabled for code quality
- EditorConfig: Enabled for consistency
- OmniSharp: Enabled for language features
### 4. Volumes (Named Volumes)
Why named volumes instead of bind mounts?
- **Better performance** on Docker Desktop (macOS/Windows)
- **Persistence** across container restarts
- **Optimization** for package caching
```yaml
candoumbe-types-nuget-cache
  ↓ Contains
  ~/.nuget/packages/     # NuGet package cache (speeds up restore)
candoumbe-types-dotnet-cache
  ↓ Contains
  ~/.dotnet/             # .NET global tools and runtime cache
```
### 5. Mount Points
```
Host                          Container
└── /project/root/       →     /workspace/
    ├── src/             →     /workspace/src/
    ├── test/            →     /workspace/test/
    ├── .git/            →     /workspace/.git/
    └── ...              →     ...
Host ~/.nuget/packages/ →     Container ~/.nuget/packages/
Host ~/.dotnet/         →     Container ~/.dotnet/
```
## Workflow
### Initial Setup (First Time)
1. User opens project in VS Code
2. VS Code detects `.devcontainer/devcontainer.json`
3. User clicks "Reopen in Container"
4. Docker builds image from Dockerfile (5-10 min)
5. Container starts with mounted volumes
6. `postCreateCommand` runs: `dotnet restore && dotnet tool restore`
7. `postStartCommand` runs: `dotnet build`
8. Extensions auto-install and configure
9. Dev container ready! ✅
### Subsequent Runs
1. Container starts (much faster - image cached)
2. Caches already present in named volumes
3. `postStartCommand` runs quick build
4. Ready to develop in seconds ⚡
### Daily Development
```
User edits file in VS Code
         ↓
File synced to /workspace (via Docker bind mount)
         ↓
IDE recognizes change (IntelliSense updates)
         ↓
User runs commands in integrated terminal
         ↓
Commands execute inside container (with full .NET SDK)
         ↓
Build artifacts written to /workspace/bin/obj (synced back)
```
## Performance Optimization
### Cache Strategy
1. **Named volumes** for NuGet packages and .NET tools
   - Persists between restarts
   - Optimized I/O on all platforms
2. **Pre-caching** in Dockerfile
   - During build, a basic .NET project is restored
   - Common packages already present
   - First `dotnet restore` is faster
3. **Lazy loading** of extensions
   - Extensions download on first container start
   - Cached for subsequent starts
### Platform-Specific Optimizations
**Windows (WSL2)**
- Uses volume mounts instead of bind mounts for better performance
- Direct Docker integration with WSL2 kernel
**macOS**
- Named volumes avoid slow file sync overhead
- Support for Apple Silicon (ARM64) native execution
- VirtioFS option available in newer Docker Desktop for faster I/O
**Linux**
- Native Docker performance (no virtualization overhead)
- Direct filesystem access with minimal latency
- FUSE-based caching available if needed
## Security Considerations
1. **Non-root user**: Container runs as `vscode` user (not root)
2. **Volume mounts**: Project mounted read-write, system files read-only
3. **Network isolation**: No exposed ports by default (can be configured)
4. **Environment variables**: Sensitive data handled via `.env` (gitignored)
## File Structure
```
.devcontainer/
├── devcontainer.json       # Main configuration (JSON - VS Code standard)
├── Dockerfile              # Custom image build
├── docker-compose.yml      # Orchestration (optional CLI usage)
├── .dockerignore           # Exclude files from Docker build context
├── .gitignore              # Git-specific ignores for .devcontainer
├── .env.example            # Example environment variables
├── README.md               # Quick start & overview
├── QUICKSTART.md           # 2-minute setup guide
├── ARCHITECTURE.md         # This file
├── TROUBLESHOOTING.md      # Common issues & solutions
└── scripts/
    ├── build.sh            # Build helper script
    ├── test.sh             # Test helper script
    └── clean.sh            # Cleanup helper script
```
## Extensibility
### Adding Tools to Dockerfile
Example: Add Node.js for web development
```dockerfile
# Add this to Dockerfile after the PowerShell install section:
RUN apt-get update && apt-get install -y \
    nodejs \
    npm \
    && rm -rf /var/lib/apt/lists/*
```
Then rebuild: `F1` → "Remote-Containers: Rebuild Container"
### Adding VS Code Extensions
Edit `devcontainer.json`, extensions array:
```json
"extensions": [
  "ms-dotnettools.csharp",
  "new-extension-id-here"
]
```
Extensions auto-install on next container start.
### Custom Environment Variables
Create `.devcontainer/.env`:
```
MY_CUSTOM_VAR=value
NUGET_VERBOSITY=detailed
```
Referenced in docker-compose.yml via `env_file: .env`
## Troubleshooting Architecture
### Image Layer Caching
- If Dockerfile changes, only affected layers rebuild
- System tools cached from first run
- .NET SDK layers cached
### Volume Debugging
```bash
# List volumes
docker volume ls | grep candoumbe-types
# Inspect volume
docker volume inspect candoumbe-types-nuget-cache
# Clean volumes (if needed)
docker volume rm candoumbe-types-nuget-cache
```
### Network Debugging
```bash
# Inside container
docker network ls
docker inspect bridge
```
## Comparison: Local vs Dev Container
| Aspect | Local Setup | Dev Container |
|--------|---|---|
| Setup time | 30+ min | 10-15 min first run |
| Version conflicts | Possible | Impossible |
| Multiple projects | Complex | Isolated |
| OS differences | Manual handling | Automatic |
| Clean up | Manual | One folder delete |
| CI/CD parity | Manual alignment | Identical to CI |
| Onboarding | Instructions required | Clone + run |
| Team consistency | Training needed | Automatic |
## Resources
- [Dev Containers Specification](https://containers.dev/)
- [Microsoft .NET Images](https://github.com/devcontainers/images/tree/main/src/dotnet)
- [Docker Documentation](https://docs.docker.com/)
- [VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)
