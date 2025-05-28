@echo off
echo === Iniciando API Gateway ===
cd /d "c:\Users\genoc\OneDrive\Documentos\Maestria\ConstrucciónDeSoftware\PAC\7_Aplicacion\EtapaDeJuicio.API.Gateway"
echo Directorio actual: %CD%
echo Iniciando gateway en puerto 7000...
dotnet run --urls "https://localhost:7000"
pause
