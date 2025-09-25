namespace FrontEndBodega.DTO
{   public class CategoriaListResponse
    {
        public List<CategoriaDTO> data { get; set; } = new();
        public bool success { get; set; }
        public string mensaje { get; set; } = string.Empty;
    }
}
