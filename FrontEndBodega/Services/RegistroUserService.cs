namespace FrontEndBodega.Services
{

 using System.Net.Http.Json;
 using FrontEndBodega.DTO;

    public class RegistroUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;

        public RegistroUserService(HttpClient httpClient, AutheService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<string> CrearUsuarioAsync(RegistroUserDTO usuario)
        {
            try
            {
                // Obtener token desde AutheService
                var token = await _authService.GetToken();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

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
    }
}
