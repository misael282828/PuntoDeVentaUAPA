using System.Data;
using System.Drawing;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class ReporteVentasDiariasForm : Form
{
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly ReportesService _service = new();
    private DateTimePicker _dtDesde = null!;
    private DateTimePicker _dtHasta = null!;
    private DataGridView _grid = null!;
    private DataTable _data = new();

    public ReporteVentasDiariasForm()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(12);
        Text = "Reporte de Ventas Diarias";
        Size = new Size(800, 480);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = CBg, ColumnCount = 1, RowCount = 3 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleRow = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CBg };
        var lblTitle = new Label { Text = "Reporte de Ventas Diarias", Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = CText, AutoSize = true, Location = new Point(2, 2) };
        titleRow.Controls.Add(lblTitle);

        // Toolbar placed under title (top), before the card
        // Compact toolbar: keep controls together, avoid large expanding spacer
        var toolbar = new TableLayoutPanel { Dock = DockStyle.Top, Height = 60, BackColor = CBg, ColumnCount = 6, RowCount = 1, AutoSize = true };
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // lblDesde
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // dtDesde
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // lblHasta
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // dtHasta
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // btnBuscar
        toolbar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // btnExportar

        var lblDesde = new Label { Text = "Desde:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left }; 
        _dtDesde = new DateTimePicker { Anchor = AnchorStyles.Left };
        var lblHasta = new Label { Text = "Hasta:", AutoSize = true, ForeColor = CText, Anchor = AnchorStyles.Left };
        _dtHasta = new DateTimePicker { Anchor = AnchorStyles.Left };

        var btnBuscar = MakeButton("Consultar", CPrimary, 100); btnBuscar.Click += (_, _) => CargarDatos();
        var btnExportar = MakeButton("Exportar CSV", CGrayBtn, 120); btnExportar.Click += (_, _) => ExportarCsv();

        toolbar.Controls.Add(lblDesde, 0, 0);
        toolbar.Controls.Add(_dtDesde, 1, 0);
        toolbar.Controls.Add(lblHasta, 2, 0);
        toolbar.Controls.Add(_dtHasta, 3, 0);
        toolbar.Controls.Add(btnBuscar, 4, 0);
        toolbar.Controls.Add(btnExportar, 5, 0);

        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(12) };
        _grid = BuildGrid();
        _grid.DataSource = _data;
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
            _data = _service.VentasDiarias(_dtDesde.Value.Date, _dtHasta.Value.Date.AddDays(1).AddTicks(-1));
            _grid.DataSource = _data;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportarCsv()
    {
        if (_data.Rows.Count == 0)
        {
            MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"ventas_diarias_{DateTime.Now:yyyyMMdd}.csv"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var lines = new List<string>();

        // Header (use column names as provided by the query)
        var headers = _data.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
        lines.Add(string.Join(",", headers));

        // Rows: format dates and numbers, and escape fields
        foreach (DataRow row in _data.Rows)
        {
            var cells = new List<string>();
            foreach (DataColumn col in _data.Columns)
            {
                var v = row[col];
                if (v == DBNull.Value)
                {
                    cells.Add("");
                    continue;
                }

                switch (v)
                {
                    case DateTime dt:
                        cells.Add(EscapeCsv(dt.ToString("yyyy-MM-dd")));
                        break;
                    case decimal dec:
                        cells.Add(EscapeCsv(dec.ToString("F2", CultureInfo.InvariantCulture)));
                        break;
                    case double d:
                        cells.Add(EscapeCsv(d.ToString("F2", CultureInfo.InvariantCulture)));
                        break;
                    case float f:
                        cells.Add(EscapeCsv(f.ToString("F2", CultureInfo.InvariantCulture)));
                        break;
                    default:
                        cells.Add(EscapeCsv(v.ToString() ?? string.Empty));
                        break;
                }
            }

            lines.Add(string.Join(",", cells));
        }

        File.WriteAllLines(dialog.FileName, lines);
        MessageBox.Show("Reporte exportado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string EscapeCsv(string s)
    {
        if (s == null) return string.Empty;
        var mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
        var escaped = s.Replace("\"", "\"\"");
        return mustQuote ? $"\"{escaped}\"" : escaped;
    }
}
