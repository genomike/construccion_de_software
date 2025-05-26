using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public enum TipoPrueba
{
    Documental,
    Testimonial,
    Pericial,
    Material
}

public abstract class PruebaJudicial
{
    public Guid Id { get; protected set; }
    public string Descripcion { get; protected set; }
    public DateTime FechaPresentacion { get; protected set; }
    public TipoPrueba Tipo { get; protected set; }
    public bool EsValida { get; protected set; }

    protected PruebaJudicial(Guid id, string descripcion, TipoPrueba tipo)
    {        if (id == Guid.Empty)
            throw new DomainException("El Id de la prueba no puede estar vacío");
            
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción de la prueba es obligatoria");

        Id = id;
        Descripcion = descripcion;
        Tipo = tipo;
        FechaPresentacion = DateTime.UtcNow;
        EsValida = true;
    }    public abstract void Validar();
    
    public abstract decimal CalcularValorProbatorio();
    
    public void MarcarComoInvalida(string motivo)
    {
        EsValida = false;
        // Aquí se podría agregar el motivo como propiedad
    }
}
