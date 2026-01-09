using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class CxcForm : Form
{
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly CxcService _service = new();
    private readonly BindingSource _binding = new();
    private DataGridView _grid = null!;
    private NumericUpDown _numMonto = null!;

    public CxcForm()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(12);
        Text = "Cuentas por Cobrar";
        Size = new Size(900, 520);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = CBg, ColumnCount = 1, RowCount = 3 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleRow = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CBg };
        var lblTitle = new Label { Text = "Gestión de CxC", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = CText, AutoSize = true, Location = new Point(2, 2) };
        titleRow.Controls.Add(lblTitle);

        var toolbar = new TableLayoutPanel { Dock = DockStyle.Top, Height = 60, BackColor = CBg, ColumnCount = 4, RowCount = 1, AutoSize = true };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lblMonto = new Label { Text = "Monto a pagar:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left };
        _numMonto = new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000, Anchor = AnchorStyles.Left };

        var btnPagar = MakeButton("Pagar", CPrimary, 100); btnPagar.Click += (_, _) => Pagar();
        var btnRefrescar = MakeButton("Refrescar", CGrayBtn, 100); btnRefrescar.Click += (_, _) => CargarDatos();

        toolbar.Controls.Add(lblMonto, 0, 0);
        toolbar.Controls.Add(_numMonto, 1, 0);
        toolbar.Controls.Add(btnPagar, 2, 0);
        toolbar.Controls.Add(btnRefrescar, 3, 0);

        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        _grid = BuildGrid();
        _grid.DataSource = _binding;
        card.Controls.Add(_grid);

        root.Controls.Add(titleRow, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(card, 0, 2);

        Controls.Add(root);
    }

    private static Button MakeButton(string text, Color bg, int width)
    {
        var b = new Button { Text = text, Width = width, Height = 34, BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        b.FlatAppearance.BorderSize = 0; return b;
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

        // Columns: IdFactura, Cliente, Balance (moneda), FechaVencimiento (fecha), Activa
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PuntoVentaPOS.Models.CuentaPorCobrar.IdFactura), HeaderText = "IdFactura", Width = 100 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PuntoVentaPOS.Models.CuentaPorCobrar.Cliente), HeaderText = "Cliente", Width = 360 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PuntoVentaPOS.Models.CuentaPorCobrar.Balance), HeaderText = "Balance", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" } });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PuntoVentaPOS.Models.CuentaPorCobrar.FechaVencimiento), HeaderText = "FechaVencimiento", Width = 160, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
        g.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(PuntoVentaPOS.Models.CuentaPorCobrar.Activa), HeaderText = "Activa", Width = 80 });

        return g;
    }

    private void CargarDatos()
    {
        try
        {
            // seed CxC from existing credit invoices if any missing
            _service.SeedFromFacturas();

            var lista = _service.ListarPendientes();
            _binding.DataSource = new BindingList<CuentaPorCobrar>(lista);
            NotificarProximas(lista);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void NotificarProximas(IEnumerable<CuentaPorCobrar> cuentas)
    {
        var proximas = cuentas
            .Where(c => c.Activa && c.FechaVencimiento.Date <= DateTime.Today.AddDays(3))
            .ToList();

        if (proximas.Count == 0)
        {
            return;
        }

        MessageBox.Show($"Hay {proximas.Count} factura(s) proximas a vencer.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Pagar()
    {
        if (_binding.Current is not CuentaPorCobrar cuenta)
        {
            MessageBox.Show("Seleccione una factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var monto = _numMonto.Value;
        if (monto <= 0)
        {
            MessageBox.Show("Ingrese un monto valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _service.Pagar(cuenta.IdFactura, monto);
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
