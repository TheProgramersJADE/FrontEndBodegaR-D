using FrontEndBodega.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontEndBodega.Services
{
    public class MovimientoService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        public MovimientoService(IHttpClientFactory httpClientFactory, AutheService authService)
        {
            // Obtenemos el HttpClient configurado en Program.cs
            _httpClient = httpClientFactory.CreateClient("Administracion");
            _authService = authService;
        }

        private async Task<bool> SetAuthorizationHeader()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        // ✅ Crear un movimiento
        public async Task<bool> CrearMovimiento(MovimientoDTO movimiento)
        {
            if (!await SetAuthorizationHeader())
                return false;

            var response = await _httpClient.PostAsJsonAsync(
                "api/movimientosEntradaSalida",
                movimiento
            );

            return response.IsSuccessStatusCode;
        }

        // ✅ Obtener lista de movimientos desde la API
        public async Task<List<MovimientoDetalleDTO>> ObtenerMovimientos()
        {
            try
            {
                if (!await SetAuthorizationHeader())
                    return new List<MovimientoDetalleDTO>();

                var response = await _httpClient.GetAsync("api/movimientosEntradaSalida/lista");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error HTTP {response.StatusCode}");
                    return new List<MovimientoDetalleDTO>();
                }

                var result = await response.Content.ReadFromJsonAsync<MovimientoListResponse>(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

                return result?.Data ?? new List<MovimientoDetalleDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción en ObtenerMovimientos: {ex.Message}");
                return new List<MovimientoDetalleDTO>();
            }
        }
    }
}
