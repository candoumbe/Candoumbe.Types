# Dev Container Documentation Index
## 📍 Start Here
### 🎯 I want to...
#### **Get started in 2 minutes**
→ Read [QUICKSTART.md](QUICKSTART.md)
#### **Understand the setup**
→ Read [README.md](README.md)
#### **Learn technical details**
→ Read [ARCHITECTURE.md](ARCHITECTURE.md)
#### **Fix a problem**
→ Read [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
#### **See what was implemented**
→ Read [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
---
## 📚 Documentation Files Guide
### **README.md** (Start here if unsure)
- 🚀 Quick start guide (2 options: VS Code & Docker Compose)
- 📋 Prerequisites by platform (Windows, macOS, Linux)
- 📦 What's included (SDK, tools, extensions)
- 🔧 Common commands
- 🌍 Platform compatibility notes
**Best for**: General overview, getting started
---
### **QUICKSTART.md** (Fastest path)
- ⚡ 2-minute minimal setup
- 💻 Just the essentials
- No deep explanations
**Best for**: Experienced developers, quick reference
---
### **ARCHITECTURE.md** (Deep understanding)
- 🏗️ Architecture diagrams and component breakdown
- 🔄 Workflow explanations (initial setup, daily development)
- ⚙️ Performance optimization details
- 🔒 Security considerations
- 🧩 How to extend/customize
**Best for**: Architects, maintainers, advanced customization
---
### **TROUBLESHOOTING.md** (When something breaks)
- 🐛 20+ common issues with solutions
- 📋 Organized by category:
  - Docker & Connectivity
  - Build & Compilation
  - VS Code & IDE
  - Permissions & File Access
  - Performance Issues
  - Tests & Debugging
  - GitHub Integration
  - Platform-specific (Windows, macOS, Linux)
- 🔍 Quick reference section
**Best for**: Debugging problems, fixing errors
---
### **IMPLEMENTATION_SUMMARY.md** (Feature overview)
- ✅ What was created (all files & scripts)
- 📋 Features provided
- 🌍 Platform support
- 🎯 Next steps for customization
- 📚 Documentation map
- ✨ Benefits summary
**Best for**: Project leads, understanding full scope, team briefing
---
## 🗂️ Configuration Files
### **devcontainer.json** (VS Code configuration)
Main configuration file following the [Dev Containers Specification](https://containers.dev/).
- Image: `mcr.microsoft.com/devcontainers/dotnet:1-10.0-jammy`
- 7 VS Code extensions pre-configured
- Auto-run commands
- Volume definitions
- Resource requirements
**Edit this to**:
- Add more VS Code extensions
- Change resource limits
- Modify auto-run commands
---
### **Dockerfile** (Custom Docker image)
Builds on Microsoft .NET SDK 10.0 base image with:
- Git, GitHub CLI, PowerShell Core
- Non-root `vscode` user
- Pre-cached .NET packages
- Multi-platform support (Linux, macOS ARM64, WSL2)
**Edit this to**:
- Add system tools
- Install additional software
- Modify pre-caching behavior
---
### **docker-compose.yml** (Docker orchestration)
Defines container services, volumes, and environment variables.
Useful for CLI-based container management.
**Edit this to**:
- Add more services (databases, etc.)
- Change volume definitions
- Add ports or expose services
---
## 🛠️ Helper Scripts
### **scripts/build.sh**
Automated build script:
```bash
./scripts/build.sh
```
Does: `dotnet clean → dotnet restore → dotnet build`
---
### **scripts/test.sh**
Automated test script:
```bash
./scripts/test.sh
```
Runs all unit tests with proper verbosity.
---
### **scripts/clean.sh**
Cleanup script:
```bash
./scripts/clean.sh
```
Removes: `bin/`, `obj/`, `.vs/`, and other build artifacts.
---
## 🔑 Key Files Reference
| File | Purpose | Edit if... |
|------|---------|-----------|
| `devcontainer.json` | VS Code configuration | Need more extensions or different setup |
| `Dockerfile` | Docker image | Need different tools or SDKs |
| `docker-compose.yml` | Container orchestration | Need services or different volumes |
| `.dockerignore` | Build context | Want to exclude more files |
| `.gitignore` | Git ignoring | Want to ignore more files |
| `.env.example` | Environment template | Need different env variables |
---
## 📖 Reading Paths
### Path 1: Just get started
1. README.md (5 min)
2. Start developing
### Path 2: Understand everything
1. README.md (5 min)
2. QUICKSTART.md (2 min)
3. ARCHITECTURE.md (15 min)
### Path 3: Troubleshoot
1. TROUBLESHOOTING.md (search your issue)
2. Try solution
3. Open GitHub issue if still stuck
### Path 4: Team briefing
1. IMPLEMENTATION_SUMMARY.md (10 min)
2. Share documentation links
3. Team members follow Path 1
---
## ⚡ Quick Commands
From inside container:
```bash
# Build
./scripts/build.sh    # or: dotnet build
# Test
./scripts/test.sh     # or: dotnet test
# Clean
./scripts/clean.sh    # or: dotnet clean
# Restore dependencies
dotnet restore
# Check .NET info
dotnet --info
```
From host (when using docker-compose):
```bash
# Start container
docker-compose up -d
# Execute command in container
docker-compose exec dev-container bash
# Stop container
docker-compose down
# View logs
docker-compose logs -f
```
---
## 🆘 I Need Help
1. **Issue not listed in TROUBLESHOOTING.md?**
   → Check `.devcontainer/` files for configuration details
   → Review ARCHITECTURE.md for technical background
2. **Still stuck?**
   → Create GitHub issue with:
     - Error message (full stack trace)
     - Your OS and Docker version
     - Steps to reproduce
     - Which documentation you consulted
3. **Want to customize?**
   → ARCHITECTURE.md section "Extensibility"
   → Edit Dockerfile or devcontainer.json
   → Rebuild with `F1` → "Remote-Containers: Rebuild Container"
---
## 📱 Platform-Specific Guides
**Windows (WSL2)**
- See: README.md → "Windows (WSL2 recommended)"
- See: TROUBLESHOOTING.md → "Windows (WSL2)"
**macOS (Intel & Apple Silicon)**
- See: README.md → "macOS (Intel and Apple Silicon)"
- See: TROUBLESHOOTING.md → "macOS (Apple Silicon)"
**Linux**
- See: README.md → "Linux"
- See: TROUBLESHOOTING.md → "Linux"
---
## 🎯 Common Tasks
### "I want to add a new VS Code extension"
1. Edit `devcontainer.json`
2. Find `"extensions"` array
3. Add extension ID: `"publisher.extension-name"`
4. Save and wait for VS Code to auto-sync
### "I want to install a system tool"
1. Edit `Dockerfile`
2. Add to apt-get install section
3. Save and rebuild: `F1` → "Remote-Containers: Rebuild Container"
### "I want to customize environment variables"
1. Copy `.env.example` → `.env`
2. Edit `.env` with your values
3. Referenced by `docker-compose.yml`
### "I'm getting permission errors"
→ See TROUBLESHOOTING.md → "Permission denied" sections
### "Everything is slow"
→ See TROUBLESHOOTING.md → "Container is very slow"
---
## 📚 External Resources
- [Dev Containers Specification](https://containers.dev/)
- [VS Code Remote Containers Documentation](https://code.visualstudio.com/docs/remote/containers)
- [Microsoft .NET Dev Container Images](https://github.com/devcontainers/images/tree/main/src/dotnet)
- [Docker Documentation](https://docs.docker.com/)
---
**Last Updated**: February 2026
**Status**: ✅ Production Ready
