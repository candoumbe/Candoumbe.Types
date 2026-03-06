#!/bin/bash
# Test script for Dev Container
set -e
echo "🧪 Running tests for Candoumbe.Types solution..."
cd /workspace
if [ ! -f "Candoumbe.Types.sln" ]; then
    echo "❌ Error: Candoumbe.Types.sln not found!"
    exit 1
fi
./build.sh unit-tests
echo "✅ All tests completed!"
