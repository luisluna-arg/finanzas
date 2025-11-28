# Fix for Windows Docker Desktop port forwarding issues
# Run this script as Administrator if ports are not accessible from host

Write-Host "=== Windows Docker Port Fix ==="
Write-Host ""

# Solution 1: Restart Docker Desktop networking
Write-Host "1. Restarting Docker Desktop (this usually fixes it)..."
Write-Host "   Please restart Docker Desktop manually from the system tray"
Write-Host "   Right-click Docker icon > Quit Docker Desktop"
Write-Host "   Then start it again"
Write-Host ""

# Solution 2: Reset port forwarding
Write-Host "2. Resetting port exclusions (run as Administrator)..."
$ports = @(5000, 5100, 6379, 5432)
foreach ($port in $ports) {
    try {
        netsh int ipv4 add excludedportrange protocol=tcp startport=$port numberofports=1 | Out-Null
        Write-Host "   Reserved port $port"
    } catch {
        Write-Host "   Port $port already reserved or needs admin rights"
    }
}
Write-Host ""

# Solution 3: Check if WSL2 is the issue
Write-Host "3. Testing alternative connection methods..."
Write-Host "   Try accessing: http://127.0.0.1:5000/swagger/index.html"
Write-Host "   Try accessing: http://[::1]:5000/swagger/index.html"
Write-Host ""

# Solution 4: Restart containers
Write-Host "4. To restart containers (often fixes Windows networking issues):"
Write-Host "   docker restart finances-shared-backend-1"
Write-Host "   docker restart finances-main-frontend-frontend-1"
Write-Host ""

# Solution 5: Reset Docker network
Write-Host "5. If nothing works, reset Docker network:"
Write-Host "   docker compose -p finances-main-frontend -f Infra/Local/Finances/docker-compose.yaml down"
Write-Host "   docker compose -p finances-shared -f Infra/Local/Shared/docker-compose.yaml down"
Write-Host "   docker network prune -f"
Write-Host "   wsl --shutdown"
Write-Host "   Then restart Docker Desktop and run start-finances-local.ps1 again"
Write-Host ""

Write-Host "=== Quick Fix (try this first) ==="
Write-Host "Press Enter to restart the containers now..."
Read-Host

docker restart finances-shared-backend-1
docker restart finances-main-frontend-frontend-1

Write-Host ""
Write-Host "Waiting 10 seconds for containers to restart..."
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "Testing connections..."
$backendWorks = Test-NetConnection -ComputerName localhost -Port 5000 -WarningAction SilentlyContinue
$frontendWorks = Test-NetConnection -ComputerName localhost -Port 5100 -WarningAction SilentlyContinue

if ($backendWorks.TcpTestSucceeded) {
    Write-Host "✓ Backend port 5000 is accessible"
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger/index.html" -UseBasicParsing -TimeoutSec 5
        Write-Host "✓ Backend HTTP is working!"
    } catch {
        Write-Host "✗ Backend HTTP failed: $($_.Exception.Message)"
        Write-Host "  This is a Windows Docker Desktop bug. Try restarting Docker Desktop."
    }
} else {
    Write-Host "✗ Backend port 5000 is not accessible"
}

if ($frontendWorks.TcpTestSucceeded) {
    Write-Host "✓ Frontend port 5100 is accessible"
} else {
    Write-Host "✗ Frontend port 5100 is not accessible"
}

Write-Host ""
Write-Host "If it still doesn't work, restart Docker Desktop from the system tray."
