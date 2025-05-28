# Script de pruebas para el API Gateway
# Ejecutar después de iniciar el sistema

Write-Host "=== Probando API Gateway - Sistema Etapa de Juicio ===" -ForegroundColor Green

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

Write-Host "Esperando 5 segundos para que los servicios se inicien..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

$testResults = @()

# Probar endpoints del Gateway
Write-Host "`n=== Probando Endpoints del API Gateway ===" -ForegroundColor Cyan

$testResults += Test-Endpoint "$gatewayUrl/api/gateway/info" "Información del Gateway"
$testResults += Test-Endpoint "$gatewayUrl/api/gateway/health" "Estado de Salud"
$testResults += Test-Endpoint "$gatewayUrl/api/gateway/routes" "Rutas Configuradas"

# Probar endpoints de proxy directo
Write-Host "`n=== Probando Proxy Directo ===" -ForegroundColor Cyan

$testResults += Test-Endpoint "$gatewayUrl/api/proxy/audiencias" "Proxy - Audiencias"
$testResults += Test-Endpoint "$gatewayUrl/api/proxy/dashboard" "Proxy - Dashboard"

# Probar enrutamiento YARP (si los microservicios están ejecutándose)
Write-Host "`n=== Probando Enrutamiento YARP ===" -ForegroundColor Cyan

$testResults += Test-Endpoint "$gatewayUrl/api/audiencias" "YARP - Audiencias"
$testResults += Test-Endpoint "$gatewayUrl/api/usuarios" "YARP - Usuarios"
$testResults += Test-Endpoint "$gatewayUrl/api/interrogatorios" "YARP - Interrogatorios"
$testResults += Test-Endpoint "$gatewayUrl/api/sentencias" "YARP - Sentencias"
$testResults += Test-Endpoint "$gatewayUrl/api/pruebas" "YARP - Pruebas"

# Resumen de resultados
Write-Host "`n=== Resumen de Pruebas ===" -ForegroundColor Cyan
$successful = ($testResults | Where-Object { $_ -eq $true }).Count
$total = $testResults.Count

Write-Host "Pruebas exitosas: $successful de $total" -ForegroundColor White

if ($successful -eq $total) {
    Write-Host "🎉 ¡Todas las pruebas pasaron! El API Gateway está funcionando correctamente." -ForegroundColor Green
} elseif ($successful -gt 0) {
    Write-Host "⚠️ Algunas pruebas fallaron. Verifica que todos los microservicios estén ejecutándose." -ForegroundColor Yellow
} else {
    Write-Host "❌ Todas las pruebas fallaron. Verifica que el API Gateway esté ejecutándose en $gatewayUrl" -ForegroundColor Red
}

Write-Host "`nURLs importantes:" -ForegroundColor White
Write-Host "- Swagger UI: $gatewayUrl/swagger" -ForegroundColor Gray
Write-Host "- Health Check: $gatewayUrl/api/gateway/health" -ForegroundColor Gray
Write-Host "- Información: $gatewayUrl/api/gateway/info" -ForegroundColor Gray

Write-Host "`nPresiona cualquier tecla para salir..." -ForegroundColor Yellow
Read-Host
