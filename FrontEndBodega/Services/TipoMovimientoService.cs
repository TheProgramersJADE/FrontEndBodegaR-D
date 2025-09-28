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
    }
}
