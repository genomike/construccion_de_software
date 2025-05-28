# Script para iniciar el API Gateway y todos los microservicios
# Ejecutar desde el directorio raíz del proyecto

Write-Host "=== Iniciando Sistema Etapa de Juicio ===" -ForegroundColor Green

# Configuración de puertos
$gatewayPort = 7000
$audienciasPort = 7001
$usuarioPort = 7002
$interrogatoriosPort = 7003
$sentenciasPort = 7004
$pruebasPort = 7005

# Directorio base
$baseDir = "c:\Users\genoc\OneDrive\Documentos\Maestria\ConstrucciónDeSoftware\PAC\7_Aplicacion"

# Función para iniciar un servicio
function Start-Service {
    param($serviceName, $port, $path)
    
    Write-Host "Iniciando $serviceName en puerto $port..." -ForegroundColor Yellow
    
    $fullPath = Join-Path $baseDir $path
    if (Test-Path $fullPath) {
        # Iniciar el servicio en una nueva ventana de PowerShell
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$fullPath'; dotnet run --urls https://localhost:$port" -WindowStyle Normal
        Write-Host "$serviceName iniciado correctamente" -ForegroundColor Green
    } else {
        Write-Host "ERROR: No se encontró el directorio $fullPath" -ForegroundColor Red
    }
}

# Esperar entrada del usuario
Write-Host "Presiona Enter para continuar o Ctrl+C para cancelar..."
Read-Host

# Iniciar API Gateway
Write-Host "`n=== Iniciando API Gateway ===" -ForegroundColor Cyan
Start-Service "API Gateway" $gatewayPort "EtapaDeJuicio.API.Gateway"

Start-Sleep -Seconds 3

# Iniciar microservicios
Write-Host "`n=== Iniciando Microservicios ===" -ForegroundColor Cyan

Start-Service "Gestor de Audiencias" $audienciasPort "EtapaDeJuicio.Microservices.GestorDeAudiencias"
Start-Sleep -Seconds 2

Start-Service "Gestor de Usuario" $usuarioPort "EtapaDeJuicio.Microservices.GestorDeUsuario"
Start-Sleep -Seconds 2

Start-Service "Gestor de Interrogatorios" $interrogatoriosPort "EtapaDeJuicio.Microservices.GestorDeInterrogatorios"
Start-Sleep -Seconds 2

Start-Service "Gestor de Sentencias" $sentenciasPort "EtapaDeJuicio.Microservices.GestorDeSentencias"
Start-Sleep -Seconds 2

Start-Service "Gestor de Pruebas" $pruebasPort "EtapaDeJuicio.Microservices.GestorDePruebas"

Write-Host "`n=== Sistema Iniciado Completamente ===" -ForegroundColor Green
Write-Host "API Gateway disponible en: https://localhost:$gatewayPort" -ForegroundColor White
Write-Host "Swagger UI disponible en: https://localhost:$gatewayPort/swagger" -ForegroundColor White

Write-Host "`nMicroservicios disponibles:" -ForegroundColor White
Write-Host "- Audiencias: https://localhost:$audienciasPort" -ForegroundColor Gray
Write-Host "- Usuario: https://localhost:$usuarioPort" -ForegroundColor Gray
Write-Host "- Interrogatorios: https://localhost:$interrogatoriosPort" -ForegroundColor Gray
Write-Host "- Sentencias: https://localhost:$sentenciasPort" -ForegroundColor Gray
Write-Host "- Pruebas: https://localhost:$pruebasPort" -ForegroundColor Gray

Write-Host "`nPresiona cualquier tecla para salir..." -ForegroundColor Yellow
Read-Host
