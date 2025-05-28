using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.UI.Web.Services
{
    public class AudienciaService : BaseApiService<Audiencia>
    {
        public AudienciaService(HttpClient httpClient) : base(httpClient, "/api/audiencias")
        {
        }

        // Aquí puedes agregar métodos específicos para audiencias si son necesarios
    }
}
