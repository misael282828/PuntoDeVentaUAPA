using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class ClientesService
{
    public List<Cliente> Listar(string? filtro)
    {
        var clientes = new List<Cliente>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text
        };

        if (int.TryParse(filtro, out var id))
        {
            command.CommandText = "SELECT * FROM Clientes WHERE IdCliente = @IdCliente";
            command.Parameters.AddWithValue("@IdCliente", id);
        }
        else if (!string.IsNullOrWhiteSpace(filtro))
        {
            command.CommandText = "SELECT * FROM Clientes WHERE Nombre LIKE @Nombre";
            command.Parameters.AddWithValue("@Nombre", $"%{filtro}%");
        }
        else
        {
            command.CommandText = "SELECT * FROM Clientes";
        }

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            clientes.Add(new Cliente
            {
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                Documento = reader["Documento"]?.ToString() ?? string.Empty,
                Telefono = reader["Telefono"]?.ToString() ?? string.Empty,
                Direccion = reader["Direccion"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                Activo = Convert.ToBoolean(reader["Activo"])
            });
        }

        return clientes;
    }

    public void Crear(Cliente cliente)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("INSERT INTO Clientes (Nombre, Documento, Telefono, Direccion, Email, Activo) VALUES (@Nombre, @Documento, @Telefono, @Direccion, @Email, @Activo)", connection);
        command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        command.Parameters.AddWithValue("@Documento", cliente.Documento);
        command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
        command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
        command.Parameters.AddWithValue("@Email", cliente.Email);
        command.Parameters.AddWithValue("@Activo", cliente.Activo);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Actualizar(Cliente cliente)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("UPDATE Clientes SET Nombre=@Nombre, Documento=@Documento, Telefono=@Telefono, Direccion=@Direccion, Email=@Email, Activo=@Activo WHERE IdCliente=@IdCliente", connection);
        command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
        command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        command.Parameters.AddWithValue("@Documento", cliente.Documento);
        command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
        command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
        command.Parameters.AddWithValue("@Email", cliente.Email);
        command.Parameters.AddWithValue("@Activo", cliente.Activo);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Eliminar(int idCliente)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("DELETE FROM Clientes WHERE IdCliente=@IdCliente", connection);
        command.Parameters.AddWithValue("@IdCliente", idCliente);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
