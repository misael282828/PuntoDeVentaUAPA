using System.Data;
using MySqlConnector;
using PuntoVentaPOS.Data;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Services;

public sealed class CxcService
{
    public List<CuentaPorCobrar> ListarPendientes()
    {
        var cuentas = new List<CuentaPorCobrar>();

        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("SELECT IdFactura, Cliente, Balance, FechaVencimiento, Activa FROM CxC WHERE Activa = 1", connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cuentas.Add(new CuentaPorCobrar
            {
                IdFactura = reader.GetInt32(reader.GetOrdinal("IdFactura")),
                Cliente = reader["Cliente"]?.ToString() ?? string.Empty,
                Balance = reader["Balance"] is decimal balance ? balance : 0m,
                FechaVencimiento = reader["FechaVencimiento"] is DateTime fecha ? fecha : DateTime.Now,
                Activa = Convert.ToBoolean(reader["Activa"])
            });
        }

        return cuentas;
    }

    // Inserta en CxC facturas a credito que no tengan registro en la tabla CxC (balance = Total, vencimiento = Fecha+30d)
    public void SeedFromFacturas()
    {
        using var connection = Db.CreateConnection();
        // Insertar facturas a credito que no existen en CxC
        var sql = @"
            INSERT INTO CxC (IdFactura, Cliente, Balance, FechaVencimiento, Activa)
            SELECT f.IdFactura, COALESCE(c.Nombre, ''), f.Total, DATE_ADD(f.Fecha, INTERVAL 30 DAY), 1
            FROM Facturas f
            LEFT JOIN CxC x ON x.IdFactura = f.IdFactura
            LEFT JOIN Clientes c ON c.IdCliente = f.IdCliente
            WHERE f.EsCredito = 1 AND x.IdFactura IS NULL";

        using var command = new MySqlCommand(sql, connection);
        connection.Open();
        try
        {
            command.ExecuteNonQuery();
        }
        catch
        {
            // ignore errors during seed
        }
    }

    public void Pagar(int idFactura, decimal monto)
    {
        using var connection = Db.CreateConnection();
        using var command = new MySqlCommand("UPDATE CxC SET Balance = Balance - @Monto, Activa = CASE WHEN Balance - @Monto <= 0 THEN 0 ELSE Activa END WHERE IdFactura=@IdFactura", connection);
        command.Parameters.AddWithValue("@IdFactura", idFactura);
        command.Parameters.AddWithValue("@Monto", monto);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
