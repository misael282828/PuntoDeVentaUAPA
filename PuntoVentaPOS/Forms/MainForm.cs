using System.Drawing;
using System.Windows.Forms;
using PuntoVentaPOS.Models;

namespace PuntoVentaPOS.Forms;

public sealed class MainForm : Form
{
    // Paleta corporativa
    private static readonly Color CBg = ColorTranslator.FromHtml("#F4F6F9");      // fondo formularios
    private static readonly Color CTop = ColorTranslator.FromHtml("#273C75");     // barra menú
    private static readonly Color CPrimary = ColorTranslator.FromHtml("#1E90FF"); // acento
    private static readonly Color CText = Color.White;

    private ToolStripMenuItem _usuariosMenu = null!;
    private ToolStripMenuItem _anularFacturaMenu = null!;
    private TabControl _tabs = null!;

    public MainForm()
    {
        InitializeComponent();
        AplicarPermisos();
    }

    private void InitializeComponent()
    {
        Text = "Punto de Venta - Menú Principal";
        WindowState = FormWindowState.Maximized;
        BackColor = CBg;
        Font = new Font("Segoe UI", 10f);

        // Menú superior
        var menu = new MenuStrip
        {
            Dock = DockStyle.Top,
            BackColor = CTop,
            ForeColor = CText,
            Padding = new Padding(10, 6, 10, 6),
            Renderer = new CorporateMenuRenderer(CTop, CPrimary, CText)
        };

        var registrosMenu = new ToolStripMenuItem("Registros");
        var clientesItem = new ToolStripMenuItem("Clientes", null, (_, _) => Abrir(new ClientesForm()));
        var productosItem = new ToolStripMenuItem("Productos", null, (_, _) => Abrir(new ProductosForm()));
        _usuariosMenu = new ToolStripMenuItem("Usuarios", null, (_, _) => Abrir(new UsuariosForm()));
        registrosMenu.DropDownItems.AddRange(new ToolStripItem[] { clientesItem, productosItem, _usuariosMenu });

        var ventasMenu = new ToolStripMenuItem("Ventas");
        var nuevaFacturaItem = new ToolStripMenuItem("Nueva Factura", null, (_, _) => Abrir(new FacturaForm()));
        var consultarFacturasItem = new ToolStripMenuItem("Consultar Facturas", null, (_, _) => Abrir(new FacturasConsultaForm()));
        _anularFacturaMenu = new ToolStripMenuItem("Anular Factura (Admin)", null, (_, _) => Abrir(new FacturasConsultaForm(true)));
        ventasMenu.DropDownItems.AddRange(new ToolStripItem[] { nuevaFacturaItem, consultarFacturasItem, _anularFacturaMenu });

        var cuentasMenu = new ToolStripMenuItem("Cuentas");
        var cxcItem = new ToolStripMenuItem("Gestión de CxC", null, (_, _) => Abrir(new CxcForm()));
        cuentasMenu.DropDownItems.Add(cxcItem);

        var reportesMenu = new ToolStripMenuItem("Reportes");
        var stockItem = new ToolStripMenuItem("Stock", null, (_, _) => Abrir(new ReporteStockForm()));
        var ventasItem = new ToolStripMenuItem("Ventas Diarias", null, (_, _) => Abrir(new ReporteVentasDiariasForm()));
        reportesMenu.DropDownItems.AddRange(new ToolStripItem[] { stockItem, ventasItem });

        var utilitariosMenu = new ToolStripMenuItem("Cerrar Sesión", null, (_, _) =>
        {
            if (MessageBox.Show("¿Cerrar sesión?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        });

        menu.Items.AddRange(new ToolStripItem[] { registrosMenu, ventasMenu, cuentasMenu, reportesMenu, utilitariosMenu });
        MainMenuStrip = menu;

        // Tabs
        _tabs = new TabControl
        {
            Dock = DockStyle.Fill
        };

        // Estilo de tabs (fondo)
        _tabs.Appearance = TabAppearance.Normal;
        _tabs.BackColor = CBg;

        Controls.Add(_tabs);
        Controls.Add(menu);
    }

    private void AplicarPermisos()
    {
        var rol = UserSession.CurrentUser?.Rol ?? UserRole.Cajero;
        var esAdmin = rol == UserRole.Admin;

        // Mejor UX: ocultar en vez de deshabilitar (más “pro”)
        _usuariosMenu.Visible = esAdmin;
        _anularFacturaMenu.Visible = esAdmin;
    }

    private void Abrir(Form form)
    {
        var title = string.IsNullOrWhiteSpace(form.Text) ? form.GetType().Name : form.Text;

        // Reusar pestaña si ya está abierta (mismo tipo y mismo título)
        foreach (TabPage tp in _tabs.TabPages)
        {
            if (tp.Tag is Form f && f.GetType() == form.GetType() && tp.Text == title)
            {
                _tabs.SelectedTab = tp;
                return;
            }
        }

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        form.BackColor = CBg;

        var tpNew = new TabPage(title)
        {
            BackColor = CBg,
            Tag = form
        };

        tpNew.Controls.Add(form);
        _tabs.TabPages.Add(tpNew);
        form.Show();
        _tabs.SelectedTab = tpNew;
    }

    // Renderer para menú corporativo
    private sealed class CorporateMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly Color _bg;
        private readonly Color _accent;
        private readonly Color _text;

        public CorporateMenuRenderer(Color bg, Color accent, Color text)
        {
            _bg = bg;
            _accent = accent;
            _text = text;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.Clear(_bg);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            var g = e.Graphics;

            var isTop = item.Owner is MenuStrip;
            var rect = new Rectangle(Point.Empty, item.Size);

            if (e.Item.Selected)
            {
                using var b = new SolidBrush(isTop ? Color.FromArgb(40, 255, 255, 255) : Color.White);
                g.FillRectangle(b, rect);

                // acento a la izquierda en dropdown
                if (!isTop)
                {
                    using var a = new SolidBrush(_accent);
                    g.FillRectangle(a, new Rectangle(0, 0, 4, rect.Height));
                }
            }
            else
            {
                using var b = new SolidBrush(isTop ? _bg : Color.White);
                g.FillRectangle(b, rect);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            var isTop = e.ToolStrip is MenuStrip;
            e.TextColor = isTop ? _text : ColorTranslator.FromHtml("#2C3E50");
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var r = new Rectangle(8, 3, e.Item.Width - 16, 1);
            using var p = new Pen(Color.FromArgb(220, 220, 220));
            e.Graphics.DrawLine(p, r.Left, r.Top, r.Right, r.Top);
        }
    }
}
