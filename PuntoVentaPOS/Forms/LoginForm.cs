using PuntoVentaPOS.Models;
using PuntoVentaPOS.Services;

namespace PuntoVentaPOS.Forms;

public sealed class LoginForm : Form
{
    private readonly AuthService _authService = new();
    private TextBox _txtUsuario = null!;
    private TextBox _txtContrasena = null!;
    private Button _btnIngresar = null!;

    public LoginForm()
    {
        InitializeComponent();
    }

    // private void InitializeComponent()
    // {
    //     Text = "Punto de Venta - Login";
    //     StartPosition = FormStartPosition.CenterScreen;
    //     Size = new Size(420, 260);
    //     FormBorderStyle = FormBorderStyle.FixedDialog;
    //     MaximizeBox = false;

    //     var lblUsuario = new Label
    //     {
    //         Text = "Nombre de usuario:",
    //         Location = new Point(30, 30),
    //         AutoSize = true
    //     };

    //     _txtUsuario = new TextBox
    //     {
    //         Location = new Point(30, 55),
    //         Width = 340
    //     };

    //     var lblContrasena = new Label
    //     {
    //         Text = "Contrasena:",
    //         Location = new Point(30, 95),
    //         AutoSize = true
    //     };

    //     _txtContrasena = new TextBox
    //     {
    //         Location = new Point(30, 120),
    //         Width = 340,
    //         PasswordChar = '*'
    //     };

    //     _btnIngresar = new Button
    //     {
    //         Text = "Ingresar",
    //         Location = new Point(30, 165),
    //         Width = 120
    //     };
    //     _btnIngresar.Click += OnIngresarClick;

    //     Controls.Add(lblUsuario);
    //     Controls.Add(_txtUsuario);
    //     Controls.Add(lblContrasena);
    //     Controls.Add(_txtContrasena);
    //     Controls.Add(_btnIngresar);

    //     AcceptButton = _btnIngresar;
    // }

    // private void OnIngresarClick(object? sender, EventArgs e)
    // {
    //     try
    //     {
    //         var nombreUsuario = _txtUsuario.Text.Trim();
    //         var contrasena = _txtContrasena.Text;

    //         if (string.IsNullOrWhiteSpace(nombreUsuario))
    //         {
    //             MessageBox.Show("Ingrese el nombre de usuario.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    //             return;
    //         }

    //         var usuario = _authService.ValidarUsuario(nombreUsuario, contrasena, out var mensaje);
    //         if (usuario == null)
    //         {
    //             MessageBox.Show(mensaje, "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    //             return;
    //         }

    //         UserSession.Start(usuario);
    //         var main = new MainForm();
    //         main.FormClosed += (_, _) =>
    //         {
    //             UserSession.End();
    //             _txtContrasena.Clear();
    //             Show();
    //         };

    //         Hide();
    //         main.Show();
    //     }
    //     catch (Exception ex)
    //     {
    //         MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //     }
    // }

    private void InitializeComponent()
 {
    // Paleta
    var cBg = ColorTranslator.FromHtml("#F4F6F9");      // fondo formularios
    var cText = ColorTranslator.FromHtml("#2C3E50");    // texto
    var cTop = ColorTranslator.FromHtml("#273C75");     // barra menú
    var cPrimary = ColorTranslator.FromHtml("#1E90FF"); // botón principal

    Text = "Punto de Venta - Login";
    StartPosition = FormStartPosition.CenterScreen;
    ClientSize = new Size(460, 310);
    FormBorderStyle = FormBorderStyle.FixedDialog;
    MaximizeBox = false;
    BackColor = cBg;
    Font = new Font("Segoe UI", 10f);

    // Header
    var header = new Panel
    {
        Dock = DockStyle.Top,
        Height = 70,
        BackColor = cTop
    };

    var lblTitle = new Label
    {
        Text = "POS - Punto de Venta",
        ForeColor = Color.White,
        Font = new Font("Segoe UI", 16f, FontStyle.Bold),
        AutoSize = true,
        Location = new Point(20, 20)
    };

    var lblSub = new Label
    {
        Text = "Inicia sesión para continuar",
        ForeColor = Color.FromArgb(230, 230, 230),
        Font = new Font("Segoe UI", 10f, FontStyle.Regular),
        AutoSize = true,
        Location = new Point(22, 46)
    };

    header.Controls.Add(lblTitle);
    header.Controls.Add(lblSub);

    // Card (contenedor)
    var card = new Panel
    {
        BackColor = Color.White,
        Location = new Point(25, 90),
        Size = new Size(410, 190)
    };

    // Labels
    var lblUsuario = new Label
    {
        Text = "Usuario",
        ForeColor = cText,
        Location = new Point(20, 18),
        AutoSize = true
    };

    _txtUsuario = new TextBox
    {
        Location = new Point(20, 40),
        Width = 360
    };

    var lblContrasena = new Label
    {
        Text = "Contraseña",
        ForeColor = cText,
        Location = new Point(20, 78),
        AutoSize = true
    };

    _txtContrasena = new TextBox
    {
        Location = new Point(20, 100),
        Width = 360,
        UseSystemPasswordChar = true
    };

    _btnIngresar = new Button
    {
        Text = "Ingresar",
        Location = new Point(20, 140),
        Width = 140,
        Height = 36,
        FlatStyle = FlatStyle.Flat,
        BackColor = cPrimary,
        ForeColor = Color.White
    };
    _btnIngresar.FlatAppearance.BorderSize = 0;
    _btnIngresar.Click += OnIngresarClick;

    // Bonus: “Enter” y focus inicial
    AcceptButton = _btnIngresar;
    Shown += (_, _) => _txtUsuario.Focus();

    // Agregar a card
    card.Controls.Add(lblUsuario);
    card.Controls.Add(_txtUsuario);
    card.Controls.Add(lblContrasena);
    card.Controls.Add(_txtContrasena);
    card.Controls.Add(_btnIngresar);

    // Agregar a form
    Controls.Add(card);
    Controls.Add(header);
    }
    private void OnIngresarClick(object? sender, EventArgs e)
{
    try
    {
        var nombreUsuario = _txtUsuario.Text.Trim();
        var contrasena = _txtContrasena.Text;

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            MessageBox.Show("Ingrese el nombre de usuario.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var usuario = _authService.ValidarUsuario(nombreUsuario, contrasena, out var mensaje);
        if (usuario == null)
        {
            MessageBox.Show(mensaje, "Acceso denegado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UserSession.Start(usuario);

        var main = new MainForm();
        main.FormClosed += (_, _) =>
        {
            UserSession.End();
            _txtContrasena.Clear();
            Show();
        };

        Hide();
        main.Show();
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

}
