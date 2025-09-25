using FrontEndBodega.DTO;
using System.Net.Http.Json;    
using FrontEndBodega.Services;

namespace FrontEndBodega.Services
    {
        public class CategoriaService
        {
            private readonly HttpClient client;
            private readonly AutheService _authService;

        // 👇 Pide el cliente "Administracion"
        public CategoriaService(IHttpClientFactory httpClientFactory, AutheService authService)
        {
            client = httpClientFactory.CreateClient("Administracion");
            _authService = authService;
        }

        // 🔹 Helper para setear el token
        private async Task<bool> SetAuthorizationHeader()
            {
                var token = await _authService.GetToken();
                if (string.IsNullOrEmpty(token))
                    return false;

            client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                return true;
            }

            // Crear categoría
            public async Task<string> CrearCategoriaAsync(CategoriaDTO categoria)
            {
                if (!await SetAuthorizationHeader()) return "Error: No autorizado";

                var response = await client.PostAsJsonAsync("api/categorias", categoria);

                if (response.IsSuccessStatusCode)
                    return "✅ Categoría creada correctamente";
                else
                    return $"❌ Error al crear categoría: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

        // Listar todas las categorías
        public async Task<List<CategoriaDTO>> GetCategoriasAsync()
        {
            if (!await SetAuthorizationHeader()) return new List<CategoriaDTO>();

            var response = await client.GetAsync("api/categorias/lista");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CategoriaListResponse>();
                return result?.data ?? new List<CategoriaDTO>();
            }

            return new List<CategoriaDTO>();
        }

        // Obtener categoría por Id
        public async Task<CategoriaDTO?> GetCategoriaByIdAsync(int id)
        {
            if (!await SetAuthorizationHeader()) return null;

            var response = await client.GetAsync($"api/categorias/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CategoriaSingleResponse>();
                return result?.data;
            }
            return null;
        }





        //  Editar categoría
        public async Task<string> EditarCategoriaAsync(int id, CategoriaDTO categoria)
            {
                if (!await SetAuthorizationHeader()) return "Error: No autorizado";

                var response = await client.PutAsJsonAsync($"api/categorias/{id}", categoria);
                if (response.IsSuccessStatusCode)
                    return "✅ Categoría actualizada correctamente";

                return $"❌ Error al actualizar: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

            //  Eliminar categoría
            public async Task<bool> EliminarCategoriaAsync(int id)
            {
                if (!await SetAuthorizationHeader()) return false;

                var response = await client.DeleteAsync($"api/categorias/{id}");
                return response.IsSuccessStatusCode;
            }
    }

}
