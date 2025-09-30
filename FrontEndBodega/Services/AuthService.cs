using FrontEndBodega.DTO;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;


namespace FrontEndBodega.Services
{
    public class AutheService
    {

        private readonly ProtectedSessionStorage _localStore;
        private readonly HttpClient _httpClient;
        private string? _token;

        public AutheService(ProtectedSessionStorage localStore, HttpClient httpClient)
        {
            _localStore = localStore;
            _httpClient = httpClient;
        }

        public async Task<string> Login(UserSession userSesion)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", userSesion);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<string>();
                return result;
            }
            return null;

        }

        public event Action? OnAuthStateChanged;

        public async Task SetToken(string token)
        {
            _token = token;
            await _localStore.SetAsync("token", token);
            OnAuthStateChanged?.Invoke(); // 🔔 Notificar cambio de sesión
        }

        public async Task<string?> GetToken()
        {
            try
            {
                var localStoreResult = await _localStore.GetAsync<string>("token");

                if (string.IsNullOrEmpty(_token))
                {
                    if (!localStoreResult.Success || string.IsNullOrEmpty(localStoreResult.Value))
                    {
                        _token = null;
                        return null;
                    }
                    _token = localStoreResult.Value;
                }
                return _token;
            }
            catch (InvalidOperationException)
            {
                // Estamos en prerender → no hay JS todavía
                return null;
            }
        }


        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();

            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);

        }

        public bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public async Task Logout()
        {
            _token = null;
            await _localStore.DeleteAsync("token");

        }

        public async Task<string?> GetUserRole()
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Busca claim de rol, puede ser "role" o "roles" según como tu API lo genere
            var roleClaim = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles");

            return roleClaim?.Value;
        }

        public async Task<List<string>> GetUserRoles()
        {
            var token = await GetToken();
            if (string.IsNullOrEmpty(token))
                return new List<string>();

            var jwtToken = new JwtSecurityToken(token);
            var roles = jwtToken.Claims
                                .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

            return roles;
        }



    }
}
