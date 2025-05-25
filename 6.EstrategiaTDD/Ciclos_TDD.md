# Ciclo TDD para el Desarrollo de la Etapa de Juicio

## Enfoque General del TDD
El desarrollo del módulo "5. Etapa de Juicio" seguirá el ciclo **Red-Green-Refactor** aplicado a una arquitectura DDD hexagonal orientada a eventos:

- **Red**: Escribir pruebas que fallen inicialmente para validar requisitos de dominio  
- **Green**: Implementar el mínimo código necesario para pasar las pruebas  
- **Refactor**: Mejorar la calidad del código sin romper las pruebas  

## Organización de Pruebas por Capas de Arquitectura

### Pruebas de Dominio (EtapaDeJuicio.Domain.Tests)

Validan las reglas del negocio en su forma más pura:

- Entidades y Agregados
- Value Objects
- Reglas de dominio
- Eventos de dominio
- Políticas y servicios de dominio

### Pruebas de Aplicación (EtapaDeJuicio.Application.Tests)

Verifican la orquestación de casos de uso:

- Comandos y sus manejadores
- Consultas y sus manejadores
- Sagas para procesos de negocio transaccionales
- Eventos y sus manejadores
- Servicios de aplicación

### Pruebas de Infraestructura (EtapaDeJuicio.Infrastructure.Tests)

Comprueban los adaptadores técnicos:

- Repositorios y persistencia
- Implementaciones de UnitOfWork
- Event Bus
- Servicios externos
- Adaptadores y gateways

### Pruebas de API (EtapaDeJuicio.API.Tests)

Validan la interfaz con el exterior:

- Controllers
- DTOs
- Filtros y middleware
- Seguridad y autenticación

### Pruebas de Integración (EtapaDeJuicio.Integration.Tests)

Aseguran la correcta interacción entre componentes:

- Flujos de trabajo completos
- Comunicación entre microservicios
- Consistencia eventual

## Plan de Implementación por Funcionalidades

### 1. Gestión de Pruebas Judiciales

#### Dominio

**Ciclo TDD:**

- **Red**: Crear pruebas para el agregado `PruebaJudicial` y sus entidades relacionadas
- **Green**: Implementar entidades, value objects y reglas de dominio
- **Refactor**: Mejorar con validaciones específicas por tipo de prueba

**Ejemplos específicos:**

```csharp
// Test de dominio para validación de prueba documental
[Fact]
public void PruebaDocumental_SinDescripcion_LanzaExcepcion()
{
    // Arrange & Act & Assert
    var exception = Assert.Throws<DomainException>(() => 
        PruebaDocumental.Crear(Guid.NewGuid(), string.Empty, "archivo.pdf"));
    
    Assert.Contains("descripción", exception.Message);
}

// Test de value object para formato de archivo
[Theory]
[InlineData("archivo.pdf", true)]
[InlineData("archivo.exe", false)]
[InlineData("", false)]
public void FormatoArchivo_ValidaCorrectamente(string nombreArchivo, bool esperado)
{
    // Act
    var formatoValido = FormatoArchivo.EsValido(nombreArchivo);
    
    // Assert
    Assert.Equal(esperado, formatoValido);
}
```

#### Aplicación

**Ciclo TDD:**

- **Red**: Probar comandos como `RegistrarPruebaJudicialCommand` y sus handlers
- **Green**: Implementar manejadores de comandos y consultas
- **Refactor**: Mejorar con validaciones específicas y manejo de errores

**Ejemplos específicos:**

```csharp
[Fact]
public async Task RegistrarPruebaJudicialHandler_ConDatosValidos_RegistraExitosamente()
{
    // Arrange
    var command = new RegistrarPruebaJudicialCommand 
    {
        TipoPrueba = TipoPrueba.Documental,
        Descripcion = "Contrato firmado",
        RutaArchivo = "/archivos/contrato.pdf"
    };
    
    var mockRepo = new Mock<IPruebasJudicialesRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    var mockEventBus = new Mock<IEventBus>();
    
    var handler = new RegistrarPruebaJudicialHandler(mockRepo.Object, mockUoW.Object, mockEventBus.Object);

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(resultado.Success);
    mockRepo.Verify(r => r.AddAsync(It.IsAny<PruebaJudicial>()), Times.Once);
    mockEventBus.Verify(eb => eb.Publish(It.IsAny<PruebaJudicialRegistradaEvent>()), Times.Once);
    mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
}
```

