using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.UI.Web.Services
{
    public class PruebaDocumentalService : BaseApiService<PruebaDocumental>
    {
        public PruebaDocumentalService(HttpClient httpClient) : base(httpClient, "/api/pruebas/documentales")
        {
        }
    }

    public class PruebaTestimonialService : BaseApiService<PruebaTestimonial>
    {
        public PruebaTestimonialService(HttpClient httpClient) : base(httpClient, "/api/pruebas/testimoniales")
        {
        }
    }

    public class PruebaPericialesService : BaseApiService<PruebaPericial>
    {
        public PruebaPericialesService(HttpClient httpClient) : base(httpClient, "/api/pruebas/periciales")
        {
        }
    }

    // Servicio general para todas las pruebas
    public class PruebasService : BaseApiService<PruebaJudicial>
    {
        public PruebasService(HttpClient httpClient) : base(httpClient, "/api/pruebas")
        {
        }
    }
}
