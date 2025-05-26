using EtapaDeJuicio.Domain.Exceptions;
using EtapaDeJuicio.Domain.ValueObjects;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class PruebaDocumental : PruebaJudicial
{
    public string RutaArchivo { get; private set; }
    public FormatoArchivo Formato { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public bool EstaVerificada { get; private set; }
    public DateTime? FechaVerificacion { get; private set; }
    public TipoPrueba TipoPrueba => TipoPrueba.Documental;

    private PruebaDocumental(Guid id, string descripcion, string rutaArchivo)
        : base(id, descripcion, TipoPrueba.Documental)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
            throw new DomainException("La ruta del archivo es obligatoria para pruebas documentales");

        RutaArchivo = rutaArchivo;
        Formato = FormatoArchivo.Create(rutaArchivo);
        FechaRegistro = DateTime.UtcNow;
        EstaVerificada = false;
    }

    public static PruebaDocumental Crear(Guid id, string descripcion, string rutaArchivo)
    {
        return new PruebaDocumental(id, descripcion, rutaArchivo);
    }

    public void ActualizarDescripcion(string nuevaDescripcion)
    {
        if (string.IsNullOrWhiteSpace(nuevaDescripcion))
            throw new DomainException("La descripción no puede estar vacía");

        Descripcion = nuevaDescripcion;
    }

    public void MarcarComoVerificada()
    {
        EstaVerificada = true;
        FechaVerificacion = DateTime.UtcNow;
    }    public override decimal CalcularValorProbatorio()
    {
        if (!EstaVerificada)
            return 0m;

        // Valor base para pruebas documentales verificadas (entre 0 y 1)
        decimal valorBase = 0.75m;

        // Bonificación por formato de archivo confiable
        if (Formato.EsConfiable())
            valorBase += 0.15m;

        // Penalización por antigüedad del registro
        var diasDesdeRegistro = (DateTime.UtcNow - FechaRegistro).Days;
        if (diasDesdeRegistro > 365)
            valorBase -= 0.10m;

        return Math.Max(0m, Math.Min(1.0m, valorBase));
    }

    public override void Validar()
    {
        if (!FormatoArchivo.EsValido(RutaArchivo))
            throw new DomainException($"El formato del archivo {RutaArchivo} no es válido");        if (!File.Exists(RutaArchivo))
        {
            MarcarComoInvalida("El archivo no existe en la ruta especificada");
        }
    }

    public override string ToString()
    {
        return $"Prueba Documental: {Descripcion}";
    }
}
