using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Entrega : System.Web.UI.Page
{
     EntregaLOGIC EntregaLOGIC = new EntregaLOGIC();

    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           // ListarTiposEntrega();
           GenerarNumeroEntrega();
            Listar_Departamento();
            TipoEntrega();

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);

            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            TXT_FecEntrega.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

            //CargarListadoEntregas();
        }
    }

    void GenerarNumeroEntrega()
    {
        try
        {
            // Verificar si la conexión está cerrada antes de abrirla
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
            }

            // Declarar y Configurar el Comando
            SqlCommand CMDNuevo = new SqlCommand();
            CMDNuevo.Connection = Global.CN;
            CMDNuevo.CommandType = CommandType.StoredProcedure;
            CMDNuevo.CommandText = "USP_Generar_NroEntrega"; // Nombre del SP creado arriba

            // Declarar el Parámetro de Salida
            SqlParameter ParamCodigo = new SqlParameter();
            ParamCodigo.ParameterName = "@NroEntrega";
            ParamCodigo.SqlDbType = SqlDbType.Char;
            ParamCodigo.Size = 12; // Mismo tamaño que en la BD
            ParamCodigo.Direction = ParameterDirection.Output; // Importante: Salida

            // Agregar Parámetro al Comando
            CMDNuevo.Parameters.Add(ParamCodigo);

            // Ejecutar (Al ser Output, usamos ExecuteNonQuery)
            CMDNuevo.ExecuteNonQuery();

            // Asignar el valor generado al TextBox de tu HTML
            this.TXT_NroEntrega.Text = ParamCodigo.Value.ToString();
        }
        catch (Exception ex)
        {
            // Manejo básico de errores (opcional: mostrar en un label)
            this.LBL_Mensaje.Text = "Error al generar código: " + ex.Message;
            this.LBL_Mensaje.CssClass = "text-danger";
        }
        finally
        {
            // Liberar recursos y cerrar conexión
            if (Global.CN.State == ConnectionState.Open)
            {
                Global.CN.Close();
            }
        }
    }


    void TipoEntrega()
    {
        try
        {
            // 1. Abrir conexión si está cerrada
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
            }

            // 2. Configurar el comando
            SqlCommand cmd = new SqlCommand("USP_Listar_TipoEntrega", Global.CN);
            cmd.CommandType = CommandType.StoredProcedure;

            // 3. Usar un DataAdapter para llenar una tabla temporal
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // 4. Limpiar el combo por si tiene basura anterior
            DDL_TipoEntrega.Items.Clear();

            // 5. Vincular los datos
            DDL_TipoEntrega.DataSource = dt;
            DDL_TipoEntrega.DataTextField = "Tipo_Entrega";      // Lo que el usuario VE (ej. Delivery Lima)
            DDL_TipoEntrega.DataValueField = "CodTipo_Entrega";  // El valor oculto (ej. TE-001)
            DDL_TipoEntrega.DataBind();

            // 6. Agregar un ítem inicial "Seleccione" (Opcional pero recomendado)
            DDL_TipoEntrega.Items.Insert(0, new ListItem("-- Seleccione Tipo --", "0"));
        }
        catch (Exception ex)
        {
            // Mostrar error si falla
            LBL_Mensaje.Text = "Error al cargar tipos: " + ex.Message;
        }
        finally
        {
            // Cerrar conexión
            if (Global.CN.State == ConnectionState.Open)
            {
                Global.CN.Close();
            }
        }
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        string mensajeLimpio = (mensaje ?? string.Empty)
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ");

        string script = $@"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensajeLimpio}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertScriptEntrega", script, true);
    }

    private void LimpiarFormulario()
    {
        TXT_NroGuia.Text = string.Empty;
        DDL_Departamento.SelectedIndex = 0;
        TXT_Direccion.Text = string.Empty;
        DDL_Estado.SelectedIndex = 0;
        TXT_Obs.Text = string.Empty;
        TXT_FecEntrega.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
       // GenerarNumeroEntrega();
    }

    void Guardar(string Tipo_Transaccion)
    {
        // 1. Validar que la conexión esté definida
        if (Global.CN.State == ConnectionState.Closed)
        {
            Global.CN.Open();
        }

        // 2. Crear Comando
        SqlCommand CMDGuardar = new SqlCommand();
        CMDGuardar.Connection = Global.CN;
        CMDGuardar.CommandType = CommandType.StoredProcedure;
        CMDGuardar.CommandText = "USP_Mantenimiento_Entregas"; // Asegúrate que este sea el nombre real en tu SQL

        // ===== DECLARACIÓN DE PARÁMETROS PARA LA TABLA DE ENTREGAS =====

        // Param: NroEntrega
        SqlParameter ParamNroEntrega = new SqlParameter();
        ParamNroEntrega.ParameterName = "@NroEntrega";
        ParamNroEntrega.SqlDbType = SqlDbType.Char;
        ParamNroEntrega.Size = 12; // Ajustar según tu BD
        ParamNroEntrega.Value = this.TXT_NroEntrega.Text;
        CMDGuardar.Parameters.Add(ParamNroEntrega);

        // Param: CodEmpleado
        SqlParameter ParamCodEmpleado = new SqlParameter();
        ParamCodEmpleado.ParameterName = "@CodEmpleado";
        ParamCodEmpleado.SqlDbType = SqlDbType.Char;
        ParamCodEmpleado.Size = 8;
        // Tomamos el valor del HiddenField, si está vacío intentamos de la sesión (lógica de respaldo)
        string codEmp = this.HFD_CodEmpleado.Value;
        if (string.IsNullOrEmpty(codEmp) && Session["MantenimientoUsuario_Empleado_CodEmpleado"] != null)
        {
            codEmp = Session["MantenimientoUsuario_Empleado_CodEmpleado"].ToString();
        }
        ParamCodEmpleado.Value = codEmp;
        CMDGuardar.Parameters.Add(ParamCodEmpleado);

        // Param: Tipo de Entrega
        SqlParameter ParamTipoEntrega = new SqlParameter();
        ParamTipoEntrega.ParameterName = "@CodTipo_Entrega";
        ParamTipoEntrega.SqlDbType = SqlDbType.Char; // o Int, depende de tu BD
        ParamTipoEntrega.Size = 5;
        ParamTipoEntrega.Value = this.DDL_TipoEntrega.SelectedValue;
        CMDGuardar.Parameters.Add(ParamTipoEntrega);

        // Param: Fecha Entrega
        SqlParameter ParamFecEntrega = new SqlParameter();
        ParamFecEntrega.ParameterName = "@Fec_Entrega";
        ParamFecEntrega.SqlDbType = SqlDbType.DateTime;
        // DateTime.Parse maneja el formato ISO del input datetime-local
        ParamFecEntrega.Value = DateTime.Parse(this.TXT_FecEntrega.Text);
        CMDGuardar.Parameters.Add(ParamFecEntrega);

        // Param: Nro Guía Remisión
        SqlParameter ParamGuia = new SqlParameter();
        ParamGuia.ParameterName = "@NroGuia_Remision";
        ParamGuia.SqlDbType = SqlDbType.VarChar;
        ParamGuia.Size = 12;
        ParamGuia.Value = this.TXT_NroGuia.Text.Trim().ToUpper();
        CMDGuardar.Parameters.Add(ParamGuia);

        // Param: Código Postal (Usando el valor del Distrito como definimos antes)
        SqlParameter ParamCPostal = new SqlParameter();
        ParamCPostal.ParameterName = "@CPostal_Entrega";
        ParamCPostal.SqlDbType = SqlDbType.VarChar;
        ParamCPostal.Size = 10;
        ParamCPostal.Value = this.DDL_Distrito.SelectedValue;
        CMDGuardar.Parameters.Add(ParamCPostal);

        // Param: Dirección
        SqlParameter ParamDireccion = new SqlParameter();
        ParamDireccion.ParameterName = "@Direccion_Entrega";
        ParamDireccion.SqlDbType = SqlDbType.VarChar;
        ParamDireccion.Size = 100;
        ParamDireccion.Value = this.TXT_Direccion.Text.Trim();
        CMDGuardar.Parameters.Add(ParamDireccion);

        // Param: Estado Entrega
        SqlParameter ParamEstado = new SqlParameter();
        ParamEstado.ParameterName = "@Estado_Entrega";
        ParamEstado.SqlDbType = SqlDbType.VarChar;
        ParamEstado.Size = 20;
        ParamEstado.Value = this.DDL_Estado.SelectedValue;
        CMDGuardar.Parameters.Add(ParamEstado);

        // Param: Observaciones
        SqlParameter ParamObs = new SqlParameter();
        ParamObs.ParameterName = "@Obs_Estado_Entrega";
        ParamObs.SqlDbType = SqlDbType.VarChar;
        ParamObs.Size = 200;
        ParamObs.Value = this.TXT_Obs.Text.Trim();
        CMDGuardar.Parameters.Add(ParamObs);

        // Param: Tipo Transacción (1 = Insertar, 2 = Actualizar, etc.)
        SqlParameter ParamTipoTransaccion = new SqlParameter();
        ParamTipoTransaccion.ParameterName = "@Tipo_Transaccion";
        ParamTipoTransaccion.SqlDbType = SqlDbType.VarChar;
        ParamTipoTransaccion.Size = 10;
        ParamTipoTransaccion.Value = Tipo_Transaccion;
        CMDGuardar.Parameters.Add(ParamTipoTransaccion);

        // Param: Mensaje (OUTPUT)
        SqlParameter ParamMensaje = new SqlParameter();
        ParamMensaje.ParameterName = "@Mensaje";
        ParamMensaje.SqlDbType = SqlDbType.VarChar;
        ParamMensaje.Size = 255;
        ParamMensaje.Direction = ParameterDirection.Output; // IMPORTANTE: Output para recibir respuesta del SP
        CMDGuardar.Parameters.Add(ParamMensaje);

        // 3. Ejecutar comando
        CMDGuardar.ExecuteNonQuery();

        // 4. Leer Mensaje de retorno
        string mensajeProcedimiento = ParamMensaje.Value.ToString();

        // Asignar el mensaje a la variable global 'mensaje' que usas en el botón
        // Nota: Como 'void' no retorna valor, asumimos que MostrarMensaje maneja la UI aquí mismo
        // o que lanzas una excepción controlada para que la capture el botón.

        // Lógica para determinar color del mensaje
        string tipoAlerta = "error";
        if (mensajeProcedimiento.ToUpper().StartsWith("OK") || mensajeProcedimiento.Contains("correctamente"))
        {
            tipoAlerta = "success";
        }
        else if (mensajeProcedimiento.ToLower().Contains("existe"))
        {
            tipoAlerta = "warning";
        }

        MostrarMensaje(mensajeProcedimiento, tipoAlerta);

        // 5. Cerrar recursos
        CMDGuardar.Dispose();
        Global.CN.Close();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        LBL_Mensaje.Text = string.Empty;
    }

    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        
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





    private void CargarListadoEntregas()
    {
        //try
        //{
        //    DataTable dt = EntregaLOGIC.Listar_Entregas(TXT_Buscar.Text.Trim());
        //    GV_Entregas.DataSource = dt;
        //    GV_Entregas.DataBind();
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al cargar listado: " + ex.Message, "error");
        //}
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        //CargarListadoEntregas();
    }

    protected void GV_Entregas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Entregas.PageIndex = e.NewPageIndex;
        //CargarListadoEntregas();
    }
}

