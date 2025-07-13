# Yönetici hakları kontrolü
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Warning "Bu script yönetici hakları gerektirir. Lütfen PowerShell'i yönetici olarak çalıştırın."
    Break
}

$serviceName = "CYSLivestockService"
$displayName = "CYS Livestock Management Service"
$description = "CYS Livestock Management System Windows Service"
$exePath = Join-Path $PSScriptRoot "bin\Release\net8.0\win-x64\publish\CYS.exe"

# Service'in var olup olmadığını kontrol et
$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($args[0] -eq "install") {
    if ($service) {
        Write-Host "Service zaten yüklü. Önce kaldırın."
        Exit 1
    }
    
    # Publish the application
    Write-Host "Uygulama publish ediliyor..."
    dotnet publish -c Release -r win-x64 --self-contained true
    
    # Create and start the service
    Write-Host "Windows Service yükleniyor..."
    New-Service -Name $serviceName -BinaryPathName $exePath -DisplayName $displayName -Description $description -StartupType Automatic
    Start-Service -Name $serviceName
    Write-Host "Service başarıyla yüklendi ve başlatıldı."
}
elseif ($args[0] -eq "uninstall") {
    if (-not $service) {
        Write-Host "Service yüklü değil."
        Exit 1
    }
    
    # Stop and remove the service
    Write-Host "Service durduruluyor ve kaldırılıyor..."
    Stop-Service -Name $serviceName -Force
    Start-Sleep -Seconds 2
    sc.exe delete $serviceName
    Write-Host "Service başarıyla kaldırıldı."
}
elseif ($args[0] -eq "start") {
    if (-not $service) {
        Write-Host "Service yüklü değil. Önce yükleyin."
        Exit 1
    }
    Start-Service -Name $serviceName
    Write-Host "Service başlatıldı."
}
elseif ($args[0] -eq "stop") {
    if (-not $service) {
        Write-Host "Service yüklü değil."
        Exit 1
    }
    Stop-Service -Name $serviceName
    Write-Host "Service durduruldu."
}
else {
    Write-Host "Kullanım: InstallService.ps1 [install|uninstall|start|stop]"
    Exit 1
} 