using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Cliente : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    void Nuevo()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Declarar y Configurar un Nuevo Comando de Datos
        SqlCommand CMDNuevo = new SqlCommand();
        CMDNuevo.Connection = Global.CN;
        CMDNuevo.CommandType = CommandType.StoredProcedure;
        CMDNuevo.CommandText = "USP_Generar_CodCliente";
        //Declarar un Nuevo Parámetro de Salida de Datos
        SqlParameter ParamCodigo = new SqlParameter();
        //Establecer Nombre, Tipo de Dato y Longitud del Parámetro
        ParamCodigo.ParameterName = "@CodCliente";
        ParamCodigo.SqlDbType = SqlDbType.Char;
        ParamCodigo.Size = 8;
        //Indicar el Tipo de Parámetro que representa: Salida de Datos (Output)
        ParamCodigo.Direction = ParameterDirection.Output;

        //Agregar Parametro al Comando de Datos
        CMDNuevo.Parameters.Add(ParamCodigo);

        //Ejecutar Comando de Datos
        CMDNuevo.ExecuteNonQuery();
        //Obtener el Código Generado
        this.TXT_CodCliente.Text = ParamCodigo.Value.ToString();
        //Liberar Recursos
        CMDNuevo.Dispose();

        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }


    void Listar_Tipo_Destinatario()
    {
        try
        {
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            SqlCommand cmd = new SqlCommand("USP_Listar_Tipo_Destinatario", Global.CN);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader lector = cmd.ExecuteReader();

            // Limpiar y cargar valor por defecto
            this.DDL_TipoDestinatario.Items.Clear();
            this.DDL_TipoDestinatario.Items.Add(new ListItem("<Seleccione Destinatario>", "0"));

            while (lector.Read())
            {
                string codigo = lector.GetValue(0).ToString(); // CodTipo_Destinatario
                string nombre = lector.GetValue(1).ToString(); // Descripcion_Destinatario

                // Value = Código (CLIE), Text = Descripción (CLIENTE)
                this.DDL_TipoDestinatario.Items.Add(new ListItem(nombre, codigo));
            }

            lector.Close();
            cmd.Dispose();
        }
        catch (Exception ex)
        {
            // Manejo de errores
            MostrarMensaje("Error de: " + ex.Message, "error");
        }
        finally
        {
            if (Global.CN.State == ConnectionState.Open) Global.CN.Close();
        }
    }
    public void Listar_Clientes()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Nuevo DataTable
        DataTable DT = new DataTable();
        //Crear un Nuevo Adaptador de Datos
        SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM V_Listar_Clientes ", Global.CN);
        //Cargar Datos del Adaptador de Datos a un DataTable
        DA.Fill(DT);
        //Cargar Datos del DataTable en el Control: GV_Clientes
        GV_Clientes.DataSource = DT;
        GV_Clientes.DataBind();

        //Liberar Recursos del Adaptador de Datos
        DA.Dispose();
        //Cerrar Conexión con la Base de Datos

        Global.CN.Close();
    }

    void Guardar(string Tipo_Transaccion)
    {
        // Bandera para saber si abrimos la conexión aquí (para cerrarla al final)
        bool conexionAbiertaLocalmente = false;

        try
        {
            // --- VALIDACIONES DE LONGITUD ---
            string tipoDoc = DDL_TipoDoc.SelectedItem.Text;
            string nroDoc = TXT_NroDocumento.Text.Trim();

            if (tipoDoc.Contains("DNI (Documento Nacional de Identidad)") && nroDoc.Length != 8)
            {
                MostrarMensaje("Error: El DNI debe tener 8 dígitos.", "warning");
                return;
            }
            if (tipoDoc.Contains("RUC (Registro Único de Contribuyente)") && nroDoc.Length != 11)
            {
                MostrarMensaje("Error: El RUC debe tener 11 dígitos.", "warning");
                return;
            }


            // 1. GESTIÓN SEGURA DE LA CONEXIÓN
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
                conexionAbiertaLocalmente = true;
            }

            // =========================================================================
            // PASO 1: GUARDAR EL CLIENTE (Tabla Principal Tb_Cliente)
            // =========================================================================

            SqlCommand CMDGuardar = new SqlCommand("USP_Mantenimiento_Cliente", Global.CN);
            CMDGuardar.CommandType = CommandType.StoredProcedure;

            // --- Parámetros Generales ---
            CMDGuardar.Parameters.AddWithValue("@CodCliente", TXT_CodCliente.Text);
            CMDGuardar.Parameters.AddWithValue("@Apellido", TXT_Apellidos.Text.ToUpper().Trim());
            CMDGuardar.Parameters.AddWithValue("@Nombre", TXT_Nombres.Text.ToUpper().Trim());

            // Validación de Fecha (Evita error si está vacío)
            if (DateTime.TryParse(TXT_FecNac.Text, out DateTime fechaNac))
                CMDGuardar.Parameters.AddWithValue("@Fec_Nac", fechaNac);
            else
                CMDGuardar.Parameters.AddWithValue("@Fec_Nac", DBNull.Value);

            CMDGuardar.Parameters.AddWithValue("@Sexo", DDL_Sexo.SelectedValue);
            // OJO: Verifica si tu SP pide Nombre o Código. Aquí envío el TEXTO según tu ejemplo anterior.
            CMDGuardar.Parameters.AddWithValue("@Nacionalidad", DDL_Nacionalidad.SelectedItem.Text);
            CMDGuardar.Parameters.AddWithValue("@Doc_Identidad", DDL_TipoDoc.SelectedItem.Text);
            CMDGuardar.Parameters.AddWithValue("@NroDoc_Identidad", TXT_NroDocumento.Text.Trim());
            CMDGuardar.Parameters.AddWithValue("@Distrito", DDL_Distrito.SelectedItem.Text);
            CMDGuardar.Parameters.AddWithValue("@Direccion", TXT_Direccion.Text.Trim());
            CMDGuardar.Parameters.AddWithValue("@Telefono", TXT_Telefono.Text.Trim());
            CMDGuardar.Parameters.AddWithValue("@Email", TXT_Email.Text.Trim());
            CMDGuardar.Parameters.AddWithValue("@FecRegistro", DateTime.Now);
            CMDGuardar.Parameters.AddWithValue("@Estado_Cliente", DDL_EstadoCliente.SelectedValue);
            CMDGuardar.Parameters.AddWithValue("@Tipo_Transaccion", Tipo_Transaccion);

            // Parámetro de Salida
            SqlParameter ParamMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 255);
            ParamMensaje.Direction = ParameterDirection.Output;
            CMDGuardar.Parameters.Add(ParamMensaje);

            // Ejecutar SP Cliente
            CMDGuardar.ExecuteNonQuery();
            string mensajeCliente = ParamMensaje.Value.ToString();

            // Si hay error en el cliente, detenemos todo aquí
            if (mensajeCliente.Contains("Error") || mensajeCliente.Contains("existe"))
            {
                MostrarMensaje(mensajeCliente, "error");
                return;
            }

            // =========================================================================
            // PASO 2: GUARDAR DATOS DE EMPRESA / RAZÓN SOCIAL (Solo si es RUC)
            // =========================================================================

            string tipoDocumento = DDL_TipoDoc.SelectedItem.Text.ToUpper();

            // Verificamos si es RUC y si NO estamos eliminando
            if (tipoDocumento.Contains("RUC (Registro Único de Contribuyente)") && Tipo_Transaccion != "DELETE")
            {
                // Validación Extra: Que haya seleccionado el Tipo de Empresa (SAC, EIRL, etc.)
                if (DDL_TipoDestinatario.SelectedValue == "0" || string.IsNullOrEmpty(DDL_TipoDestinatario.SelectedValue))
                {
                    MostrarMensaje("Atención: Cliente guardado, pero falta seleccionar el Tipo de Empresa.", "warning");
                    return;
                }

                SqlCommand CMDRazon = new SqlCommand("USP_Mantenimiento_Razon_Social", Global.CN);
                CMDRazon.CommandType = CommandType.StoredProcedure;

                // Parámetros para la tabla Tb_Razon_Social
                CMDRazon.Parameters.AddWithValue("@CodCliente", TXT_CodCliente.Text);
                CMDRazon.Parameters.AddWithValue("@Razon_Social", TXT_RazonSocial.Text.ToUpper().Trim());

                // Usamos el mismo número de documento principal como RUC
                CMDRazon.Parameters.AddWithValue("@Ruc", TXT_NroDocumento.Text.Trim());

                // 🚀 AQUÍ ESTÁ EL CAMBIO CLAVE: Enviamos el valor seleccionado (SAC, SRL, JUR)
                CMDRazon.Parameters.AddWithValue("@CodTipo_Persona", DDL_TipoDestinatario.SelectedValue);

                CMDRazon.Parameters.AddWithValue("@Estado_Ruc", DDL_EstadoRuc.SelectedValue);
                CMDRazon.Parameters.AddWithValue("@Tipo_Transaccion", Tipo_Transaccion); // GUARDAR o ACTUALIZAR

                SqlParameter ParamMensajeRS = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                ParamMensajeRS.Direction = ParameterDirection.Output;
                CMDRazon.Parameters.Add(ParamMensajeRS);

                // Ejecutar SP Razón Social
                CMDRazon.ExecuteNonQuery();
                string mensajeRS = ParamMensajeRS.Value.ToString();

                // Verificar errores específicos de la empresa
                if (mensajeRS.Contains("Error"))
                {
                    MostrarMensaje("Cliente base guardado, pero hubo error en datos de empresa: " + mensajeRS, "warning");
                    return;
                }
            }

            // Si llegamos aquí, todo fue ÉXITO
            MostrarMensaje(mensajeCliente, "success"); // O usa un mensaje genérico "Proceso completado"

            // Limpiar formulario si es nuevo registro
            if (Tipo_Transaccion == "GUARDAR")
            {
                // LimpiarControles(); // Si tienes un método para limpiar
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error crítico: " + ex.Message, "error");
        }
        finally
        {
            // Cerrar conexión solo si este método la abrió
            if (conexionAbiertaLocalmente && Global.CN.State == ConnectionState.Open)
            {
                Global.CN.Close();
            }
        }
    }




    void Listar_Nacionalidad()
    {

        NacionalidadLOGIC service = new NacionalidadLOGIC();
        var lista = service.ObtenerNacionalidades();

        DDL_Nacionalidad.Items.Clear();
        DDL_Nacionalidad.Items.Add("<Seleccione>");

        foreach (string nac in lista)
        {
            DDL_Nacionalidad.Items.Add(nac);
        }
    }

    //Crear el Método: Listar_Doc_Identidad()

   void Listar_Doc_Identidad(string Nacionalidad)
    {
        DocumentoIdentidadLOGIC service = new DocumentoIdentidadLOGIC();
        var lista = service.ObtenerTiposDocumento(Nacionalidad);

        DDL_TipoDoc.Items.Clear();
        DDL_TipoDoc.Items.Add("<Seleccione>");

        foreach (var item in lista)
        {
            DDL_TipoDoc.Items.Add(item);
        }
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
            LimpiarFormulario();
            Listar_Nacionalidad();
            ActualizarEstadisticas();
            Listar_Departamento();
            Listar_Clientes();
            Listar_Tipo_Destinatario();

            this.DDL_Distrito.Items.Add("-Seleccione-");
            this.DDL_Provincia.Items.Add("-Seleccione-");

            
            TXT_FecNac.Text = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");

            //Evaluar si la Variable de tipo Session "MantenimientoEmpleado_CodEmpleado" es diferente de vacio
            if (Session["MantenimientoCliente_CodCliente"].ToString() != "")
            {

            //    cargarDatos();
               Session["MantenimientoCliente_Tipo_Transaccion"] = "ACTUALIZAR";
              
            }
            else
            {
              
                //    Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }
        }
        
       
    }

 

    // Método para actualizar estadísticas de clientes
    private void ActualizarEstadisticas()
    {
        try
        {
            //Abrir Conexión con la Base de Datos
            Global.CN.Open();
           

            // Total de clientes
            SqlCommand cmdTotal = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente", Global.CN);
                int totalClientes = Convert.ToInt32(cmdTotal.ExecuteScalar());
                LBL_TotalClientes.Text = totalClientes.ToString();

                // Clientes activos
                SqlCommand cmdActivos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Estado_Cliente = 'Activo'", Global.CN);
                int clientesActivos = Convert.ToInt32(cmdActivos.ExecuteScalar());
                LBL_ClientesActivos.Text = clientesActivos.ToString();

                // Clientes inactivos
                SqlCommand cmdInactivos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Estado_Cliente = 'Inactivo'", Global.CN);
                int clientesInactivos = Convert.ToInt32(cmdInactivos.ExecuteScalar());
                LBL_ClientesInactivos.Text = clientesInactivos.ToString();

                // Clientes nuevos (último mes)
                SqlCommand cmdNuevos = new SqlCommand("SELECT COUNT(*) FROM Tb_Cliente WHERE Fec_Registro >= DATEADD(month, -1, GETDATE())", Global.CN);
                int clientesNuevos = Convert.ToInt32(cmdNuevos.ExecuteScalar());
                LBL_ClientesNuevos.Text = clientesNuevos.ToString();

            Global.CN.Close();
            
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al actualizar estadísticas: " + ex.Message, "warning");
        }
    }

    // Método para registrar un nuevo cliente
    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        try
        {
            // Validar datos del lado del servidor
            if (string.IsNullOrEmpty(TXT_CodCliente.Text) || TXT_CodCliente.Text.Length != 8)
            {
                MostrarMensaje("El código de cliente debe tener 8 caracteres.", "warning");
                return;
            }

            if (string.IsNullOrEmpty(TXT_Apellidos.Text) || string.IsNullOrEmpty(TXT_Nombres.Text))
            {
                MostrarMensaje("Apellidos y nombres son obligatorios.", "warning");
                return;
            }

            if (string.IsNullOrEmpty(TXT_FecNac.Text))
            {
                MostrarMensaje("La fecha de nacimiento es obligatoria.", "warning");
                return;
            }

            if (DDL_Sexo.SelectedValue == "")
            {
                MostrarMensaje("Debe seleccionar el sexo.", "warning");
                return;
            }

            //Invocar al Método: Guardar()
            this.Guardar(Session["MantenimientoCliente_Tipo_Transaccion"].ToString());

            Session["MantenimientoCliente_CodCliente"] = "";

            Listar_Clientes();
            ActualizarEstadisticas();
            LimpiarFormulario();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            Nuevo();
 
            BTN_Registrar.Text = "Guardar";

        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al registrar cliente: " + ex.Message, "danger");
        }
    }


    private void LimpiarFormulario()
    {
       

        TXT_Apellidos.Text = string.Empty;
        TXT_Nombres.Text = string.Empty;
        TXT_FecNac.Text = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd");
        DDL_Sexo.SelectedIndex = 0;
        DDL_Nacionalidad.SelectedIndex = 0;
   
        TXT_NroDocumento.Text = string.Empty;
        DDL_Departamento.SelectedIndex = 0;
        DDL_Provincia.SelectedIndex = 0;
        DDL_Distrito.SelectedIndex = 0;
        TXT_Direccion.Text = string.Empty;
        TXT_Telefono.Text = string.Empty;
        TXT_Email.Text = string.Empty;
        DDL_EstadoCliente.SelectedIndex = 0;

        // Habilitar el campo de código de cliente
        TXT_CodCliente.Enabled = true;

        Panel_DatosEmpresa.Visible = false;

        // Limpiar las cajas de texto de empresa para que no quede basura
        TXT_RazonSocial.Text = string.Empty;

        if (DDL_TipoDestinatario.Items.Count > 0) DDL_TipoDestinatario.SelectedIndex = 0;
        if (DDL_EstadoRuc.Items.Count > 0) DDL_EstadoRuc.SelectedIndex = 0;

        // Resetear las variables de Sesión para evitar conflictos al Editar/Guardar
        Session["ActivarPanelEmpresa"] = "NO";
        Session["MantenimientoCliente_RazonSocial"] = "";
        Session["MantenimientoCliente_TipoPersona"] = "NAT";
    }

    

    // Método para buscar un cliente por código
    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
       
            string filtro = TXT_Busqueda.Text.Trim();

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM V_Listar_Clientes WHERE Apellido LIKE @filtro OR CodCliente LIKE @filtro", Global.CN);
            da.SelectCommand.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
            da.Fill(dt);

            GV_Clientes.DataSource = dt;
            GV_Clientes.DataBind();
   
    }

   
   
  


    protected void GV_Clientes_RowCommand(object source, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Editar")
            {
                //obtener el indice del comandos seleccionado
                int indice = Convert.ToInt32(e.CommandArgument);



                Session["MantenimientoCliente_CodCliente"] = GV_Clientes.DataKeys[indice].Values["CodCliente"].ToString();
               
                Session["MantenimientoCliente_Apellido"] = GV_Clientes.DataKeys[indice].Values["Apellido"].ToString();
                Session["MantenimientoCliente_Nombre"] = GV_Clientes.DataKeys[indice].Values["Nombre"].ToString();
                Session["MantenimientoCliente_Fec_Nac"] = GV_Clientes.DataKeys[indice].Values["Fec_Nac"].ToString();
                Session["MantenimientoCliente_Sexo"] = GV_Clientes.DataKeys[indice].Values["Sexo"].ToString();
                Session["MantenimientoCliente_Nacionalidad"] = GV_Clientes.DataKeys[indice].Values["Nacionalidad"].ToString();

                // ✅ USAR EL CAMPO REAL DE LA VIEW
                Session["MantenimientoCliente_Documento_Identidad"] = GV_Clientes.DataKeys[indice].Values["Documento_Identidad"].ToString();

                Session["MantenimientoCliente_NroDoc_Identidad"] = GV_Clientes.DataKeys[indice].Values["NroDoc_Identidad"].ToString();
                Session["MantenimientoCliente_Departamento"] = GV_Clientes.DataKeys[indice].Values["Departamento"].ToString();
                Session["MantenimientoCliente_Provincia"] = GV_Clientes.DataKeys[indice].Values["Provincia"].ToString();
                Session["MantenimientoCliente_Distrito"] = GV_Clientes.DataKeys[indice].Values["Distrito"].ToString();
                Session["MantenimientoCliente_Direccion"] = GV_Clientes.DataKeys[indice].Values["Direccion"].ToString();
                Session["MantenimientoCliente_Telefono"] = GV_Clientes.DataKeys[indice].Values["Telefono"].ToString();
                Session["MantenimientoCliente_Email"] = GV_Clientes.DataKeys[indice].Values["Email"].ToString();
                Session["MantenimientoCliente_Estado_Cliente"] = GV_Clientes.DataKeys[indice].Values["Estado_Cliente"].ToString();

                string documento = GV_Clientes.DataKeys[indice].Values["Documento_Identidad"].ToString();
                string codCliente = e.CommandArgument.ToString();

                if (documento.Contains("RUC (Registro Único de Contribuyente)"))
                {
                    // Llamamos al método para llenar los datos de empresa
                    ObtenerDatosEmpresa(codCliente);

                    // Indicamos al formulario que muestre el panel de empresa
                    Session["ActivarPanelEmpresa"] = "SI";
                }
                else
                {
                    Session["ActivarPanelEmpresa"] = "NO";
                }

                // 5. CARGAR EN PANTALLA
                cargarDatos();
                Session["MantenimientoCliente_Tipo_Transaccion"] = "ACTUALIZAR";
                BTN_Registrar.Text = "Actualizar";

                cargarDatos();

              
            }

           
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar comando: " + ex.Message, "danger");
        }
    }


    private void ObtenerDatosEmpresa(string codCliente)
    {
        try
        {
            // 1. Abrir conexión si está cerrada
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            // 2. Consulta SQL Directa (Sin trucos, solo los datos crudos)
            string query = "SELECT Razon_Social, CodTipo_Persona, Estado_Ruc FROM Tb_Razon_Social WHERE CodCliente = @CodCliente";

            SqlCommand cmd = new SqlCommand(query, Global.CN);
            cmd.Parameters.AddWithValue("@CodCliente", codCliente);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // 3. Guardar en Sesión TAL CUAL vienen de la BD
                Session["MantenimientoCliente_RazonSocial"] = dr["Razon_Social"].ToString();
                Session["MantenimientoCliente_TipoPersona"] = dr["CodTipo_Persona"].ToString(); // Ej: JUR
                Session["MantenimientoCliente_EstadoRuc"] = dr["Estado_Ruc"].ToString();       // Ej: ACTIVO
            }
            else
            {
                // Si no encuentra nada (por seguridad), limpiamos variables
                Session["MantenimientoCliente_RazonSocial"] = "";
                Session["MantenimientoCliente_TipoPersonaJur"] = "";
                Session["MantenimientoCliente_EstadoRuc"] = "";
            }

            dr.Close();
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al obtener datos de empresa: " + ex.Message, "error");
        }
        // Cierra la conexión si es necesario en tu arquitectura
        if (Global.CN.State == ConnectionState.Open) Global.CN.Close();
    }


    public void cargarDatos() 
    {

        TXT_CodCliente.Text = Session["MantenimientoCliente_CodCliente"].ToString();
        
        if (Session["MantenimientoCliente_Fec_Nac"] != null)
        {
            DateTime fecha = DateTime.Parse(Session["MantenimientoCliente_Fec_Nac"].ToString());
            TXT_FecNac.Text = fecha.ToString("yyyy-MM-dd");
        }
        TXT_Apellidos.Text = Session["MantenimientoCliente_Apellido"].ToString();
        TXT_Nombres.Text = Session["MantenimientoCliente_Nombre"].ToString();
        DDL_Sexo.SelectedValue = Session["MantenimientoCliente_Sexo"].ToString();
        // Combos dependientes → primero cargas listas
        Listar_Nacionalidad();
        DDL_Nacionalidad.SelectedValue = Session["MantenimientoCliente_Nacionalidad"].ToString();
        Listar_Doc_Identidad(DDL_Nacionalidad.SelectedValue);

        DDL_TipoDoc.SelectedValue = Session["MantenimientoCliente_Documento_Identidad"].ToString();
        TXT_NroDocumento.Text = Session["MantenimientoCliente_NroDoc_Identidad"].ToString();
        // Ubigeo (departamento → provincia → distrito)
        Listar_Departamento();
        DDL_Departamento.SelectedValue = Session["MantenimientoCliente_Departamento"].ToString();

        Listar_Procincia(DDL_Departamento.SelectedValue);
        DDL_Provincia.SelectedValue = Session["MantenimientoCliente_Provincia"].ToString();

        Listar_Distrito(DDL_Provincia.SelectedValue);
        DDL_Distrito.SelectedValue = Session["MantenimientoCliente_Distrito"].ToString();

        TXT_Direccion.Text = Session["MantenimientoCliente_Direccion"].ToString();
        TXT_Telefono.Text = Session["MantenimientoCliente_Telefono"].ToString();
        TXT_Email.Text = Session["MantenimientoCliente_Email"].ToString();

        DDL_EstadoCliente.SelectedValue = Session["MantenimientoCliente_Estado_Cliente"].ToString();

        // Si detectamos que es empresa (por la variable de sesión que creamos antes)
        if (Session["ActivarPanelEmpresa"] != null && Session["ActivarPanelEmpresa"].ToString() == "SI")
        {
            Panel_DatosEmpresa.Visible = true;

            // Cargar Razón Social
            if (Session["MantenimientoCliente_RazonSocial"] != null)
                TXT_RazonSocial.Text = Session["MantenimientoCliente_RazonSocial"].ToString();

            // Cargar Tipo de Persona (El cambio solicitado)
            if (Session["MantenimientoCliente_TipoPersona"] != null)
            {
                string tipoJur = Session["MantenimientoCliente_TipoPersona"].ToString();
                // Verificamos si existe en el combo antes de asignarlo para evitar errores
                if (DDL_TipoDestinatario.Items.FindByValue(tipoJur) != null)
                    DDL_TipoDestinatario.SelectedValue = tipoJur;
            }

            // Cargar Estado RUC
            if (Session["MantenimientoCliente_EstadoRuc"] != null)
                DDL_EstadoRuc.SelectedValue = Session["MantenimientoCliente_EstadoRuc"].ToString();
        }
        else
        {
            Panel_DatosEmpresa.Visible = false;
            // Resetear combos
            DDL_TipoDestinatario.SelectedIndex = 0;
        }


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
        GV_Clientes.PageIndex = e.NewPageIndex;
        Listar_Clientes();
    }

    // Método para mostrar mensajes al usuario
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


    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();

        Nuevo();

        //Asignar Valor a la variable:
        Session["MantenimientoCliente_Tipo_Transaccion"] = "GUARDAR";
    }


    protected void DDL_TipoDoc_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Obtenemos el texto seleccionado (ej: "DNI", "RUC", "PASAPORTE")
        string seleccion = DDL_TipoDoc.SelectedItem.Text;

        // Verificamos si es RUC (puedes ajustar la condición según tus valores exactos)
        // También podrías usar: if (DDL_TipoDoc.SelectedValue == "6") si usas códigos de SUNAT
        if (seleccion.Contains("RUC (Registro Único de Contribuyente)"))
        {
            Panel_DatosEmpresa.Visible = true;

            // Opcional: Limpiar campos al mostrar
            TXT_RazonSocial.Focus();
        }
        else
        {
            Panel_DatosEmpresa.Visible = false;

            // Opcional: Limpiar valores para no guardar basura
            TXT_RazonSocial.Text = string.Empty;
            DDL_TipoDestinatario.SelectedIndex = -1;
        }
    }


    protected void TXT_NombreApellido_TextChanged(object sender, EventArgs e)
    {

    
        // Si el panel de empresa está visible, sugerimos la Razón Social
        if (Panel_DatosEmpresa.Visible)
        {
            string nombre = TXT_Nombres.Text.Trim();
            string apellido = TXT_Apellidos.Text.Trim();

            // Solo llenamos si la caja Razón Social está vacía para no borrar lo que el usuario haya escrito
            if (string.IsNullOrEmpty(TXT_RazonSocial.Text))
            {
                TXT_RazonSocial.Text = (nombre + " " + apellido).ToUpper();
            }
        }
    }
}