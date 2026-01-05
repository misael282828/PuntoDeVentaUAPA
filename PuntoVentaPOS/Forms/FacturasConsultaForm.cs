using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class FacturasConsultaForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CDanger = ColorTranslator.FromHtml("#E74C3C");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly FacturasService _service = new();
    private readonly BindingSource _binding = new();
    private readonly bool _permitirAnular;
    private DateTimePicker _dtDesde = null!;
    private DateTimePicker _dtHasta = null!;
    private DataGridView _grid = null!;

    public FacturasConsultaForm(bool permitirAnular = false)
    {
        _permitirAnular = permitirAnular;
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(12);
        Size = new Size(900, 520);

        Text = _permitirAnular ? "Facturas (Admin)" : "Facturas";

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
            Text = Text,
            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = CText,
            AutoSize = true,
            Location = new Point(2, 2)
        };
        titleRow.Controls.Add(lblTitle);

        // Toolbar with date filters
        var toolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = CBg };
        var lblDesde = new Label { Text = "Desde:", AutoSize = true, ForeColor = CText, Location = new Point(2, 18) };
        _dtDesde = new DateTimePicker { Location = new Point(62, 14), Width = 140 };

        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, ForeColor = CText, Location = new Point(220, 18) };
        _dtHasta = new DateTimePicker { Location = new Point(270, 14), Width = 140 };

        var btnBuscar = MakeButton("Buscar", CGrayBtn, 100);
        btnBuscar.Location = new Point(430, 12);
        btnBuscar.Click += (_, _) => CargarDatos();

        toolbar.Controls.AddRange(new Control[] { lblDesde, _dtDesde, lblHasta, _dtHasta, btnBuscar });

        if (_permitirAnular)
        {
            var btnAnular = MakeButton("Anular", CDanger, 100);
            btnAnular.Location = new Point(540, 12);
            btnAnular.Click += (_, _) => Anular();
            toolbar.Controls.Add(btnAnular);
        }

        // Card with grid
        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        _grid = BuildGrid();
        _grid.DataSource = _binding;
        card.Controls.Add(_grid);

        // Footer with count
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = CBg };
        var lblCount = new Label { Text = "0 registros", ForeColor = Color.FromArgb(90, 90, 90), AutoSize = true, Location = new Point(2, 6) };
        footer.Controls.Add(lblCount);

        root.Controls.Add(titleRow, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(card, 0, 2);
        root.Controls.Add(footer, 0, 3);

        Controls.Add(root);

        void UpdateCount() { if (_binding.DataSource is BindingList<Factura> list) lblCount.Text = $"{list.Count} registros"; }
        _binding.ListChanged += (_, _) => UpdateCount();
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

        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Factura.IdFactura), HeaderText = "Id", Width = 70 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Factura.Cliente), HeaderText = "Cliente", Width = 260 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Factura.Fecha), HeaderText = "Fecha", Width = 160 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Factura.Total), HeaderText = "Total", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Factura.EsCredito), HeaderText = "Crédito", Width = 90, Name = "Credito" });

        g.CellFormatting += (_, e) =>
        {
            if (g.Columns[e.ColumnIndex].DataPropertyName == nameof(Factura.Total) && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var val))
                {
                    e.Value = val.ToString("C2");
                    e.FormattingApplied = true;
                }
            }

            if (g.Columns[e.ColumnIndex].Name == "Credito" && e.Value != null)
            {
                bool es = false; try { es = Convert.ToBoolean(e.Value); } catch { }
                e.Value = es ? "Sí" : "No";
                e.FormattingApplied = true;
            }
        };

        return g;
    }

    private void CargarDatos()
    {
        try
        {
            var lista = _service.Listar(_dtDesde.Value.Date, _dtHasta.Value.Date.AddDays(1).AddTicks(-1));
            _binding.DataSource = new BindingList<Factura>(lista);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Anular()
    {
        if (_binding.Current is not Factura factura)
        {
            MessageBox.Show("Seleccione una factura.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("Confirmar anulacion de la factura seleccionada?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.AnularFactura(factura.IdFactura);
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
