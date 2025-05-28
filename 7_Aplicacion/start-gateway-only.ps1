# Script para iniciar solo el API Gateway
Write-Host "=== Iniciando API Gateway - Sistema Etapa de Juicio ===" -ForegroundColor Green

$gatewayPath = "c:\Users\genoc\OneDrive\Documentos\Maestria\ConstrucciónDeSoftware\PAC\7_Aplicacion\EtapaDeJuicio.API.Gateway"

# Verificar que el directorio existe
if (-not (Test-Path $gatewayPath)) {
    Write-Host "ERROR: No se encontró el directorio del gateway: $gatewayPath" -ForegroundColor Red
    Read-Host "Presiona Enter para salir..."
    exit 1
}

Write-Host "Directorio del gateway: $gatewayPath" -ForegroundColor Gray
Write-Host "Cambiando al directorio del gateway..." -ForegroundColor Yellow

Set-Location $gatewayPath

Write-Host "Verificando archivos del proyecto..." -ForegroundColor Yellow
if (Test-Path "EtapaDeJuicio.API.Gateway.csproj") {
    Write-Host "✓ Archivo de proyecto encontrado" -ForegroundColor Green
} else {
    Write-Host "✗ Archivo de proyecto no encontrado" -ForegroundColor Red
    Read-Host "Presiona Enter para salir..."
    exit 1
}

Write-Host "`nCompilando el proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "✗ Error en la compilación" -ForegroundColor Red
    Write-Host "Resultado: $buildResult" -ForegroundColor Gray
    Read-Host "Presiona Enter para salir..."
    exit 1
}

Write-Host "`nIniciando API Gateway en puerto 7000..." -ForegroundColor Cyan
Write-Host "URLs disponibles:" -ForegroundColor White
Write-Host "- Gateway: https://localhost:7000" -ForegroundColor Gray
Write-Host "- Swagger: https://localhost:7000/swagger" -ForegroundColor Gray
Write-Host "- Health: https://localhost:7000/api/gateway/health" -ForegroundColor Gray

Write-Host "`nPresiona Ctrl+C para detener el gateway" -ForegroundColor Yellow
Write-Host "===============================================" -ForegroundColor Green

# Iniciar el gateway
dotnet run --urls "https://localhost:7000"
