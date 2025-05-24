# Definición de Estándares de Codificación

## Introducción
Este documento establece los estándares de codificación y guías de estilo que el equipo seguirá durante la construcción del **Sistema de Gestión Procesal Penal**. Estos estándares son obligatorios para todos los miembros del equipo y tienen como objetivo garantizar la calidad, mantenibilidad, seguridad y consistencia del código fuente.

## Convenciones Generales

### Estructura de Archivos
- Organizar los módulos según la estructura del proceso judicial (recepción, evaluación, investigación, formalización, juicio, sentencia, ejecución).
- Utilizar nombres descriptivos para los archivos que reflejen su funcionalidad.
- Limitar cada archivo a una sola responsabilidad o componente.

### Nomenclatura
- **PascalCase** para clases y componentes: `EmisionSentencia`, `GestionExpediente`.
- **camelCase** para variables locales: `expedienteActual`.
- **PascalCase** para métodos y propiedades: `ObtenerSentencia()`, `ExpedienteActual`.
- **_camelCase** con guión bajo para campos privados: `_repositorio`, `_servicioUsuarios`.
- **MAYÚSCULAS_CON_GUIONES** para constantes: `ESTADO_ARCHIVADO`.
- Prefijos claros para interfaces: `ISentencia`, `IRepositorioDocumentos`.

### Documentación
- Documentar todas las clases, métodos y funciones con comentarios XML (///).
- Incluir información sobre parámetros, valores de retorno y excepciones.
- Mantener la documentación actualizada cuando se modifique el código.

## Estándares Específicos por Lenguaje

### C# / .NET
- Indentación de 4 espacios (no tabuladores).
- Apertura de llaves en una nueva línea.
- Una instrucción por línea.
- Líneas no superiores a 120 caracteres.
- Uso obligatorio de bloques `try-catch` para manejo de excepciones.
- Preferir tipos de propiedades automáticas cuando sea posible.
- Utilizar `async/await` para operaciones asíncronas.
- Aplicar principios SOLID en el diseño de clases y componentes.
- Uso de inyección de dependencias mediante el contenedor IoC de .NET.

### SQL Server
- Palabras clave en MAYÚSCULAS.
- Nombres de tablas en `PascalCase`.
- Cada cláusula en una línea distinta.
- Comentarios explicativos para consultas complejas.
- Utilizar procedimientos almacenados para operaciones complejas.
- Índices adecuados para optimizar consultas frecuentes.
- Seguir convenciones de T-SQL para comandos específicos de SQL Server.

## Seguridad y Calidad

### Prácticas de Seguridad
- No incluir credenciales o tokens en el código fuente.
- Implementar validación de entrada para todos los datos externos.
- Utilizar consultas parametrizadas para prevenir inyección SQL.
- Implementar encriptación para datos sensibles.
- Utilizar Identity Server o Azure AD para la autenticación cuando sea posible.
- Seguir las directrices de seguridad de OWASP para aplicaciones .NET.

### Control de Calidad
- Cobertura mínima de pruebas unitarias: **80%**.
- Revisión de código obligatoria antes de integración.
- Análisis estático de código con **SonarQube** y **StyleCop**.
- Prohibido hacer commit de código con *warnings* o errores.
- Implementar pruebas de integración para APIs y servicios.

## Estándares para APIs
- Seguir principios REST para APIs web.
- Implementar APIs con ASP.NET Core.
- Versionado explícito de endpoints: `/api/v1/sentencias`.
- Respuestas consistentes con códigos HTTP apropiados.
- Documentación automática con **Swagger/OpenAPI**.
- Utilizar DTOs para transferencia de datos entre capas.

## Control de Versiones
- Utilizar Git con modelo de ramificación **GitFlow**.
- Formato de mensajes de commit:
  - Descriptivos y claros sobre el cambio realizado.
- Prohibido hacer commit directamente a las ramas `principal` y `desarrollo`.

## Conclusión
Estos estándares fueron acordados por todo el equipo y su cumplimiento será supervisado durante las revisiones de código. Se realizarán actualizaciones a este documento según sea necesario para adaptarse a los requisitos cambiantes del proyecto.
