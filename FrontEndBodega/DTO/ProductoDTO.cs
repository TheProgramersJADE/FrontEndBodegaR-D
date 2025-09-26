using System.Text.Json.Serialization;

namespace FrontEndBodega.DTO
{
    public class ProductoDTO
    {

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("nombre")]
            public string Nombre { get; set; } = string.Empty;

            [JsonPropertyName("descripcion")]
            public string Descripcion { get; set; } = string.Empty;

            [JsonPropertyName("precio_compra")]
            public decimal PrecioCompra { get; set; }

            [JsonPropertyName("precio_venta")]
            public decimal PrecioVenta { get; set; }

            [JsonPropertyName("costo_promedio")]
            public decimal CostoPromedio { get; set; }

            [JsonPropertyName("stock_actual")]
            public int StockActual { get; set; }

            [JsonPropertyName("stock_minimo")]
            public int StockMinimo { get; set; }

            [JsonPropertyName("imagen_url")]
            public string ImagenUrl { get; set; } = string.Empty;

            [JsonPropertyName("categoria_nombre")]
            public string? CategoriaNombre { get; set; }

            [JsonPropertyName("proveedor_nombre")]
            public string? ProveedorNombre { get; set; }

            [JsonPropertyName("estado_stock")]
            public string EstadoStock { get; set; } = string.Empty;
        }
}
