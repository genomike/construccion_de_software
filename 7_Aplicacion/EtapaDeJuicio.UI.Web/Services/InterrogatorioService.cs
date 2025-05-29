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
        }

        public async Task<bool> CreateAsync(Interrogatorio interrogatorio)
        {
            try
            {
                var dto = ConvertToDto(interrogatorio);
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating interrogatorio: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, Interrogatorio interrogatorio)
        {
            try
            {
                var dto = ConvertToDto(interrogatorio);
                var json = JsonSerializer.Serialize(dto, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating interrogatorio: {ex.Message}");
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
                Console.WriteLine($"Error deleting interrogatorio: {ex.Message}");
                return false;
            }
        }        private Interrogatorio ConvertToEntity(InterrogatorioApiDto dto)
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

        private InterrogatorioApiDto ConvertToDto(Interrogatorio interrogatorio)
        {
            // Como la API espera pregunta/respuesta pero el dominio tiene una estructura diferente,
            // vamos a usar la descripción como pregunta y null como respuesta por defecto
            return new InterrogatorioApiDto
            {
                Id = interrogatorio.Id,
                Pregunta = interrogatorio.Descripcion,
                Respuesta = null, // El dominio de Interrogatorio no tiene una respuesta directa
                FechaHora = interrogatorio.FechaHora,
                Tipo = interrogatorio.Tipo
            };
        }
    }

    // DTO que coincide con lo que devuelve la API
    public class InterrogatorioApiDto
    {
        public Guid Id { get; set; }
        public string? Pregunta { get; set; }
        public string? Respuesta { get; set; }
        public DateTime FechaHora { get; set; }
        public TipoInterrogatorio? Tipo { get; set; }
    }
}
