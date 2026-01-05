namespace PuntoVentaPOS.Models;

public sealed class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public UserRole Rol { get; set; } = UserRole.Cajero;
}
