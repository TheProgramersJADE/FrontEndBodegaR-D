using System.Text.Json.Serialization;

namespace FrontEndBodega.DTO
{
    public class ProductoDTO
    {
            public int Id { get; set; }

            public string Nombre { get; set; } = string.Empty;

            public string Descripcion { get; set; } = string.Empty;

            public decimal PrecioCompra { get; set; }

            public decimal PrecioVenta { get; set; }

            public decimal CostoPromedio { get; set; }

            public int StockActual { get; set; }

            public int StockMinimo { get; set; }

            public string ImagenUrl { get; set; } = string.Empty;

            public string? CategoriaNombre { get; set; }

            public string? ProveedorNombre { get; set; }

            public string EstadoStock { get; set; } = string.Empty;
        }
}
