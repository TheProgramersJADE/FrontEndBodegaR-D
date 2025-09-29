namespace FrontEndBodega.DTO
{
    public class MovimientoDetalleDTO
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int IdTipoMovimiento { get; set; }
        public string TipoMovimientoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string FechaStr { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

}