#### Infraestructura

**Ciclo TDD:**

- **Red**: Probar implementaciones de repositorios y persistencia
- **Green**: Implementar repositorios y UnitOfWork
- **Refactor**: Optimizar consultas y manejo de transacciones

**Ejemplos específicos:**

```csharp
[Fact]
public async Task PruebasJudicialesRepository_GuardarPrueba_PersisteDatos()
{
    // Arrange
    var dbContextOptions = new DbContextOptionsBuilder<EtapaDeJuicioDbContext>()
        .UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString())
        .Options;
    
    using var dbContext = new EtapaDeJuicioDbContext(dbContextOptions);
    var repository = new PruebasJudicialesRepository(dbContext);
    
    var prueba = PruebaDocumental.Crear(
        Guid.NewGuid(), 
        "Contrato de arrendamiento", 
        "contrato.pdf"
    );

    // Act
    await repository.AddAsync(prueba);
    await dbContext.SaveChangesAsync();

    // Assert
    var pruebaGuardada = await dbContext.PruebasJudiciales.FirstOrDefaultAsync();
    Assert.NotNull(pruebaGuardada);
    Assert.Equal(prueba.Id, pruebaGuardada.Id);
    Assert.Equal("Contrato de arrendamiento", pruebaGuardada.Descripcion);
}
```

#### API

**Ciclo TDD:**

- **Red**: Probar endpoints de API para gestión de pruebas
- **Green**: Implementar controladores y acciones
- **Refactor**: Mejorar validación de entrada y manejo de errores

**Ejemplos específicos:**

```csharp
[Fact]
public async Task RegistrarPruebaEndpoint_ConDatosValidos_RetornaCreated()
{
    // Arrange
    var mockMediator = new Mock<IMediator>();
    mockMediator.Setup(m => m.Send(It.IsAny<RegistrarPruebaJudicialCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult { Success = true, Id = Guid.NewGuid() });
                
    var controller = new PruebasJudicialesController(mockMediator.Object);
    var request = new RegistrarPruebaRequest 
    {
        TipoPrueba = "Documental",
        Descripcion = "Contrato firmado",
        RutaArchivo = "/archivos/contrato.pdf"
    };

    // Act
    var resultado = await controller.RegistrarPrueba(request);

    // Assert
    var actionResult = Assert.IsType<CreatedAtActionResult>(resultado);
    Assert.Equal(201, actionResult.StatusCode);
}
```

### 2. Gestión de Interrogatorios

#### Dominio

**Ciclo TDD:**

- **Red**: Probar entidades como `Interrogatorio`, `Pregunta` y `Testimonio`
- **Green**: Implementar entidades y reglas de dominio
- **Refactor**: Mejorar con reglas de validación de preguntas improcedentes

**Ejemplos específicos:**

```csharp
[Fact]
public void Pregunta_Sugestiva_NoEsPermitida()
{
    // Arrange
    var interrogatorio = Interrogatorio.Crear(
        Guid.NewGuid(),
        "Testimonio principal",
        DateTime.Now,
        TipoInterrogatorio.Directo
    );

    // Act & Assert
    var exception = Assert.Throws<DomainException>(() => 
        interrogatorio.AgregarPregunta("¿No es cierto que usted estaba en el lugar el día 15?"));
    
    Assert.Contains("sugestiva", exception.Message.ToLower());
}

[Theory]
[InlineData("¿Qué observó ese día?", true)]
[InlineData("¿No cree que el acusado actuó mal?", false)]
[InlineData("¿Podría decirme qué sucedió?", true)]
public void ValidadorPreguntas_EvaluaDiferentesTipos(string textoPregunta, bool esperado)
{
    // Act
    var esValida = ValidadorPreguntas.EsValida(textoPregunta, TipoInterrogatorio.Directo);
    
    // Assert
    Assert.Equal(esperado, esValida);
}
```

