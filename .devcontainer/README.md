# Dev Container - Candoumbe.Types
Environnement de développement complet et unifié pour **Candoumbe.Types** qui fonctionne sur Windows, Linux et macOS.
## 🚀 Démarrage rapide
### Option 1 : VS Code (Recommandé)
1. Installez l'extension **Remote - Containers** dans VS Code
2. Ouvrez le projet Candoumbe.Types
3. Une notification apparaît : cliquez **"Reopen in Container"**
4. Attendez la construction (5-10 minutes la première fois)
### Option 2 : Docker Compose
```bash
cd .devcontainer
docker-compose up -d
docker-compose exec dev-container bash
cd /workspace && dotnet build
```
## 📋 Prérequis
- **VS Code** : https://code.visualstudio.com/
- **Docker Desktop** :
  - Windows : https://www.docker.com/products/docker-desktop (WSL2)
  - macOS : https://www.docker.com/products/docker-desktop
  - Linux : Docker Engine + Docker Compose
## 📦 Contenu
- ✅ .NET SDK 10.0 (netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0)
- ✅ Outils : Nuke, GitVersion, CodeCov, Stryker
- ✅ Extensions VS Code : C# Dev Kit, EditorConfig, GitLens
- ✅ CLI : Git, GitHub CLI, PowerShell
- ✅ Caches optimisés : NuGet, .NET
## 🔧 Commandes courantes
```bash
dotnet restore        # Restaurer dépendances
dotnet build          # Compiler
dotnet test           # Tests
dotnet clean          # Nettoyer
dotnet tool restore   # Outils globaux
```
## 🛠️ Dépannage
| Problème | Solution |
|----------|----------|
| Docker non trouvé | Installez Docker Desktop |
| Permissions (Linux) | `sudo usermod -aG docker $USER` |
| NuGet lent | Normal première exécution (cache persistant ensuite) |
| Extensions VS Code | Attendez la sync, rechargez la fenêtre |
## 🌍 Plateformes
✅ Windows (WSL2)  
✅ macOS (Intel & Apple Silicon)  
✅ Linux
## 📚 Fichiers
- `devcontainer.json` : Configuration VS Code Remote Containers
- `Dockerfile` : Image Docker personnalisée .NET 10.0
- `docker-compose.yml` : Orchestration Docker
- `README.md` : Cette documentation
## ℹ️ Notes
- Les volumes nommés optimisent la performance sur macOS/Windows
- Les variables d'environnement `DOTNET_*` désactivent la télémétrie
- Allouez min 4 CPU, 8 GB RAM, 10 GB disque
- Apple Silicon (M1/M2/M3) : Supporté natif via image ARM64
## 📖 Ressources
- [Dev Containers Documentation](https://containers.dev/)
- [Microsoft Dev Containers for .NET](https://github.com/devcontainers/images/tree/main/src/dotnet)
- [Candoumbe.Types Repository](https://github.com/candoumbe/Candoumbe.Types)
