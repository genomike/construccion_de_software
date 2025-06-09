using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.UI.Web.Models;
using System.Text.Json;

namespace EtapaDeJuicio.UI.Web.Services
{
    public class InterrogatorioService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseEndpoint = "/api/interrogatorios";

        public InterrogatorioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<Interrogatorio>> GetAllAsync()
        {
            try
            {
                Console.WriteLine($"Making request to: {_baseEndpoint}");
                var response = await _httpClient.GetAsync(_baseEndpoint);
                
                Console.WriteLine($"Response status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"HTTP Error {response.StatusCode}: {errorContent}");
                    return new List<Interrogatorio>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw API Response: {json}");
                
                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    Console.WriteLine("Empty or null response from API");
                    return new List<Interrogatorio>();
                }

                // Intentar deserializar como array de DTOs de la API
                var interrogatoriosDto = JsonSerializer.Deserialize<List<InterrogatorioApiDto>>(json, _jsonOptions);
                Console.WriteLine($"Deserialized {interrogatoriosDto?.Count ?? 0} interrogatorios from API");
                
                if (interrogatoriosDto == null || !interrogatoriosDto.Any())
                {
                    return new List<Interrogatorio>();
                }

                // Convertir DTOs a entidades de dominio
                var interrogatorios = new List<Interrogatorio>();
                foreach (var dto in interrogatoriosDto)
                {
                    try
                    {
                        var interrogatorio = ConvertToEntity(dto);
                        interrogatorios.Add(interrogatorio);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting interrogatorio {dto.Id}: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"Successfully converted {interrogatorios.Count} interrogatorios to domain entities");
                return interrogatorios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting interrogatorios: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<Interrogatorio>();
            }
        }        public async Task<bool> CreateAsync(Interrogatorio interrogatorio, Guid audienciaId)
        {
            try
            {
                Console.WriteLine($"Creating interrogatorio with ID: {interrogatorio.Id} for audiencia: {audienciaId}");
                
                // Crear el request que espera el controlador (ahora incluye AudienciaId)
                var request = new CrearInterrogatorioRequest(
                    interrogatorio.Id,
                    audienciaId, // Nuevo parámetro requerido
                    interrogatorio.Descripcion, // Usar descripción como pregunta
                    null, // Respuesta puede ser null
                    interrogatorio.FechaHora,
                    interrogatorio.Tipo.ToString() // Convertir enum a string
                );
                
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                Console.WriteLine($"Sending request: {json}");
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseEndpoint, content);
                
                Console.WriteLine($"Create response status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Create error response: {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating interrogatorio: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }        public async Task<bool> UpdateAsync(Guid id, Interrogatorio interrogatorio, Guid audienciaId)
        {
            try
            {
                Console.WriteLine($"Updating interrogatorio with ID: {id}");
                
                // Crear el request que espera el controlador
                var request = new CrearInterrogatorioRequest(
                    interrogatorio.Id,
                    audienciaId,
                    interrogatorio.Descripcion,
                    null,
                    interrogatorio.FechaHora,
                    interrogatorio.Tipo.ToString()
                );
                
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                Console.WriteLine($"Sending update request: {json}");
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{id}", content);
                
                Console.WriteLine($"Update response status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Update error response: {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating interrogatorio: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting interrogatorio: {ex.Message}");
                return false;
            }
        }

        // Nuevos métodos específicos para interrogatorios
        public async Task<Guid?> IniciarInterrogatorioTestigoAsync(Guid audienciaId, Guid testigoId, Guid interrogadorId, string tipoInterrogador)
        {
            try
            {
                Console.WriteLine($"Iniciando interrogatorio a testigo {testigoId} en audiencia {audienciaId}");
                
                var request = new IniciarInterrogatorioTestigoRequest(audienciaId, testigoId, interrogadorId, tipoInterrogador);
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseEndpoint}/testigo", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Guid.TryParse(responseJson.Trim('"'), out var actividadId))
                    {
                        return actividadId;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error iniciando interrogatorio a testigo: {ex.Message}");
                return null;
            }
        }

        public async Task<Guid?> IniciarInterrogatorioAcusadoAsync(Guid audienciaId, Guid acusadoId, Guid interrogadorId, string tipoInterrogador)
        {
            try
            {
                Console.WriteLine($"Iniciando interrogatorio a acusado {acusadoId} en audiencia {audienciaId}");
                
                var request = new IniciarInterrogatorioAcusadoRequest(audienciaId, acusadoId, interrogadorId, tipoInterrogador);
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseEndpoint}/acusado", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Guid.TryParse(responseJson.Trim('"'), out var actividadId))
                    {
                        return actividadId;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error iniciando interrogatorio a acusado: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RealizarPreguntaAsync(Guid audienciaId, Guid actividadId, string pregunta, Guid preguntadoPor)
        {
            try
            {
                var request = new RealizarPreguntaRequest(pregunta, preguntadoPor, DateTime.UtcNow);
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseEndpoint}/{audienciaId}/actividades/{actividadId}/preguntas", content);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error realizando pregunta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegistrarRespuestaAsync(Guid audienciaId, Guid actividadId, string respuesta, string? observaciones = null)
        {
            try
            {
                var request = new RegistrarRespuestaRequest(respuesta, DateTime.UtcNow, observaciones);
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseEndpoint}/{audienciaId}/actividades/{actividadId}/respuestas", content);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registrando respuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FinalizarInterrogatorioAsync(Guid audienciaId, Guid actividadId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseEndpoint}/{audienciaId}/actividades/{actividadId}/finalizar", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finalizando interrogatorio: {ex.Message}");
                return false;
            }
        }private Interrogatorio ConvertToEntity(InterrogatorioApiDto dto)
        {
            // Parse tipo de interrogatorio
            if (!Enum.TryParse<TipoInterrogatorio>(dto.Tipo?.ToString() ?? "", out var tipo))
            {
                tipo = TipoInterrogatorio.Directo; // Usar un valor válido por defecto
            }

            // Crear el interrogatorio con la descripción de la pregunta como descripción
            var descripcion = !string.IsNullOrEmpty(dto.Pregunta) ? dto.Pregunta : "Interrogatorio";
            return Interrogatorio.Crear(dto.Id, descripcion, dto.FechaHora, tipo);
        }
    }// DTO que coincide con lo que devuelve la API
    public class InterrogatorioApiDto
    {
        public Guid Id { get; set; }
        public string? Pregunta { get; set; }
        public string? Respuesta { get; set; }
        public DateTime FechaHora { get; set; }
        public TipoInterrogatorio? Tipo { get; set; }
    }    // Request que espera el controlador para crear interrogatorios
    public record CrearInterrogatorioRequest(Guid Id, Guid AudienciaId, string? Pregunta, string? Respuesta, DateTime FechaHora, string? Tipo);
    
    // Nuevos requests para los endpoints específicos
    public record IniciarInterrogatorioTestigoRequest(Guid AudienciaId, Guid TestigoId, Guid InterrogadorId, string TipoInterrogador);
    public record IniciarInterrogatorioAcusadoRequest(Guid AudienciaId, Guid AcusadoId, Guid InterrogadorId, string TipoInterrogador);
    public record RealizarPreguntaRequest(string Pregunta, Guid PreguntadoPor, DateTime Timestamp);
    public record RegistrarRespuestaRequest(string Respuesta, DateTime Timestamp, string? Observaciones);
}
