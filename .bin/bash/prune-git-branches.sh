#!/bin/bash
# Git Branch Pruning Script
# Prune git branches by removing local branches that no longer exist on the remote.

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default values
PRUNE_MERGED=false
FORCE=false

# Function to show help
show_help() {
    echo -e "${BLUE}🌿 Git Branch Pruning Script${NC}"
    echo "=============================="
    echo ""
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -m, --merged     Also remove local branches that have been merged"
    echo "  -f, --force      Skip confirmation prompts (use with caution)"
    echo "  -h, --help       Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                    # Basic pruning of remote tracking branches"
    echo "  $0 --merged           # Prune remote branches and remove merged local branches"
    echo "  $0 --merged --force   # Prune all branches without confirmation"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -m|--merged)
            PRUNE_MERGED=true
            shift
            ;;
        -f|--force)
            FORCE=true
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo -e "${RED}❌ Unknown option: $1${NC}"
            show_help
            exit 1
            ;;
    esac
done

echo -e "${BLUE}🌿 Git Branch Pruning Script${NC}"
echo "=============================="
echo ""

# Check if we're in a git repository
if [ ! -d ".git" ]; then
    echo -e "${RED}❌ Error: Not in a git repository${NC}"
    exit 1
fi

# Get current branch
CURRENT_BRANCH=$(git branch --show-current)
echo -e "${BLUE}Current branch: ${GREEN}$CURRENT_BRANCH${NC}"

# Fetch with prune to update remote tracking branches
echo -e "\n${YELLOW}📡 Fetching and pruning remote branches...${NC}"
if ! git fetch --all --prune; then
    echo -e "${RED}❌ Error fetching from remote${NC}"
    exit 1
fi

# Remove local branches that track deleted remote branches
echo -e "\n${YELLOW}🧹 Cleaning up local tracking branches...${NC}"
GONE_BRANCHES=$(git for-each-ref --format="%(refname:short) %(upstream:track)" refs/heads | grep '\[gone\]' | awk '{print $1}' || true)

if [ -n "$GONE_BRANCHES" ]; then
    echo -e "${RED}Branches tracking deleted remotes:${NC}"
    echo "$GONE_BRANCHES" | while read -r branch; do
        echo "  - $branch"
    done
    
    if [ "$FORCE" = true ]; then
        CONFIRM="y"
    else
        echo -n -e "\nRemove these branches? (y/N): "
        read -r CONFIRM
    fi
    
    if [ "$CONFIRM" = "y" ] || [ "$CONFIRM" = "Y" ]; then
        echo "$GONE_BRANCHES" | while read -r branch; do
            if [ "$branch" = "$CURRENT_BRANCH" ]; then
                echo -e "${YELLOW}⚠️  Skipping current branch: $branch${NC}"
                continue
            fi
            echo -e "${RED}🗑️  Removing: $branch${NC}"
            git branch -D "$branch"
        done
    fi
else
    echo -e "${GREEN}✅ No local branches tracking deleted remotes${NC}"
fi

# Optional: Remove merged branches
if [ "$PRUNE_MERGED" = true ]; then
    echo -e "\n${YELLOW}🔍 Finding merged branches...${NC}"
    
    # Get default branch (usually main or master)
    DEFAULT_BRANCH=$(git symbolic-ref refs/remotes/origin/HEAD 2>/dev/null | sed 's@^refs/remotes/origin/@@' || echo "main")
    if [ -z "$DEFAULT_BRANCH" ]; then
        DEFAULT_BRANCH="main"
        echo -e "${YELLOW}⚠️  Could not determine default branch, using 'main'${NC}"
    fi
    
    # Find merged branches (excluding current and default branch)
    MERGED_BRANCHES=$(git branch --merged "$DEFAULT_BRANCH" | grep -v "^\*" | grep -v "^  $DEFAULT_BRANCH$" | grep -v "^  master$" | grep -v "^  main$" | grep -v "^  $CURRENT_BRANCH$" | sed 's/^  //' || true)
    
    if [ -n "$MERGED_BRANCHES" ]; then
        echo -e "${RED}Merged branches (safe to delete):${NC}"
        echo "$MERGED_BRANCHES" | while read -r branch; do
            echo "  - $branch"
        done
        
        if [ "$FORCE" = true ]; then
            CONFIRM="y"
        else
            echo -n -e "\nRemove these merged branches? (y/N): "
            read -r CONFIRM
        fi
        
        if [ "$CONFIRM" = "y" ] || [ "$CONFIRM" = "Y" ]; then
            echo "$MERGED_BRANCHES" | while read -r branch; do
                echo -e "${RED}🗑️  Removing merged branch: $branch${NC}"
                git branch -d "$branch"
            done
        fi
    else
        echo -e "${GREEN}✅ No merged branches to remove${NC}"
    fi
fi

# Show remaining local branches
echo -e "\n${BLUE}📋 Remaining local branches:${NC}"
git branch -vv

echo -e "\n${GREEN}✅ Branch pruning complete!${NC}"