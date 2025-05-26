using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Sentencias;

public enum EstadoDeliberacion
{
    Iniciada,
    EnAnalisis,
    Pausada,
    Finalizada
}

public class Deliberacion
{
    private readonly List<ConsiderandoLegal> _considerandos = new();
    private readonly List<ValoracionPrueba> _valoraciones = new();

    public Guid Id { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }
    public bool EstaFinalizada { get; private set; }
    public EstadoDeliberacion Estado { get; private set; }
    public IReadOnlyList<ConsiderandoLegal> Considerandos => _considerandos.AsReadOnly();
    public IReadOnlyList<ValoracionPrueba> Valoraciones => _valoraciones.AsReadOnly();
      // Mantener compatibilidad con DeliberacionJudicialTests
    public string CasoNumero => Descripcion;
    public IReadOnlyList<ValoracionPrueba> ValoracionesPruebas => _valoraciones.AsReadOnly();
    public DateTime? FechaFinalizacion => FechaFin;

    private Deliberacion(Guid id, string descripcion, DateTime fechaInicio)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la deliberación no puede estar vacío");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción de la deliberación es obligatoria");

        Id = id;
        Descripcion = descripcion;
        FechaInicio = fechaInicio;
        EstaFinalizada = false;
        Estado = EstadoDeliberacion.Iniciada;
    }    // Constructor para compatibilidad con DeliberacionJudicialTests
    private Deliberacion(Guid id, string casoNumero) 
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la deliberación no puede estar vacío");

        if (string.IsNullOrWhiteSpace(casoNumero))
            throw new DomainException("El número de caso es obligatorio");

        Id = id;
        Descripcion = casoNumero;
        FechaInicio = DateTime.UtcNow;
        EstaFinalizada = false;
        Estado = EstadoDeliberacion.Iniciada;
    }

    public static Deliberacion Crear(Guid id, string descripcion, DateTime fechaInicio)
    {
        return new Deliberacion(id, descripcion, fechaInicio);
    }

    public static Deliberacion Crear(Guid id, string casoNumero)
    {
        return new Deliberacion(id, casoNumero);
    }    public void GenerarConsiderandos()
    {
        if (_valoraciones.Count == 0)
            throw new DomainException("No se puede generar considerandos sin pruebas valoradas");

        if (EstaFinalizada)
            throw new DomainException("No se pueden generar considerandos en una deliberación finalizada");

        // Generar considerandos basados en las valoraciones
        var considerandoPruebas = ConsiderandoLegal.Crear(
            "CONSIDERANDO que las pruebas aportadas han sido debidamente valoradas según los principios de la sana crítica",
            _considerandos.Count + 1);

        _considerandos.Add(considerandoPruebas);
    }

    public void AgregarValoracionPrueba(Guid pruebaId, decimal valor, string justificacion)
    {
        if (EstaFinalizada)
            throw new DomainException("No se pueden agregar valoraciones a una deliberación finalizada");

        var valoracion = ValoracionPrueba.Crear(pruebaId, valor, justificacion);
        _valoraciones.Add(valoracion);
    }    public void AgregarConsiderando(string contenido)
    {
        if (Estado == EstadoDeliberacion.Finalizada)
            throw new DomainException("No se pueden agregar considerandos a una deliberación finalizada");

        var considerando = ConsiderandoLegal.Crear(contenido, _considerandos.Count + 1);
        _considerandos.Add(considerando);
    }

    public void AgregarConsiderando(string fundamentoLegal, string analisis)
    {
        if (Estado == EstadoDeliberacion.Finalizada)
            throw new DomainException("No se pueden agregar considerandos a una deliberación finalizada");

        var considerando = ConsiderandoLegal.Crear(fundamentoLegal, analisis, _considerandos.Count + 1);
        _considerandos.Add(considerando);
    }

    public void AgregarValoracion(Entities.Pruebas.PruebaJudicial prueba, decimal valoracion, string justificacion)
    {
        if (EstaFinalizada)
            throw new DomainException("No se pueden agregar valoraciones a una deliberación finalizada");        var valoracionPrueba = ValoracionPrueba.Crear(prueba, valoracion, justificacion);
        _valoraciones.Add(valoracionPrueba);
    }

    public decimal CalcularValorTotalPruebas()
    {
        if (_valoraciones.Count == 0)
            return 0m;

        return _valoraciones.Sum(v => v.Valor) / _valoraciones.Count;
    }

    public TimeSpan ObtenerDuracion()
    {
        var fechaFin = FechaFin ?? DateTime.UtcNow;
        return fechaFin - FechaInicio;
    }

    public bool ValidarCompletitud()
    {
        return _considerandos.Count > 0 && _valoraciones.Count > 0;
    }    public string ObtenerResumen()
    {
        var considerandosCount = _considerandos.Count;
        var valoracionesCount = _valoraciones.Count;
        var valorPromedio = CalcularValorTotalPruebas();
        
        var valoracionesTexto = valoracionesCount == 1 ? "1 valoración" : $"{valoracionesCount} valoraciones";
        
        return $"Deliberación: {Descripcion}. " +
               $"Considerandos: {considerandosCount}. " +
               $"{valoracionesTexto}. " +
               $"Valor promedio: {valorPromedio.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}. " +
               $"Estado: {Estado}";
    }

    public bool PuedeModificar()
    {
        return Estado != EstadoDeliberacion.Finalizada;
    }    public void Finalizar()
    {
        if (_considerandos.Count == 0)
            throw new DomainException("No se puede finalizar una deliberación sin considerandos");

        EstaFinalizada = true;
        FechaFin = DateTime.UtcNow;
        Estado = EstadoDeliberacion.Finalizada;
    }

    public void IniciarAnalisis()
    {
        if (Estado == EstadoDeliberacion.Finalizada)
            throw new DomainException("No se puede modificar una deliberación finalizada");
        
        Estado = EstadoDeliberacion.EnAnalisis;
    }

    public void Pausar()
    {
        if (Estado == EstadoDeliberacion.Finalizada)
            throw new DomainException("No se puede pausar una deliberación finalizada");
        
        Estado = EstadoDeliberacion.Pausada;
    }

    public void Reanudar()
    {
        if (Estado == EstadoDeliberacion.Finalizada)
            throw new DomainException("No se puede reanudar una deliberación finalizada");
        
        Estado = EstadoDeliberacion.EnAnalisis;
    }
}
