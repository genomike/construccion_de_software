# REPORTE DE PROBLEMAS DE ENDPOINTS - Sistema EtapaDeJuicio
**Fecha:** 28 de mayo de 2025
**Estado:** Análisis sistemático completado

## ✅ PROBLEMA RESUELTO: InterrogatoriosController
- **Problema:** UI llamaba a `POST /api/interrogatorios` pero solo existían endpoints específicos
- **Solución:** ✅ IMPLEMENTADA - Agregado endpoint genérico `[HttpPost]` en InterrogatoriosController
- **Estado:** Funcionando correctamente

## ❌ PROBLEMAS IDENTIFICADOS

### 1. PruebasController - Falta endpoint GET genérico
**Problema:** 
- El UI Service `PruebasService` llama a `GET /api/pruebas`
- El controller `PruebasController` NO tiene endpoint `[HttpGet]` genérico
- Solo tiene endpoints específicos: `GET {id}` y `GET audiencia/{audienciaId}`

**Servicios UI afectados:**
```csharp
// PruebasServices.cs línea 28
public class PruebasService : BaseApiService<PruebaJudicial>
{
    public PruebasService(HttpClient httpClient) : base(httpClient, "/api/pruebas") // ❌ FALLA
}
```

**Endpoints disponibles en PruebasController:**
- ❌ `GET /api/pruebas` - NO EXISTE
- ✅ `GET /api/pruebas/{id}` - Existe
- ✅ `GET /api/pruebas/audiencia/{audienciaId}` - Existe
- ✅ `POST /api/pruebas/documentales` - Existe
- ✅ `POST /api/pruebas/testimoniales` - Existe  
- ✅ `POST /api/pruebas/periciales` - Existe

### 2. PruebasController - Falta endpoint POST genérico
**Problema:**
- El UI Service hereda de `BaseApiService<T>` que llama a `POST /api/pruebas`
- El controller NO tiene endpoint `[HttpPost]` genérico

### 3. SentenciasController - Falta endpoint GET genérico  
**Problema:**
- El UI Service `SentenciaService` llama a `GET /api/sentencias`
- El controller `SentenciasController` NO tiene endpoint `[HttpGet]` genérico

**Servicios UI afectados:**
```csharp
// SentenciaServices.cs línea 8
public class SentenciaService : BaseApiService<Sentencia>
{
    public SentenciaService(HttpClient httpClient) : base(httpClient, "/api/sentencias") // ❌ FALLA GET
}
```

**Endpoints disponibles en SentenciasController:**
- ❌ `GET /api/sentencias` - NO EXISTE
- ✅ `GET /api/sentencias/{id}` - Existe
- ✅ `POST /api/sentencias` - Existe (línea 93)

### 4. DeliberacionService - Endpoint inexistente
**Problema:**
- El UI Service `DeliberacionService` llama a `/api/sentencias/deliberaciones`
- El controller `SentenciasController` NO tiene rutas con "deliberaciones"

**Servicios UI afectados:**
```csharp
// SentenciaServices.cs línea 94
public class DeliberacionService : BaseApiService<Deliberacion>
{
    public DeliberacionService(HttpClient httpClient) : base(httpClient, "/api/sentencias/deliberaciones") // ❌ NO EXISTE
}
```

## 📊 RESUMEN DE MICROSERVICIOS

### ✅ FUNCIONANDO CORRECTAMENTE:
- **AudienciasController:** Tiene endpoints GET y POST genéricos
- **InterrogatoriosController:** ✅ ARREGLADO - Ahora tiene endpoint POST genérico
- **GestorDeUsuario:** Endpoints específicos funcionando

### ❌ REQUIEREN CORRECCIÓN:
- **PruebasController:** Faltan endpoints GET y POST genéricos
- **SentenciasController:** Falta endpoint GET genérico
- **SentenciasController:** Faltan endpoints para deliberaciones

## 🔧 ACCIONES REQUERIDAS

### Prioridad Alta:
1. **PruebasController:** Agregar `[HttpGet]` y `[HttpPost]` genéricos
2. **SentenciasController:** Agregar `[HttpGet]` genérico
3. **SentenciasController:** Agregar endpoints para deliberaciones o corregir DeliberacionService

### Patron de Solución:
Seguir el mismo patrón usado en InterrogatoriosController:
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<TipoDto>>> ObtenerTodos()

[HttpPost]
public async Task<ActionResult<Guid>> Crear([FromBody] CrearTipoRequest request)
```

## 📝 NOTAS
- El API Gateway está configurado correctamente para enrutar a todos los microservicios
- Los problemas son específicamente en los controllers de los microservicios
- El patrón `BaseApiService<T>` del UI espera endpoints estándar REST
