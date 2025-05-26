using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.Domain.Interfaces.Services;

public interface IServicioValoracionPruebas
{
    decimal CalcularValorProbatorio(IEnumerable<PruebaJudicial> pruebas);
    Dictionary<string, decimal> ObtenerValoracionDetallada(IEnumerable<PruebaJudicial> pruebas);
}

public class ServicioValoracionPruebas : IServicioValoracionPruebas
{
    // Pesos por tipo de prueba según criterios legales
    private readonly Dictionary<TipoPrueba, decimal> _pesosPorTipo = new()
    {
        { TipoPrueba.Documental, 0.9m },
        { TipoPrueba.Pericial, 0.85m },
        { TipoPrueba.Testimonial, 0.7m },
        { TipoPrueba.Material, 0.8m }
    };

    public decimal CalcularValorProbatorio(IEnumerable<PruebaJudicial> pruebas)
    {
        if (!pruebas.Any())
            return 0m;

        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();
        if (!pruebasValidas.Any())
            return 0m;

        decimal valorTotal = 0m;
        decimal pesoTotal = 0m;

        foreach (var prueba in pruebasValidas)
        {
            var peso = _pesosPorTipo[prueba.Tipo];
            var valorPrueba = CalcularValorIndividual(prueba);
            
            valorTotal += valorPrueba * peso;
            pesoTotal += peso;
        }

        return Math.Min(valorTotal / pesoTotal, 1m); // Normalizar entre 0 y 1
    }

    public Dictionary<string, decimal> ObtenerValoracionDetallada(IEnumerable<PruebaJudicial> pruebas)
    {
        var valoracion = new Dictionary<string, decimal>();
        
        foreach (var prueba in pruebas.Where(p => p.EsValida))
        {
            var valor = CalcularValorIndividual(prueba) * _pesosPorTipo[prueba.Tipo];
            valoracion.Add($"{prueba.Tipo}_{prueba.Id}", valor);
        }

        return valoracion;
    }

    private decimal CalcularValorIndividual(PruebaJudicial prueba)
    {        return prueba.Tipo switch
        {
            TipoPrueba.Documental => 0.9m, // Alta confiabilidad
            TipoPrueba.Pericial => 0.85m,  // Muy alta confiabilidad
            TipoPrueba.Material => 0.8m,   // Alta confiabilidad
            TipoPrueba.Testimonial when prueba is PruebaTestimonial testimonial => testimonial.CalcularValorProbatorio(),
            _ => 0.5m // Valor por defecto
        };
    }
}
