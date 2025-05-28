using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.UI.Web.Models;

namespace EtapaDeJuicio.UI.Web.Services
{
    public class SentenciaService : BaseApiService<Sentencia>
    {
        public SentenciaService(HttpClient httpClient) : base(httpClient, "/api/sentencias")
        {
        }

        // Métodos para trabajar con DTOs
        public async Task<List<SentenciaDto>> GetAllDtosAsync()
        {
            var sentencias = await GetAllAsync();
            return sentencias.Select(ConvertToDto).ToList();
        }

        public async Task<SentenciaDto?> GetDtoByIdAsync(Guid id)
        {
            var sentencia = await GetByIdAsync(id);
            return sentencia != null ? ConvertToDto(sentencia) : null;
        }        public async Task<SentenciaDto> CreateFromDtoAsync(SentenciaDto dto)
        {
            try
            {
                // Parseamos el JuezId
                if (!Guid.TryParse(dto.JuezId, out Guid juezId))
                    throw new ArgumentException("JuezId no es un GUID válido", nameof(dto.JuezId));

                // Parseamos el tipo de sentencia
                if (!Enum.TryParse<TipoSentencia>(dto.TipoSentencia, out var tipoSentencia))
                    throw new ArgumentException("TipoSentencia no es válido", nameof(dto.TipoSentencia));

                var sentencia = Sentencia.Crear(
                    dto.Id,
                    juezId,
                    dto.Descripcion,
                    tipoSentencia
                );                // Agregar resolutivos si los hay
                if (!string.IsNullOrEmpty(dto.Resolutivos))
                {
                    var resolutivos = dto.Resolutivos.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < resolutivos.Length; i++)
                    {
                        sentencia.AgregarResolutivo(resolutivos[i]);
                    }
                }                var creationResult = await CreateAsync(sentencia);
                if (creationResult)
                {
                    return ConvertToDto(sentencia);
                }
                else
                {
                    throw new InvalidOperationException("Error al crear la sentencia");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SentenciaDto> UpdateFromDtoAsync(SentenciaDto dto)
        {
            var existingSentencia = await GetByIdAsync(dto.Id);
            if (existingSentencia == null)
                throw new InvalidOperationException("Sentencia no encontrada");

            // Para actualizar una sentencia, necesitaríamos métodos específicos en la entidad
            // Por ahora, devolvemos el DTO como está
            return dto;
        }        private SentenciaDto ConvertToDto(Sentencia sentencia)
        {
            return new SentenciaDto
            {
                Id = sentencia.Id,
                JuezId = sentencia.JuezId.ToString(),
                Descripcion = sentencia.Descripcion,
                TipoSentencia = sentencia.Tipo.ToString(),
                FechaEmision = sentencia.FechaEmision,
                Resolutivos = string.Join("\n", sentencia.Resolutivos.Select(r => $"{r.Numeracion}: {r.Contenido}")),
                EstadoSentencia = "Emitida", // Estado por defecto
                ResumenDecision = sentencia.Descripcion, // Usar descripción como resumen
                FundamentosLegales = "", // Campo adicional del DTO
                AudienciaId = "", // Campo adicional del DTO
                FechaNotificacion = null // Campo adicional del DTO
            };
        }
    }

    public class DeliberacionService : BaseApiService<Deliberacion>
    {
        public DeliberacionService(HttpClient httpClient) : base(httpClient, "/api/sentencias/deliberaciones")
        {
        }
    }
}
