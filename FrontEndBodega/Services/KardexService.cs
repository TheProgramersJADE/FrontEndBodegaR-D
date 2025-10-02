using FrontEndBodega.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FrontEndBodega.Services
{
    public class KardexService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        // Endpoint para la descarga del Kardex completo en formato PDF.
        private const string ApiEndpointPdf = "api/kardex/pdf/todos";

        public KardexService(IHttpClientFactory httpClientFactory, AutheService authService)
        {
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

        public async Task<List<KardexDTO>> GetKardexAsync()
        {
            try
            {
                if (!await SetAuthorizationHeader())
                {
                    Console.WriteLine("⚠️ No se encontró token en sesión");
                    return new List<KardexDTO>();
                }

                var response = await _httpClient.GetAsync("api/kardex/todos");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error HTTP {response.StatusCode}: {error}");
                    return new List<KardexDTO>();
                }

                var data = await response.Content.ReadFromJsonAsync<List<KardexDTO>>(
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return data ?? new List<KardexDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción en GetKardexAsync: {ex.Message}");
                return new List<KardexDTO>();
            }
        }

        /// <summary>
        /// Llama a la API para obtener el Kardex completo como un archivo binario PDF, incluyendo la validación del token.
        /// </summary>
        /// <returns>Array de bytes (byte[]) que contiene el archivo PDF.</returns>
        public async Task<byte[]> ObtenerKardexPdfAsync()
        {
            try
            {
                // 1. Verificar y establecer el token de autenticación antes de la solicitud
                if (!await SetAuthorizationHeader())
                {
                    Console.WriteLine("⚠️ No se encontró token en sesión para descargar PDF");
                    // Devolver array vacío si no hay token
                    return Array.Empty<byte>();
                }

                // 2. Realiza la solicitud GET usando el _httpClient existente
                var response = await _httpClient.GetAsync(ApiEndpointPdf);

                // 3. Manejo de errores de la respuesta HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error HTTP {response.StatusCode} al descargar PDF: {error}");
                    // Devolver array vacío en caso de error HTTP
                    return Array.Empty<byte>();
                }

                // 4. Lee el contenido binario (el PDF) de la respuesta
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                // Captura errores inesperados (conexión, etc.)
                Console.WriteLine($"⚠️ Excepción en ObtenerKardexPdfAsync: {ex.Message}");
                // Devolver array vacío en caso de excepción
                return Array.Empty<byte>();
            }
        }
    }
}
