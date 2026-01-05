using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class UsuariosService
{
    public List<Usuario> Listar()
    {
        var usuarios = new List<Usuario>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("SELECT IdUsuario, NombreUsuario, Rol FROM Usuarios", connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            usuarios.Add(new Usuario
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                NombreUsuario = reader["NombreUsuario"]?.ToString() ?? string.Empty,
                Rol = string.Equals(reader["Rol"]?.ToString(), "Admin", StringComparison.OrdinalIgnoreCase)
                    ? UserRole.Admin
                    : UserRole.Cajero
            });
        }

        return usuarios;
    }

    public void Crear(Usuario usuario, string contrasena)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("INSERT INTO Usuarios (NombreUsuario, Contrasena, Rol) VALUES (@NombreUsuario, @Contrasena, @Rol)", connection);
        command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
        command.Parameters.AddWithValue("@Contrasena", contrasena);
        command.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Actualizar(Usuario usuario)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("UPDATE Usuarios SET NombreUsuario=@NombreUsuario, Rol=@Rol WHERE IdUsuario=@IdUsuario", connection);
        command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
        command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
        command.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Eliminar(int idUsuario)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("DELETE FROM Usuarios WHERE IdUsuario=@IdUsuario", connection);
        command.Parameters.AddWithValue("@IdUsuario", idUsuario);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
