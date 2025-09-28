namespace FrontEndBodega.DTO
{
    
        // DTO para enviar al backend
        public class TipoMovimientoDTO
        {
            public string Nombre { get; set; } = string.Empty;
            public bool EditarCosto { get; set; }
            public int Tipo { get; set; }
        }
    }
