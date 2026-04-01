#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper function for formatted output
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Main configuration section
log_info "=========================================="
log_info "Candoumbe.Types DevContainer Post-Start Setup"
log_info "=========================================="

# Check GitHub CLI availability (installed via devcontainer feature)
if command_exists gh; then
    log_success "GitHub CLI (gh) is available."
else
    log_warning "GitHub CLI (gh) is not available. Ensure it is installed via the devcontainer image or postCreate script."
fi

# Restore NuGet packages and build
log_info "Restoring NuGet packages and building Candoumbe.Types..."
if output=$(./build.sh restore 2>&1); then
    log_success "NuGet packages restored and build completed."
else
    log_error "NuGet packages restore/build failed. Output:"
    echo "$output"
    exit 1
fi

# Check GitHub authentication status
log_info ""
log_info "Checking GitHub authentication status..."
if gh auth status > /dev/null 2>&1; then
    log_success "You are authenticated with GitHub."
    GH_USERNAME=$(gh api user -q '.login' 2>/dev/null || echo "unknown")
    log_info "Logged in as: $GH_USERNAME"
else
    log_warning "You are not authenticated with GitHub."
    log_info "To authenticate, run:"
    log_info "  gh auth login"
fi

# Final setup instructions
log_info ""
log_info "=========================================="
log_success "Post-start setup complete!"
log_info "=========================================="
log_info ""
log_info "Next steps:"
log_info "1. Authenticate with GitHub (if not already done):"
log_info "   gh auth login"
log_info ""
log_info "2. Start developing!"
log_info ""
