using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class PruebaPericial : PruebaJudicial
{
    public Guid PeritoId { get; private set; }
    public string ExpertoCertificado { get; private set; }
    public string InformePericial { get; private set; }
    public string Especialidad { get; private set; }

    private PruebaPericial(Guid id, string descripcion, Guid peritoId, string expertoCertificado, string especialidad) 
        : base(id, descripcion, TipoPrueba.Pericial)
    {
        if (peritoId == Guid.Empty)
            throw new DomainException("El ID del perito es obligatorio");
            
        if (string.IsNullOrWhiteSpace(expertoCertificado))
            throw new DomainException("El nombre del experto certificado es obligatorio");
            
        if (string.IsNullOrWhiteSpace(especialidad))
            throw new DomainException("La especialidad del perito es obligatoria");

        PeritoId = peritoId;
        ExpertoCertificado = expertoCertificado;
        Especialidad = especialidad;
        InformePericial = string.Empty;
    }    public static PruebaPericial Crear(Guid id, string descripcion, string expertoCertificado)
    {
        return new PruebaPericial(id, descripcion, Guid.NewGuid(), expertoCertificado, "General");
    }

    public static PruebaPericial Crear(Guid id, string descripcion, Guid peritoId, string expertoCertificado, string especialidad)
    {
        return new PruebaPericial(id, descripcion, peritoId, expertoCertificado, especialidad);
    }

    public void AgregarInforme(string informe)
    {
        if (string.IsNullOrWhiteSpace(informe))
            throw new DomainException("El informe pericial no puede estar vacío");
            
        InformePericial = informe;
    }

    public override void Validar()
    {
        if (string.IsNullOrWhiteSpace(InformePericial))
        {
            MarcarComoInvalida("El informe pericial no ha sido completado");
        }
    }

    public override decimal CalcularValorProbatorio()
    {
        if (string.IsNullOrWhiteSpace(InformePericial))
            return 0m;
            
        // Las pruebas periciales tienen alto valor probatorio cuando están completas
        decimal valorBase = 0.85m;
        
        // Bonificación por especialidad específica (no "General")
        if (Especialidad != "General")
            valorBase += 0.10m;
            
        return Math.Min(valorBase, 1.0m);
    }

    public override string ToString()
    {
        return $"Prueba Pericial - {Especialidad}: {Descripcion}";
    }
}
