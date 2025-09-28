namespace FrontEndBodega.DTO
{
    
        public class ProveedoresResponse
        {
            public ProveedoresPage data { get; set; } = new();
            public bool success { get; set; }
            public string mensaje { get; set; } = "";
        }

        public class ProveedoresPage
        {
            public List<ProveedorDTO> content { get; set; } = new();
            public int totalPages { get; set; }
            public int totalElements { get; set; }
            public int number { get; set; }
            public int size { get; set; }
        }

}
