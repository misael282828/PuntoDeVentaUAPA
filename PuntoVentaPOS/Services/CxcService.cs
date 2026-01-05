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
