#!/bin/bash
# Clean script for Dev Container
echo "🧹 Cleaning build artifacts..."
cd /workspace
dotnet clean
find . -name "bin" -o -name "obj" | xargs rm -rf
find . -name ".vs" | xargs rm -rf
echo "✅ Cleanup completed!"
