namespace PuntoVentaPOS.Models;

public sealed class CuentaPorCobrar
{
    public int IdFactura { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Activa { get; set; } = true;
}
