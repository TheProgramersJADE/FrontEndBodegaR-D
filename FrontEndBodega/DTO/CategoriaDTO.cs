namespace FrontEndBodega.DTO
{
    public class CategoriaDTO
    {
        public int Id { get; set; } // Para listar/editar/eliminar
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
