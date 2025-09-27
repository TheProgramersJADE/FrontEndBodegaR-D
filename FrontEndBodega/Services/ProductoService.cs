using FrontEndBodega.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace FrontEndBodega.Services
{
    public class ProductoService
    {
        private readonly HttpClient client;
        private readonly AutheService _authService;

        public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
        {
            var token = await _authService.GetToken(); // Método que devuelve el token
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<List<CategoriaDTO>>("api/categorias");
        }

        public async Task<List<ProveedorDTO>> ObtenerProveedoresAsync()
        {
            var token = await _authService.GetToken();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<List<ProveedorDTO>>("api/proveedores");
        }


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


        public async Task<string> CrearProductoAsync(ProductoDTO producto, Stream? fileStream = null, string? fileName = null, string? contentType = null)
        {
            if (!await SetAuthorizationHeader()) return "❌ Token inválido";

            using var form = new MultipartFormDataContent();

            // ✅ Campos normales
            form.Add(new StringContent(producto.Nombre), "nombre");
            form.Add(new StringContent(producto.Descripcion), "descripcion");
            form.Add(new StringContent(producto.PrecioCompra.ToString()), "precio_compra");
            form.Add(new StringContent(producto.PrecioVenta.ToString()), "precio_venta");
            form.Add(new StringContent(producto.CostoPromedio.ToString()), "costo_promedio");
            form.Add(new StringContent(producto.StockActual.ToString()), "stock_actual");
            form.Add(new StringContent(producto.StockMinimo.ToString()), "stock_minimo");

            if (!string.IsNullOrEmpty(producto.CategoriaNombre))
                form.Add(new StringContent(producto.CategoriaNombre), "categoria_nombre");

            if (!string.IsNullOrEmpty(producto.ProveedorNombre))
                form.Add(new StringContent(producto.ProveedorNombre), "proveedor_nombre");

            // ✅ Imagen (si se sube)
            if (fileStream != null && fileName != null)
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "image/jpeg");
                form.Add(fileContent, "imagen", fileName);
            }

            var response = await client.PostAsync("api/productos", form);

            if (response.IsSuccessStatusCode)
            {
                return "✅ Producto creado con éxito";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"❌ Error al crear producto. Código: {response.StatusCode}. Detalle: {error}";
            }
        }


        // 📌 Editar producto
        public async Task<string> EditarProductoAsync(int id, ProductoDTO producto, Stream? fileStream = null, string? fileName = null, string? contentType = null)
        {
            if (!await SetAuthorizationHeader()) return "❌ Token inválido";

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(producto.Nombre ?? ""), "nombre");
            form.Add(new StringContent(producto.Descripcion ?? ""), "descripcion");
            form.Add(new StringContent(producto.PrecioCompra.ToString(System.Globalization.CultureInfo.InvariantCulture)), "precio_compra");
            form.Add(new StringContent(producto.PrecioVenta.ToString(System.Globalization.CultureInfo.InvariantCulture)), "precio_venta");
            form.Add(new StringContent(producto.CostoPromedio.ToString(System.Globalization.CultureInfo.InvariantCulture)), "costo_promedio");
            form.Add(new StringContent(producto.StockActual.ToString()), "stock_actual");
            form.Add(new StringContent(producto.StockMinimo.ToString()), "stock_minimo");

            if (!string.IsNullOrEmpty(producto.CategoriaNombre))
                form.Add(new StringContent(producto.CategoriaNombre), "categoria_nombre");

            if (!string.IsNullOrEmpty(producto.ProveedorNombre))
                form.Add(new StringContent(producto.ProveedorNombre), "proveedor_nombre");

            if (fileStream != null && fileName != null)
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "image/jpeg");
                form.Add(fileContent, "imagen", fileName);
            }

            var response = await client.PutAsync($"api/productos/{id}", form);

            if (response.IsSuccessStatusCode)
            {
                return "✅ Producto actualizado con éxito";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"❌ Error al editar producto. Código: {response.StatusCode}. Detalle: {error}";
            }

            //CODIGO ANTIGUO - NO FUNCIONA PARA IMAGENES    
            //try
            //{
            //    var response = await client.PutAsJsonAsync($"api/productos/{id}", producto);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        return "✅ Producto actualizado con éxito";
            //    }
            //    else
            //    {
            //        var error = await response.Content.ReadAsStringAsync();
            //        return $"❌ Error al editar producto: {error}";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return $"❌ Excepción al editar producto: {ex.Message}";
            //}
        }

        // Ejemplo de ProductoService
        public async Task<bool> EliminarProductoAsync(int id)
        {
            // Asegúrate de que la URL sea la correcta para el endpoint de eliminación
            var response = await client.DeleteAsync($"api/productos/{id}");

            return response.IsSuccessStatusCode;
        }
        // Dentro de ProductoService.cs
        //public async Task<string> SubirImagenAsync(Stream fileStream, string fileName, string contentType)
        //{
        //    // 1. Usar MultipartFormDataContent para simular el envío del formulario de Hoppscotch
        //    using var content = new MultipartFormDataContent();
        //    using var streamContent = new StreamContent(fileStream);

        //    // 2. Definir el tipo de contenido y nombre del archivo que la API espera
        //    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        //    // 'file' es típicamente el nombre del campo que la API espera para el archivo
        //    content.Add(streamContent, "file", fileName);

        //    // 3. Reemplaza 'api/uploads' con el endpoint de tu API para subir archivos
        //    var response = await client.PostAsync("api/tu-endpoint-de-subida-de-archivos", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // 4. Leer la respuesta. Tu API debe devolver la URL de la imagen
        //        var result = await response.Content.ReadAsStringAsync();
        //        // **IMPORTANTE**: Ajusta cómo extraes la URL según el formato de la respuesta de tu API.
        //        // Si tu API devuelve un JSON como: { "url": "http://..." }, necesitas deserializarlo.

        //        // Ejemplo simple (si devuelve solo la URL como texto plano):
        //        return result.Trim('"');

        //        // Ejemplo (si devuelve un JSON):
        //        // var uploadResult = JsonSerializer.Deserialize<UploadResponse>(result);
        //        // return uploadResult.Url;
        //    }
        //    else
        //    {
        //        // **DEPURACIÓN CLAVE**: Imprime o loguea el error de la API
        //        var error = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Error al subir imagen: {response.StatusCode} - {error}");
        //        return string.Empty; // Devuelve vacío para que falle la validación en Blazor
        //    }
        //}
    }
}
