using EtapaDeJuicio.Domain.Exceptions;
using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.Domain.Entities.Sentencias;

public class ServicioValoracionPruebas
{
    public decimal CalcularValorProbatorio(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        if (!pruebas.Any())
            return 0;

        decimal valorTotal = 0;
        decimal pesoDocumental = 0.3m;
        decimal pesoTestimonial = 0.25m;
        decimal pesoPericial = 0.4m;
        decimal pesoMaterial = 0.35m;

        foreach (var prueba in pruebas.Where(p => p.EsValida))
        {
            switch (prueba.Tipo)
            {
                case Entities.Pruebas.TipoPrueba.Documental:
                    valorTotal += pesoDocumental;
                    break;                case Entities.Pruebas.TipoPrueba.Testimonial:
                    var testimonial = prueba as Entities.Pruebas.PruebaTestimonial;
                    valorTotal += pesoTestimonial * ObtenerValorCredibilidad(testimonial?.Credibilidad ?? CredibilidadTestigo.Media);
                    break;
                case Entities.Pruebas.TipoPrueba.Pericial:
                    valorTotal += pesoPericial;
                    break;
                case Entities.Pruebas.TipoPrueba.Material:
                    valorTotal += pesoMaterial;
                    break;
            }
        }

        return Math.Min(valorTotal, 1.0m); // Normalizar entre 0 y 1
    }

    public Dictionary<string, decimal> ObtenerValoracionDetallada(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        var valoracion = new Dictionary<string, decimal>();
        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();

        valoracion["Documentales"] = pruebasValidas.Count(p => p.Tipo == Entities.Pruebas.TipoPrueba.Documental) * 0.3m;        valoracion["Testimoniales"] = pruebasValidas.Where(p => p.Tipo == Entities.Pruebas.TipoPrueba.Testimonial)
            .Sum(p => 0.25m * ObtenerValorCredibilidad(((p as Entities.Pruebas.PruebaTestimonial)?.Credibilidad ?? CredibilidadTestigo.Media)));
        valoracion["Periciales"] = pruebasValidas.Count(p => p.Tipo == Entities.Pruebas.TipoPrueba.Pericial) * 0.4m;
        valoracion["Materiales"] = pruebasValidas.Count(p => p.Tipo == Entities.Pruebas.TipoPrueba.Material) * 0.35m;        return valoracion;
    }

    private decimal ObtenerValorCredibilidad(CredibilidadTestigo credibilidad)
    {
        return credibilidad switch
        {
            CredibilidadTestigo.Alta => 0.9m,
            CredibilidadTestigo.Media => 0.6m,
            CredibilidadTestigo.Baja => 0.3m,
            _ => 0.5m
        };
    }    public decimal CalcularValorTotal(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        if (!pruebas.Any())
            return 0;

        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();
        if (!pruebasValidas.Any())
            return 0;

        // Para una sola prueba, retornar directamente su valor probatorio
        if (pruebasValidas.Count == 1)
            return pruebasValidas[0].CalcularValorProbatorio();

        // Para múltiples pruebas, usar el método de valoración complejo
        return CalcularValorProbatorio(pruebasValidas);
    }

    public decimal CalcularValorPonderado(Dictionary<Entities.Pruebas.PruebaJudicial, decimal> pruebasConPesos)
    {
        if (pruebasConPesos.Values.Sum() > 1.0m)
            throw new DomainException("La suma de los pesos no puede exceder 1.0");

        decimal valorPonderado = 0;
        foreach (var item in pruebasConPesos)
        {
            var prueba = item.Key;
            var peso = item.Value;
            valorPonderado += prueba.CalcularValorProbatorio() * peso;
        }

        return valorPonderado;
    }

    public IEnumerable<Entities.Pruebas.PruebaJudicial> ObtenerPruebasRelevantes(
        IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas, 
        decimal umbralMinimo)
    {
        return pruebas.Where(p => p.EsValida && p.CalcularValorProbatorio() >= umbralMinimo);
    }

