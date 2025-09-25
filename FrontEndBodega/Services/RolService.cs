using FrontEndBodega.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FrontEndBodega.Services
{
    public class RolService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        public RolService(HttpClient httpClient, AutheService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<string> CrearRolAsync(RolDTO rol)
        {
            if (!await SetAuthorizationHeader())
                return "Error: No autorizado";

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/roles/", rol);
                if (response.IsSuccessStatusCode)
                    return "Rol creado correctamente ✅";

                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error al crear rol ❌: {response.StatusCode} - {errorContent}";
            }
            catch (Exception ex)
            {
                return $"Excepción: {ex.Message}";
            }
        }

        // 🔹 Helper para poner token en header
        private async Task<bool> SetAuthorizationHeader()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<List<RolDTO>> GetRolesAsync()
        {
            if (!await SetAuthorizationHeader())
                return new List<RolDTO>();

            try
            {
                var response = await _httpClient.GetAsync("api/roles/");
                if (response.IsSuccessStatusCode)
                {
                    var roles = await response.Content.ReadFromJsonAsync<List<RolDTO>>();
                    return roles ?? new List<RolDTO>();
                }
            }
            catch
            {
                // ignorar errores por ahora
            }

            return new List<RolDTO>();
        }

        public async Task<RolDTO?> GetRolByIdAsync(int id)
        {
            if (!await SetAuthorizationHeader()) return null;
            return await _httpClient.GetFromJsonAsync<RolDTO>($"api/roles/{id}");
        }

        public async Task<bool> ActualizarRolAsync(RolDTO rol)
        {
            if (!await SetAuthorizationHeader()) return false;
            var response = await _httpClient.PutAsJsonAsync($"api/roles/{rol.Id}", rol);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarRolAsync(int id)
        {
            if (!await SetAuthorizationHeader()) return false;
            var response = await _httpClient.DeleteAsync($"api/roles/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}