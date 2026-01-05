using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class ClientesForm : Form
{
  // Paleta corporativa (como la referencia)
  private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
  private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
  private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
  private static readonly Color CDanger = ColorTranslator.FromHtml("#E74C3C");
  private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
  private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

  private TextBox _txtBuscar = null!;
  private Button _btnBuscar = null!;
  private Button _btnNuevo = null!;
  private Button _btnEditar = null!;
  private Button _btnEliminar = null!;
  private Button _btnRefrescar = null!;
  private DataGridView _grid = null!;
  private Label _lblCount = null!;

  private readonly ClientesService _service = new();
  private List<Cliente> _current = new();

  public ClientesForm()
  {
    Text = "Clientes";
    InitializeComponent();
    Load += (_, _) => Refrescar();
  }

  private void InitializeComponent()
  {
    BackColor = CBg;
    Font = new Font("Segoe UI", 10f);
    Padding = new Padding(16);

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
      Text = "Clientes",
      Font = new Font("Segoe UI", 18f, FontStyle.Bold),
      ForeColor = CText,
      AutoSize = true,
      Location = new Point(2, 2)
    };
    titleRow.Controls.Add(lblTitle);

    // Toolbar
    var toolbar = BuildToolbar();

    // Card (grid)
    var card = new Panel
    {
      Dock = DockStyle.Fill,
      BackColor = Color.White,
      Padding = new Padding(12)
    };

    _grid = BuildGrid();
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

    _txtBuscar = new TextBox { Width = 260, Location = new Point(62, 12) };
    _txtBuscar.KeyDown += (_, e) =>
    {
      if (e.KeyCode == Keys.Enter) Buscar();
    };

    _btnBuscar = MakeButton("Buscar", CGrayBtn, 90);
    _btnBuscar.Location = new Point(330, 10);
    _btnBuscar.Click += (_, _) => Buscar();

    _btnNuevo = MakeButton("+ Nuevo", CPrimary, 110);
    _btnNuevo.Location = new Point(440, 10);
    _btnNuevo.Click += (_, _) => Nuevo();

    _btnEditar = MakeButton("Editar", CGrayBtn, 100);
    _btnEditar.Location = new Point(560, 10);
    _btnEditar.Click += (_, _) => Editar();

    _btnEliminar = MakeButton("Eliminar", CDanger, 110);
    _btnEliminar.Location = new Point(670, 10);
    _btnEliminar.Click += (_, _) => Eliminar();

    _btnRefrescar = MakeButton("Refrescar", CGrayBtn, 110);
    _btnRefrescar.Location = new Point(790, 10);
    _btnRefrescar.Click += (_, _) => Refrescar();

    p.Controls.Add(lblBuscar);
    p.Controls.Add(_txtBuscar);
    p.Controls.Add(_btnBuscar);
    p.Controls.Add(_btnNuevo);
    p.Controls.Add(_btnEditar);
    p.Controls.Add(_btnEliminar);
    p.Controls.Add(_btnRefrescar);

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

    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdCliente", HeaderText = "Id", Width = 60 });
    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 180 });
    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Documento", HeaderText = "Documento", Width = 140 });
    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono", Width = 120 });
    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Direccion", HeaderText = "Dirección", Width = 190 });
    g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 180 });

    var activoCol = new DataGridViewTextBoxColumn
    {
      DataPropertyName = "Activo",
      HeaderText = "Activo",
      Width = 70,
      Name = "Activo"
    };
    g.Columns.Add(activoCol);

    g.CellFormatting += (_, e) =>
    {
      if (g.Columns[e.ColumnIndex].Name == "Activo" && e.Value != null)
      {
        bool activo = false;
        try { activo = Convert.ToBoolean(e.Value); } catch { }

        e.Value = activo ? "✔" : "✖";
        e.CellStyle.ForeColor = activo ? Color.FromArgb(30, 130, 60) : Color.FromArgb(200, 60, 60);
        e.CellStyle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
        e.FormattingApplied = true;
      }
    };

    g.CellDoubleClick += (_, _) => Editar();
    return g;
  }

  private void Refrescar()
  {
    try
    {
      _txtBuscar.Clear();
      _current = _service.Listar(null);          // ✅ tu método real
      Bind(_current);
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private void Buscar()
  {
    try
    {
      var term = _txtBuscar.Text.Trim();
      _current = _service.Listar(string.IsNullOrWhiteSpace(term) ? null : term); // ✅ tu método real
      Bind(_current);
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }

  private void Bind(List<Cliente> data)
  {
    _grid.DataSource = null;
    _grid.DataSource = data;
    _lblCount.Text = $"{data.Count} registros";
    if (data.Count > 0) _grid.ClearSelection();
  }

  private Cliente? SelectedCliente()
  {
    if (_grid.CurrentRow?.DataBoundItem is Cliente c) return c;
    return null;
  }

  private void Nuevo()
  {
    using var frm = new ClienteEditorForm(new Cliente());
    if (frm.ShowDialog(this) == DialogResult.OK)
    {
      _service.Crear(frm.Cliente); // ✅ tu método real
      Refrescar();
    }
  }

  private void Editar()
  {
    var c = SelectedCliente();
    if (c == null)
    {
      MessageBox.Show("Selecciona un cliente para editar.", "Validación",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    using var frm = new ClienteEditorForm(c);
    if (frm.ShowDialog(this) == DialogResult.OK)
    {
      _service.Actualizar(frm.Cliente); // ✅ tu método real
      Refrescar();
    }
  }

  private void Eliminar()
  {
    var c = SelectedCliente();
    if (c == null)
    {
      MessageBox.Show("Selecciona un cliente para eliminar.", "Validación",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    var ok = MessageBox.Show($"¿Eliminar el cliente '{c.Nombre}'?", "Confirmar",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    if (ok != DialogResult.Yes) return;

    try
    {
      _service.Eliminar(c.IdCliente); // ✅ tu método real
      Refrescar();
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
