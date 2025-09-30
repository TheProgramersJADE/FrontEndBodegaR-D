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
        //public async Task<bool> CrearMovimiento(MovimientoDTO movimiento)
        //{
        //    if (!await SetAuthorizationHeader())
        //        return false;

        //    var response = await _httpClient.PostAsJsonAsync(
        //        "api/movimientosEntradaSalida",
        //        movimiento
        //    );

        //    return response.IsSuccessStatusCode;
        //}

        //public async Task<bool> CrearMovimiento(MovimientoDTO movimiento)
        //{
        //    if (!await SetAuthorizationHeader())
        //        return false;

        //    var response = await _httpClient.PostAsJsonAsync(
        //        "api/movimientosEntradaSalida/crear",
        //        movimiento
        //    );

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"❌ Error al crear movimiento: {response.StatusCode} - {error}");
        //    }

        //    return response.IsSuccessStatusCode;
        //}

        public async Task<bool> CrearMovimiento(MovimientoDTO movimiento)
        {
            if (!await SetAuthorizationHeader())
                return false;

            Console.WriteLine("➡️ Enviando movimiento: " +
                System.Text.Json.JsonSerializer.Serialize(movimiento));

            var response = await _httpClient.PostAsJsonAsync(
                "api/movimientosEntradaSalida", // 👈 revisa si en tu API es /crear o sin /crear
                movimiento
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al crear movimiento: {response.StatusCode} - {error}");
            }

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

        public async Task<MovimientoDetalleDTO?> ObtenerMovimientoPorId(int id)
        {
            var lista = await ObtenerMovimientos(); // llama al GET lista
            return lista.FirstOrDefault(m => m.Id == id);
        }


        public async Task<bool> ActualizarMovimiento(MovimientoDetalleDTO movimiento)
        {
            if (!await SetAuthorizationHeader())
                return false;

            // Solo enviar campos válidos para la API
            var update = new
            {
                idProducto = movimiento.IdProducto,
                idTipoMovimiento = movimiento.IdTipoMovimiento,
                cantidad = movimiento.Cantidad,
                precio = movimiento.Precio,
                observaciones = movimiento.Observaciones
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"api/movimientosEntradaSalida/{movimiento.Id}",
                update
            );

            return response.IsSuccessStatusCode;
        }


    }
}
