using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class AuthService
{
    public Usuario? ValidarUsuario(string nombreUsuario, string contrasena, out string mensaje)
    {
        mensaje = string.Empty;

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("ValidarUsuario", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("pNombreUsuario", nombreUsuario);
        command.Parameters.AddWithValue("pContrasena", contrasena);

        var output = new MySqlParameter("pMensaje", MySqlDbType.VarChar, 200)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(output);

        connection.Open();
        Usuario? usuario = null;
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                    NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                    Rol = ParseRol(reader["Rol"]?.ToString())
                };
            }
        }

        mensaje = output.Value?.ToString() ?? (usuario == null ? "Credenciales invalidas." : "OK");
        return usuario;
    }

    private static UserRole ParseRol(string? rol)
    {
        if (string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return UserRole.Admin;
        }

        return UserRole.Cajero;
    }
}