#### Aplicación

**Ciclo TDD:**

- **Red**: Probar comandos y handlers para registro y validación de interrogatorios
- **Green**: Implementar handlers y validaciones
- **Refactor**: Mejorar con validaciones más sofisticadas

**Ejemplos específicos:**

```csharp
[Fact]
public async Task RegistrarTestimonioHandler_ConDatosValidos_RegistraExitosamente()
{
    // Arrange
    var command = new RegistrarTestimonioCommand 
    {
        IdTestigo = Guid.NewGuid(),
        Descripcion = "Testimonio del testigo presencial",
        FechaHora = DateTime.Now
    };
    
    var mockRepo = new Mock<IInterrogatoriosRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    var mockEventBus = new Mock<IEventBus>();
    
    var handler = new RegistrarTestimonioHandler(mockRepo.Object, mockUoW.Object, mockEventBus.Object);

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(resultado.Success);
    mockRepo.Verify(r => r.AddTestimonioAsync(It.IsAny<Testimonio>()), Times.Once);
    mockEventBus.Verify(eb => eb.Publish(It.IsAny<TestimonioRegistradoEvent>()), Times.Once);
}

[Fact]
public async Task ValidarPreguntaHandler_ConPreguntaCapciosa_RetornaInvalida()
{
    // Arrange
    var command = new ValidarPreguntaCommand 
    {
        TextoPregunta = "¿No es verdad que usted mintió en su declaración anterior?",
        TipoInterrogatorio = TipoInterrogatorio.Directo
    };
    
    var mockValidator = new Mock<IValidadorPreguntasService>();
    mockValidator.Setup(v => v.EsValida(command.TextoPregunta, command.TipoInterrogatorio))
                 .Returns(false);
    
    var handler = new ValidarPreguntaHandler(mockValidator.Object);

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.False(resultado.EsValida);
    Assert.Contains("capciosa", resultado.MotivoRechazo.ToLower());
}
```

### 3. Deliberación Judicial

#### Dominio

**Ciclo TDD:**

- **Red**: Probar entidades de `Deliberacion`, `ConsiderandoLegal` y `ValoracionPrueba`
- **Green**: Implementar entidades y servicios de dominio
- **Refactor**: Mejorar con algoritmos de valoración probatoria

**Ejemplos específicos:**

```csharp
[Fact]
public void Deliberacion_SinPruebas_NoPermiteGenerarConsiderandos()
{
    // Arrange
    var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Caso 123/2023");

    // Act & Assert
    var exception = Assert.Throws<DomainException>(() => deliberacion.GenerarConsiderandos());
    Assert.Contains("sin pruebas", exception.Message.ToLower());
}

[Fact]
public void ValoracionPruebas_CalculaCorrectamente()
{
    // Arrange
    var servicio = new ServicioValoracionPruebas();
    var pruebas = new List<PruebaJudicial>
    {
        PruebaDocumental.Crear(Guid.NewGuid(), "Documento oficial", "doc1.pdf"),
        PruebaTestimonial.Crear(Guid.NewGuid(), "Testimonio testigo presencial", 0.8m),
        PruebaPericial.Crear(Guid.NewGuid(), "Análisis pericial", "Experto certificado")
    };

    // Act
    decimal valorTotal = servicio.CalcularValorProbatorio(pruebas);

    // Assert
    Assert.True(valorTotal > 0);
    Assert.True(valorTotal <= 1); // Valor normalizado entre 0 y 1
}
```

#### Aplicación

**Ciclo TDD:**

- **Red**: Probar generación de considerandos y análisis de pruebas
- **Green**: Implementar manejadores para deliberación
- **Refactor**: Mejorar algoritmos y manejo de casos especiales

**Ejemplos específicos:**

