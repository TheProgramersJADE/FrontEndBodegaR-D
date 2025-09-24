
using System.Net.Http.Json;
using FrontEndBodega.DTO;

namespace FrontEndBodega.Services
{
    public class RegistroUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        public RegistroUserService(HttpClient httpClient, AutheService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        // Helper para reutilizar token
        private async Task<bool> SetAuthorizationHeader()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return true;
        }
        public async Task<string> CrearUsuarioAsync(RegistroUserDTO usuario)
        {
            try
            {
                if (!await SetAuthorizationHeader())
                    return "Error: No autorizado";

                var response = await _httpClient.PostAsJsonAsync("api/users/", usuario);
                if (response.IsSuccessStatusCode)
                {
                    return "Usuario creado correctamente ✅";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al crear usuario ❌: {response.StatusCode} - {errorContent}";
                }
            }

            catch (Exception ex)
            {
                return $"Excepción: {ex.Message}";
            }
        }

        // Obtener lista de usuarios
        public async Task<List<UsuarioDTO>> GetUsuariosAsync()
        {
            if (!await SetAuthorizationHeader())
                return new List<UsuarioDTO>();

            var response = await _httpClient.GetAsync("api/users/");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<UsuarioDTO>>() ?? new List<UsuarioDTO>();

            return new List<UsuarioDTO>();
        }

        // Eliminar usuario
        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            if (!await SetAuthorizationHeader())
                return false;

            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }

        // Obtener usuario por Id
        public async Task<UsuarioDTO?> GetUsuarioByIdAsync(int id)
        {
            if (!await SetAuthorizationHeader())
                return null;

            var response = await _httpClient.GetAsync($"api/users/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UsuarioDTO>();

            return null;
        }

        // Editar usuario
        public async Task<string> EditarUsuarioAsync(int id, UsuarioDTO usuario)
        {
            if (!await SetAuthorizationHeader())
                return "Error: No autorizado";

            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", usuario);
            if (response.IsSuccessStatusCode)
                return "Usuario actualizado ✅";

            var errorContent = await response.Content.ReadAsStringAsync();
            return $"Error al actualizar usuario ❌: {response.StatusCode} - {errorContent}";
        }
    }
}
