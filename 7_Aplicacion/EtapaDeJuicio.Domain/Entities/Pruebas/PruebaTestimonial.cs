using System.Collections.Generic;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public enum CredibilidadTestigo
{
    Alta,
    Media,
    Baja
}

public class PruebaTestimonial : PruebaJudicial
{    public Guid TestigoId { get; private set; }
    public Guid IdTestigo => TestigoId; // Alias para compatibilidad con tests
    public string Declaracion { get; private set; }
    public CredibilidadTestigo Credibilidad { get; private set; }
    public CredibilidadTestigo CredibilidadTestigo => Credibilidad; // Alias para compatibilidad con tests
    public TipoPrueba TipoPrueba => Tipo; // Alias para compatibilidad con tests
    public IReadOnlyCollection<string> Observaciones => _observaciones.AsReadOnly();
    private readonly List<string> _observaciones = new();

    private PruebaTestimonial(Guid id, string descripcion, Guid testigoId, string declaracion, CredibilidadTestigo credibilidad)
        : base(id, descripcion, TipoPrueba.Testimonial)
    {
        if (testigoId == Guid.Empty)
            throw new DomainException("El ID del testigo es obligatorio");
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción de la prueba testimonial es obligatoria");
        if (string.IsNullOrWhiteSpace(declaracion))
            throw new DomainException("La declaración del testigo no puede estar vacía");
        TestigoId = testigoId;
        Declaracion = declaracion;
        Credibilidad = credibilidad;
    }

    public static PruebaTestimonial Crear(Guid id, string descripcion, Guid testigoId, CredibilidadTestigo credibilidad)
    {
        return new PruebaTestimonial(id, descripcion, testigoId, "Testimonio pendiente", credibilidad);
    }

    public static PruebaTestimonial Crear(Guid id, string descripcion, Guid testigoId, string declaracion, CredibilidadTestigo credibilidad)
    {
        return new PruebaTestimonial(id, descripcion, testigoId, declaracion, credibilidad);
    }    public override decimal CalcularValorProbatorio()
    {
        return Credibilidad switch
        {
            CredibilidadTestigo.Alta => 0.9m,
            CredibilidadTestigo.Media => 0.6m,
            CredibilidadTestigo.Baja => 0.3m,
            _ => 0.0m
        };
    }

    public void ActualizarCredibilidad(CredibilidadTestigo nuevaCredibilidad)
    {
        Credibilidad = nuevaCredibilidad;
    }

    public void AgregarObservacion(string observacion)
    {
        if (string.IsNullOrWhiteSpace(observacion))
            throw new DomainException("La observación no puede estar vacía");
        _observaciones.Add(observacion);
    }

    public bool EsConfiable()
    {
        return Credibilidad == CredibilidadTestigo.Alta;
    }

    public override void Validar()
    {
        if (string.IsNullOrWhiteSpace(Declaracion) || Declaracion == "Testimonio pendiente")
        {
            MarcarComoInvalida("El testimonio no ha sido registrado");
        }
    }

    public override string ToString()
    {
        return $"{Descripcion} (Credibilidad: {Credibilidad})";
    }
}
