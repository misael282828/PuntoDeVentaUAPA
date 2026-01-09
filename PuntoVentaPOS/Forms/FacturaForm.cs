using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class FacturaForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CDanger = ColorTranslator.FromHtml("#E74C3C");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly ClientesService _clientesService = new();
    private readonly ProductosService _productosService = new();
    private readonly FacturasService _facturasService = new();

    private ComboBox _cmbClientes = null!;
    private ComboBox _cmbProductos = null!;
    private NumericUpDown _numCantidad = null!;
    private CheckBox _chkCredito = null!;
    private Label _lblTotal = null!;
    private DataGridView _grid = null!;
    private BindingList<FacturaDetalle> _detalles = new();
    private List<Producto> _productos = new();

    public FacturaForm()
    {
        Text = "Nueva Factura";
        InitializeComponent();
        Load += (_, _) => CargarCatalogos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(12);
        Size = new Size(1000, 600);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = CBg,
            ColumnCount = 1,
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Title
        var titleRow = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CBg };
        var lblTitle = new Label
        {
            Text = "Nueva Factura",
            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = CText,
            AutoSize = true,
            Location = new Point(2, 2)
        };
        titleRow.Controls.Add(lblTitle);

        // Toolbar (cliente, producto, cantidad, acciones) using TableLayoutPanel to avoid overlaps
        var toolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = CBg,
            ColumnCount = 6,
            RowCount = 2,
            AutoSize = false
        };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // lblCliente
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f)); // cmbClientes
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // lblProducto
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f)); // cmbProductos
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // spacer/buttons
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // guardar

        // Row styles
        toolbar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        toolbar.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var lblCliente = new Label { Text = "Cliente:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left | AnchorStyles.Top };
        _cmbClientes = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right }; 

        var lblProducto = new Label { Text = "Producto:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left | AnchorStyles.Top };
        _cmbProductos = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Left | AnchorStyles.Right };

        var lblCantidad = new Label { Text = "Cantidad:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left };
        _numCantidad = new NumericUpDown { Minimum = 1, Maximum = 10000, Value = 1, Anchor = AnchorStyles.Left };

        var btnAgregar = MakeButton("Agregar", CPrimary, 100);
        btnAgregar.Click += (_, _) => AgregarDetalle();

        _chkCredito = new CheckBox { Text = "Venta a credito", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left };

        var btnQuitar = MakeButton("Quitar Linea", CGrayBtn, 120);
        btnQuitar.Click += (_, _) => QuitarDetalle();

        var btnGuardar = MakeButton("Guardar Factura", CPrimary, 140);
        btnGuardar.Click += (_, _) => Guardar();

        // First row: Cliente label, cliente combo, Producto label, producto combo, spacer, guardar
        toolbar.Controls.Add(lblCliente, 0, 0);
        toolbar.Controls.Add(_cmbClientes, 1, 0);
        toolbar.Controls.Add(lblProducto, 2, 0);
        toolbar.Controls.Add(_cmbProductos, 3, 0);
        toolbar.Controls.Add(new Panel { Width = 8, Height = 1, BackColor = Color.Transparent }, 4, 0);
        toolbar.Controls.Add(btnGuardar, 5, 0);

        // Second row: Cantidad, numeric, agregar, credito, quitar
        toolbar.Controls.Add(lblCantidad, 0, 1);
        toolbar.Controls.Add(_numCantidad, 1, 1);
        toolbar.Controls.Add(btnAgregar, 2, 1);
        toolbar.Controls.Add(_chkCredito, 3, 1);
        toolbar.Controls.Add(btnQuitar, 5, 1);

        // Make combos expand
        _cmbClientes.Width = 320;
        _cmbProductos.Width = 280;

        // Card with grid
        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        _grid = BuildGrid();
        _grid.DataSource = _detalles;
        card.Controls.Add(_grid);

        // Footer with total
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = CBg };
        _lblTotal = new Label { Text = "Total: 0.00", AutoSize = true, Location = new Point(2, 10), Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = CText };
        footer.Controls.Add(_lblTotal);

        root.Controls.Add(titleRow, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(card, 0, 2);
        root.Controls.Add(footer, 0, 3);

        Controls.Add(root);
    }

    private static Button MakeButton(string text, Color bg, int width)
    {
        var b = new Button
        {
            Text = text,
            Width = width,
            Height = 34,
            BackColor = bg,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    private void CargarCatalogos()
    {
        try
        {
            var clientes = _clientesService.Listar(null);
            _cmbClientes.DataSource = clientes;
            _cmbClientes.DisplayMember = "Nombre";
            _cmbClientes.ValueMember = "IdCliente";
            // Ensure DB has a reasonable number of sample products (will insert if missing)
            _productosService.SeedSampleProducts(10);
            _productos = _productosService.Listar(null);
            _cmbProductos.DataSource = _productos;
            _cmbProductos.DisplayMember = "Nombre";
            _cmbProductos.ValueMember = "IdProducto";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AgregarDetalle()
    {
        if (_cmbProductos.SelectedItem is not Producto producto)
        {
            return;
        }

        var cantidad = (int)_numCantidad.Value;
        // Si ya existe la línea para el mismo producto, sumar cantidades en lugar de duplicar
        var existente = _detalles.FirstOrDefault(d => d.IdProducto == producto.IdProducto);
        if (existente != null)
        {
            existente.Cantidad += cantidad;
            // Notificar cambio de item para que la grid refresque
            var idx = _detalles.IndexOf(existente);
            if (idx >= 0) _detalles.ResetItem(idx);
        }
        else
        {
            var detalle = new FacturaDetalle
            {
                IdProducto = producto.IdProducto,
                Producto = producto.Nombre,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };

            _detalles.Add(detalle);
        }

        CalcularTotal();
    }

    private void QuitarDetalle()
    {
        if (_grid.CurrentRow?.DataBoundItem is FacturaDetalle detalle)
        {
            _detalles.Remove(detalle);
            CalcularTotal();
        }
    }

    private void CalcularTotal()
    {
        var total = _detalles.Sum(d => d.Total);
        _lblTotal.Text = $"Total: {total:N2}";
    }

    private DataGridView BuildGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AutoGenerateColumns = false
        };

        g.EnableHeadersVisualStyles = false;
        g.ColumnHeadersDefaultCellStyle.BackColor = CTop;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        g.ColumnHeadersHeight = 38;

        g.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
        g.DefaultCellStyle.ForeColor = CText;
        g.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#D6E4FF");
        g.DefaultCellStyle.SelectionForeColor = Color.Black;
        g.AlternatingRowsDefaultCellStyle.BackColor = CBg;

        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(FacturaDetalle.IdProducto), HeaderText = "Id", Width = 60 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(FacturaDetalle.Producto), HeaderText = "Producto", Width = 360 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(FacturaDetalle.Cantidad), HeaderText = "Cantidad", Width = 120 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(FacturaDetalle.PrecioUnitario), HeaderText = "Precio", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });

        g.CellFormatting += (_, e) =>
        {
            if (g.Columns[e.ColumnIndex].DataPropertyName == nameof(FacturaDetalle.PrecioUnitario) && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var val))
                {
                    e.Value = val.ToString("C2", CultureInfo.CurrentCulture);
                    e.FormattingApplied = true;
                }
            }

            if (g.Columns[e.ColumnIndex].HeaderText == "Total" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var val))
                {
                    e.Value = val.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        };

        return g;
    }

    private void Guardar()
    {
        if (_cmbClientes.SelectedItem is not Cliente cliente)
        {
            MessageBox.Show("Seleccione un cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_detalles.Count == 0)
        {
            MessageBox.Show("Agregue productos a la factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var factura = new Factura
        {
            IdCliente = cliente.IdCliente,
            Cliente = cliente.Nombre,
            Fecha = DateTime.Now,
            EsCredito = _chkCredito.Checked,
            Total = _detalles.Sum(d => d.Total),
            Detalles = _detalles.ToList()
        };

        try
        {
            var id = _facturasService.RegistrarFactura(factura, out var mensaje);
            MessageBox.Show($"Factura registrada: {id}. {mensaje}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
