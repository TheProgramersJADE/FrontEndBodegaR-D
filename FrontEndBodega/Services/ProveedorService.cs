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
            var token = await _authService.GetToken(); // ✅ este sí existe en tu AutheService
            if (string.IsNullOrEmpty(token))
                return false;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<string> CrearProveedor(ProveedorDTO proveedor)
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token))
                return "❌ No hay token. Debes iniciar sesión.";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/proveedores", proveedor);

            if (response.IsSuccessStatusCode)
            {
                return "✅ Proveedor registrado correctamente";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(); // <- aquí lees el detalle
                return $"❌ Error {response.StatusCode}: {errorContent}";
            }
        }

        // Actualizar proveedor
        public async Task<string> ActualizarProveedorAsync(ProveedorDTO proveedor)
        {
            if (!await SetAuthorizationHeader())
                return "❌ Token inválido o expirado";

            try
            {
                var response = await client.PutAsJsonAsync($"api/proveedores/{proveedor.Id}", proveedor);
                if (response.IsSuccessStatusCode)
                    return "✅ Proveedor actualizado correctamente";

                var error = await response.Content.ReadAsStringAsync();
                return $"❌ Error {response.StatusCode}: {error}";
            }
            catch (Exception ex)
            {
                return $"❌ Error inesperado: {ex.Message}";
            }
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

            try
            {
                var response = await client.GetAsync("api/proveedores/lista");

                if (response.IsSuccessStatusCode)
                {
                    var listaResponse = await response.Content.ReadFromJsonAsync<ProveedoresResponse>();
                    return listaResponse?.data ?? new List<ProveedorDTO>();
                }

                return new List<ProveedorDTO>();
            }
            catch
            {
                return new List<ProveedorDTO>();
            }
        }

        public async Task<List<ProveedorDTO>> BuscarProveedoresAsync(string nombreEmpresa, int page = 0, int size = 5)
        {
            if (!await SetAuthorizationHeader())
                return new List<ProveedorDTO>();

            try
            {
                // Construir la URL con query params
                var url = $"proveedores/buscar?nombreEmpresa={Uri.EscapeDataString(nombreEmpresa)}&page={page}&size={size}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var listaResponse = await response.Content.ReadFromJsonAsync<ProveedoresResponse>();
                    return listaResponse?.data ?? new List<ProveedorDTO>();
                }

                return new List<ProveedorDTO>();
            }
            catch
            {
                return new List<ProveedorDTO>();
            }
        }

    }
}
