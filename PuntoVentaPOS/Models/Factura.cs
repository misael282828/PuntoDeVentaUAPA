namespace PuntoVentaPOS.Models;

public sealed class Factura
{
    public int IdFactura { get; set; }
    public int IdCliente { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool EsCredito { get; set; }
    public decimal Total { get; set; }
    public List<FacturaDetalle> Detalles { get; set; } = new();
}
