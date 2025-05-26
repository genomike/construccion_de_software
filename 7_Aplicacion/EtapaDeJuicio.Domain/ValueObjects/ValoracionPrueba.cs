namespace EtapaDeJuicio.Domain.ValueObjects;

public record ValoracionPrueba
{
    public TipoPrueba Tipo { get; }
    public decimal Peso { get; }
    public decimal FactorCredibilidad { get; }
    public decimal ValorFinal { get; }

    public ValoracionPrueba(TipoPrueba tipo, decimal peso, decimal factorCredibilidad)
    {
        Tipo = tipo;
        Peso = peso;
        FactorCredibilidad = factorCredibilidad;
        ValorFinal = peso * factorCredibilidad;
    }
}

public enum TipoPrueba
{
    Documental,
    Testimonial,
    Pericial,
    Material
}
