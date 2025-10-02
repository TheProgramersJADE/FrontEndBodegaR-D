namespace FrontEndBodega.DTO
{
    public class KardexDTO
    {
        public int IdMovimiento { get; set; }
        public string NombreProducto { get; set; }
        public string Categoria { get; set; }
        public string Proveedor { get; set; }
        public string TipoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public decimal CantidadEntrada { get; set; }
        public decimal CantidadSalida { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal CostoPromedio { get; set; }
        public decimal StockActual { get; set; }
        public string Observaciones { get; set; }
    }
}
