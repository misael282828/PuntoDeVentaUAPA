using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Forms;

public sealed class ProductoEditorForm : Form
{
    private TextBox _txtCodigo = null!;
    private TextBox _txtNombre = null!;
    private NumericUpDown _numPrecio = null!;
    private NumericUpDown _numStock = null!;
    private CheckBox _chkActivo = null!;

    public Producto Producto { get; private set; }

    public ProductoEditorForm(Producto producto)
    {
        Producto = producto;
        InitializeComponent();
        Cargar();
    }

    private void InitializeComponent()
    {
        Text = "Producto";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(420, 360);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lblCodigo = new Label { Text = "Codigo:", Location = new Point(20, 20), AutoSize = true };
        _txtCodigo = new TextBox { Location = new Point(20, 45), Width = 360 };

        var lblNombre = new Label { Text = "Nombre:", Location = new Point(20, 80), AutoSize = true };
        _txtNombre = new TextBox { Location = new Point(20, 105), Width = 360 };

        var lblPrecio = new Label { Text = "Precio:", Location = new Point(20, 140), AutoSize = true };
        _numPrecio = new NumericUpDown { Location = new Point(20, 165), Width = 120, DecimalPlaces = 2, Maximum = 1000000 };

        var lblStock = new Label { Text = "Stock:", Location = new Point(200, 140), AutoSize = true };
        _numStock = new NumericUpDown { Location = new Point(200, 165), Width = 120, Maximum = 100000 };

        _chkActivo = new CheckBox { Text = "Activo", Location = new Point(20, 210), AutoSize = true };

        var btnGuardar = new Button { Text = "Guardar", Location = new Point(20, 250), Width = 120 };
        btnGuardar.Click += (_, _) => Guardar();

        var btnCancelar = new Button { Text = "Cancelar", Location = new Point(160, 250), Width = 120 };
        btnCancelar.Click += (_, _) => Close();

        Controls.AddRange([
            lblCodigo, _txtCodigo,
            lblNombre, _txtNombre,
            lblPrecio, _numPrecio,
            lblStock, _numStock,
            _chkActivo,
            btnGuardar, btnCancelar
        ]);
    }

    private void Cargar()
    {
        _txtCodigo.Text = Producto.Codigo;
        _txtNombre.Text = Producto.Nombre;
        _numPrecio.Value = Producto.Precio;
        _numStock.Value = Producto.Stock;
        _chkActivo.Checked = Producto.Activo;
    }

    private void Guardar()
    {
        Producto.Codigo = _txtCodigo.Text.Trim();
        Producto.Nombre = _txtNombre.Text.Trim();
        Producto.Precio = _numPrecio.Value;
        Producto.Stock = (int)_numStock.Value;
        Producto.Activo = _chkActivo.Checked;

        DialogResult = DialogResult.OK;
        Close();
    }
}
