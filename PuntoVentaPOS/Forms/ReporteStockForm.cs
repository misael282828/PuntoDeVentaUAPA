using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class ReporteStockForm : Form
{
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly ReportesService _service = new();
    private readonly BindingSource _binding = new();
    private NumericUpDown _numUmbral = null!;
    private DataGridView _grid = null!;

    public ReporteStockForm()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(12);
        Text = "Reporte de Stock";
        Size = new Size(800, 480);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = CBg, ColumnCount = 1, RowCount = 3 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleRow = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CBg };
        var lblTitle = new Label { Text = "Reporte de Stock", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = CText, AutoSize = true, Location = new Point(2, 2) };
        titleRow.Controls.Add(lblTitle);

        var toolbar = new TableLayoutPanel { Dock = DockStyle.Top, Height = 60, BackColor = CBg, ColumnCount = 3, RowCount = 1, AutoSize = true };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var lblUmbral = new Label { Text = "Umbral:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left };
        _numUmbral = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 10, Anchor = AnchorStyles.Left };
        var btnBuscar = MakeButton("Consultar", CPrimary, 100); btnBuscar.Click += (_, _) => CargarDatos();

        toolbar.Controls.Add(lblUmbral, 0, 0);
        toolbar.Controls.Add(_numUmbral, 1, 0);
        toolbar.Controls.Add(btnBuscar, 2, 0);

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
            AutoGenerateColumns = true
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

        return g;
    }

    private void CargarDatos()
    {
        try
        {
            var lista = _service.StockBajo((int)_numUmbral.Value);
            _binding.DataSource = new BindingList<Producto>(lista);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
