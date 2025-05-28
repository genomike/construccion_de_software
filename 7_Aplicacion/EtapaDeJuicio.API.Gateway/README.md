# EtapaDeJuicio.API.Gateway

API Gateway para el sistema de microservicios "Etapa de Juicio" usando YARP (Yet Another Reverse Proxy).

## ✅ Estado del Proyecto

**IMPLEMENTACIÓN COMPLETA** - El API Gateway está totalmente funcional con:

- ✅ Configuración YARP completa
- ✅ Enrutamiento a todos los microservicios  
- ✅ Health checks automatizados
- ✅ Middleware de logging personalizado
- ✅ Swagger UI integrado
- ✅ CORS configurado para desarrollo
- ✅ Scripts de inicio y pruebas automatizadas
- ✅ Documentación completa

## Descripción
Este es el API Gateway unificado para el sistema de Etapa de Juicio. Actúa como punto de entrada único para todos los microservicios del sistema.

## Funcionalidades

### 🚪 Proxy Reverso (YARP)
- Enruta automáticamente las peticiones a los microservicios correspondientes
- Balanceeo de carga automático
- Configuración basada en archivos JSON

### 🏥 Health Checks
- Monitoreo en tiempo real del estado de todos los microservicios
- Endpoint: `GET /health`

### 📊 Dashboard
- Vista agregada de datos de múltiples microservicios
- Endpoint: `GET /api/proxy/dashboard`

### 🔍 Logging
- Logging detallado de todas las peticiones
- Request ID único para trazabilidad
- Métricas de tiempo de respuesta

## Endpoints Principales

### Información del Gateway
```
GET /api/gateway/services    - Lista de microservicios disponibles
GET /api/gateway/health      - Estado de salud del gateway
GET /api/gateway/routes      - Rutas configuradas
```

### Health Check
```
GET /health                  - Estado completo del sistema
```

### Proxy Directo
```
GET /api/proxy/audiencias    - Proxy directo a audiencias
GET /api/proxy/dashboard     - Dashboard agregado
```

### Rutas de Microservicios (YARP)
```
/api/audiencias/**           → GestorDeAudiencias (Puerto 7001)
/api/usuarios/**             → GestorDeUsuario (Puerto 7002)
/api/interrogatorios/**      → GestorDeInterrogatorios (Puerto 7003)
/api/sentencias/**           → GestorDeSentencias (Puerto 7004)
/api/pruebas/**              → GestorDePruebas (Puerto 7005)
```

## Configuración de Puertos

| Servicio | Puerto HTTPS | Puerto HTTP |
|----------|--------------|-------------|
| API Gateway | 7000 | 5000 |
| Gestor de Audiencias | 7001 | 5001 |
| Gestor de Usuario | 7002 | 5002 |
| Gestor de Interrogatorios | 7003 | 5003 |
| Gestor de Sentencias | 7004 | 5004 |
| Gestor de Pruebas | 7005 | 5005 |

## Cómo Usar

### 1. Iniciar todos los microservicios
```bash
# Terminal 1 - API Gateway
cd EtapaDeJuicio.API.Gateway
dotnet run

# Terminal 2 - Gestor de Audiencias
cd EtapaDeJuicio.GestorDeAudiencias
dotnet run

# Terminal 3 - Gestor de Usuario
cd EtapaDeJuicio.GestorDeUsuario
dotnet run

# ... y así para cada microservicio
```

### 2. Acceder al Gateway
- **Swagger UI**: https://localhost:7000
- **Health Check**: https://localhost:7000/health
- **Servicios**: https://localhost:7000/api/gateway/services

### 3. Usar las APIs a través del Gateway
```bash
# Crear una audiencia
curl -X POST "https://localhost:7000/api/audiencias" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Audiencia de Prueba",
    "fechaHoraProgramada": "2025-06-01T10:00:00",
    "tipo": 1,
    "duracionEstimadaMinutos": 120
  }'

# Obtener audiencias
curl "https://localhost:7000/api/audiencias?page=1&pageSize=10"

# Ver dashboard agregado
curl "https://localhost:7000/api/proxy/dashboard"
```

## Características Técnicas

### Middleware Personalizado
1. **RequestLoggingMiddleware**: Logging detallado de peticiones
2. **HealthCheckMiddleware**: Verificación de salud de microservicios

### Configuración YARP
- Configuración basada en `appsettings.json`
- Rutas dinámicas con wildcards
- Transformaciones de URL automáticas

### CORS
- Configurado para desarrollo local
- Permite todos los orígenes, métodos y headers

### Logging
- Niveles configurables por categoría
- Request ID para trazabilidad
- Métricas de tiempo de respuesta

## Monitoreo

### Health Check Response
```json
{
  "status": "Healthy",
  "timestamp": "2025-05-27T...",
  "services": [
    {
      "service": "API Gateway",
      "status": "Healthy",
      "responseTime": "0ms"
    },
    {
      "service": "GestorDeAudiencias",
      "status": "Healthy",
      "responseTime": "45ms"
    }
  ]
}
```

### Request Logging
```
[12:34:56] Request abc-123 started: GET /api/audiencias from 127.0.0.1
[12:34:56] Request abc-123 completed: GET /api/audiencias responded 200 in 234ms
```

## Desarrollo

### Agregar Nuevo Microservicio
1. Actualizar `appsettings.json` con la nueva configuración:
```json
"Microservices": {
  "NuevoServicio": {
    "BaseUrl": "https://localhost:7006"
  }
}
```

2. Agregar ruta en `ReverseProxy.Routes`:
```json
"nuevo-servicio-route": {
  "ClusterId": "nuevo-servicio-cluster",
  "Match": {
    "Path": "/api/nuevo-servicio/{**catch-all}"
  }
}
```

3. Agregar cluster en `ReverseProxy.Clusters`:
```json
"nuevo-servicio-cluster": {
  "Destinations": {
    "destination1": {
      "Address": "https://localhost:7006/"
    }
  }
}
```

## Próximas Mejoras
- [ ] Autenticación y autorización centralizada
- [ ] Rate limiting
- [ ] Circuit breaker pattern
- [ ] Caching distribuido
- [ ] Métricas avanzadas con Prometheus
- [ ] Load balancing avanzado
- [ ] Service discovery automático
