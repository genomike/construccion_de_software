# Script para probar todos los endpoints del sistema EtapaDeJuicio
# Ejecutar después de que todos los servicios estén corriendo

Write-Host "=== PRUEBA SISTEMÁTICA DE ENDPOINTS ===" -ForegroundColor Green
Write-Host "Fecha: $(Get-Date)" -ForegroundColor Yellow

$gatewayUrl = "https://localhost:7000"
$testResults = @()

# Headers comunes
$headers = @{
    'Content-Type' = 'application/json'
    'Accept' = 'application/json'
}

# Datos de prueba básicos
$testData = @{
    interrogatorio = @{
        id = "12345678-1234-1234-1234-123456789012"
        pregunta = "Pregunta de prueba"
        respuesta = $null
        fechaHora = (Get-Date).ToString('yyyy-MM-ddTHH:mm:ss')
        tipo = "Directo"
    }
    audiencia = @{
        id = "87654321-4321-4321-4321-876543210987"
        numeroExpediente = "EXP-2025-001"
        fechaHora = (Get-Date).ToString('yyyy-MM-ddTHH:mm:ss')
        sala = "Sala 1"
        estado = "Programada"
    }
    sentencia = @{
        id = "11111111-2222-3333-4444-555555555555"
        audienciaId = "87654321-4321-4321-4321-876543210987"
        tipo = "Absolutoria"
        decisiones = @("Decisión de prueba")
        fechaEmision = (Get-Date).ToString('yyyy-MM-ddTHH:mm:ss')
    }
}

function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Description,
        [object]$Body = $null
    )
    
    try {
        Write-Host "Testing: $Method $Url - $Description" -ForegroundColor Cyan
        
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $headers
            SkipCertificateCheck = $true
        }
        
        if ($Body -and ($Method -eq "POST" -or $Method -eq "PUT")) {
            $params.Body = $Body | ConvertTo-Json -Depth 10
            Write-Host "Body: $($params.Body)" -ForegroundColor Gray
        }
        
        $response = Invoke-RestMethod @params
        
        $result = @{
            Method = $Method
            Url = $Url
            Description = $Description
            Status = "✅ SUCCESS"
            Response = $response
            Error = $null
        }
        
        Write-Host "✅ SUCCESS: $response" -ForegroundColor Green
        return $result
        
    } catch {
        $result = @{
            Method = $Method
            Url = $Url
            Description = $Description
            Status = "❌ FAILED"
            Response = $null
            Error = $_.Exception.Message
        }
        
        Write-Host "❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        return $result
    }
}

Write-Host "`n=== PROBANDO ENDPOINTS DE INTERROGATORIOS ===" -ForegroundColor Yellow

# Interrogatorios - GET
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/interrogatorios" -Description "Obtener todos los interrogatorios"

# Interrogatorios - POST (ya funciona)
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/interrogatorios" -Description "Crear interrogatorio" -Body $testData.interrogatorio

Write-Host "`n=== PROBANDO ENDPOINTS DE AUDIENCIAS ===" -ForegroundColor Yellow

# Audiencias - GET
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/audiencias" -Description "Obtener todas las audiencias"

# Audiencias - POST
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/audiencias" -Description "Crear audiencia" -Body $testData.audiencia

Write-Host "`n=== PROBANDO ENDPOINTS DE SENTENCIAS ===" -ForegroundColor Yellow

# Sentencias - GET
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/sentencias" -Description "Obtener todas las sentencias"

# Sentencias - POST
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/sentencias" -Description "Crear sentencia" -Body $testData.sentencia

# Deliberaciones - GET 
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/sentencias/deliberaciones" -Description "Obtener deliberaciones"

# Deliberaciones - POST
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/sentencias/deliberaciones" -Description "Crear deliberación" -Body $testData.sentencia

Write-Host "`n=== PROBANDO ENDPOINTS DE PRUEBAS ===" -ForegroundColor Yellow

# Pruebas - GET genérico
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/pruebas" -Description "Obtener todas las pruebas"

# Pruebas - POST genérico
$pruebaData = @{
    id = "99999999-8888-7777-6666-555555555555"
    descripcion = "Prueba de test"
    tipo = "Documental"
    presentadaPor = "12345678-1234-1234-1234-123456789012"
}
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/pruebas" -Description "Crear prueba genérica" -Body $pruebaData

# Pruebas específicas - GET
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/pruebas/documentales" -Description "Obtener pruebas documentales"
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/pruebas/testimoniales" -Description "Obtener pruebas testimoniales"
$testResults += Test-Endpoint -Method "GET" -Url "$gatewayUrl/api/pruebas/periciales" -Description "Obtener pruebas periciales"

# Pruebas específicas - POST (estos deberían funcionar)
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/pruebas/documentales" -Description "Crear prueba documental" -Body $pruebaData
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/pruebas/testimoniales" -Description "Crear prueba testimonial" -Body $pruebaData
$testResults += Test-Endpoint -Method "POST" -Url "$gatewayUrl/api/pruebas/periciales" -Description "Crear prueba pericial" -Body $pruebaData

Write-Host "`n=== RESUMEN DE RESULTADOS ===" -ForegroundColor Green

$successful = ($testResults | Where-Object { $_.Status -eq "✅ SUCCESS" }).Count
$failed = ($testResults | Where-Object { $_.Status -eq "❌ FAILED" }).Count

Write-Host "Total pruebas: $($testResults.Count)" -ForegroundColor White
Write-Host "Exitosas: $successful" -ForegroundColor Green
Write-Host "Fallidas: $failed" -ForegroundColor Red

Write-Host "`n=== ENDPOINTS FALLIDOS ===" -ForegroundColor Red
$testResults | Where-Object { $_.Status -eq "❌ FAILED" } | ForEach-Object {
    Write-Host "$($_.Method) $($_.Url) - $($_.Description)" -ForegroundColor Red
    Write-Host "  Error: $($_.Error)" -ForegroundColor Gray
    Write-Host ""
}

# Guardar reporte en archivo
$reportPath = "endpoint-test-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$testResults | ConvertTo-Json -Depth 10 | Out-File -FilePath $reportPath -Encoding UTF8
Write-Host "Reporte guardado en: $reportPath" -ForegroundColor Yellow
