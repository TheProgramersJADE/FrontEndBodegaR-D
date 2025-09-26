using FrontEndBodega.DTO;
using global::FrontEndBodega.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace FrontEndBodega.Services
{
        public class ProductoService
        {
            private readonly HttpClient client;
            private readonly AutheService _authService;

            public ProductoService(IHttpClientFactory httpClientFactory, AutheService authService)
            {
                client = httpClientFactory.CreateClient("Administracion");
                _authService = authService;
            }

            private async Task<bool> SetAuthorizationHeader()
            {
                var token = await _authService.GetToken();
                if (string.IsNullOrEmpty(token)) return false;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }

            // Listar todos
            public async Task<List<ProductoDTO>> GetProductosAsync()
            {
                if (!await SetAuthorizationHeader()) return new List<ProductoDTO>();
                var resp = await client.GetAsync("api/productos/lista");
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<ProductoListResponse>();
                    return result?.Data ?? new List<ProductoDTO>();
                }
                return new List<ProductoDTO>();
            }

            // Obtener por id
            public async Task<ProductoDTO?> GetProductoByIdAsync(int id)
            {
                if (!await SetAuthorizationHeader()) return null;
                var resp = await client.GetAsync($"api/productos/{id}");
                if (resp.IsSuccessStatusCode)
                {
                    var result = await resp.Content.ReadFromJsonAsync<ProductoSingleResponse>();
                    return result?.Data;
                }
                return null;
            }

            // Crear (JSON)
            public async Task<string> CrearProductoAsync(ProductoDTO producto)
            {
                if (!await SetAuthorizationHeader()) return "Error: No autorizado";
                var resp = await client.PostAsJsonAsync("api/productos", producto);
                if (resp.IsSuccessStatusCode) return "✅ Producto creado correctamente";
                return $"❌ Error al crear: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync()}";
            }

            // Editar
            public async Task<string> EditarProductoAsync(int id, ProductoDTO producto)
            {
                if (!await SetAuthorizationHeader()) return "Error: No autorizado";
                var resp = await client.PutAsJsonAsync($"api/productos/{id}", producto);
                if (resp.IsSuccessStatusCode) return "✅ Producto actualizado correctamente";
                return $"❌ Error al actualizar: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync()}";
            }

            // Eliminar
            public async Task<bool> EliminarProductoAsync(int id)
            {
                if (!await SetAuthorizationHeader()) return false;
                var resp = await client.DeleteAsync($"api/productos/{id}");
                return resp.IsSuccessStatusCode;
            }

            // ---- Opcional: subir imagen (si tu API tiene endpoint de uploads)
            // Ajusta la ruta "api/uploads/productos" según tu backend
            public async Task<string?> SubirImagenAsync(Stream imageStream, string fileName, string contentType)
            {
                if (!await SetAuthorizationHeader()) return null;

                using var content = new MultipartFormDataContent();
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(imageContent, "file", fileName);

                var resp = await client.PostAsync("api/uploads/productos", content); // AJUSTAR si la ruta es otra
                if (resp.IsSuccessStatusCode)
                {
                    // Suponiendo que el backend devuelve la URL como texto o JSON. Ajusta según tu API.
                    var text = await resp.Content.ReadAsStringAsync();
                    return text;
                }
                return null;
            }
        }

}
