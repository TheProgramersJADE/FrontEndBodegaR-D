using System.Text.Json.Serialization;

namespace FrontEndBodega.DTO
{
    public class ProductoListResponse
    {
        [JsonPropertyName("data")]
        public List<ProductoDTO> Data { get; set; } = new();

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