```csharp
[Fact]
public async Task GenerarConsiderandosHandler_ConPruebasDisponibles_GeneraCorrectamente()
{
    // Arrange
    var command = new GenerarConsiderandosCommand
    {
        IdDeliberacion = Guid.NewGuid(),
        IdsPruebas = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
    };
    
    var mockDeliberacionRepo = new Mock<IDeliberacionRepository>();
    var mockPruebasRepo = new Mock<IPruebasJudicialesRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    var mockEventBus = new Mock<IEventBus>();
    
    var pruebas = new List<PruebaJudicial>
    {
        PruebaDocumental.Crear(command.IdsPruebas[0], "Documento oficial", "doc1.pdf"),
        PruebaTestimonial.Crear(command.IdsPruebas[1], "Testimonio clave", 0.9m)
    };
    
    mockPruebasRepo.Setup(r => r.GetByIdsAsync(command.IdsPruebas))
                   .ReturnsAsync(pruebas);
                   
    var deliberacion = Deliberacion.Crear(command.IdDeliberacion, "Caso de prueba");
    mockDeliberacionRepo.Setup(r => r.GetByIdAsync(command.IdDeliberacion))
                        .ReturnsAsync(deliberacion);
    
    var handler = new GenerarConsiderandosHandler(
        mockDeliberacionRepo.Object,
        mockPruebasRepo.Object,
        mockUoW.Object,
        mockEventBus.Object
    );

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(resultado.Success);
    Assert.NotEmpty(resultado.Considerandos);
}

[Fact]
public async Task CalcularValorProbatorioHandler_EvaluaCategoriasPruebas()
{
    // Arrange
    var query = new CalcularValorProbatorioQuery
    {
        IdsPruebas = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
    };
    
    var mockPruebasRepo = new Mock<IPruebasJudicialesRepository>();
    var mockValoracionService = new Mock<IServicioValoracionPruebas>();
    
    var pruebas = new List<PruebaJudicial>
    {
        PruebaDocumental.Crear(query.IdsPruebas[0], "Acta notarial", "acta.pdf"),
        PruebaPericial.Crear(query.IdsPruebas[1], "Análisis forense", "Perito oficial"),
        PruebaTestimonial.Crear(query.IdsPruebas[2], "Testigo presencial", 0.7m)
    };
    
    mockPruebasRepo.Setup(r => r.GetByIdsAsync(query.IdsPruebas))
                   .ReturnsAsync(pruebas);
                   
    mockValoracionService.Setup(s => s.CalcularValorProbatorio(It.IsAny<IEnumerable<PruebaJudicial>>()))
                         .Returns(0.85m);
    
    var handler = new CalcularValorProbatorioHandler(
        mockPruebasRepo.Object,
        mockValoracionService.Object
    );

    // Act
    var resultado = await handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.Equal(0.85m, resultado.ValorProbatorio);
    Assert.NotEmpty(resultado.ValoracionDetallada);
}
```

### 4. Emisión de Sentencia

#### Dominio

**Ciclo TDD:**

- **Red**: Probar entidades para `Sentencia`, `Resolutivo` y `Notificacion`
- **Green**: Implementar entidades, value objects y reglas de dominio
- **Refactor**: Mejorar validaciones y metadatos adicionales

**Ejemplos específicos:**

