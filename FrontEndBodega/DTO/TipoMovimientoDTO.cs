namespace FrontEndBodega.DTO
{
    
        // DTO para enviar al backend
        public class TipoMovimientoDTO
        {
            public string Nombre { get; set; } = string.Empty;
            public bool EditarCosto { get; set; }
            public int Tipo { get; set; }
        }

        // DTO para recibir la respuesta
        public class TipoMovimientoResponse
        {
            public TipoMovimientoData? Data { get; set; }
            public bool Success { get; set; }
            public string? Mensaje { get; set; }
        }

        public class TipoMovimientoData
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public bool EditarCosto { get; set; }
            public int Tipo { get; set; }
        }
    }
