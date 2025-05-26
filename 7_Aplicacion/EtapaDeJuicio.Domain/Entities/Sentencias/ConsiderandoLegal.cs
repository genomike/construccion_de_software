using EtapaDeJuicio.Domain.Exceptions;
using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.Domain.Entities.Sentencias;

public class ConsiderandoLegal
{
    public Guid Id { get; private set; }
    public string Contenido { get; private set; }
    public int Orden { get; private set; }
    public List<Guid> PruebasReferenciadas { get; private set; } = new();
      // Propiedades para compatibilidad con tests
    public string? FundamentoLegal { get; private set; }
    public string? Analisis { get; private set; }private ConsiderandoLegal(Guid id, string contenido, int orden)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del considerando no puede estar vacío");

        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido del considerando es obligatorio");

        if (orden <= 0)
            throw new DomainException("El orden del considerando debe ser mayor a cero");

        Id = id;
        Contenido = contenido;
        Orden = orden;
    }

    private ConsiderandoLegal(Guid id, string fundamentoLegal, string analisis, int orden)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del considerando no puede estar vacío");

        if (string.IsNullOrWhiteSpace(fundamentoLegal))
            throw new DomainException("El fundamento legal es obligatorio");

        if (string.IsNullOrWhiteSpace(analisis))
            throw new DomainException("El análisis es obligatorio");

        if (orden <= 0)
            throw new DomainException("El orden del considerando debe ser mayor a cero");

        Id = id;
        FundamentoLegal = fundamentoLegal;
        Analisis = analisis;
        Contenido = $"{fundamentoLegal}: {analisis}";
        Orden = orden;
    }    public static ConsiderandoLegal Crear(string contenido, int orden)
    {
        return new ConsiderandoLegal(Guid.NewGuid(), contenido, orden);
    }

    public static ConsiderandoLegal Crear(string fundamentoLegal, string analisis, int orden)
    {
        return new ConsiderandoLegal(Guid.NewGuid(), fundamentoLegal, analisis, orden);
    }

    public void AgregarReferenciaAPrueba(Guid pruebaId)
    {
        if (pruebaId == Guid.Empty)
            throw new DomainException("El ID de la prueba no puede estar vacío");

        if (!PruebasReferenciadas.Contains(pruebaId))
        {
            PruebasReferenciadas.Add(pruebaId);
        }
    }
}

public class ValoracionPrueba
{
    public Guid PruebaId { get; private set; }
    public decimal Valor { get; private set; }
    public string Justificacion { get; private set; }
      // Propiedad para compatibilidad con tests
    public PruebaJudicial? Prueba { get; private set; }    private ValoracionPrueba(Guid pruebaId, decimal valor, string justificacion)
    {
        if (pruebaId == Guid.Empty)
            throw new DomainException("El ID de la prueba no puede estar vacío");

        if (valor < 0 || valor > 1)
            throw new DomainException("El valor de la prueba debe estar entre 0 y 1");

        if (string.IsNullOrWhiteSpace(justificacion))
            throw new DomainException("La justificación es obligatoria");

        PruebaId = pruebaId;
        Valor = valor;
        Justificacion = justificacion;
    }

    private ValoracionPrueba(PruebaJudicial prueba, decimal valor, string justificacion)
    {
        if (prueba == null)
            throw new DomainException("La prueba no puede ser nula");

        if (valor < 0 || valor > 1)
            throw new DomainException("El valor de la prueba debe estar entre 0 y 1");

        if (string.IsNullOrWhiteSpace(justificacion))
            throw new DomainException("La justificación es obligatoria");

        PruebaId = prueba.Id;
        Prueba = prueba;
        Valor = valor;
        Justificacion = justificacion;
    }    public static ValoracionPrueba Crear(Guid pruebaId, decimal valor, string justificacion)
    {
        return new ValoracionPrueba(pruebaId, valor, justificacion);
    }

    public static ValoracionPrueba Crear(PruebaJudicial prueba, decimal valor, string justificacion)
    {
        return new ValoracionPrueba(prueba, valor, justificacion);
    }
}
