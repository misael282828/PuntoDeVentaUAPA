namespace PuntoVentaPOS.Models;

public sealed class FacturaDetalle
{
    public int IdProducto { get; set; }
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total => Cantidad * PrecioUnitario;
}
