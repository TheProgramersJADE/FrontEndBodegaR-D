namespace FrontEndBodega.DTO
{
    public class CategoriaSingleResponse
    {
        public CategoriaDTO? data { get; set; }
        public bool success { get; set; }
        public string mensaje { get; set; } = string.Empty;
    }
}
