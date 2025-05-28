using System.Text.Json;

namespace EtapaDeJuicio.UI.Web.Services
{
    public abstract class BaseApiService<T> where T : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly string _baseEndpoint;

        protected BaseApiService(HttpClient httpClient, string baseEndpoint)
        {
            _httpClient = httpClient;
            _baseEndpoint = baseEndpoint;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseEndpoint);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting {typeof(T).Name}: {ex.Message}");
                return new List<T>();
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseEndpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting {typeof(T).Name} {id}: {ex.Message}");
                return null;
            }
        }

        public virtual async Task<bool> CreateAsync(T entity)
        {
            try
            {
                var json = JsonSerializer.Serialize(entity, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(Guid id, T entity)
        {
            try
            {
                var json = JsonSerializer.Serialize(entity, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseEndpoint}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {typeof(T).Name} {id}: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {typeof(T).Name} {id}: {ex.Message}");
                return false;
            }
        }
    }
}
