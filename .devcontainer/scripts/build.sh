#!/bin/bash
# Build script for Dev Container
set -e
echo "🔨 Building Candoumbe.Types solution..."
cd /workspace
if [ ! -f "Candoumbe.Types.sln" ]; then
    echo "❌ Error: Candoumbe.Types.sln not found!"
    exit 1
fi
dotnet clean
dotnet restore
dotnet build
echo "✅ Build completed successfully!"
