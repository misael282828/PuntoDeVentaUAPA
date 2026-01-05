using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class UsuariosForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CDanger = ColorTranslator.FromHtml("#E74C3C");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private readonly UsuariosService _service = new();
    private readonly BindingSource _binding = new();

    private DataGridView _grid = null!;
    private Label _lblCount = null!;

    public UsuariosForm()
    {
        Text = "Usuarios";
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);
        Padding = new Padding(16);
        Size = new Size(900, 520);

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
            Text = "Usuarios",
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

        var btnNuevo = MakeButton("+ Nuevo", CPrimary, 120);
        btnNuevo.Location = new Point(2, 10);
        btnNuevo.Click += (_, _) => Crear();

        var btnEditar = MakeButton("Editar", CGrayBtn, 110);
        btnEditar.Location = new Point(130, 10);
        btnEditar.Click += (_, _) => Editar();

        var btnEliminar = MakeButton("Eliminar", CDanger, 120);
        btnEliminar.Location = new Point(248, 10);
        btnEliminar.Click += (_, _) => Eliminar();

        var btnRefrescar = MakeButton("Refrescar", CGrayBtn, 120);
        btnRefrescar.Location = new Point(376, 10);
        btnRefrescar.Click += (_, _) => CargarDatos();

        p.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar, btnRefrescar });
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

        // Columns
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Usuario.IdUsuario),
            HeaderText = "Id",
            Width = 70
        });

        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Usuario.NombreUsuario),
            HeaderText = "Usuario",
            Width = 220
        });

        var colRol = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Usuario.Rol),
            HeaderText = "Rol",
            Width = 140,
            Name = "Rol",
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        };
        g.Columns.Add(colRol);

        // Rol con color
        g.CellFormatting += (_, e) =>
        {
            if (g.Columns[e.ColumnIndex].Name == "Rol" && e.Value != null)
            {
                var rol = e.Value.ToString() ?? "";
                if (rol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(30, 130, 60);
                    e.CellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(70, 70, 70);
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
            var lista = _service.Listar();
            _binding.DataSource = new BindingList<Usuario>(lista);
            _lblCount.Text = $"{lista.Count} registros";
            if (lista.Count > 0) _grid.ClearSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Usuario? ObtenerSeleccionado() => _binding.Current as Usuario;

    private void Crear()
    {
        var editor = new UsuarioEditorForm(new Usuario(), true);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _service.Crear(editor.Usuario, editor.Contrasena);
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
            MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Copia para no “editar en vivo” si cancelas
        var copia = new Usuario
        {
            IdUsuario = seleccionado.IdUsuario,
            NombreUsuario = seleccionado.NombreUsuario,
            Rol = seleccionado.Rol
        };

        var editor = new UsuarioEditorForm(copia, false);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _service.Actualizar(editor.Usuario);
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
            MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"¿Eliminar el usuario '{seleccionado.NombreUsuario}'?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.Eliminar(seleccionado.IdUsuario);
            CargarDatos();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
