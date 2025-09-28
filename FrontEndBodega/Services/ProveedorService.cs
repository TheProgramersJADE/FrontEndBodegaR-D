using FrontEndBodega.DTO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FrontEndBodega.Services
{
    public class ProveedorService
    {
        private readonly HttpClient client;
        private readonly AutheService _authService;

        public ProveedorService(IHttpClientFactory httpClientFactory, AutheService authService)
        {
            client = httpClientFactory.CreateClient("Administracion");
            _authService = authService;
        }

        private async Task<bool> SetAuthorizationHeader()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token))
                return false;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<string> CrearProveedor(ProveedorDTO proveedor)
        {
            if (!await SetAuthorizationHeader())
                return "❌ No hay token. Debes iniciar sesión.";

            var response = await client.PostAsJsonAsync("api/proveedores", proveedor);

            if (response.IsSuccessStatusCode)
                return "✅ Proveedor registrado correctamente";

            var error = await response.Content.ReadAsStringAsync();
            return $"❌ Error {response.StatusCode}: {error}";
        }

        public async Task<string> ActualizarProveedorAsync(ProveedorDTO proveedor)
        {
            if (!await SetAuthorizationHeader())
                return "❌ Token inválido o expirado";

            var response = await client.PutAsJsonAsync($"api/proveedores/{proveedor.Id}", proveedor);
            if (response.IsSuccessStatusCode)
                return "✅ Proveedor actualizado correctamente";

            var error = await response.Content.ReadAsStringAsync();
            return $"❌ Error {response.StatusCode}: {error}";
        }

        public async Task<string> EliminarProveedorAsync(ProveedorDTO proveedor)
        {
            if (!await SetAuthorizationHeader())
                return "❌ Token inválido o expirado";

            var response = await client.DeleteAsync($"api/proveedores/{proveedor.Id}");
            if (response.IsSuccessStatusCode)
                return "✅ Proveedor eliminado correctamente";

            var error = await response.Content.ReadAsStringAsync();
            return $"❌ Error {response.StatusCode}: {error}";
        }

        public async Task<List<ProveedorDTO>> ObtenerProveedoresAsync()
        {
            if (!await SetAuthorizationHeader())
                return new List<ProveedorDTO>();

            var response = await client.GetAsync("api/proveedores/lista");

            if (response.IsSuccessStatusCode)
            {
                var listaResponse = await response.Content.ReadFromJsonAsync<ProveedoresListaResponse>();
                return listaResponse?.data ?? new List<ProveedorDTO>();
            }

            return new List<ProveedorDTO>();
        }


        public async Task<List<ProveedorDTO>> BuscarProveedoresAsync(string nombreEmpresa, int page = 0, int size = 100)
        {
            if (!await SetAuthorizationHeader())
                return new List<ProveedorDTO>();

            var url = $"api/proveedores/buscar?nombre={Uri.EscapeDataString(nombreEmpresa)}&page={page}&size={size}"; var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var listaResponse = await response.Content.ReadFromJsonAsync<ProveedoresResponse>();
                return listaResponse?.data?.content ?? new List<ProveedorDTO>();
            }

            return new List<ProveedorDTO>();
        }


    }
}