```csharp
[Fact]
public void Sentencia_SinJuez_LanzaExcepcion()
{
    // Arrange, Act & Assert
    var exception = Assert.Throws<DomainException>(() => 
        Sentencia.Crear(
            Guid.NewGuid(), 
            null, // Juez nulo
            "Sentencia definitiva",
            TipoSentencia.Definitiva
        )
    );
    
    Assert.Contains("juez", exception.Message.ToLower());
}

[Theory]
[InlineData("")]
[InlineData(null)]
[InlineData("   ")]
public void Sentencia_SinDescripcion_LanzaExcepcion(string descripcion)
{
    // Arrange
    var juezId = Guid.NewGuid();

    // Act & Assert
    var exception = Assert.Throws<DomainException>(() => 
        Sentencia.Crear(
            Guid.NewGuid(), 
            juezId,
            descripcion,
            TipoSentencia.Definitiva
        )
    );
    
    Assert.Contains("descripción", exception.Message.ToLower());
}

[Fact]
public void Sentencia_AgregarResolutivos_FuncionaCorrectamente()
{
    // Arrange
    var sentencia = Sentencia.Crear(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Sentencia caso 123/2023",
        TipoSentencia.Definitiva
    );

    // Act
    sentencia.AgregarResolutivo("PRIMERO: Se declara procedente la acción");
    sentencia.AgregarResolutivo("SEGUNDO: Se condena al demandado");

    // Assert
    Assert.Equal(2, sentencia.Resolutivos.Count);
    Assert.Equal("PRIMERO", sentencia.Resolutivos[0].Numeracion);
    Assert.Equal("SEGUNDO", sentencia.Resolutivos[1].Numeracion);
}
```

#### Aplicación

**Ciclo TDD:**

- **Red**: Probar comandos para emitir sentencias y generar notificaciones
- **Green**: Implementar handlers de comandos y eventos
- **Refactor**: Mejorar validaciones y manejo de errores

**Ejemplos específicos:**

```csharp
[Fact]
public async Task EmitirSentenciaHandler_ConDatosCompletos_EmiteCorrectamente()
{
    // Arrange
    var command = new EmitirSentenciaCommand
    {
        JuezId = Guid.NewGuid(),
        ExpedienteId = Guid.NewGuid(),
        Descripcion = "Sentencia definitiva caso 123/2023",
        TipoSentencia = TipoSentencia.Definitiva,
        Considerandos = new List<string> { 
            "CONSIDERANDO que las pruebas aportadas son suficientes", 
            "CONSIDERANDO que se han respetado las formalidades del proceso" 
        },
        Resolutivos = new List<string> { 
            "Se declara PROCEDENTE la acción", 
            "Se CONDENA a la parte demandada" 
        }
    };
    
    var mockSentenciaRepo = new Mock<ISentenciaRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    var mockEventBus = new Mock<IEventBus>();
    
    var handler = new EmitirSentenciaHandler(
        mockSentenciaRepo.Object,
        mockUoW.Object,
        mockEventBus.Object
    );

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(resultado.Success);
    mockSentenciaRepo.Verify(r => r.AddAsync(It.IsAny<Sentencia>()), Times.Once);
    mockEventBus.Verify(eb => eb.Publish(It.IsAny<SentenciaEmitidaEvent>()), Times.Once);
    mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
}

[Fact]
public async Task NotificarSentenciaHandler_EnviaNotificacionesATodasLasPartes()
{
    // Arrange
    var sentenciaId = Guid.NewGuid();
    var command = new NotificarSentenciaCommand
    {
        SentenciaId = sentenciaId,
        PartesANotificar = new List<ParteProcesalDto>
        {
            new ParteProcesalDto { Id = Guid.NewGuid(), Tipo = TipoParteProcesalEnum.Demandante },
            new ParteProcesalDto { Id = Guid.NewGuid(), Tipo = TipoParteProcesalEnum.Demandado }
        }
    };
    
    var mockSentenciaRepo = new Mock<ISentenciaRepository>();
    var mockNotificacionService = new Mock<INotificacionService>();
    var mockUoW = new Mock<IUnitOfWork>();
    
    var sentencia = Sentencia.Crear(
        sentenciaId,
        Guid.NewGuid(),
        "Sentencia definitiva caso 123/2023",
        TipoSentencia.Definitiva
    );
    
    mockSentenciaRepo.Setup(r => r.GetByIdAsync(sentenciaId))
                     .ReturnsAsync(sentencia);
    
    var handler = new NotificarSentenciaHandler(
        mockSentenciaRepo.Object,
        mockNotificacionService.Object,
        mockUoW.Object
    );

    // Act
    var resultado = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(resultado.Success);
    mockNotificacionService.Verify(
        s => s.EnviarNotificacion(
            It.IsAny<Guid>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<TipoParteProcesalEnum>()),
        Times.Exactly(2));
}
```

