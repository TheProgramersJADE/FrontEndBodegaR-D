namespace FrontEndBodega.DTO
{
    public class TipoMovimientoListResponse
    {
        public List<TipoMovimientoData> Data { get; set; } = new List<TipoMovimientoData>();
        public bool Success { get; set; }
        public string? Mensaje { get; set; }
    }
}
