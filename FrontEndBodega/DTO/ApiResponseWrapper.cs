namespace FrontEndBodega.DTO
{
    public class ApiResponseWrapper<T>
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<T> Data { get; set; } = new List<T>();
    }
}
