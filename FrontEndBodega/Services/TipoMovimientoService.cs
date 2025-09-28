namespace FrontEndBodega.Services
{
    using global::FrontEndBodega.DTO;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class TipoMovimientoService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        public TipoMovimientoService(IHttpClientFactory httpClientFactory, AutheService authService)
        {
            // Obtener el HttpClient con la configuración registrada en Program.cs
            _httpClient = httpClientFactory.CreateClient("Administracion");
            _authService = authService;
        }

        public async Task<TipoMovimientoResponse?> CrearTipoMovimiento(TipoMovimientoDTO tipoMovimiento)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Solo la ruta relativa
            var response = await _httpClient.PostAsJsonAsync("api/tipoMovimientos", tipoMovimiento);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TipoMovimientoResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result;
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error al crear tipo de movimiento: " + errorMsg);
            return null;
        }

        public async Task<List<TipoMovimientoData>> GetTiposMovimientoAsync()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return new List<TipoMovimientoData>();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/tipoMovimientos/lista");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TipoMovimientoListResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Data ?? new List<TipoMovimientoData>();
            }

            return new List<TipoMovimientoData>();
        }

        // Obtener un tipo de movimiento por ID
        public async Task<TipoMovimientoData?> GetTipoMovimientoByIdAsync(int id)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/tipoMovimientos/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TipoMovimientoResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Data;
            }

            return null;
        }

        // Editar un tipo de movimiento
        public async Task<string> EditarTipoMovimientoAsync(int id, TipoMovimientoDTO tipoMovimiento)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return "No autorizado";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"api/tipoMovimientos/{id}", tipoMovimiento);
            if (response.IsSuccessStatusCode)
                return "Tipo de movimiento actualizado ✅";

            var errorContent = await response.Content.ReadAsStringAsync();
            return $"Error al actualizar ❌: {response.StatusCode} - {errorContent}";
        }

        // Eliminar un tipo de movimiento
        public async Task<bool> EliminarTipoMovimientoAsync(int id)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/tipoMovimientos/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TipoMovimientoData>> BuscarTipoMovimientosAsync(string nombre, int pagina = 0, int tamaño = 5)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return new List<TipoMovimientoData>();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/tipoMovimientos/buscar?nombre={Uri.EscapeDataString(nombre)}&page={pagina}&size={tamaño}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // La respuesta tiene "data.content"
                using var doc = JsonDocument.Parse(json);
                var content = doc.RootElement.GetProperty("data").GetProperty("content");

                var result = JsonSerializer.Deserialize<List<TipoMovimientoData>>(content.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<TipoMovimientoData>();
            }

            return new List<TipoMovimientoData>();
        }


    }
}
