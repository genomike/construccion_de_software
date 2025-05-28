# Script de pruebas simplificado para el API Gateway
# Ejecutar después de iniciar solo el gateway

Write-Host "=== Probando API Gateway Solo ===" -ForegroundColor Green

$gatewayUrl = "https://localhost:7000"

# Función para hacer peticiones HTTP y manejar errores
function Test-Endpoint {
    param($url, $name)
    
    Write-Host "`nProbando $name..." -ForegroundColor Yellow
    Write-Host "URL: $url" -ForegroundColor Gray
    
    try {
        # Ignorar errores de certificado SSL en desarrollo
        [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
        
        $response = Invoke-RestMethod -Uri $url -Method GET -TimeoutSec 10 -ErrorAction Stop
        Write-Host "✓ $name - ÉXITO" -ForegroundColor Green
        
        if ($response) {
            # Mostrar solo las primeras líneas de la respuesta
            $responseText = $response | ConvertTo-Json -Depth 2 -Compress
            if ($responseText.Length -gt 200) {
                $responseText = $responseText.Substring(0, 200) + "..."
            }
            Write-Host "Respuesta: $responseText" -ForegroundColor Gray
        }
        
        return $true
    }
    catch {
        Write-Host "✗ $name - ERROR: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

Write-Host "Esperando 5 segundos para que el gateway se inicie..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

$testResults = @()

# Probar endpoints básicos del Gateway (no requieren microservicios)
Write-Host "`n=== Probando Endpoints Básicos del Gateway ===" -ForegroundColor Cyan

$testResults += Test-Endpoint "$gatewayUrl/api/gateway/info" "Información del Gateway"
$testResults += Test-Endpoint "$gatewayUrl/api/gateway/health" "Estado de Salud"
$testResults += Test-Endpoint "$gatewayUrl/api/gateway/routes" "Rutas Configuradas"

# Probar Swagger UI
Write-Host "`nProbando Swagger UI..." -ForegroundColor Yellow
try {
    $swaggerResponse = Invoke-WebRequest -Uri "$gatewayUrl/swagger" -TimeoutSec 10 -ErrorAction Stop
    if ($swaggerResponse.StatusCode -eq 200) {
        Write-Host "✓ Swagger UI - ÉXITO" -ForegroundColor Green
        $testResults += $true
    }
}
catch {
    Write-Host "✗ Swagger UI - ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $testResults += $false
}

# Resumen de resultados
Write-Host "`n=== Resumen de Pruebas del Gateway ===" -ForegroundColor Cyan
$successful = ($testResults | Where-Object { $_ -eq $true }).Count
$total = $testResults.Count

Write-Host "Pruebas exitosas: $successful de $total" -ForegroundColor White

if ($successful -eq $total) {
    Write-Host "🎉 ¡El API Gateway está funcionando correctamente!" -ForegroundColor Green
    Write-Host "Ahora puedes iniciar los microservicios para probar el enrutamiento completo." -ForegroundColor Yellow
} elseif ($successful -gt 0) {
    Write-Host "⚠️ Algunas pruebas básicas funcionan. Verifica la configuración." -ForegroundColor Yellow
} else {
    Write-Host "❌ El API Gateway no está respondiendo correctamente." -ForegroundColor Red
}

Write-Host "`nURLs importantes:" -ForegroundColor White
Write-Host "- Gateway: $gatewayUrl" -ForegroundColor Gray
Write-Host "- Swagger UI: $gatewayUrl/swagger" -ForegroundColor Gray
Write-Host "- Health Check: $gatewayUrl/api/gateway/health" -ForegroundColor Gray
Write-Host "- Información: $gatewayUrl/api/gateway/info" -ForegroundColor Gray

Write-Host "`nPara probar con microservicios, ejecuta:" -ForegroundColor Yellow
Write-Host ".\start-system.ps1" -ForegroundColor Gray

Write-Host "`nPresiona cualquier tecla para salir..." -ForegroundColor Yellow
Read-Host
