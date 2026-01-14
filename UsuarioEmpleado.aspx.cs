using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UsuarioEmpleado : System.Web.UI.Page
{

    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();
    UsuarioEmpleadoLOGIC UsuarioEmpleadoLOGIC = new UsuarioEmpleadoLOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        { 
            
            LimpiarFormulario();
            Listar_Empleado();
      
            CargarUsuarios();
            TXT_FecCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");


            if (Session["MantenimientoUsuario_Empleado_CodEmpleado"].ToString() != "")
            {
                Session["MantenimientoUsuario_Empleado_Tipo_Transaccion"] = "ACTUALIZAR";
                //Invocar al Método: Cargar_Datos()
               // this.CargarEmpleado(this.TXT_CodEmpleado.Text);
            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }
        }

    }

    void GuardarUsuario(string Tipo_Transaccion)
    {
        // Abrir conexión con la base de datos
        Global.CN.Open();

        // Crear comando SQL
        SqlCommand CMDGuardar = new SqlCommand();
        CMDGuardar.Connection = Global.CN;
        CMDGuardar.CommandType = CommandType.StoredProcedure;
        CMDGuardar.CommandText = "USP_Mantenimiento_Usuario_Empleado";

        // Declarar parámetros
        SqlParameter ParamCodUsuario = new SqlParameter();
        SqlParameter ParamCodEmpleado = new SqlParameter();
        SqlParameter ParamFec_Creacion = new SqlParameter();
        SqlParameter ParamPassword = new SqlParameter();
        SqlParameter ParamRol = new SqlParameter();
        SqlParameter ParamEstadoUsuario = new SqlParameter();
        SqlParameter ParamTipoTransaccion = new SqlParameter();
        SqlParameter ParamMensaje = new SqlParameter();

        // Configurar parámetros
        ParamCodUsuario.ParameterName = "@CodUsuario";
        ParamCodUsuario.SqlDbType = SqlDbType.VarChar;
        ParamCodUsuario.Size = 20;
        ParamCodUsuario.Value = this.TXT_CodUsuario.Text;


        string valorEmpleado = this.DDL_Empleado.SelectedValue;
        string codEmpleado = valorEmpleado.Split('-')[0].Trim();  // Obtiene solo el código (ejemplo: "E0001")

        ParamCodEmpleado.ParameterName = "@CodEmpleado";
        ParamCodEmpleado.SqlDbType = SqlDbType.Char;
        ParamCodEmpleado.Size = 5;
        ParamCodEmpleado.Value = codEmpleado;


        ParamFec_Creacion.ParameterName = "@Fec_Creacion";
        ParamFec_Creacion.SqlDbType = SqlDbType.DateTime;
        ParamFec_Creacion.Size = 50;
        ParamFec_Creacion.Value = DateTime.Now;

        ParamPassword.ParameterName = "@Password";
        ParamPassword.SqlDbType = SqlDbType.VarChar;
        ParamPassword.Size = 100;
        ParamPassword.Value = this.TXT_Password.Text.Trim();

        ParamRol.ParameterName = "@Perfil";
        ParamRol.SqlDbType = SqlDbType.VarChar;
        ParamRol.Size = 50;
        ParamRol.Value = this.DDL_Perfil.SelectedValue;

        ParamEstadoUsuario.ParameterName = "@Estado_Usuario";
        ParamEstadoUsuario.SqlDbType = SqlDbType.VarChar;
        ParamEstadoUsuario.Size = 20;
        ParamEstadoUsuario.Value = this.DDL_EstadoUsuario.SelectedValue;

 

        ParamTipoTransaccion.ParameterName = "@Tipo_Transaccion";
        ParamTipoTransaccion.SqlDbType = SqlDbType.VarChar;
        ParamTipoTransaccion.Size = 10;
        ParamTipoTransaccion.Value = Tipo_Transaccion;

        ParamMensaje.ParameterName = "@Mensaje";
        ParamMensaje.SqlDbType = SqlDbType.VarChar;
        ParamMensaje.Size = 255;
        ParamMensaje.Direction = ParameterDirection.Output;

        // Agregar parámetros al comando
        CMDGuardar.Parameters.Add(ParamCodUsuario);
        CMDGuardar.Parameters.Add(ParamCodEmpleado);
        CMDGuardar.Parameters.Add(ParamFec_Creacion);
        CMDGuardar.Parameters.Add(ParamPassword);
        CMDGuardar.Parameters.Add(ParamRol);
        CMDGuardar.Parameters.Add(ParamEstadoUsuario);
        CMDGuardar.Parameters.Add(ParamTipoTransaccion);
        CMDGuardar.Parameters.Add(ParamMensaje);

        // Ejecutar el comando
        CMDGuardar.ExecuteNonQuery();

        string mensajeRespuesta = ParamMensaje.Value.ToString();

        // Mostrar mensaje devuelto por el procedimiento almacenado


        if (mensajeRespuesta.ToLower().Contains("correctamente"))
        {
            MostrarMensaje(mensajeRespuesta, "success");
        }
        else
        {
            // Para cualquier otro caso (errores, validaciones), mostramos error
            MostrarMensaje(mensajeRespuesta, "error");
        }

        // Liberar recursos y cerrar conexión
        CMDGuardar.Dispose();
        Global.CN.Close();
    }


    void Listar_Empleado()
    {
        // Abrir conexión
        Global.CN.Open();

        // Crear comando
        SqlCommand cmd = new SqlCommand("USP_Listar_Empleado", Global.CN);
        cmd.CommandType = CommandType.StoredProcedure;

        // Ejecutar lector
        SqlDataReader lector = cmd.ExecuteReader();

        // Limpiar DropDownList
        DDL_Empleado.Items.Clear();
        DDL_Empleado.Items.Add("<Seleccione>");

        // Llenar DropDownList
        while (lector.Read())
        {
            DDL_Empleado.Items.Add(lector["Empleado"].ToString());
        }

        // Liberar recursos
        cmd.Dispose();
        lector.Close();

        // Cerrar conexión
        Global.CN.Close();
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

        Session["MantenimientoUsuario_Empleado_Tipo_Transaccion"] = "GUARDAR";

        MostrarMensaje("Formulario listo para nuevo usuario", "alert-info");
    }

    private void CargarUsuarios()
    {

        // El GridView detecta automáticamente las columnas del DataTable
        GV_Usuarios.DataSource = UsuarioEmpleadoLOGIC.ObtenerUsuariosParaGrid();
        GV_Usuarios.DataBind();
    }

    protected void BTN_Guardar_Click(object sender, EventArgs e)
    {
        // Validaciones servidor
        if (DDL_Empleado.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(TXT_CodUsuario.Text) || string.IsNullOrWhiteSpace(TXT_Password.Text) || DDL_Perfil.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(TXT_FecCreacion.Text) || DDL_EstadoUsuario.SelectedIndex <= 0)
        {
            MostrarMensaje("Complete todos los campos obligatorios", "alert-warning");
            return;
        }

        CargarUsuarios();
        GuardarUsuario(Session["MantenimientoUsuario_Empleado_Tipo_Transaccion"].ToString());

        MostrarMensaje("Usuario registrado exitosamente", "alert-success");
        LimpiarFormulario();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        MostrarMensaje("Operación cancelada", "alert-info");
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
    void EliminarUsuarioBD(string codUsuarioAEliminar)
    {
        try
        {
            // 1. Validar conexión
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            SqlCommand CMDEliminar = new SqlCommand();
            CMDEliminar.Connection = Global.CN;
            CMDEliminar.CommandType = CommandType.StoredProcedure;
            CMDEliminar.CommandText = "USP_Mantenimiento_Usuario_Empleado";

            // 2. Parámetro CLAVE: El código que viene de la grilla
            CMDEliminar.Parameters.AddWithValue("@CodUsuario", codUsuarioAEliminar);

            // 3. Parámetro CLAVE: La acción
            CMDEliminar.Parameters.AddWithValue("@Tipo_Transaccion", "DELETE");

            // 4. Parámetros DE RELLENO (Dummy)
            // El SP los pide obligatoriamente, pero no los usa para el DELETE.
            // Mandamos vacíos para que no de error de "Falta parámetro".
            CMDEliminar.Parameters.AddWithValue("@CodEmpleado", "");
            CMDEliminar.Parameters.AddWithValue("@Fec_Creacion", DateTime.Now);
            CMDEliminar.Parameters.AddWithValue("@Password", "");
            CMDEliminar.Parameters.AddWithValue("@Perfil", "");
            CMDEliminar.Parameters.AddWithValue("@Estado_Usuario", "");

            // 5. Parámetro de Salida (Mensaje)
            SqlParameter ParamMensaje = new SqlParameter();
            ParamMensaje.ParameterName = "@Mensaje";
            ParamMensaje.SqlDbType = SqlDbType.VarChar;
            ParamMensaje.Size = 255;
            ParamMensaje.Direction = ParameterDirection.Output;
            CMDEliminar.Parameters.Add(ParamMensaje);

            // 6. Ejecutar
            CMDEliminar.ExecuteNonQuery();

            // 7. Mostrar resultado
            string msj = ParamMensaje.Value.ToString();

            // Usamos tu estilo de alerta o el método MostrarMensaje si lo tienes
            // Response.Write("<script>alert('" + msj + "');</script>"); 
            MostrarMensaje(msj, "alert-success");

            // 8. Cerrar
            CMDEliminar.Dispose();
            Global.CN.Close();

            // 9. Recargar la grilla para ver que desapareció
            CargarUsuarios();
        }
        catch (Exception ex)
        {
            Global.CN.Close();
            MostrarMensaje("Error al eliminar: " + ex.Message, "alert-danger");
        }
    }

    protected void GV_Usuarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {// 1. Capturamos el código del usuario seleccionado
        string codUsuario = e.CommandArgument.ToString();

        // ¡IMPORTANTE!: He borrado la línea de 'ViewState["UsuariosEmpleado"]'. 
        // Ya no la necesitamos y es la que causaba el error de "Valor nulo".

        if (e.CommandName == "EliminarUsuario")
        {
            // LLAMADA AL MÉTODO DE ELIMINAR
            // Este método (que creamos en el paso anterior) ya se encarga de:
            // 1. Borrar en SQL.
            // 2. Mostrar el mensaje de éxito.
            // 3. Recargar la grilla (CargarUsuarios).
            EliminarUsuarioBD(codUsuario);
        }
        else if (e.CommandName == "EditarUsuario")
        {
            UsuarioEmpleadoLOGIC logica = new UsuarioEmpleadoLOGIC();

            // 1. CAMBIO IMPORTANTE: 
            // Llamamos al método que consulta la VISTA (V_Listar_UsuarioEmpleado).
            // Como ya modificaste la vista en SQL, ahora este DataTable trae la columna "Password".
            DataTable dt = logica.ObtenerUsuariosParaGrid();

            // 2. Buscamos la fila específica usando LINQ (en memoria)
            DataRow row = dt.AsEnumerable()
                            .FirstOrDefault(r => r["CodUsuario"].ToString().Equals(codUsuario, StringComparison.OrdinalIgnoreCase));

            if (row != null)
            {
                // 1. Llenar Cajas de Texto
                TXT_CodUsuario.Text = row["CodUsuario"].ToString();
                TXT_CodUsuario.Enabled = false;

                // 2. Llenar DropDownList de Empleado
                string codEmp = row["CodEmpleado"].ToString();
                if (DDL_Empleado.Items.FindByValue(codEmp) != null)
                {
                    DDL_Empleado.SelectedValue = codEmp;
                }

                // 3. Llenar Password 
                // ¡AHORA SÍ FUNCIONA! Porque agregaste el campo a la vista SQL.
                // Si sale error aquí, es que no ejecutaste el ALTER VIEW en SQL Server.
                if (dt.Columns.Contains("Password"))
                {
                    TXT_Password.Text = row["Password"].ToString();
                }

                // 4. Llenar los otros DropDowns
                string perfil = row["Perfil"].ToString();
                if (DDL_Perfil.Items.FindByValue(perfil) != null)
                    DDL_Perfil.SelectedValue = perfil;

                string estado = row["Estado_Usuario"].ToString();
                if (DDL_EstadoUsuario.Items.FindByValue(estado) != null)
                    DDL_EstadoUsuario.SelectedValue = estado;

                // 5. Llenar Fecha
                if (row["Fec_Creacion"] != DBNull.Value)
                {
                    // Ajusta el formato según tu control (yyyy-MM-ddTHH:mm para input type="datetime-local")
                    TXT_FecCreacion.Text = Convert.ToDateTime(row["Fec_Creacion"]).ToString("yyyy-MM-ddTHH:mm");
                }

                MostrarMensaje("Datos cargados para edición.", "alert-info");

                Session["MantenimientoUsuario_Empleado_Tipo_Transaccion"] = "ACTUALIZAR";
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
        DDL_Empleado.SelectedIndex = 0;
        TXT_CodUsuario.Text = "";
        TXT_Password.Text = "";
        DDL_Perfil.SelectedIndex = 0;
        TXT_FecCreacion.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        DDL_EstadoUsuario.SelectedIndex = 0;
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje",
         $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensaje}', confirmButtonText: 'Aceptar' }});", true);
    }
}