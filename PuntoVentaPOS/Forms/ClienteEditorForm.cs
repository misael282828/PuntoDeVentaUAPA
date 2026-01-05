using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Forms;

public sealed class ClienteEditorForm : Form
{
    private TextBox _txtNombre = null!;
    private TextBox _txtDocumento = null!;
    private TextBox _txtTelefono = null!;
    private TextBox _txtDireccion = null!;
    private TextBox _txtEmail = null!;
    private CheckBox _chkActivo = null!;

    public Cliente Cliente { get; private set; }

    public ClienteEditorForm(Cliente cliente)
    {
        Cliente = cliente;
        InitializeComponent();
        Cargar();
    }

    private void InitializeComponent()
    {
        Text = "Cliente";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(420, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lblNombre = new Label { Text = "Nombre:", Location = new Point(20, 20), AutoSize = true };
        _txtNombre = new TextBox { Location = new Point(20, 45), Width = 360 };

        var lblDocumento = new Label { Text = "Documento:", Location = new Point(20, 80), AutoSize = true };
        _txtDocumento = new TextBox { Location = new Point(20, 105), Width = 360 };

        var lblTelefono = new Label { Text = "Telefono:", Location = new Point(20, 140), AutoSize = true };
        _txtTelefono = new TextBox { Location = new Point(20, 165), Width = 360 };

        var lblDireccion = new Label { Text = "Direccion:", Location = new Point(20, 200), AutoSize = true };
        _txtDireccion = new TextBox { Location = new Point(20, 225), Width = 360 };

        var lblEmail = new Label { Text = "Email:", Location = new Point(20, 260), AutoSize = true };
        _txtEmail = new TextBox { Location = new Point(20, 285), Width = 360 };

        _chkActivo = new CheckBox { Text = "Activo", Location = new Point(20, 320), AutoSize = true };

        var btnGuardar = new Button { Text = "Guardar", Location = new Point(20, 350), Width = 120 };
        btnGuardar.Click += (_, _) => Guardar();

        var btnCancelar = new Button { Text = "Cancelar", Location = new Point(160, 350), Width = 120 };
        btnCancelar.Click += (_, _) => Close();

        Controls.AddRange([
            lblNombre, _txtNombre,
            lblDocumento, _txtDocumento,
            lblTelefono, _txtTelefono,
            lblDireccion, _txtDireccion,
            lblEmail, _txtEmail,
            _chkActivo,
            btnGuardar, btnCancelar
        ]);
    }

    private void Cargar()
    {
        _txtNombre.Text = Cliente.Nombre;
        _txtDocumento.Text = Cliente.Documento;
        _txtTelefono.Text = Cliente.Telefono;
        _txtDireccion.Text = Cliente.Direccion;
        _txtEmail.Text = Cliente.Email;
        _chkActivo.Checked = Cliente.Activo;
    }

    private void Guardar()
    {
        Cliente.Nombre = _txtNombre.Text.Trim();
        Cliente.Documento = _txtDocumento.Text.Trim();
        Cliente.Telefono = _txtTelefono.Text.Trim();
        Cliente.Direccion = _txtDireccion.Text.Trim();
        Cliente.Email = _txtEmail.Text.Trim();
        Cliente.Activo = _chkActivo.Checked;

        DialogResult = DialogResult.OK;
        Close();
    }
}
