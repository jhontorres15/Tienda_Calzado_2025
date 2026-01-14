using Microsoft.Ajax.Utilities;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Guia_Remision : System.Web.UI.Page
{
   
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Listar_Departamento_Partida();
            Listar_Departamento_Destino();
            Listar_Tipo_Traslado();
            Listar_Tipo_Destinatario();
            CargarGuias();
            Listar_Doc_Identidad_destinatario();
            Listar_Doc_Identidad_Conductor();
           
            this.TXT_FecInicio.Text = DateTime.Now.ToString("yyyy-MM-dd");

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);

            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField 
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            if (Session["MantenimientoGuiaRemision_NroGuia"] != null &&
            Session["MantenimientoGuiaRemision_NroGuia"].ToString() != "")
            {
                Session["MantenimientoGuiaRemision_Tipo_Transaccion"] = "ACTUALIZAR";
                
            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }
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


    void Listar_Tipo_Traslado()
    {
        try
        {
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            SqlCommand cmd = new SqlCommand("USP_Listar_Tipo_Traslado", Global.CN);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataReader lector = cmd.ExecuteReader();

            // Limpiar y cargar valor por defecto
            this.DDL_TipoTraslado.Items.Clear();
            this.DDL_TipoTraslado.Items.Add(new ListItem("<Seleccione Motivo>", "0"));

            while (lector.Read())
            {
                string codigo = lector.GetValue(0).ToString(); // CodTipo_Traslado
                string nombre = lector.GetValue(1).ToString(); // Motivo_Traslado

                // Value = Código (01), Text = Descripción (VENTA)
                this.DDL_TipoTraslado.Items.Add(new ListItem(nombre, codigo));
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

    // Método para cargar el Combo de Tipos de Destinatario
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


    
  

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        LBL_Mensaje.Text = "";
        Generar_Numero_Guia();
    }

    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        
            // 1. Recolección de Datos
            var nroGuia = TXT_NroGuia.Text.Trim();
            var empleado = HFD_CodEmpleado.Value.Trim();
            var serie = TXT_NroSerie.Text.Trim();

            DateTime fecInicio = DateTime.TryParse(TXT_FecInicio.Text, out var fi) ? fi : DateTime.Now.Date;

            var cpPartida = DDL_Distrito.Text.Trim();
            var dirPartida = TXT_DireccionPartida.Text.Trim();
            var cpLlegada = DDL_Distrito_Destino.Text.Trim();
            var dirLlegada = TXT_DireccionLlegada.Text.Trim();

            var tipoDest = DDL_TipoDestinatario.SelectedValue;
            var destinatario = TXT_Destinatario.Text.Trim();
            var docDest = DDL_DocDestinatario.SelectedValue;
            var nroDocIdent = TXT_NroDocIdentidad.Text.Trim();

            var tipoTraslado = DDL_TipoTraslado.SelectedValue;
            var obsTraslado = TXT_ObsTraslado.Text.Trim();
            decimal costo = decimal.TryParse(TXT_CostoTraslado.Text, out var c) ? c : 0m;

            var razonTransp = TXT_RazonTransportista.Text.Trim();
            var rucTransp = TXT_RucTransportista.Text.Trim();
            var conductor = TXT_Conductor.Text.Trim();
            var docConductor = DDL_DocConductor.SelectedValue;
            var nroDocConductor = TXT_NroDocConductor.Text.Trim();
            var placa = TXT_NroPlaca.Text.Trim();
           
            var obsGuia = TXT_ObsGuia.Text.Trim();

            // --- BLOQUE DE VALIDACIONES ---
            if (string.IsNullOrEmpty(nroGuia)) { MostrarMensaje("El Número de Guía es obligatorio.", "warning"); return; }
            if (string.IsNullOrEmpty(serie)) { MostrarMensaje("El Número de Serie es obligatorio.", "warning"); return; }

            // Validaciones de Partida y Llegada
            if (string.IsNullOrEmpty(dirPartida)) { MostrarMensaje("La Dirección de Partida es obligatoria.", "warning"); return; }
            if (cpPartida == "0" || string.IsNullOrEmpty(cpPartida)) { MostrarMensaje("Seleccione el Distrito de Partida.", "warning"); return; }

            if (string.IsNullOrEmpty(dirLlegada)) { MostrarMensaje("La Dirección de Llegada es obligatoria.", "warning"); return; }
            if (cpLlegada == "0" || string.IsNullOrEmpty(cpLlegada)) { MostrarMensaje("Seleccione el Distrito de Llegada.", "warning"); return; }

            // Validaciones de Destinatario
            if (string.IsNullOrEmpty(destinatario)) { MostrarMensaje("El nombre del Destinatario es obligatorio.", "warning"); return; }
            if (string.IsNullOrEmpty(nroDocIdent)) { MostrarMensaje("El Documento del Destinatario es obligatorio.", "warning"); return; }
            if (tipoDest == "0") { MostrarMensaje("Seleccione el Tipo de Destinatario.", "warning"); return; }

            // Validaciones de Traslado
            if (tipoTraslado == "0") { MostrarMensaje("Seleccione el Motivo de Traslado.", "warning"); return; }

            // Validaciones de Transporte (Conductor y Vehículo)
            if (string.IsNullOrEmpty(conductor)) { MostrarMensaje("El nombre del Conductor es obligatorio.", "warning"); return; }
            if (string.IsNullOrEmpty(nroDocConductor)) { MostrarMensaje("El Documento del Conductor es obligatorio.", "warning"); return; }
            if (string.IsNullOrEmpty(placa)) { MostrarMensaje("La Placa del Vehículo es obligatoria.", "warning"); return; }

            // Validaciones de Empresa de Transporte (Si aplica, a veces es transporte propio)
            // Puedes comentar estas si permites transporte propio sin RUC
            if (string.IsNullOrEmpty(razonTransp)) { MostrarMensaje("La Razón Social del Transportista es obligatoria.", "warning"); return; }
            if (string.IsNullOrEmpty(rucTransp)) { MostrarMensaje("El RUC del Transportista es obligatorio.", "warning"); return; }

            // ------------------------------
            try
            {
                // 1. Usar la conexión Global
                if (Global.CN.State == ConnectionState.Closed)
                {
                    Global.CN.Open();
                }

                // 2. Crear el comando usando la conexión Global existente
                // IMPORTANTE: El nombre del SP debe ser el correcto (USP_Registrar_Guia_Remision)
                using (SqlCommand cmd = new SqlCommand("USP_Registrar_Guia_Remision", Global.CN))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 3. Agregar parámetros (Deben coincidir EXACTAMENTE con el SQL)

                    // @NroGuia_Remision
                    cmd.Parameters.AddWithValue("@NroGuia_Remision", nroGuia);

                    // @CodEmpleado (Asegurar longitud máxima de 5)
                    cmd.Parameters.AddWithValue("@CodEmpleado", empleado.Length >= 5 ? empleado.Substring(0, 5) : empleado);

                    // @NroSerie_Facturacion
                    cmd.Parameters.AddWithValue("@NroSerie_Facturacion", serie);

                    // @Fec_Inicio_Traslado
                    cmd.Parameters.AddWithValue("@Fec_Inicio_Traslado", fecInicio);

                    // @Postal_Partida (Nota: En el SQL lo llamaste Postal_Partida, no CPostal_Partida)
                    cmd.Parameters.AddWithValue("@Postal_Partida", cpPartida);

                    // @Direccion_Partida
                    cmd.Parameters.AddWithValue("@Direccion_Partida", dirPartida);

                    // @Postal_Llegada (Igual aquí, el nombre en SQL es Postal_Llegada)
                    cmd.Parameters.AddWithValue("@Postal_Llegada", cpLlegada);

                    // @Direccion_Llegada
                    cmd.Parameters.AddWithValue("@Direccion_Llegada", dirLlegada);

                    // @CodTipo_Destinatario
                    cmd.Parameters.AddWithValue("@CodTipo_Destinatario", tipoDest);

                    // @Destinatario
                    cmd.Parameters.AddWithValue("@Destinatario", destinatario);

                    // @CodDoc_Destinatario
                    cmd.Parameters.AddWithValue("@CodDoc_Destinatario", docDest);

                    // @NroDoc_Identidad
                    cmd.Parameters.AddWithValue("@NroDoc_Identidad", nroDocIdent);

                    // @CodTipo_Traslado
                    cmd.Parameters.AddWithValue("@CodTipo_Traslado", tipoTraslado);

                    // @Observacion_Traslado
                    cmd.Parameters.AddWithValue("@Observacion_Traslado", obsTraslado);

                    // @Costo_Traslado
                    cmd.Parameters.AddWithValue("@Costo_Traslado", costo);

                    // @RazonSocial_Transportista
                    cmd.Parameters.AddWithValue("@RazonSocial_Transportista", razonTransp);

                    // @Ruc_Trasnportista (Manteniendo el error de tipeo del SQL)
                    cmd.Parameters.AddWithValue("@Ruc_Trasnportista", rucTransp);

                    // @Conductor
                    cmd.Parameters.AddWithValue("@Conductor", conductor);

                    // @CodDoc_Conductor
                    cmd.Parameters.AddWithValue("@CodDoc_Conductor", docConductor);

                    // @NroDoc_Conductor
                    cmd.Parameters.AddWithValue("@NroDoc_Conductor", nroDocConductor);

                    // @NroPlaca_Vehiculo
                    cmd.Parameters.AddWithValue("@NroPlaca_Vehiculo", placa);

                    // @Observacion_GuiaRemision
                    cmd.Parameters.AddWithValue("@Observacion_GuiaRemision", obsGuia);

                    // NOTA: Ya NO enviamos @Estado_GuiaRemision porque el SQL lo pone automático

                    // 4. Ejecutar
                    cmd.ExecuteNonQuery();
                }

                // 5. Respuesta Exitosa
                MostrarMensaje("Guía de Remisión registrada correctamente.", "success");
                LimpiarFormulario();
            CargarGuias();
            }
            catch (SqlException sqlEx)
            {
                // Errores de SQL (ej. duplicados)
                MostrarMensaje("Error de Base de Datos: " + sqlEx.Message, "error");
            }
            catch (Exception ex)
            {
                // Errores generales
                MostrarMensaje("Ocurrió un error inesperado: " + ex.Message, "error");
            }
        }

    void Listar_Doc_Identidad_Conductor()
    {
        try
        {
            // Abrir la Conexión con la Base de Datos si está cerrada
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            // Crear y configurar el comando
            SqlCommand CMDListar_Documento = new SqlCommand();
            CMDListar_Documento.Connection = Global.CN;
            CMDListar_Documento.CommandType = CommandType.StoredProcedure;

            // ACTUALIZADO: Usar el nombre exacto del SP sin parámetros
            CMDListar_Documento.CommandText = "USP_Listar_Documento_Identidad_Guia_Remision";

            // NOTA: Se eliminó la línea de .Parameters.Add porque el SP ya no filtra por Nacionalidad

            // Ejecutar lector
            SqlDataReader Lector = CMDListar_Documento.ExecuteReader();

            // Limpiar control
            this.DDL_DocConductor.Items.Clear();

            // Agregar ítem por defecto. Usamos ListItem para manejar Texto y Valor.
            // Texto: <Seleccione>, Valor: 0
            this.DDL_DocConductor.Items.Add(new ListItem("<Seleccione>", "0"));

            // Leer Datos
            while (Lector.Read())
            {
                // Recuperamos los datos del SP (Columna 0: Codigo, Columna 1: Descripcion)
                string codigo = Lector.GetValue(0).ToString();
                string nombre = Lector.GetValue(1).ToString();

                // Agregamos un ListItem: El usuario ve el 'nombre', pero el sistema usa el 'codigo'
                this.DDL_DocConductor.Items.Add(new ListItem(nombre, codigo));
            }

            // Cerrar lector y liberar comando
            Lector.Close();
            CMDListar_Documento.Dispose();
        }
        catch (Exception ex)
        {
           
            MostrarMensaje("Error de: " + ex.Message, "error");
        }
        finally
        {
            // SIEMPRE cerrar la conexión en el finally
            if (Global.CN.State == ConnectionState.Open) Global.CN.Close();
        }
    }


    void Listar_Doc_Identidad_destinatario()
    {
        try
        {
            // Abrir la Conexión con la Base de Datos si está cerrada
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            // Crear y configurar el comando
            SqlCommand CMDListar_Documento = new SqlCommand();
            CMDListar_Documento.Connection = Global.CN;
            CMDListar_Documento.CommandType = CommandType.StoredProcedure;

            // ACTUALIZADO: Usar el nombre exacto del SP sin parámetros
            CMDListar_Documento.CommandText = "USP_Listar_Documento_Identidad_Guia_Remision";

            // NOTA: Se eliminó la línea de .Parameters.Add porque el SP ya no filtra por Nacionalidad

            // Ejecutar lector
            SqlDataReader Lector = CMDListar_Documento.ExecuteReader();

            // Limpiar control
            this.DDL_DocDestinatario.Items.Clear();

            // Agregar ítem por defecto. Usamos ListItem para manejar Texto y Valor.
            // Texto: <Seleccione>, Valor: 0
            this.DDL_DocDestinatario.Items.Add(new ListItem("<Seleccione>", "0"));

            // Leer Datos
            while (Lector.Read())
            {
                // Recuperamos los datos del SP (Columna 0: Codigo, Columna 1: Descripcion)
                string codigo = Lector.GetValue(0).ToString();
                string nombre = Lector.GetValue(1).ToString();

                // Agregamos un ListItem: El usuario ve el 'nombre', pero el sistema usa el 'codigo'
                this.DDL_DocDestinatario.Items.Add(new ListItem(nombre, codigo));
            }

            // Cerrar lector y liberar comando
            Lector.Close();
            CMDListar_Documento.Dispose();
        }
        catch (Exception ex)
        {
            // Manejo básico de errores (opcional: mostrar en un label o log)
            MostrarMensaje("Error de: " + ex.Message, "error");
        }
        finally
        {
            // SIEMPRE cerrar la conexión en el finally
            if (Global.CN.State == ConnectionState.Open) Global.CN.Close();
        }
    }

    private void LimpiarFormulario()
    {
        TXT_NroGuia.Text = "";
        
        TXT_NroSerie.Text = "";
       
       
        DDL_Distrito_Destino.SelectedIndex = 0;
         DDL_Departamento.SelectedIndex = 0;
        DDL_Distrito.SelectedIndex = 0;
       
        TXT_DireccionPartida.Text = "";
      
        TXT_DireccionLlegada.Text = "";
        DDL_TipoDestinatario.SelectedIndex = 0;
        TXT_Destinatario.Text = "";
        DDL_DocDestinatario.SelectedIndex = 0;
        TXT_NroDocIdentidad.Text = "";
        DDL_TipoTraslado.SelectedIndex = 0;
        TXT_ObsTraslado.Text = "";
        TXT_CostoTraslado.Text = "";
        TXT_RazonTransportista.Text = "";
        TXT_RucTransportista.Text = "";
        TXT_Conductor.Text = "";
        DDL_DocConductor.SelectedIndex = 0;
        TXT_NroDocConductor.Text = "";
        TXT_NroPlaca.Text = "";
       
        TXT_ObsGuia.Text = "";
    }

    protected void GV_Guias_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
        // 1. Cambiar al índice de la nueva página (0, 1, 2...)
        GV_Guias.PageIndex = e.NewPageIndex;

        // 2. Volver a cargar los datos para mostrar la nueva página
        CargarGuias();
    }

    private void CargarGuias()
    {
        try
        {
            // 1. Verificar conexión Global
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
            }

            // 2. Configurar comando
            using (SqlCommand cmd = new SqlCommand("USP_Listar_Guias_Remision", Global.CN))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. Usar DataAdapter para llenar un DataTable (Más fácil para GridViews)
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // 4. Asignar al GridView que definiste en el HTML
                    GV_Guias.DataSource = dt;
                    GV_Guias.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            // Opcional: Mostrar error en consola o alerta
            // Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
        }
    }

    protected void BTN_BuscarGuia_Click(object sender, EventArgs e)
    {
        var criterio = TXT_BuscarGuia.Text.Trim().ToUpperInvariant();
       
    }


    void Listar_Departamento_Partida()
    {
        Global.CN.Open();

        SqlDataReader Lector;

        SqlCommand CMDListar_Dep = new SqlCommand();
        CMDListar_Dep.Connection = Global.CN;

        CMDListar_Dep.CommandType = CommandType.StoredProcedure;
        CMDListar_Dep.CommandText = "USP_Listar_Departamento";

        Lector = CMDListar_Dep.ExecuteReader();

        this.DDL_Departamento.Items.Clear();
        this.DDL_Departamento.Items.Add("--Seleccione--");
        while (Lector.Read())
        {
            this.DDL_Departamento.Items.Add(Lector.GetValue(0).ToString());

        }
        CMDListar_Dep.Dispose();
        Global.CN.Close();
    }

    void Listar_Provincia_Partida(String Departamento)
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
        this.DDL_Provincia.Items.Add("--Seleccione--");

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

    void Listar_Distrito_Partida(String Provincia)
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
        this.DDL_Distrito.Items.Add("--Seleccione--");

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
        this.Listar_Provincia_Partida(this.DDL_Departamento.Text.ToString());

        //
        this.DDL_Provincia_SelectedIndexChanged(null, null);

        //Colocar el foco de atención en el control: DDL_Provincia
        this.DDL_Provincia.Focus();
    }

    protected void DDL_Provincia_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Listar_Distrito_Partida(this.DDL_Provincia.Text.ToString());

        //Colocar el foco de atención en el control: DDL_Distrito
        this.DDL_Distrito.Focus();
    }


    //---------------------------------------------------------------

    void Listar_Departamento_Destino()
    {
        Global.CN.Open();

        SqlDataReader Lector;

        SqlCommand CMDListar_Dep = new SqlCommand();
        CMDListar_Dep.Connection = Global.CN;

        CMDListar_Dep.CommandType = CommandType.StoredProcedure;
        CMDListar_Dep.CommandText = "USP_Listar_Departamento";

        Lector = CMDListar_Dep.ExecuteReader();

        this.DDL_Departamento_Destino.Items.Clear();
        this.DDL_Departamento_Destino.Items.Add("--Seleccione--");
        while (Lector.Read())
        {
            this.DDL_Departamento_Destino.Items.Add(Lector.GetValue(0).ToString());

        }
        CMDListar_Dep.Dispose();
        Global.CN.Close();
    }

    void Listar_Provincia_Destino(String Departamento)
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
        this.DDL_Provincia_Destino.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Provincia_Destino.Items.Add("--Seleccione--");

        //Leer Datos

        while (Lector.Read())

        {

            //Agregar Datos al (/ Site
            this.DDL_Provincia_Destino.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos

        CMDListar_Provincia.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }

    void Listar_Distrito_Destino(String Provincia)
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
        this.DDL_Distrito_Destino.Items.Clear();
        //Agregar un Primer Elemento al Control: DDL_Provincia
        this.DDL_Distrito_Destino.Items.Add("--Seleccione--");

        //Leer Datos

        while (Lector.Read())

        {

            //Agregar Datos al (/ Site
            this.DDL_Distrito_Destino.Items.Add(Lector.GetValue(0).ToString());
        }

        //Liberar Recursos

        CMDListar_Distrito.Dispose();
        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }


    void Generar_Numero_Guia()
    {
        try
        {
            // Abrir Conexión (Igual que tu ejemplo)
            if (Global.CN.State == ConnectionState.Closed) Global.CN.Open();

            SqlCommand CMDGenerar = new SqlCommand();

            // Conectar y Configurar el Comando de Datos
            CMDGenerar.Connection = Global.CN;
            CMDGenerar.CommandType = CommandType.StoredProcedure;
            CMDGenerar.CommandText = "USP_Generar_NroGuia";

            // NOTA: No requiere parámetros de entrada porque el SP calcula solo.

            // Ejecutar el Comando:
            // Usamos ExecuteScalar() porque el SP devuelve un solo valor (una celda text)
            // en lugar de una lista de filas.
            string nuevoNumero = CMDGenerar.ExecuteScalar().ToString();

            // Asignar el valor a tu caja de texto (Asumo que se llama TXT_NroGuia)
            this.TXT_NroGuia.Text = nuevoNumero;

            // Liberar Recursos
            CMDGenerar.Dispose();

            // Cerrar Conexión
            Global.CN.Close();
        }
        catch (Exception ex)
        {
            // Siempre es bueno cerrar la conexión si hay error
            if (Global.CN.State == ConnectionState.Open) Global.CN.Close();
            // Mostrar error (MessageBox o Response.Write segun sea Web o Desktop)
            MostrarMensaje("Error de: " + ex.Message, "error");
        }
    }

    protected void DDL_Departamento_Destino_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Invocar al método Listar_Provincia
        this.Listar_Provincia_Destino(this.DDL_Departamento_Destino.Text.ToString());

        //
        this.DDL_Provincia_Destino_SelectedIndexChanged(null, null);

        //Colocar el foco de atención en el control: DDL_Provincia
        this.DDL_Provincia_Destino.Focus();
    }

    protected void DDL_Provincia_Destino_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.Listar_Distrito_Destino(this.DDL_Provincia_Destino.Text.ToString());

        //Colocar el foco de atención en el control: DDL_Distrito
        this.DDL_Distrito_Destino.Focus();
    }
}