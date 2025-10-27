using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Cliente : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    void Listar_Nacionalidad()
    {

        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Lector de Datos
        SqlDataReader Lector;
        //Crear un Comando de Datos
        SqlCommand CMDLista_Nac = new SqlCommand();
        //Conectar Comando de Datos con la Conexión de la BD
        CMDLista_Nac.Connection = Global.CN;
        //Establecer el Tipo de Comando de Datos a Ejecutar: Procedimiento Almacenado
        CMDLista_Nac.CommandType = CommandType.StoredProcedure;
        //Establecer el Nombre del Comando de Datos
        CMDLista_Nac.CommandText = "USP_Listar_Nacionalidad";
        //Ejecutar Comando de Datos
        Lector = CMDLista_Nac.ExecuteReader();
        //Limpiar Datos del Control: DDL_Nacionalidad
        this.DDL_Nacionalidad.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL] Nacionalidad
        this.DDL_Nacionalidad.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read()) {
            //Agregar Valores al Control: DDL_Nacionalidad
            this.DDL_Nacionalidad.Items.Add(Lector.GetValue(0).ToString());
        }

        //LIberar Recursos
        CMDLista_Nac.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }

    //Crear el Método: Listar_Doc_Identidad()

   void Listar_Doc_Identidad(string Nacionalidad)
    {
        //Abrir la Conexión con la Base de Datos
        Global.CN.Open();

        //Declarar Lector de Datos
        SqlDataReader Lector;

        //Crear un Nuevo Comando de Datos
        SqlCommand CMDListar_Documento = new SqlCommand();

        //Conectar Comando de Datos con la BD
        CMDListar_Documento.Connection = Global.CN;

        //Configurar Comando de Datos
        CMDListar_Documento.CommandType = CommandType.StoredProcedure;
        CMDListar_Documento.CommandText = "USP_Listar_Documento_Identidad";

        //Agregar Parámetro a Comando de Datos
        CMDListar_Documento.Parameters.Add("@Nacionalidad", SqlDbType.VarChar, 60).Value = Nacionalidad;
        //Ejecutar Comando de Datos sobre el Lector de Datos
        Lector = CMDListar_Documento.ExecuteReader();
        //Limpiar Elementos del Control:
        this.DDL_TipoDoc.Items.Clear();

        //Agregar un Primer Elemento al Control: DDL_TipoDoc
        this.DDL_TipoDoc.Items.Add("<Seleccione>");

        //Leer Datos
        while (Lector.Read()) {
            //Agregar Valores al Control: DDL_TipoDoc
            this.DDL_TipoDoc.Items.Add(Lector.GetValue(1).ToString());
        }

        CMDListar_Documento.Dispose();

        //Cerrar la Conexión con la Base de Datos
        Global.CN.Close();
    }


    //Crear el Método: Listar_Departamento()
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

    protected void Page_Load(object sender, EventArgs e)
    {


        

        if (IsPostBack == false)
        {
            //Evitar el recordatorio de datos en el control: Webform
            this.Form.Attributes.Add("autocomplete", "off");

            //listar metodo
             Listar_Nacionalidad();

            Listar_Departamento();

            this.DDL_Distrito.Items.Add("<Seleccione>");
            this.DDL_Provincia.Items.Add("<Seleccione>");


            // Establecer fecha de registro como la fecha actual
            TXT_FecNac.Text = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");


        }
        
       
    }

    // Método para cargar la lista de clientes
    private void CargarClientes(string filtro = "")
    {
        //try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT CodCliente, Apellidos, Nombres, Telefono, Email, Estado_Cliente FROM Tb_Cliente";

        //        if (!string.IsNullOrEmpty(filtro))
        //        {
        //            query += " WHERE CodCliente LIKE @Filtro OR Apellidos LIKE @Filtro OR Nombres LIKE @Filtro OR Email LIKE @Filtro";
        //        }

        //        query += " ORDER BY Apellidos, Nombres";

        //        SqlCommand cmd = new SqlCommand(query, conn);

        //        if (!string.IsNullOrEmpty(filtro))
        //        {
        //            cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");
        //        }

        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();

        //        conn.Open();
        //        da.Fill(dt);

        //        GV_Clientes.DataSource = dt;
        //        GV_Clientes.DataBind();

        //        // Si no hay registros, mostrar mensaje
        //        if (dt.Rows.Count == 0)
        //        {
        //            MostrarMensaje("No se encontraron clientes con el filtro especificado.", "info");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al cargar clientes: " + ex.Message, "danger");
        //}
    }

    // Método para actualizar estadísticas de clientes
    private void ActualizarEstadisticas()
    {
      //  try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        // Total de clientes
        //        SqlCommand cmdTotal = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente", conn);
        //        int totalClientes = Convert.ToInt32(cmdTotal.ExecuteScalar());
        //        LBL_TotalClientes.Text = totalClientes.ToString();

        //        // Clientes activos
        //        SqlCommand cmdActivos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Estado_Cliente = 'Activo'", conn);
        //        int clientesActivos = Convert.ToInt32(cmdActivos.ExecuteScalar());
        //        LBL_ClientesActivos.Text = clientesActivos.ToString();

        //        // Clientes inactivos
        //        SqlCommand cmdInactivos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Estado_Cliente = 'Inactivo'", conn);
        //        int clientesInactivos = Convert.ToInt32(cmdInactivos.ExecuteScalar());
        //        LBL_ClientesInactivos.Text = clientesInactivos.ToString();

        //        // Clientes nuevos (último mes)
        //        SqlCommand cmdNuevos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Fec_Registro >= DATEADD(month, -1, GETDATE())", conn);
        //        int clientesNuevos = Convert.ToInt32(cmdNuevos.ExecuteScalar());
        //        LBL_ClientesNuevos.Text = clientesNuevos.ToString();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al actualizar estadísticas: " + ex.Message, "warning");
        //}
    }

    // Método para registrar un nuevo cliente
    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    // Validar datos del lado del servidor
        //    if (string.IsNullOrEmpty(TXT_CodCliente.Text) || TXT_CodCliente.Text.Length != 8)
        //    {
        //        MostrarMensaje("El código de cliente debe tener 8 caracteres.", "warning");
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(TXT_Apellidos.Text) || string.IsNullOrEmpty(TXT_Nombres.Text))
        //    {
        //        MostrarMensaje("Apellidos y nombres son obligatorios.", "warning");
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(TXT_FecNac.Text))
        //    {
        //        MostrarMensaje("La fecha de nacimiento es obligatoria.", "warning");
        //        return;
        //    }

        //    if (DDL_Sexo.SelectedValue == "")
        //    {
        //        MostrarMensaje("Debe seleccionar el sexo.", "warning");
        //        return;
        //    }

        //    // Validar que el código de cliente no exista
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmdVerificar = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE CodCliente = @CodCliente", conn);
        //        cmdVerificar.Parameters.AddWithValue("@CodCliente", TXT_CodCliente.Text);

        //        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());
        //        if (existe > 0)
        //        {
        //            MostrarMensaje("El código de cliente ya existe. Utilice otro código.", "warning");
        //            return;
        //        }

        //        // Insertar el nuevo cliente
        //        SqlCommand cmd = new SqlCommand(@"
        //            INSERT INTO Tb_Cliente (
        //                CodCliente, Apellidos, Nombres, Fec_Nac, Sexo, CodPais, 
        //                CodDoc_Identidad, NroDoc_Identidad, CPostal, Direccion, 
        //                Telefono, Email, Fec_Registro, Estado_Cliente
        //            ) VALUES (
        //                @CodCliente, @Apellidos, @Nombres, @Fec_Nac, @Sexo, @CodPais,
        //                @CodDoc_Identidad, @NroDoc_Identidad, @CPostal, @Direccion,
        //                @Telefono, @Email, @Fec_Registro, @Estado_Cliente
        //            )", conn);

        //        cmd.Parameters.AddWithValue("@CodCliente", TXT_CodCliente.Text);
        //        cmd.Parameters.AddWithValue("@Apellidos", TXT_Apellidos.Text);
        //        cmd.Parameters.AddWithValue("@Nombres", TXT_Nombres.Text);
        //        cmd.Parameters.AddWithValue("@Fec_Nac", Convert.ToDateTime(TXT_FecNac.Text));
        //        cmd.Parameters.AddWithValue("@Sexo", DDL_Sexo.SelectedValue);
        //        cmd.Parameters.AddWithValue("@CodPais", TXT_CodPais.Text);
        //        cmd.Parameters.AddWithValue("@CodDoc_Identidad", TXT_CodDocIdentidad.Text);
        //        cmd.Parameters.AddWithValue("@NroDoc_Identidad", TXT_NroDocIdentidad.Text);
        //        cmd.Parameters.AddWithValue("@CPostal", TXT_CPostal.Text);
        //        cmd.Parameters.AddWithValue("@Direccion", TXT_Direccion.Text);
        //        cmd.Parameters.AddWithValue("@Telefono", TXT_Telefono.Text);
        //        cmd.Parameters.AddWithValue("@Email", TXT_Email.Text);
        //        cmd.Parameters.AddWithValue("@Fec_Registro", DateTime.Now);
        //        cmd.Parameters.AddWithValue("@Estado_Cliente", DDL_EstadoCliente.SelectedValue);

        //        int filasAfectadas = cmd.ExecuteNonQuery();

        //        if (filasAfectadas > 0)
        //        {
        //            MostrarMensaje("Cliente registrado correctamente.", "success");
        //            LimpiarFormulario();
        //            CargarClientes();
        //            ActualizarEstadisticas();
        //        }
        //        else
        //        {
        //            MostrarMensaje("No se pudo registrar el cliente.", "danger");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al registrar cliente: " + ex.Message, "danger");
        //}
    }

    // Método para limpiar el formulario
    protected void BTN_Limpiar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        TXT_CodCliente.Text = string.Empty;
        TXT_Apellidos.Text = string.Empty;
        TXT_Nombres.Text = string.Empty;
        TXT_FecNac.Text = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");
        DDL_Sexo.SelectedIndex = 0;
        DDL_Nacionalidad.SelectedIndex = 0;
        DDL_TipoDoc.SelectedIndex = 0;
        TXT_NroDocIdentidad.Text = string.Empty;
      
        TXT_Direccion.Text = string.Empty;
        TXT_Telefono.Text = string.Empty;
        TXT_Email.Text = string.Empty;
        DDL_EstadoCliente.SelectedIndex = 0;

        // Habilitar el campo de código de cliente
        TXT_CodCliente.Enabled = true;
    }

    

    // Método para buscar un cliente por código
    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    if (string.IsNullOrEmpty(TXT_CodCliente.Text))
        //    {
        //        MostrarMensaje("Ingrese un código de cliente para buscar.", "warning");
        //        return;
        //    }

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM Tb_Cliente WHERE CodCliente = @CodCliente", conn);
        //        cmd.Parameters.AddWithValue("@CodCliente", TXT_CodCliente.Text);

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            // Cargar datos en el formulario
        //            TXT_Apellidos.Text = reader["Apellidos"].ToString();
        //            TXT_Nombres.Text = reader["Nombres"].ToString();
        //            TXT_FecNac.Text = Convert.ToDateTime(reader["Fec_Nac"]).ToString("yyyy-MM-dd");
        //            DDL_Sexo.SelectedValue = reader["Sexo"].ToString();
        //            TXT_CodPais.Text = reader["CodPais"].ToString();
        //            TXT_CodDocIdentidad.Text = reader["CodDoc_Identidad"].ToString();
        //            TXT_NroDocIdentidad.Text = reader["NroDoc_Identidad"].ToString();
        //            TXT_CPostal.Text = reader["CPostal"].ToString();
        //            TXT_Direccion.Text = reader["Direccion"].ToString();
        //            TXT_Telefono.Text = reader["Telefono"].ToString();
        //            TXT_Email.Text = reader["Email"].ToString();
        //            DDL_EstadoCliente.SelectedValue = reader["Estado_Cliente"].ToString();

        //            // Deshabilitar el campo de código de cliente
        //            TXT_CodCliente.Enabled = false;

        //            MostrarMensaje("Cliente encontrado.", "success");
        //        }
        //        else
        //        {
        //            MostrarMensaje("Cliente no encontrado.", "warning");
        //            LimpiarFormulario();
        //        }

        //        reader.Close();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al buscar cliente: " + ex.Message, "danger");
        //}
    }

    // Método para buscar en la lista de clientes
    protected void BTN_BuscarLista_Click(object sender, EventArgs e)
    {
        CargarClientes(TXT_Busqueda.Text);
    }

    // Método para exportar a CSV
    protected void BTN_Exportar_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT * FROM Tb_Cliente ORDER BY Apellidos, Nombres";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();

        //        conn.Open();
        //        da.Fill(dt);

        //        if (dt.Rows.Count > 0)
        //        {
        //            StringBuilder sb = new StringBuilder();

        //            // Encabezados
        //            for (int i = 0; i < dt.Columns.Count; i++)
        //            {
        //                sb.Append(dt.Columns[i].ColumnName);
        //                if (i < dt.Columns.Count - 1)
        //                    sb.Append(",");
        //            }
        //            sb.AppendLine();

        //            // Datos
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                for (int i = 0; i < dt.Columns.Count; i++)
        //                {
        //                    string value = row[i].ToString();
        //                    // Escapar comillas y comas
        //                    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        //                    {
        //                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
        //                    }
        //                    sb.Append(value);

        //                    if (i < dt.Columns.Count - 1)
        //                        sb.Append(",");
        //                }
        //                sb.AppendLine();
        //            }

        //            // Descargar archivo
        //            Response.Clear();
        //            Response.Buffer = true;
        //            Response.AddHeader("content-disposition", "attachment;filename=Clientes_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
        //            Response.Charset = "";
        //            Response.ContentType = "application/text";
        //            Response.Output.Write(sb.ToString());
        //            Response.Flush();
        //            Response.End();
        //        }
        //        else
        //        {
        //            MostrarMensaje("No hay datos para exportar.", "warning");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al exportar datos: " + ex.Message, "danger");
        //}
    }

    // Método para manejar comandos del GridView
    protected void GV_Clientes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Editar")
            {
                string codCliente = e.CommandArgument.ToString();
                CargarDatosEdicion(codCliente);

                // Mostrar modal de edición usando JavaScript
                ScriptManager.RegisterStartupScript(this, GetType(), "EditarModal", "$('#modalEditar').modal('show');", true);
            }
            else if (e.CommandName == "Eliminar")
            {
                string codCliente = e.CommandArgument.ToString();
                EliminarCliente(codCliente);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar comando: " + ex.Message, "danger");
        }
    }

    // Método para cargar datos en el modal de edición
    private void CargarDatosEdicion(string codCliente)
    {
        //try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM Tb_Cliente WHERE CodCliente = @CodCliente", conn);
        //        cmd.Parameters.AddWithValue("@CodCliente", codCliente);

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            HF_CodCliente.Value = codCliente;
        //            TXT_EditApellidos.Text = reader["Apellidos"].ToString();
        //            TXT_EditNombres.Text = reader["Nombres"].ToString();
        //            TXT_EditFecNac.Text = Convert.ToDateTime(reader["Fec_Nac"]).ToString("yyyy-MM-dd");
        //            DDL_EditSexo.SelectedValue = reader["Sexo"].ToString();
        //            TXT_EditTelefono.Text = reader["Telefono"].ToString();
        //            TXT_EditEmail.Text = reader["Email"].ToString();
        //            TXT_EditDireccion.Text = reader["Direccion"].ToString();
        //            DDL_EditEstadoCliente.SelectedValue = reader["Estado_Cliente"].ToString();
        //        }

        //        reader.Close();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al cargar datos para edición: " + ex.Message, "danger");
        //}
    }

    // Método para guardar cambios de edición
    protected void BTN_GuardarEdicion_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand(@"
        //            UPDATE Tb_Cliente SET 
        //                Apellidos = @Apellidos,
        //                Nombres = @Nombres,
        //                Fec_Nac = @Fec_Nac,
        //                Sexo = @Sexo,
        //                Telefono = @Telefono,
        //                Email = @Email,
        //                Direccion = @Direccion,
        //                Estado_Cliente = @Estado_Cliente
        //            WHERE CodCliente = @CodCliente", conn);

        //        cmd.Parameters.AddWithValue("@CodCliente", HF_CodCliente.Value);
        //        cmd.Parameters.AddWithValue("@Apellidos", TXT_EditApellidos.Text);
        //        cmd.Parameters.AddWithValue("@Nombres", TXT_EditNombres.Text);
        //        cmd.Parameters.AddWithValue("@Fec_Nac", Convert.ToDateTime(TXT_EditFecNac.Text));
        //        cmd.Parameters.AddWithValue("@Sexo", DDL_EditSexo.SelectedValue);
        //        cmd.Parameters.AddWithValue("@Telefono", TXT_EditTelefono.Text);
        //        cmd.Parameters.AddWithValue("@Email", TXT_EditEmail.Text);
        //        cmd.Parameters.AddWithValue("@Direccion", TXT_EditDireccion.Text);
        //        cmd.Parameters.AddWithValue("@Estado_Cliente", DDL_EditEstadoCliente.SelectedValue);

        //        int filasAfectadas = cmd.ExecuteNonQuery();

        //        if (filasAfectadas > 0)
        //        {
        //            MostrarMensaje("Cliente actualizado correctamente.", "success");
        //            CargarClientes();
        //            ActualizarEstadisticas();

        //            // Cerrar modal usando JavaScript
        //            ScriptManager.RegisterStartupScript(this, GetType(), "CerrarModal", "$('#modalEditar').modal('hide');", true);
        //        }
        //        else
        //        {
        //            MostrarMensaje("No se pudo actualizar el cliente.", "danger");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al guardar cambios: " + ex.Message, "danger");
        //}
    }

    // Método para eliminar un cliente
    private void EliminarCliente(string codCliente)
    {
        //try
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("DELETE FROM Tb_Cliente WHERE CodCliente = @CodCliente", conn);
        //        cmd.Parameters.AddWithValue("@CodCliente", codCliente);

        //        int filasAfectadas = cmd.ExecuteNonQuery();

        //        if (filasAfectadas > 0)
        //        {
        //            MostrarMensaje("Cliente eliminado correctamente.", "success");
        //            CargarClientes();
        //            ActualizarEstadisticas();
        //        }
        //        else
        //        {
        //            MostrarMensaje("No se pudo eliminar el cliente.", "danger");
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MostrarMensaje("Error al eliminar cliente: " + ex.Message, "danger");
        //}
    }

    // Método para manejar la paginación del GridView
    protected void GV_Clientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //GV_Clientes.PageIndex = e.NewPageIndex;
        //CargarClientes(TXT_Busqueda.Text);
    }

    // Método para mostrar mensajes al usuario
    private void MostrarMensaje(string mensaje, string tipo)
    {
        //ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje",
        //    $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensaje}', confirmButtonText: 'Aceptar' }});", true);
    }

    protected void DDL_Nacionalidad_SelectedIndexChanged(object sender, EventArgs e)
    {
      //invocar al método Listar_Doc_Identidad
        this.Listar_Doc_Identidad(this.DDL_Nacionalidad.Text.ToString());

        this.DDL_TipoDoc.Focus();
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

}