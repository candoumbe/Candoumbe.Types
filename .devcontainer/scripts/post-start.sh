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

# Update package manager
log_info "Updating package manager..."
sudo apt-get update > /dev/null 2>&1
log_success "Package manager updated."

# Install GitHub CLI if not present
if ! command_exists gh; then
    log_info "Installing GitHub CLI (gh)..."
    if sudo apt-get install -y gh > /dev/null 2>&1; then
        log_success "GitHub CLI installed successfully."
    else
        log_error "Failed to install GitHub CLI. Attempting alternative installation..."
        if curl --proto '=https' --tlsv1.2 -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg > /dev/null 2>&1 && \
           echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages focal main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null && \
           sudo apt-get update > /dev/null 2>&1 && \
           sudo apt-get install -y gh > /dev/null 2>&1; then
            log_success "GitHub CLI installed successfully (from archive)."
        else
            log_warning "Could not install GitHub CLI."
        fi
    fi
else
    log_success "GitHub CLI already installed."
fi

# Restore NuGet packages and build
log_info "Restoring NuGet packages and building Candoumbe.Types..."
./build.sh restore > /dev/null 2>&1
log_success "NuGet packages restored and build completed."

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
