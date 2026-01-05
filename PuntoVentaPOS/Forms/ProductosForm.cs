using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class ProductosForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CDanger = ColorTranslator.FromHtml("#E74C3C");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly ProductosService _service = new();
    private readonly BindingSource _binding = new();

    private TextBox _txtBuscar = null!;
    private DataGridView _grid = null!;
    private Label _lblCount = null!;

    public ProductosForm()
    {
        Text = "Productos";
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(16);
        Size = new Size(980, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = CBg,
            ColumnCount = 1,
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Title
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Toolbar
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Grid
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Footer

        // Title
        var titleRow = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CBg };
        var lblTitle = new Label
        {
            Text = "Productos",
            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = CText,
            AutoSize = true,
            Location = new Point(2, 2)
        };
        titleRow.Controls.Add(lblTitle);

        // Toolbar
        var toolbar = BuildToolbar();

        // Grid card
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12)
        };

        _grid = BuildGrid();
        _grid.DataSource = _binding;
        card.Controls.Add(_grid);

        // Footer
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = CBg };
        _lblCount = new Label
        {
            Text = "0 registros",
            ForeColor = Color.FromArgb(90, 90, 90),
            AutoSize = true,
            Location = new Point(2, 6)
        };
        footer.Controls.Add(_lblCount);

        root.Controls.Add(titleRow, 0, 0);
        root.Controls.Add(toolbar, 0, 1);
        root.Controls.Add(card, 0, 2);
        root.Controls.Add(footer, 0, 3);

        Controls.Add(root);
    }

    private Panel BuildToolbar()
    {
        var p = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = CBg };

        var lblBuscar = new Label
        {
            Text = "Buscar:",
            AutoSize = true,
            ForeColor = CText,
            Location = new Point(2, 16)
        };

        _txtBuscar = new TextBox { Location = new Point(62, 12), Width = 260 };
        _txtBuscar.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) CargarDatos();
        };

        var btnBuscar = MakeButton("Buscar", CGrayBtn, 90);
        btnBuscar.Location = new Point(330, 10);
        btnBuscar.Click += (_, _) => CargarDatos();

        var btnNuevo = MakeButton("+ Nuevo", CPrimary, 110);
        btnNuevo.Location = new Point(440, 10);
        btnNuevo.Click += (_, _) => Crear();

        var btnEditar = MakeButton("Editar", CGrayBtn, 100);
        btnEditar.Location = new Point(560, 10);
        btnEditar.Click += (_, _) => Editar();

        var btnEliminar = MakeButton("Eliminar", CDanger, 110);
        btnEliminar.Location = new Point(670, 10);
        btnEliminar.Click += (_, _) => Eliminar();

        var btnRefrescar = MakeButton("Refrescar", CGrayBtn, 110);
        btnRefrescar.Location = new Point(790, 10);
        btnRefrescar.Click += (_, _) => CargarDatos();

        p.Controls.AddRange(new Control[]
        {
            lblBuscar, _txtBuscar, btnBuscar, btnNuevo, btnEditar, btnEliminar, btnRefrescar
        });

        return p;
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

        // Header
        g.EnableHeadersVisualStyles = false;
        g.ColumnHeadersDefaultCellStyle.BackColor = CTop;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        g.ColumnHeadersHeight = 38;

        // Rows
        g.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
        g.DefaultCellStyle.ForeColor = CText;
        g.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#D6E4FF");
        g.DefaultCellStyle.SelectionForeColor = Color.Black;
        g.AlternatingRowsDefaultCellStyle.BackColor = CBg;

        // Columns ordenadas
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Producto.IdProducto), HeaderText = "Id", Width = 60 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Producto.Codigo), HeaderText = "Código", Width = 120 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Producto.Nombre), HeaderText = "Nombre", Width = 220 });

        var colPrecio = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Producto.Precio),
            HeaderText = "Precio",
            Width = 120,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
        };
        g.Columns.Add(colPrecio);

        var colStock = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Producto.Stock),
            HeaderText = "Stock",
            Width = 90,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        g.Columns.Add(colStock);

        var colActivo = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Producto.Activo),
            HeaderText = "Activo",
            Width = 80,
            Name = "Activo",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        g.Columns.Add(colActivo);

        // Formatos: Precio dinero + Activo ✔/✖ + Stock bajo resaltado
        g.CellFormatting += (_, e) =>
        {
            var colName = g.Columns[e.ColumnIndex].Name;

            // Precio en formato moneda
            if (g.Columns[e.ColumnIndex].DataPropertyName == nameof(Producto.Precio) && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var val))
                {
                    // Si estás en RD, te saldrá con cultura del sistema; si quieres RD fijo, me dices
                    e.Value = val.ToString("C2", CultureInfo.CurrentCulture);
                    e.FormattingApplied = true;
                }
            }

            // Activo ✔ / ✖
            if (colName == "Activo" && e.Value != null)
            {
                bool activo = false;
                try { activo = Convert.ToBoolean(e.Value); } catch { }

                e.Value = activo ? "✔" : "✖";
                e.CellStyle.ForeColor = activo ? Color.FromArgb(30, 130, 60) : Color.FromArgb(200, 60, 60);
                e.CellStyle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
                e.FormattingApplied = true;
            }

            // Stock bajo (ej. < 5)
            if (g.Columns[e.ColumnIndex].DataPropertyName == nameof(Producto.Stock) && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out var stock) && stock < 5)
                {
                    e.CellStyle.ForeColor = Color.FromArgb(200, 60, 60);
                    e.CellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                }
            }
        };

        // Doble click -> editar
        g.CellDoubleClick += (_, _) => Editar();

        return g;
    }

    private void CargarDatos()
    {
        try
        {
            var filtro = _txtBuscar.Text.Trim();
            var lista = _service.Listar(string.IsNullOrWhiteSpace(filtro) ? null : filtro);

            _binding.DataSource = new BindingList<Producto>(lista);
            _lblCount.Text = $"{lista.Count} registros";
            if (lista.Count > 0) _grid.ClearSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Producto? ObtenerSeleccionado() => _binding.Current as Producto;

    private void Crear()
    {
        var editor = new ProductoEditorForm(new Producto());
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _service.Crear(editor.Producto);
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void Editar()
    {
        var seleccionado = ObtenerSeleccionado();
        if (seleccionado == null)
        {
            MessageBox.Show("Seleccione un producto.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Copia para evitar editar el objeto ligado si cancelas
        var copia = new Producto
        {
            IdProducto = seleccionado.IdProducto,
            Codigo = seleccionado.Codigo,
            Nombre = seleccionado.Nombre,
            Precio = seleccionado.Precio,
            Stock = seleccionado.Stock,
            Activo = seleccionado.Activo
        };

        var editor = new ProductoEditorForm(copia);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _service.Actualizar(editor.Producto);
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void Eliminar()
    {
        var seleccionado = ObtenerSeleccionado();
        if (seleccionado == null)
        {
            MessageBox.Show("Seleccione un producto.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var ok = MessageBox.Show(
            $"¿Eliminar el producto '{seleccionado.Nombre}'?",
            "Confirmar",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (ok != DialogResult.Yes) return;

        try
        {
            _service.Eliminar(seleccionado.IdProducto);
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
