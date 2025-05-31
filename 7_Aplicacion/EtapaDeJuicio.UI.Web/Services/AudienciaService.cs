using EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.UI.Web.Models;
using System.Text.Json;

namespace EtapaDeJuicio.UI.Web.Services
{
    public class AudienciaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseEndpoint = "/api/audiencias";

        public AudienciaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }        public async Task<List<Audiencia>> GetAllAsync()
        {
            try
            {
                Console.WriteLine($"=== AudienciaService.GetAllAsync() STARTED ===");
                Console.WriteLine($"Making request to: {_baseEndpoint}");
                Console.WriteLine($"Full URL: {_httpClient.BaseAddress}{_baseEndpoint}");
                
                var response = await _httpClient.GetAsync(_baseEndpoint);
                
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response headers: {response.Headers}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"HTTP Error {response.StatusCode}: {errorContent}");
                    return new List<Audiencia>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw API Response: {json}");
                
                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    Console.WriteLine("Empty or null response from API");
                    return new List<Audiencia>();
                }

                // Intentar deserializar como array de DTOs de la API
                var audienciasDto = JsonSerializer.Deserialize<List<AudienciaApiDto>>(json, _jsonOptions);
                Console.WriteLine($"Deserialized {audienciasDto?.Count ?? 0} audiencias from API");
                
                if (audienciasDto == null || !audienciasDto.Any())
                {
                    Console.WriteLine("No audiencias after deserialization");
                    return new List<Audiencia>();
                }

                // Convertir DTOs a entidades de dominio
                var audiencias = new List<Audiencia>();
                foreach (var dto in audienciasDto)
                {
                    try
                    {
                        Console.WriteLine($"Converting DTO: ID={dto.Id}, Titulo={dto.Titulo}, Tipo={dto.TipoAudiencia}, Fecha={dto.FechaProgramada}");
                        var audiencia = ConvertToEntity(dto);
                        audiencias.Add(audiencia);
                        Console.WriteLine($"Successfully converted audiencia {dto.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting audiencia {dto.Id}: {ex.Message}");
                        Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    }
                }
                
                Console.WriteLine($"Successfully converted {audiencias.Count} audiencias to domain entities");
                Console.WriteLine($"=== AudienciaService.GetAllAsync() COMPLETED ===");
                return audiencias;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** EXCEPTION IN AudienciaService.GetAllAsync(): {ex.Message} ***");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return new List<Audiencia>();
            }
        }

        public async Task<bool> CreateAsync(Audiencia audiencia)
        {
            try
            {
                var dto = ConvertToDto(audiencia);
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating audiencia: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, Audiencia audiencia)
        {
            try
            {
                var dto = ConvertToDto(audiencia);
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating audiencia: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting audiencia: {ex.Message}");
                return false;
            }
        }        private Audiencia ConvertToEntity(AudienciaApiDto dto)
        {
            // Parse tipo de audiencia desde string
            if (!Enum.TryParse<TipoAudiencia>(dto.TipoAudiencia ?? "", out var tipo))
            {
                tipo = TipoAudiencia.JuicioOral;
            }

            // Parse estado de audiencia desde string
            if (!Enum.TryParse<EstadoAudiencia>(dto.Estado ?? "", out var estado))
            {
                estado = EstadoAudiencia.Programada;
            }

            // Usar Reconstruir para cargar desde API sin validación de fecha futura
            return Audiencia.Reconstruir(dto.Id, dto.Titulo ?? "", dto.FechaProgramada, tipo, estado);
        }private AudienciaApiDto ConvertToDto(Audiencia audiencia)
        {
            return new AudienciaApiDto
            {
                Id = audiencia.Id,
                Titulo = audiencia.Titulo,
                FechaProgramada = audiencia.FechaHoraProgramada,
                TipoAudiencia = audiencia.Tipo.ToString(),
                Estado = audiencia.Estado.ToString()
            };
        }
    }    // DTO que coincide con lo que devuelve la API
    public class AudienciaApiDto
    {
        public Guid Id { get; set; }
        public string? Titulo { get; set; }
        public DateTime FechaProgramada { get; set; }
        public string? TipoAudiencia { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Duracion { get; set; }
        public List<object>? Participantes { get; set; }
        public List<object>? Actividades { get; set; }
    }
}