## Ejemplos de Pruebas por Casos de Uso

### Para Presentación de Pruebas

#### Pruebas de Dominio
- Verificar que rechace pruebas documentales sin descripción
- Verificar validación de formatos de archivo permitidos
- Verificar creación y validación de pruebas periciales
- Verificar clasificación correcta por tipo de prueba

#### Pruebas de Aplicación
- Verificar registro exitoso de prueba documental
- Verificar que no permita registrar pruebas duplicadas
- Verificar registro de metadatos adicionales de una prueba

#### Pruebas de Infraestructura
- Verificar persistencia correcta en base de datos
- Verificar recuperación por ID y por tipo de prueba
- Verificar publicación de eventos tras registro de pruebas

### Para Interrogatorio

#### Pruebas de Dominio
- Verificar limitación de preguntas por interrogatorio
- Verificar reglas de validación de preguntas según tipo de interrogatorio
- Verificar registro correcto de testimonios y respuestas

#### Pruebas de Aplicación
- Verificar flujo del interrogatorio mediante una saga
- Verificar validadores de preguntas improcedentes
- Verificar cambios de estado durante el interrogatorio

#### Pruebas de Integración
- Verificar límites de tiempo para interrogatorios
- Verificar flujo completo de interrogatorio con preguntas y respuestas
- Verificar interacción con el repositorio de testigos

### Para Deliberación

#### Pruebas de Dominio
- Verificar cálculo de valor probatorio por categoría de prueba
- Verificar generación de considerandos basados en pruebas
- Verificar reglas de fundamentación legal en considerandos

#### Pruebas de Aplicación
- Verificar ponderación de pruebas combinadas
- Verificar generación de considerandos legales
- Verificar vinculación de pruebas con hechos probados

#### Pruebas de API
- Verificar consultas de avance de deliberación
- Verificar registro de fundamentos legales
- Verificar historial de modificaciones en deliberación

### Para Sentencia

#### Pruebas de Dominio
- Verificar estructura correcta de la sentencia
- Verificar validación de datos obligatorios (juez, descripción)
- Verificar adición y numeración correcta de resolutivos

#### Pruebas de Aplicación
- Verificar emisión de sentencia completa
- Verificar notificación a todas las partes procesales
- Verificar generación de versión oficial firmada

#### Pruebas de Integración
- Verificar flujo completo desde deliberación hasta sentencia
- Verificar firma digital del juez
- Verificar disponibilidad de sentencia para consulta

## Estrategias de Pruebas para Componentes de Arquitectura

### Pruebas para Repository Pattern

```csharp
[Fact]
public async Task Repository_GuardarYRecuperar_FuncionaCorrectamente()
{
    // Arrange
    var dbContextOptions = new DbContextOptionsBuilder<EtapaDeJuicioDbContext>()
        .UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString())
        .Options;
    
    // Repositorio de pruebas
    using var dbContext = new EtapaDeJuicioDbContext(dbContextOptions);
    var repository = new GenericRepository<Audiencia>(dbContext);
    
    var audiencia = Audiencia.Crear(
        Guid.NewGuid(),
        "Audiencia de juicio",
        DateTime.Now.AddDays(1),
        60 // duración en minutos
    );

    // Act - Guardar
    await repository.AddAsync(audiencia);
    await dbContext.SaveChangesAsync();
    
    // Act - Recuperar
    var audienciaRecuperada = await repository.GetByIdAsync(audiencia.Id);

    // Assert
    Assert.NotNull(audienciaRecuperada);
    Assert.Equal(audiencia.Id, audienciaRecuperada.Id);
    Assert.Equal(audiencia.Titulo, audienciaRecuperada.Titulo);
}
```

### Pruebas para Unit of Work

