namespace FrontEndBodega.DTO
{
    public class MovimientoListResponse
    {
        public List<MovimientoDetalleDTO> Data { get; set; } = new();
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

}
