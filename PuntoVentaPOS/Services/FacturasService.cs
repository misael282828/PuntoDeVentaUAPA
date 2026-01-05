using System.Data;
using System.Text.Json;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class FacturasService
{
    public int RegistrarFactura(Factura factura, out string mensaje)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("RegistrarFactura", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("pIdCliente", factura.IdCliente);
        command.Parameters.AddWithValue("pFecha", factura.Fecha);
        command.Parameters.AddWithValue("pEsCredito", factura.EsCredito);
        command.Parameters.AddWithValue("pTotal", factura.Total);

        var detalleJson = JsonSerializer.Serialize(
            factura.Detalles.Select(d => new
            {
                d.IdProducto,
                d.Cantidad,
                d.PrecioUnitario
            }));

        command.Parameters.Add(new MySqlParameter("pDetalle", MySqlDbType.JSON)
        {
            Value = detalleJson
        });

        var outputId = new MySqlParameter("pIdFactura", MySqlDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputId);

        var outputMsg = new MySqlParameter("pMensaje", MySqlDbType.VarChar, 200)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputMsg);

        connection.Open();
        command.ExecuteNonQuery();

        mensaje = outputMsg.Value?.ToString() ?? "OK";
        return outputId.Value is int id ? id : Convert.ToInt32(outputId.Value);
    }

    public List<Factura> Listar(DateTime? desde, DateTime? hasta)
    {
        var facturas = new List<Factura>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand(
            "SELECT f.IdFactura, f.IdCliente, c.Nombre AS Cliente, f.Fecha, f.EsCredito, f.Total " +
            "FROM Facturas f INNER JOIN Clientes c ON f.IdCliente = c.IdCliente " +
            "WHERE (@Desde IS NULL OR f.Fecha >= @Desde) AND (@Hasta IS NULL OR f.Fecha <= @Hasta)",
            connection);
        command.Parameters.AddWithValue("@Desde", (object?)desde ?? DBNull.Value);
        command.Parameters.AddWithValue("@Hasta", (object?)hasta ?? DBNull.Value);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            facturas.Add(new Factura
            {
                IdFactura = reader.GetInt32(reader.GetOrdinal("IdFactura")),
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                Cliente = reader["Cliente"]?.ToString() ?? string.Empty,
                Fecha = reader["Fecha"] is DateTime fecha ? fecha : DateTime.Now,
                EsCredito = Convert.ToBoolean(reader["EsCredito"]),
                Total = reader["Total"] is decimal total ? total : 0m
            });
        }

        return facturas;
    }

    public void AnularFactura(int idFactura)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("UPDATE Facturas SET Estado = 'Anulada' WHERE IdFactura=@IdFactura", connection);
        command.Parameters.AddWithValue("@IdFactura", idFactura);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
