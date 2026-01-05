using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class ProductosService
{
    public List<Producto> Listar(string? filtro)
    {
        var productos = new List<Producto>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand
        {
            Connection = connection,
            CommandType = CommandType.Text
        };

        if (int.TryParse(filtro, out var id))
        {
            command.CommandText = "SELECT * FROM Productos WHERE IdProducto = @IdProducto";
            command.Parameters.AddWithValue("@IdProducto", id);
        }
        else if (!string.IsNullOrWhiteSpace(filtro))
        {
            command.CommandText = "SELECT * FROM Productos WHERE Nombre LIKE @Nombre OR Codigo LIKE @Codigo";
            command.Parameters.AddWithValue("@Nombre", $"%{filtro}%");
            command.Parameters.AddWithValue("@Codigo", $"%{filtro}%");
        }
        else
        {
            command.CommandText = "SELECT * FROM Productos";
        }

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            productos.Add(new Producto
            {
                IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                Codigo = reader["Codigo"]?.ToString() ?? string.Empty,
                Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                Precio = reader["Precio"] is decimal precio ? precio : 0m,
                Stock = reader["Stock"] is int stock ? stock : 0,
                Activo = Convert.ToBoolean(reader["Activo"])
            });
        }

        return productos;
    }

    public void Crear(Producto producto)
    {
        if (producto.Precio <= 0)
        {
            throw new InvalidOperationException("El precio debe ser mayor a cero.");
        }

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("INSERT INTO Productos (Codigo, Nombre, Precio, Stock, Activo) VALUES (@Codigo, @Nombre, @Precio, @Stock, @Activo)", connection);
        command.Parameters.AddWithValue("@Codigo", producto.Codigo);
        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
        command.Parameters.AddWithValue("@Precio", producto.Precio);
        command.Parameters.AddWithValue("@Stock", producto.Stock);
        command.Parameters.AddWithValue("@Activo", producto.Activo);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Actualizar(Producto producto)
    {
        if (producto.Precio <= 0)
        {
            throw new InvalidOperationException("El precio debe ser mayor a cero.");
        }

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("UPDATE Productos SET Codigo=@Codigo, Nombre=@Nombre, Precio=@Precio, Stock=@Stock, Activo=@Activo WHERE IdProducto=@IdProducto", connection);
        command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
        command.Parameters.AddWithValue("@Codigo", producto.Codigo);
        command.Parameters.AddWithValue("@Nombre", producto.Nombre);
        command.Parameters.AddWithValue("@Precio", producto.Precio);
        command.Parameters.AddWithValue("@Stock", producto.Stock);
        command.Parameters.AddWithValue("@Activo", producto.Activo);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Eliminar(int idProducto)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("DELETE FROM Productos WHERE IdProducto=@IdProducto", connection);
        command.Parameters.AddWithValue("@IdProducto", idProducto);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
