namespace FrontEndBodega.DTO
{
    public class TipoMovimientoData
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EditarCosto { get; set; }
        public int Tipo { get; set; }
    }
}
