using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Forms;

public sealed class UsuarioEditorForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF");
    private static readonly Color CGrayBtn = ColorTranslator.FromHtml("#6C757D");
    private static readonly Color CText = ColorTranslator.FromHtml("#2C3E50");

    private TextBox _txtNombre = null!;
    private TextBox _txtContrasena = null!;
    private ComboBox _cmbRol = null!;
    private Label _lblContrasena = null!;

    public Usuario Usuario { get; private set; }
    public string Contrasena { get; private set; } = string.Empty;

    private readonly bool _pedirContrasena;

    public UsuarioEditorForm(Usuario usuario, bool pedirContrasena)
    {
        Usuario = usuario;
        _pedirContrasena = pedirContrasena;

        InitializeComponent();
        Cargar();
    }

    private void InitializeComponent()
    {
        Text = _pedirContrasena ? "Nuevo Usuario" : "Editar Usuario";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);

        // Tamaño según si pide contraseña o no
        ClientSize = new Size(420, _pedirContrasena ? 360 : 300);

        // Header azul
        var header = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = CTop };
        var lblTitle = new Label
        {
            Text = _pedirContrasena ? "Usuarios - Crear" : "Usuarios - Editar",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 16)
        };
        var lblSub = new Label
        {
            Text = _pedirContrasena ? "Complete los datos del nuevo usuario" : "Actualice los datos del usuario",
            ForeColor = Color.FromArgb(220, 230, 255),
            Font = new Font("Segoe UI", 10f, FontStyle.Regular),
            AutoSize = true,
            Location = new Point(22, 44)
        };
        header.Controls.Add(lblTitle);
        header.Controls.Add(lblSub);

        // Card blanca
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18)
        };

        // Layout
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 10
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var lblNombre = new Label { Text = "Usuario", ForeColor = CText, AutoSize = true };
        _txtNombre = new TextBox { Width = 340 };

        var lblRol = new Label { Text = "Rol", ForeColor = CText, AutoSize = true };
        _cmbRol = new ComboBox
        {
            Width = 220,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cmbRol.DataSource = Enum.GetValues(typeof(UserRole));

        _lblContrasena = new Label { Text = "Contraseña", ForeColor = CText, AutoSize = true };
        _txtContrasena = new TextBox
        {
            Width = 340,
            PasswordChar = '•'
        };

        // Mostrar/ocultar contraseña según modo
        _lblContrasena.Visible = _pedirContrasena;
        _txtContrasena.Visible = _pedirContrasena;

        // Botones
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 14, 0, 0)
        };

        var btnGuardar = MakeButton("Guardar", CPrimary, 120);
        btnGuardar.Click += (_, _) => Guardar();

        var btnCancelar = MakeButton("Cancelar", CGrayBtn, 120);
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttons.Controls.Add(btnGuardar);
        buttons.Controls.Add(btnCancelar);

        // Spacing helpers
        Control Space(int h) => new Panel { Height = h, Dock = DockStyle.Top };

        layout.Controls.Add(lblNombre);
        layout.Controls.Add(_txtNombre);
        layout.Controls.Add(Space(8));
        layout.Controls.Add(lblRol);
        layout.Controls.Add(_cmbRol);

        if (_pedirContrasena)
        {
            layout.Controls.Add(Space(8));
            layout.Controls.Add(_lblContrasena);
            layout.Controls.Add(_txtContrasena);
        }

        card.Controls.Add(buttons);
        card.Controls.Add(Space(10));
        card.Controls.Add(layout);

        Controls.Add(card);
        Controls.Add(header);

        AcceptButton = btnGuardar;
    }

    private static Button MakeButton(string text, Color bg, int width)
    {
        var b = new Button
        {
            Text = text,
            Width = width,
            Height = 36,
            BackColor = bg,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    private void Cargar()
    {
        _txtNombre.Text = Usuario.NombreUsuario;
        _cmbRol.SelectedItem = Usuario.Rol;

        if (_pedirContrasena)
            _txtContrasena.Clear();
    }

    private void Guardar()
    {
        var nombre = _txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("El usuario es obligatorio.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNombre.Focus();
            return;
        }

        if (_pedirContrasena)
        {
            var pass = _txtContrasena.Text;
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("La contraseña es obligatoria para crear el usuario.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtContrasena.Focus();
                return;
            }
            Contrasena = pass;
        }

        Usuario.NombreUsuario = nombre;
        Usuario.Rol = (UserRole)_cmbRol.SelectedItem!;

        DialogResult = DialogResult.OK;
        Close();
    }
}
