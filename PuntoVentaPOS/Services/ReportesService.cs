using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class ReportesService
{
    public List<Producto> StockBajo(int umbral)
    {
        var productos = new List<Producto>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("ListarProductosBajoStock", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("pUmbral", umbral);
        command.Parameters.Add(new MySqlParameter("pMensaje", MySqlDbType.VarChar, 200)
        {
            Direction = ParameterDirection.Output
        });

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

    public DataTable VentasDiarias(DateTime desde, DateTime hasta)
    {
        var table = new DataTable();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("SELECT CAST(Fecha AS DATE) AS Dia, SUM(Total) AS Total FROM Facturas WHERE Fecha BETWEEN @Desde AND @Hasta GROUP BY CAST(Fecha AS DATE)", connection);
        command.Parameters.AddWithValue("@Desde", desde);
        command.Parameters.AddWithValue("@Hasta", hasta);

        connection.Open();
        using var adapter = new MySqlDataAdapter(command);
        adapter.Fill(table);

        return table;
    }
}
