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

    // Inserta productos de ejemplo si la tabla tiene menos de `minCount` registros.
    public void SeedSampleProducts(int minCount = 10)
    {
        var existentes = Listar(null);
        if (existentes.Count >= minCount) return;

        var ejemplos = new List<Producto>
        {
            new Producto { Codigo = "P200", Nombre = "Leche 1L", Precio = 35m, Stock = 100, Activo = true },
            new Producto { Codigo = "P201", Nombre = "Pan Integral 500g", Precio = 20m, Stock = 200, Activo = true },
            new Producto { Codigo = "P202", Nombre = "Huevos 12u", Precio = 80m, Stock = 120, Activo = true },
            new Producto { Codigo = "P203", Nombre = "Aceite 1L", Precio = 150m, Stock = 60, Activo = true },
            new Producto { Codigo = "P204", Nombre = "Arroz 1kg", Precio = 50m, Stock = 180, Activo = true },
            new Producto { Codigo = "P205", Nombre = "Frijoles 1kg", Precio = 62m, Stock = 140, Activo = true },
            new Producto { Codigo = "P206", Nombre = "Azucar 1kg", Precio = 45m, Stock = 120, Activo = true },
            new Producto { Codigo = "P207", Nombre = "Harina 1kg", Precio = 40m, Stock = 90, Activo = true },
            new Producto { Codigo = "P208", Nombre = "Cafe 250g", Precio = 120m, Stock = 70, Activo = true },
            new Producto { Codigo = "P209", Nombre = "Lechuga Unit", Precio = 15m, Stock = 60, Activo = true }
        };

        foreach (var p in ejemplos)
        {
            // evitar duplicados por nombre
            if (!existentes.Any(x => string.Equals(x.Nombre, p.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    Crear(p);
                }
                catch
                {
                    // ignore insert errors for seed
                }
            }
        }
    }
}
