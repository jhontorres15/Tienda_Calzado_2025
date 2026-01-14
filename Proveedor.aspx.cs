using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Proveedor : System.Web.UI.Page
{
    ProveedorLOGIC logic = new ProveedorLOGIC();
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GenerarCodigo();
            CargarEmpresas();
            CargarListado();
            Listar_Departamento();
            BTN_Actualizar.Visible = false;
        }
    }

    void GenerarCodigo()
    {
        TXT_CodProveedor.Text = logic.Generar_CodProveedor();
        TXT_CodProveedor.Enabled = false;
    }

    void CargarEmpresas()
    {
        var dt = logic.Listar_Empresas();
        DDL_Empresa.DataSource = dt;
     
        DDL_Empresa.DataTextField = dt.Columns.Contains("Razon_Social") ? "Razon_Social" : dt.Columns[1].ColumnName;
        DDL_Empresa.DataValueField = dt.Columns.Contains("CodEmpresa") ? "CodEmpresa" : dt.Columns[0].ColumnName;
        DDL_Empresa.DataBind();
        
    }

    void Mostrar(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalProveedor", script, true);
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        Limpiar();
        GenerarCodigo();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        Limpiar();
    }

    void Limpiar()
    {
        TXT_Apellidos.Text = "";
        TXT_Nombres.Text = "";
        TXT_Telefono.Text = "";
        TXT_Email.Text = "";
        DDL_Departamento.SelectedIndex = 0;
        TXT_Direccion.Text = "";
        DDL_Empresa.SelectedIndex = 0;
        DDL_Estado.SelectedIndex = 0;
    }


    void Listar_Departamento()
    {
        Global.CN.Open();

        SqlDataReader Lector;

        SqlCommand CMDListar_Dep = new SqlCommand();
        CMDListar_Dep.Connection = Global.CN;

        CMDListar_Dep.CommandType = CommandType.StoredProcedure;
        CMDListar_Dep.CommandText = "USP_Listar_Departamento";

        Lector = CMDListar_Dep.ExecuteReader();

        this.DDL_Departamento.Items.Clear();
        this.DDL_Departamento.Items.Add("<Seleccione>");
        while (Lector.Read())
        {
            this.DDL_Departamento.Items.Add(Lector.GetValue(0).ToString());

        }
        CMDListar_Dep.Dispose();
        Global.CN.Close();
    }

    void Listar_Procincia(String Departamento)
    {
        Global.CN.Open();

        SqlDataReader Lector;
        SqlCommand CMDListar_Provincia = new SqlCommand();
        //Conectar y Configurar el Comando de Datos
        CMDListar_Provincia.Connection = Global.CN;
        CMDListar_Provincia.CommandType = CommandType.StoredProcedure;
        CMDListar_Provincia.CommandText = "USP_Listar_Provincia";
        //Agregar Parámetro de Datos
        CMDListar_Provincia.Parameters.Add("@Departamento", SqlDbType.VarChar, 40).Value = Departamento;
        //Ejecutar el Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Provincia.ExecuteReader();
        //Limpiar Datos del Control: DDL_Provincia
        this.DDL_Provincia.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Provincia.Items.Add("<Seleccione>");

        //Leer Datos

        while (Lector.Read())

        {

            //Agregar Datos al (/ Site
            this.DDL_Provincia.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos

        CMDListar_Provincia.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Distrito(String Provincia)
    {
        Global.CN.Open();

        SqlDataReader Lector;
        SqlCommand CMDListar_Distrito = new SqlCommand();
        //Conectar y Configurar el Comando de Datos
        CMDListar_Distrito.Connection = Global.CN;
        CMDListar_Distrito.CommandType = CommandType.StoredProcedure;
        CMDListar_Distrito.CommandText = "USP_Listar_Distrito";
        //Agregar Parámetro de Datos
        CMDListar_Distrito.Parameters.Add("@Provincia", SqlDbType.VarChar, 40).Value = Provincia;

        //Ejecutar el Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Distrito.ExecuteReader();
        //Limpiar Datos del Control: DDL_Provincia
        this.DDL_Distrito.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Distrito.Items.Add("<Seleccione>");

        //Leer Datos

        while (Lector.Read())

        {

            //Agregar Datos al (/ Site
            this.DDL_Distrito.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos

        CMDListar_Distrito.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }


    protected void DDL_Departamento_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Invocar al método Listar_Provincia
        this.Listar_Procincia(this.DDL_Departamento.Text.ToString());

        //
        this.DDL_Provincia_SelectedIndexChanged(null, null);

        //Colocar el foco de atención en el control: DDL_Provincia
        this.DDL_Provincia.Focus();
    }

    protected void DDL_Provincia_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Listar_Distrito(this.DDL_Provincia.Text.ToString());

        //Colocar el foco de atención en el control: DDL_Distrito
        this.DDL_Distrito.Focus();
    }


    protected void BTN_Guardar_Click(object sender, EventArgs e)
    {
        // VALIDACIONES
        if (string.IsNullOrWhiteSpace(TXT_Apellidos.Text))
        {
            Mostrar("Debe ingresar los apellidos.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Nombres.Text))
        {
            Mostrar("Debe ingresar los nombres.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Telefono.Text) ||
            !TXT_Telefono.Text.All(char.IsDigit) ||
            TXT_Telefono.Text.Length < 6)
        {
            Mostrar("Ingrese un número de teléfono válido.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Email.Text) ||
            !System.Text.RegularExpressions.Regex.IsMatch(TXT_Email.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            Mostrar("Ingrese un email válido.", "warning");
            return;
        }

        if (DDL_Distrito.SelectedIndex == 0)
        {
            Mostrar("Debe seleccionar un distrito.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Direccion.Text))
        {
            Mostrar("Debe ingresar la dirección.", "warning");
            return;
        }

        string msg = logic.Mantener(TXT_CodProveedor.Text.Trim(), TXT_Apellidos.Text.Trim(), TXT_Nombres.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Distrito.SelectedItem.ToString(), TXT_Direccion.Text.Trim(), DDL_Empresa.SelectedValue, DDL_Estado.SelectedValue, "GUARDAR");
        if (msg.StartsWith("OK") || msg.StartsWith("Proveedor")) { Mostrar("Registro guardado.", "success"); Limpiar(); CargarListado(); } else Mostrar(msg, "error");
    }

    protected void BTN_Actualizar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TXT_CodProveedor.Text))
        {
            Mostrar("Debe seleccionar un proveedor para actualizar.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Apellidos.Text))
        {
            Mostrar("Debe ingresar los apellidos.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Nombres.Text))
        {
            Mostrar("Debe ingresar los nombres.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Telefono.Text) ||
            !TXT_Telefono.Text.All(char.IsDigit) ||
            TXT_Telefono.Text.Length < 6)
        {
            Mostrar("Ingrese un número de teléfono válido.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Email.Text) ||
            !System.Text.RegularExpressions.Regex.IsMatch(TXT_Email.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            Mostrar("Ingrese un email válido.", "warning");
            return;
        }

        if (DDL_Distrito.SelectedIndex == 0)
        {
            Mostrar("Debe seleccionar un distrito.", "warning");
            return;
        }

        if (string.IsNullOrWhiteSpace(TXT_Direccion.Text))
        {
            Mostrar("Debe ingresar la dirección.", "warning");
            return;
        }

 

        string msg = logic.Mantener(TXT_CodProveedor.Text.Trim(), TXT_Apellidos.Text.Trim(), TXT_Nombres.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Distrito.SelectedItem.ToString(), TXT_Direccion.Text.Trim(), DDL_Empresa.SelectedValue, DDL_Estado.SelectedValue, "ACTUALIZAR");
        if (msg.StartsWith("OK") || msg.StartsWith("Datos")) 
        { 
            Mostrar("Actualización correcta.", "success"); 
            CargarListado();
            Limpiar();
            BTN_Guardar.Visible = true;
            BTN_Actualizar.Visible = false;
        } 
        else 
            Mostrar(msg, "error");
    }

    protected void BTN_Eliminar_Click(object sender, EventArgs e)
    {
        string msg = logic.Mantener(TXT_CodProveedor.Text.Trim(), TXT_Apellidos.Text.Trim(), TXT_Nombres.Text.Trim(), TXT_Telefono.Text.Trim(), TXT_Email.Text.Trim(), DDL_Distrito.SelectedItem.ToString(), TXT_Direccion.Text.Trim(), DDL_Empresa.SelectedValue, DDL_Estado.SelectedValue, "DELETE");
        if (msg.StartsWith("OK") || msg.StartsWith("Proveedor")) { Mostrar("Eliminación correcta.", "success"); Limpiar(); CargarListado(); } else Mostrar(msg, "error");
    }

    void CargarListado()
    {
        var dt = logic.Listar_Proveedores(TXT_Buscar.Text.Trim());
        GV_Proveedores.DataSource = dt;
        GV_Proveedores.DataBind();
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        CargarListado();
    }

    protected void GV_Proveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Proveedores.PageIndex = e.NewPageIndex;
        CargarListado();
    }

    protected void GV_Proveedores_RowCommand(object source, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Editar")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GV_Proveedores.Rows[index];

                // --- Campos directos ---
                TXT_CodProveedor.Text = row.Cells[0].Text;
                TXT_Apellidos.Text = row.Cells[1].Text;
                TXT_Nombres.Text = row.Cells[2].Text;
                TXT_Telefono.Text = row.Cells[3].Text;
                TXT_Email.Text = row.Cells[4].Text;
                TXT_Direccion.Text = row.Cells[9].Text;

                // --- Empresa ---
                if (DDL_Empresa.Items.FindByText(row.Cells[10].Text) != null)
                    DDL_Empresa.SelectedValue = DDL_Empresa.Items.FindByText(row.Cells[10].Text).Value;

                // --- Estado ---
                DDL_Estado.SelectedValue = row.Cells[11].Text;

                // --- Ubigeo (DEP - PROV - DIST) ---
                string departamento = row.Cells[5].Text;
                string provincia = row.Cells[6].Text;
                string distrito = row.Cells[7].Text;

                // 1. Seleccionar Departamento
                if (DDL_Departamento.Items.FindByText(departamento) != null)
                    DDL_Departamento.SelectedValue = DDL_Departamento.Items.FindByText(departamento).Value;

                // 2. Recargar provincias
                Listar_Procincia(DDL_Departamento.SelectedValue);

                // 3. Seleccionar Provincia
                if (DDL_Provincia.Items.FindByText(provincia) != null)
                    DDL_Provincia.SelectedValue = DDL_Provincia.Items.FindByText(provincia).Value;

                // 4. Recargar distritos
                Listar_Distrito(DDL_Provincia.SelectedValue);

                // 5. Seleccionar Distrito
                if (DDL_Distrito.Items.FindByText(distrito) != null)
                    DDL_Distrito.SelectedValue = DDL_Distrito.Items.FindByText(distrito).Value;

                // --- Código Postal ---
                DDL_Distrito.SelectedValue = row.Cells[8].Text;

                // Mensaje/UI
                LBL_Mensaje.Text = "Modo edición activado";
                LBL_Mensaje.ForeColor = System.Drawing.Color.Blue;

                BTN_Guardar.Visible = false;
                BTN_Actualizar.Visible = true;

                Session["MantenimientoProveedor_CodProveedor"] = TXT_CodProveedor.Text;
                Session["MantenimientoProveedor_Tipo_Transaccion"] = "ACTUALIZAR";
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar comando: " + ex.Message, "danger");
        }
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        // Reemplazamos las comillas simples por su código HTML ('&#39;') para evitar errores de sintaxis JS
        string mensajeLimpio = mensaje.Replace("'", "&#39;");

        // El 'tipo' debe ser una cadena válida de SweetAlert2 (ej: 'success', 'error', 'warning', 'info', 'question')
        string script = $@"
        Swal.fire({{ 
            icon: '{tipo}', 
            title: 'Información', 
            text: '{mensajeLimpio}', 
            confirmButtonText: 'Aceptar' 
        }});";

        // Registra el script en la página. El 'true' final significa que se incluyen las etiquetas <script>.
        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertScript", script, true);
    }
}