```csharp
[Fact]
public async Task UnitOfWork_GestionaTransaccionesCorrectamente()
{
    // Arrange
    var dbContextOptions = new DbContextOptionsBuilder<EtapaDeJuicioDbContext>()
        .UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString())
        .Options;
    
    using var dbContext = new EtapaDeJuicioDbContext(dbContextOptions);
    var unitOfWork = new UnitOfWork(dbContext);
    
    var sentenciaRepo = new SentenciaRepository(dbContext);
    var notificacionRepo = new NotificacionRepository(dbContext);
    
    var sentencia = Sentencia.Crear(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Sentencia final",
        TipoSentencia.Definitiva
    );
    
    // Act
    await unitOfWork.BeginTransactionAsync();
    try
    {
        await sentenciaRepo.AddAsync(sentencia);
        
        var notificacion = Notificacion.Crear(
            Guid.NewGuid(),
            sentencia.Id,
            "Notificación de sentencia",
            DateTime.Now
        );
        
        await notificacionRepo.AddAsync(notificacion);
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await unitOfWork.RollbackTransactionAsync();
        throw;
    }

    // Assert
    var sentenciaGuardada = await sentenciaRepo.GetByIdAsync(sentencia.Id);
    Assert.NotNull(sentenciaGuardada);
}
```

### Pruebas para Event Bus

```csharp
[Fact]
public async Task EventBus_PublicarYConsumirEvento_FuncionaCorrectamente()
{
    // Arrange
    var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
    var eventBus = new InMemoryEventBus(mockServiceScopeFactory.Object);
    
    var eventoRecibido = false;
    var eventoId = Guid.Empty;
    
    // Register handler
    eventBus.Subscribe<PruebaJudicialRegistradaEvent>(async (@event) => {
        eventoRecibido = true;
        eventoId = @event.PruebaId;
        await Task.CompletedTask;
    });
    
    var pruebaId = Guid.NewGuid();
    var evento = new PruebaJudicialRegistradaEvent(pruebaId);

    // Act
    await eventBus.Publish(evento);

    // Assert
    Assert.True(eventoRecibido);
    Assert.Equal(pruebaId, eventoId);
}
```

### Pruebas para Saga Pattern

```csharp
[Fact]
public async Task InterrogatorioSaga_CompletaFlujoCorrectamente()
{
    // Arrange
    var mockEventBus = new Mock<IEventBus>();
    var mockInterrogatorioRepo = new Mock<IInterrogatoriosRepository>();
    var mockTestigoRepo = new Mock<ITestigoRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    
    var saga = new InterrogatorioSaga(
        mockEventBus.Object,
        mockInterrogatorioRepo.Object,
        mockTestigoRepo.Object,
        mockUoW.Object
    );
    
    var interrogatorioId = Guid.NewGuid();
    var testigoId = Guid.NewGuid();
    
    // Act
    await saga.Iniciar(interrogatorioId, testigoId, DateTime.Now);
    await saga.RegistrarPregunta(interrogatorioId, "¿Qué observó ese día?");
    await saga.RegistrarRespuesta(interrogatorioId, "Vi al acusado salir del edificio.");
    await saga.Finalizar(interrogatorioId);

    // Assert
    mockInterrogatorioRepo.Verify(r => r.AddAsync(It.IsAny<Interrogatorio>()), Times.Once);
    mockEventBus.Verify(eb => eb.Publish(It.IsAny<InterrogatorioFinalizadoEvent>()), Times.Once);
}
```

## Herramientas a Utilizar

- **xUnit**: Framework de pruebas principal para .NET
- **Moq**: Para la creación de mocks en pruebas unitarias
- **FluentAssertions**: Para aserciones más expresivas
- **EF Core InMemory**: Para pruebas de repositorios sin base de datos real
- **Testcontainers**: Para pruebas con dependencias containerizadas
- **Coverlet**: Para medición de cobertura de código

## Configuración del Flujo CI/CD para TDD

1. **Pre-commit**: Ejecución de pruebas unitarias y análisis estático de código
2. **CI Pipeline**: Ejecución de pruebas unitarias, integración y análisis de cobertura
3. **Quality Gate**: Verificación de cobertura mínima del 80% antes de aceptar PR
4. **Reporting**: Generación de informes de pruebas y cobertura