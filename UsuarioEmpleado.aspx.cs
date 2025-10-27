using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UsuarioEmpleado : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarEmpleados();
            InicializarTablaUsuarios();
            CargarUsuarios();
            TXT_FecCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        }
    }

    private void CargarEmpleados()
    {
        DDL_CodEmpleado.Items.Clear();
        DDL_CodEmpleado.Items.Add(new ListItem("-- Seleccionar Empleado --", "0"));
        DDL_CodEmpleado.Items.Add(new ListItem("EMP01 - Pedro Sánchez", "EMP01"));
        DDL_CodEmpleado.Items.Add(new ListItem("EMP02 - Laura Torres", "EMP02"));
        DDL_CodEmpleado.Items.Add(new ListItem("EMP03 - Miguel Flores", "EMP03"));
        DDL_CodEmpleado.Items.Add(new ListItem("EMP04 - Carmen Ruiz", "EMP04"));
        DDL_CodEmpleado.Items.Add(new ListItem("EMP05 - Roberto Díaz", "EMP05"));
    }

    private void InicializarTablaUsuarios()
    {
        if (ViewState["UsuariosEmpleado"] == null)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CodEmpleado");
            dt.Columns.Add("CodUsuario");
            dt.Columns.Add("Password");
            dt.Columns.Add("Perfil");
            dt.Columns.Add("Fec_Creacion", typeof(DateTime));
            dt.Columns.Add("Estado_Usuario");

            // Datos iniciales de ejemplo
            dt.Rows.Add("EMP01", "admin", "admin123", "Administrador", DateTime.Now.AddDays(-7), "Activo");
            dt.Rows.Add("EMP02", "ltorres", "pass2024", "Supervisor", DateTime.Now.AddDays(-3), "Activo");
            dt.Rows.Add("EMP03", "mflores", "vta12345", "Vendedor", DateTime.Now.AddDays(-1), "Inactivo");

            ViewState["UsuariosEmpleado"] = dt;
        }
    }

    private void CargarUsuarios(DataTable dt = null)
    {
        if (dt == null) dt = (DataTable)ViewState["UsuariosEmpleado"];
        GV_Usuarios.DataSource = dt;
        GV_Usuarios.DataBind();
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        MostrarMensaje("Formulario listo para nuevo usuario", "alert-info");
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        // Validaciones servidor
        if (DDL_CodEmpleado.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(TXT_CodUsuario.Text) || string.IsNullOrWhiteSpace(TXT_Password.Text) || DDL_Perfil.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(TXT_FecCreacion.Text) || DDL_EstadoUsuario.SelectedIndex <= 0)
        {
            MostrarMensaje("Complete todos los campos obligatorios", "alert-warning");
            return;
        }

        DataTable dt = (DataTable)ViewState["UsuariosEmpleado"];

        // Evitar duplicados por CodUsuario
        var existe = dt.AsEnumerable().Any(r => r.Field<string>("CodUsuario").Equals(TXT_CodUsuario.Text.Trim(), StringComparison.OrdinalIgnoreCase));
        if (existe)
        {
            MostrarMensaje("El usuario ya existe", "alert-danger");
            return;
        }

        // Agregar registro
        dt.Rows.Add(
            DDL_CodEmpleado.SelectedValue,
            TXT_CodUsuario.Text.Trim(),
            TXT_Password.Text.Trim(), // Nota: en producción, hashear password
            DDL_Perfil.SelectedValue,
            DateTime.Parse(TXT_FecCreacion.Text),
            DDL_EstadoUsuario.SelectedValue
        );

        ViewState["UsuariosEmpleado"] = dt;
        CargarUsuarios();

        MostrarMensaje("Usuario registrado exitosamente", "alert-success");
        LimpiarFormulario();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        MostrarMensaje("Operación cancelada", "alert-info");
    }

    protected void BTN_Imprimir_Click(object sender, EventArgs e)
    {
        // Simulación de impresión
        string script = "alert('Función de impresión de usuarios');";
        ClientScript.RegisterStartupScript(this.GetType(), "ImprimirUsuarios", script, true);
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        string filtro = TXT_Buscar.Text.Trim().ToLower();
        DataTable dt = (DataTable)ViewState["UsuariosEmpleado"];

        if (string.IsNullOrWhiteSpace(filtro))
        {
            CargarUsuarios();
            MostrarMensaje("Búsqueda reiniciada", "alert-info");
            return;
        }

        var query = dt.AsEnumerable()
            .Where(r => r.Field<string>("CodUsuario").ToLower().Contains(filtro) || r.Field<string>("Perfil").ToLower().Contains(filtro) || r.Field<string>("Estado_Usuario").ToLower().Contains(filtro));

        DataTable dtFiltrado = dt.Clone();
        foreach (var row in query) dtFiltrado.ImportRow(row);

        CargarUsuarios(dtFiltrado);
        MostrarMensaje("Búsqueda aplicada", "alert-info");
    }

    protected void GV_Usuarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string codUsuario = e.CommandArgument.ToString();
        DataTable dt = (DataTable)ViewState["UsuariosEmpleado"];

        if (e.CommandName == "EliminarUsuario")
        {
            var row = dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("CodUsuario").Equals(codUsuario, StringComparison.OrdinalIgnoreCase));
            if (row != null) dt.Rows.Remove(row);
            ViewState["UsuariosEmpleado"] = dt;
            CargarUsuarios();
            MostrarMensaje("Usuario eliminado", "alert-success");
        }
        else if (e.CommandName == "EditarUsuario")
        {
            var row = dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("CodUsuario").Equals(codUsuario, StringComparison.OrdinalIgnoreCase));
            if (row != null)
            {
                DDL_CodEmpleado.SelectedValue = row.Field<string>("CodEmpleado");
                TXT_CodUsuario.Text = row.Field<string>("CodUsuario");
                TXT_Password.Text = row.Field<string>("Password");
                DDL_Perfil.SelectedValue = row.Field<string>("Perfil");
                TXT_FecCreacion.Text = row.Field<DateTime>("Fec_Creacion").ToString("yyyy-MM-ddTHH:mm");
                DDL_EstadoUsuario.SelectedValue = row.Field<string>("Estado_Usuario");
                MostrarMensaje("Datos cargados para edición", "alert-info");
            }
        }
    }

    protected void GV_Usuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Usuarios.PageIndex = e.NewPageIndex;
        CargarUsuarios();
    }

    private void LimpiarFormulario()
    {
        DDL_CodEmpleado.SelectedIndex = 0;
        TXT_CodUsuario.Text = string.Empty;
        TXT_Password.Text = string.Empty;
        DDL_Perfil.SelectedIndex = 0;
        TXT_FecCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        DDL_EstadoUsuario.SelectedIndex = 0;
    }

    private void MostrarMensaje(string mensaje, string tipoCss)
    {
        LBL_Mensaje.Text = mensaje;
        LBL_Mensaje.CssClass = $"alert {tipoCss} d-block";
    }
}