    public string GenerarReporteValoracion(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();
        var valorTotal = CalcularValorTotal(pruebasValidas);
        
        var reporte = new System.Text.StringBuilder();
        reporte.AppendLine("=== REPORTE DE VALORACIÓN DE PRUEBAS ===");
        reporte.AppendLine($"Número total de pruebas: {pruebasValidas.Count}");
        reporte.AppendLine();
        
        foreach (var prueba in pruebasValidas)
        {
            reporte.AppendLine($"- {prueba.Descripcion}: {prueba.CalcularValorProbatorio():F3}");
        }
        
        reporte.AppendLine();
        reporte.AppendLine($"Valor total: {valorTotal:F3}");
        
        return reporte.ToString();
    }

    public decimal AplicarFactorConfiabilidad(Entities.Pruebas.PruebaJudicial prueba, decimal factor)
    {
        if (factor < 0)
            throw new DomainException("El factor de confiabilidad no puede ser negativo");

        var valorOriginal = prueba.CalcularValorProbatorio();
        var valorConFactor = valorOriginal * factor;
        
        return Math.Min(valorConFactor, 1.0m); // No debe exceder 1.0
    }

    public string ClasificarFuerzaProbatoria(decimal valor)
    {
        return valor switch
        {
            0.0m => "Nula",
            <= 0.4m => "Baja",
            <= 0.7m => "Media",
            <= 0.9m => "Alta",
            _ => "Plena"
        };
    }

    public decimal EvaluarCoherenciaInterna(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();
        
        if (pruebasValidas.Count <= 1)
            return 1.0m; // Con una o menos pruebas, coherencia es máxima
        
        // Análisis básico de coherencia basado en tipos de prueba y credibilidad
        var testimoniales = pruebasValidas.OfType<Entities.Pruebas.PruebaTestimonial>().ToList();
          if (testimoniales.Count >= 2)
        {
            // Si hay testimonios con credibilidades similares, mayor coherencia
            var credibilidades = testimoniales.Select(t => (int)t.Credibilidad).ToList();
            
            // Calcular varianza manualmente
            var promedio = credibilidades.Average();
            var varianza = credibilidades.Sum(c => Math.Pow(c - promedio, 2)) / credibilidades.Count;
            
            // Menor varianza = mayor coherencia
            return Math.Max(0.3m, 1.0m - (decimal)varianza / 2);
        }
        
        return 0.8m; // Coherencia alta por defecto para otros casos
    }

    public decimal CalcularIndiceCredibilidad(IEnumerable<Entities.Pruebas.PruebaJudicial> pruebas)
    {
        var pruebasValidas = pruebas.Where(p => p.EsValida).ToList();
        
        if (!pruebasValidas.Any())
            return 0;
        
        decimal indiceAcumulado = 0;
        foreach (var prueba in pruebasValidas)
        {
            decimal factorTipo = prueba.Tipo switch
            {
                Entities.Pruebas.TipoPrueba.Documental => 0.9m,  // Documentos son muy confiables
                Entities.Pruebas.TipoPrueba.Pericial => 0.95m,   // Peritajes son los más confiables
                Entities.Pruebas.TipoPrueba.Material => 0.85m,   // Evidencia física es confiable
                Entities.Pruebas.TipoPrueba.Testimonial => GetFactorCredibilidadTestimonial(prueba),
                _ => 0.5m
            };
            
            indiceAcumulado += prueba.CalcularValorProbatorio() * factorTipo;
        }
        
        return Math.Min(indiceAcumulado / pruebasValidas.Count, 1.0m);
    }

    private decimal GetFactorCredibilidadTestimonial(Entities.Pruebas.PruebaJudicial prueba)
    {
        if (prueba is Entities.Pruebas.PruebaTestimonial testimonial)
        {
            return testimonial.Credibilidad switch
            {
                CredibilidadTestigo.Alta => 0.8m,
                CredibilidadTestigo.Media => 0.6m,
                CredibilidadTestigo.Baja => 0.4m,
                _ => 0.5m
            };
        }
        return 0.5m;
    }
}
