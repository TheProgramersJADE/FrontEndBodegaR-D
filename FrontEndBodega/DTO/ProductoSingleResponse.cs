using System.Text.Json.Serialization;

namespace FrontEndBodega.DTO
{
    public class ProductoSingleResponse
    {
        [JsonPropertyName("data")]
        public ProductoDTO? Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
