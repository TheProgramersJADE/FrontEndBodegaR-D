using System.ComponentModel.DataAnnotations;
namespace FrontEndBodega.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }  // Necesario para editar y eliminar
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }
        public string NombreCompleto { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Username { get; set; }
        public int IdRol { get; set; }
        public int Status { get; set; }
    }
}