# Dev Container Troubleshooting Guide
## Common Issues and Solutions
### Docker & Connectivity
#### "Docker daemon is not running"
**Error**: `Cannot connect to the Docker daemon`
**Solutions**:
- **Windows/macOS**: Launch Docker Desktop
- **Linux**: Run `sudo systemctl start docker`
- Check Docker installation: `docker --version`
#### "Cannot connect to Docker socket (WSL2)"
**Error**: `Cannot connect to /var/run/docker.sock`
**Solutions** (Windows WSL2):
1. Ensure Docker Desktop has WSL2 integration enabled
2. In Docker Desktop → Settings → Resources → WSL Integration → toggle on
3. Restart Docker Desktop
4. Run `docker --version` in WSL2 terminal
#### "Docker takes too much disk space"
**Solutions**:
```bash
# Clean up unused images and containers
docker system prune -a --volumes
# Check disk usage
docker system df
```
---
### Build & Compilation
#### "dotnet: command not found"
**Error**: `/bin/bash: dotnet: command not found`
**Solutions**:
1. Ensure you're inside the Dev Container (check bottom-left in VS Code: should show `[Dev Container]`)
2. Verify .NET SDK is installed: `dotnet --info`
3. If still failing, rebuild the container: `F1` → "Remote-Containers: Rebuild Container"
#### "Build takes very long first time"
**Why**: NuGet packages are being downloaded and cached
**Solutions**:
- This is normal! First build takes 5-15 minutes depending on internet
- Subsequent builds are much faster (packages are cached)
- Check internet connection for package sources
- Increase Docker memory: Settings → Resources → Memory (min 8GB recommended)
#### "NuGet restore fails"
**Error**: `Unable to load the service index for source...`
**Solutions**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
# Try restore again
dotnet restore --interactive
# Check internet connectivity
curl -I https://api.nuget.org/v3/index.json
```
#### ".NET Framework version mismatch"
**Error**: `This project targets framework '.NETXxx' which is not installed`
**Solutions**:
```bash
# Check available .NET versions
dotnet --list-sdks
dotnet --list-runtimes
# Update within container
# The Dockerfile supports net8.0, net9.0, net10.0
```
---
### VS Code & IDE
#### "C# extension not working"
**Symptoms**: No IntelliSense, syntax highlighting issues
**Solutions**:
1. Wait for extension sync (watch bottom status bar in VS Code)
2. Run: `F1` → "Developer: Reload Window"
3. Check extension is installed in container: 
   - Open Extensions sidebar
   - Look for "ms-dotnettools.csharp"
   - Should show "Installed" next to it
#### "Projects not recognized by IDE"
**Symptoms**: Red squiggly lines, missing files
**Solutions**:
1. Rebuild container: `F1` → "Remote-Containers: Rebuild Container"
2. Run in terminal: `dotnet restore`
3. Try: `F1` → "OmniSharp: Restart OmniSharp" (if using OmniSharp)
#### "Format on Save not working"
**Solutions**:
1. Check settings in `.devcontainer/devcontainer.json`
2. Verify C# extension is active
3. Try manual format: `F1` → "Format Document"
---
### Permissions & File Access
#### "Permission denied" (Linux)
**Error**: `permission denied while trying to connect to Docker daemon`
**Solutions**:
```bash
# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker
# Verify
docker ps
```
#### "Cannot write to /workspace"
**Error**: `Permission denied` when creating/editing files
**Solutions**:
1. Check user inside container: `id` (should be `vscode` user)
2. Fix ownership from host:
   ```bash
   sudo chown -R $(id -u):$(id -g) /path/to/Candoumbe.Types
   ```
3. Rebuild container with `--privileged` flag (if absolutely necessary)
---
### Performance Issues
#### "Container is very slow"
**Solutions** (order by priority):
1. **Memory allocation**: 
   - Windows/macOS: Docker Desktop → Preferences → Resources
   - Set to at least 8 GB RAM
   - Set CPUs to at least 4
2. **Disk**: Ensure min 10 GB free space on Docker drive
3. **Volumes**: The setup uses named volumes which are more performant:
   - `candoumbe-types-nuget-cache` for NuGet packages
   - `candoumbe-types-dotnet-cache` for .NET files
4. **macOS specific**: 
   - Native file sharing is slower
   - Consider using Docker Desktop's new VirtioFS for better performance
   - Settings → Resources → File Sharing → Enable VirtioFS
#### "Hot reload / File sync delay (macOS/Windows)"
**Solutions**:
1. This is normal for Docker on macOS/Windows (file sync overhead)
2. Increase watch delay in settings if needed
3. On Linux, you'll see immediate file sync (native Docker)
---
### Tests & Debugging
#### "Tests fail with 'Framework not found'"
**Error**: `Test execution failed for project XYZ`
**Solutions**:
```bash
# Rebuild everything
dotnet clean
dotnet build
dotnet test
# If specific framework is missing
dotnet --list-sdks
```
#### "Stryker (mutation testing) fails"
**Error**: Issues running mutation tests
**Solutions**:
```bash
# Ensure stryker is installed
dotnet tool list --global
# Restore tools explicitly
dotnet tool restore
# Try again
dotnet stryker
```
#### "CodeCov upload fails"
**Error**: `Failed to upload coverage report`
**Solutions**:
1. Check environment variables (token, repo)
2. Verify internet connectivity
3. Check codecov.io account settings
4. Review GitHub Actions logs for CI context
---
### GitHub Integration
#### "GitHub CLI (gh) authentication fails"
**Error**: `Error: You are not authenticated`
**Solutions**:
```bash
# Inside container
gh auth login
# Follow prompts to authenticate
# Verify
gh auth status
```
#### "Git operations fail"
**Error**: `fatal: not a git repository` or permission issues
**Solutions**:
```bash
# Ensure you're in workspace
cd /workspace
# Verify git setup
git config --list
git status
# If needed, reinitialize git config
git config user.email "your.email@example.com"
git config user.name "Your Name"
```
---
### Platform-Specific Issues
#### Windows (WSL2)
- **Issue**: Slow file access
  - **Fix**: Use native WSL2 paths (`/home/user/...`), not Windows paths (`/mnt/c/...`)
- **Issue**: Container won't start
  - **Fix**: Ensure WSL2 is updated: `wsl --update`
#### macOS (Apple Silicon)
- **Issue**: Some images don't support ARM64
  - **Fix**: Our Dockerfile uses `jammy` which supports ARM64 natively
- **Issue**: Very slow performance
  - **Fix**: Increase Docker Desktop memory allocation (min 8GB, recommended 16GB)
#### Linux
- **Issue**: Permission errors
  - **Fix**: Run `sudo usermod -aG docker $USER`
- **Issue**: SELinux blocking access
  - **Fix**: Configure SELinux to allow Docker: `sudo semanage fcontext -a -t container_file_t "/path/to/project(/.*)?" && sudo restorecon -R /path/to/project`
---
## Getting Help
If you still have issues:
1. **Check logs**:
   ```bash
   # VS Code terminal output
   # Docker Desktop logs
   # Container logs: docker logs candoumbe-types-dev
   ```
2. **Search GitHub issues**: https://github.com/candoumbe/Candoumbe.Types/issues
3. **Create new issue** with:
   - Error message (full stack trace)
   - OS and Docker version (`docker --version`)
   - What you were trying to do
   - Steps to reproduce
4. **Dev Container specs**: https://containers.dev/
---
## Quick Reference
```bash
# Common commands in Dev Container
# Rebuild container
F1 → "Remote-Containers: Rebuild Container"
# Reopen in container
F1 → "Remote-Containers: Reopen in Container"
# Clean and rebuild
dotnet clean && dotnet build
# Run all tests
dotnet test
# Run specific test project
dotnet test ./test/Candoumbe.Types.Calendar.UnitTests
# Restore tools
dotnet tool restore
# Check .NET info
dotnet --info
# Clear caches
docker volume rm candoumbe-types-nuget-cache candoumbe-types-dotnet-cache
```
