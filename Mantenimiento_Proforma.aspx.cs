using System;
using System.Data;
using System.Data.Entity.Core.Mapping;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Mantenimiento_Proforma : System.Web.UI.Page
{


    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();


    AlmacenLOGIC AlmacenLOGIC = new AlmacenLOGIC();
    ProformaLOGIC ProformaLOGIC = new ProformaLOGIC();

    void Nuevo()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Declarar y Configurar un Nuevo Comando de Datos
        SqlCommand CMDNuevo = new SqlCommand();
        CMDNuevo.Connection = Global.CN;
        CMDNuevo.CommandType = CommandType.StoredProcedure;
        CMDNuevo.CommandText = "USP_Generar_NroProforma";
        //Declarar un Nuevo Parámetro de Salida de Datos
        SqlParameter ParamCodigo = new SqlParameter();
        //Establecer Nombre, Tipo de Dato y Longitud del Parámetro
        ParamCodigo.ParameterName = "@NroProforma";
        ParamCodigo.SqlDbType = SqlDbType.Char;
        ParamCodigo.Size = 10;
        //Indicar el Tipo de Parámetro que representa: Salida de Datos (Output)
        ParamCodigo.Direction = ParameterDirection.Output;

        //Agregar Parametro al Comando de Datos
        CMDNuevo.Parameters.Add(ParamCodigo);

        //Ejecutar Comando de Datos
        CMDNuevo.ExecuteNonQuery();
        //Obtener el Código Generado
        this.TXT_NroProforma.Text = ParamCodigo.Value.ToString();
        //Liberar Recursos
        CMDNuevo.Dispose();

        //Cerrar Conexión con la Base de Datos
        Global.CN.Close();
    }


    public string ObtenerClaseEstado(string estado)
    {
        // Aseguramos que no haya espacios extra
        switch (estado.Trim())
        {
            case "Pendiente":
                return "estado-pendiente";

            case "Aprobada":
                return "estado-aprobada";

            case "Anulada":
                return "estado-anulada";

            case "Vencida":
                return "estado-vencida";

            default:
                return ""; // O puedes retornar "text-secondary" por defecto
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            Listar_Sucursal();
          
           
            TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            TXT_FecCaducidad.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
            CargarProformas();

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);

            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField (para guardar en BD)
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            //Evaluar si la Variable de tipo Session "MantenimientoEmpleado_CodEmpleado" es diferente de vacio
            if (Session["MantenimientoProforma_NroProforma"].ToString() != "")
            {
                Session["MantenimientoProforma_Tipo_Transaccion"] = "ACTUALIZAR";
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


    protected void BTN_AbrirModalBuscar_Click(object sender, EventArgs e)
    {
        // 1. VALIDAR que se haya seleccionado una sucursal
        if (DDL_Sucursal.SelectedIndex == 0)
        {
            MostrarMensaje("Debe seleccionar una Sucursal primero para buscar productos.", "warning");
            return;
        }

        // 2. PREPARAR EL MODAL
        LBL_Modal_Sucursal.Text = DDL_Sucursal.SelectedItem.Text;

        // 3. LIMPIAR DATOS ANTERIORES
        // Limpia los DDLs. El 'ActualizarFiltrosYGrid' los volverá a llenar.
        DDL_FiltroMarca.Items.Clear();
        DDL_FiltroModelo.Items.Clear();
        DDL_FiltroTalla.Items.Clear();
        DDL_FiltroColor.Items.Clear();

        // Establece el valor "Todos" ("") para todos (excepto el primero)
        // para que la primera carga muestre todo lo de la sucursal.
        DDL_FiltroMarca.SelectedValue = "";
        DDL_FiltroModelo.SelectedValue = "";
        DDL_FiltroTalla.SelectedValue = "";
        DDL_FiltroColor.SelectedValue = "";

        // 4. LLAMAR AL CEREBRO
        // Esto ejecutará el SP con todos los filtros vacíos,
        // llenando el Grid con TODO el stock y los DDLs con TODAS las opciones.
        ActualizarFiltrosYGrid();

        // 5. MOSTRAR EL MODAL (usando JavaScript)
        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "AbrirModalProductoScript",
            "var modalProducto = new bootstrap.Modal(document.getElementById('modalBuscarProducto')); modalProducto.show();",
            true
        );
    }



    void Listar_Sucursal()
    {
        // 1. Llama al método de la capa lógica que SÍ devuelve un DataTable
        DataTable PROFORMALOGIC = ProformaLOGIC.ListarSucursales();

        // 2. Asigna el origen de datos
        DDL_Sucursal.DataSource = PROFORMALOGIC;

        // 3. ¡ESTA ES LA PARTE IMPORTANTE!
        // Asignamos el CÓDIGO (ej: "S0001") al "Value" (valor interno)
        DDL_Sucursal.DataValueField = "CodSucursal";

        // Asignamos el NOMBRE (ej: "gygsports") al "Text" (lo que ve el usuario)
        DDL_Sucursal.DataTextField = "Sucursal";

        // 4. Carga los datos
        DDL_Sucursal.DataBind();

        // 5. (Opcional) Agrega un item por defecto
        DDL_Sucursal.Items.Insert(0, new ListItem("-- Seleccione Sucursal --", ""));
    }




    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
        // --- 1. VALIDACIÓN (Todo esto ya está perfecto) ---
        if (DDL_Sucursal.SelectedIndex == 0)
        {
            MostrarMensaje("Debe seleccionar una Sucursal.", "warning");
            return;
        }
        if (string.IsNullOrEmpty(HFD_NroSerie.Value))
        {
            MostrarMensaje("Debe buscar y seleccionar un producto.", "warning");
            return;
        }
        // ... (Validaciones de Cantidad, Precio, Stock...)
        int cantidad = Convert.ToInt32(TXT_Cantidad.Text);
        int stock = Convert.ToInt32(HFD_StockDisponible.Value);
        if (cantidad > stock)
        {
            MostrarMensaje($"La cantidad ({cantidad}) supera el stock disponible ({stock}).", "error");
            TXT_Cantidad.Focus();
            return;
        }

        // --- 2. PREPARAR EL DATATABLE (EN SESIÓN) ---
        DataTable dtDetalle = Session["MantenimientoDetalleProforma"] as DataTable;
        if (dtDetalle == null)
        {
            dtDetalle = new DataTable();
            // Columnas REALES para la BD
            dtDetalle.Columns.Add("CodSucursal");
            dtDetalle.Columns.Add("ProductoDescripcion");
            dtDetalle.Columns.Add("NroSerie_Producto");
            dtDetalle.Columns.Add("Prec_Venta_Menor", typeof(decimal));
            dtDetalle.Columns.Add("Prec_Venta_Mayor", typeof(decimal));
            dtDetalle.Columns.Add("Prec_Cotizado", typeof(decimal));
            dtDetalle.Columns.Add("Cantidad", typeof(int));
            dtDetalle.Columns.Add("Importe", typeof(decimal));
            // Columna VISUAL para el GridView

        }

      
        string nroSerieParaAgregar = HFD_NroSerie.Value;

        // Recorremos la tabla en la Sesión
        foreach (DataRow drExistente in dtDetalle.Rows)
        {
            // Comparamos el NroSerie
            if (drExistente["NroSerie_Producto"].ToString().Equals(nroSerieParaAgregar))
            {
                // Si ya existe, mostramos un error y salimos
                MostrarMensaje($"El producto con serie {nroSerieParaAgregar} ya está en el detalle. No puede agregarlo dos veces.", "warning");
                return;
            }
        }

        // --- 3. AGREGAR LA NUEVA FILA (Si pasó la validación) ---
        decimal precCotizado = Convert.ToDecimal(TXT_PrecCotizado.Text);
        decimal importe = cantidad * precCotizado;

        DataRow row = dtDetalle.NewRow();
        row["CodSucursal"] = DDL_Sucursal.SelectedValue;
        row["ProductoDescripcion"] = TXT_ProductoNombre.Text;
        row["NroSerie_Producto"] = HFD_NroSerie.Value;
        row["Prec_Venta_Menor"] = Convert.ToDecimal(HFD_PrecioUnitario.Value);
        row["Prec_Venta_Mayor"] = Convert.ToDecimal(HFD_PrecioMayor.Value);
      
        row["Cantidad"] = cantidad;
        row["Prec_Cotizado"] = precCotizado;
        row["Importe"] = importe;

        dtDetalle.Rows.Add(row);
        Session["MantenimientoDetalleProforma"] = dtDetalle; // 

        // --- 4. ACTUALIZAR LA INTERFAZ ---
        GV_DetalleProforma.DataSource = dtDetalle;
        GV_DetalleProforma.DataBind();

        CalcularTotal();
        LimpiarDetalle(); // Limpia los campos de entrada
    }

    private void CalcularTotal()
    {
        DataTable dtDetalle = Session["MantenimientoDetalleProforma"] as DataTable;
        if (dtDetalle != null)
        {
            decimal total = 0;
            foreach (DataRow row in dtDetalle.Rows)
            {
                total += Convert.ToDecimal(row["Importe"]);
            }
            LBL_Total.Text = total.ToString("C2");
        }
    }



    private void LimpiarDetalle()
    {
        DDL_Sucursal.SelectedIndex = 0;
        TXT_ProductoNombre.Text = "";
        TXT_Cantidad.Text = "";
        TXT_PrecCotizado.Text = "";
        LBL_PrecMenor.Text = "0.00";
        LBL_PrecMayor.Text = "0.00";
        LBL_Importe.Text = "0.00";
    }

    protected void GV_DetalleProforma_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EliminarDetalle")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtDetalle = Session["MantenimientoDetalleProforma"] as DataTable;
            if (dtDetalle != null && index < dtDetalle.Rows.Count)
            {
                dtDetalle.Rows.RemoveAt(index);
                Session["MantenimientoDetalleProforma"] = dtDetalle;

                GV_DetalleProforma.DataSource = dtDetalle;
                GV_DetalleProforma.DataBind();

                CalcularTotal();
            }
        }
    }

    protected void GV_DetalleProforma_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_DetalleProforma.PageIndex = e.NewPageIndex;
        DataTable dtDetalle = Session["MantenimientoDetalleProforma"] as DataTable;
        if (dtDetalle != null)
        {
            GV_DetalleProforma.DataSource = dtDetalle;
            GV_DetalleProforma.DataBind();
        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();

        Nuevo();
        CargarProformas(); 

        //Asignar Valor a la variable:
        Session["MantenimientoProforma_Tipo_Transaccion"] = "GUARDAR";
    }

    private void LimpiarFormulario()
    {
        TXT_NroProforma.Text = "";
        TXT_Cliente.Text = "";
      
        TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        TXT_FecCaducidad.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
     
        LBL_Total.Text = "S/. 0.00";

        Session["MantenimientoDetalleProforma"] = null;
        GV_DetalleProforma.DataSource = null;
        GV_DetalleProforma.DataBind();

        LimpiarDetalle();
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        string mensaje = ""; // Variable para guardar el mensaje de éxito o error

        // Limpiamos el mensaje anterior en cada clic
      

        try
        {
            // --- 1. VALIDACIONES DEL FORMULARIO ---
            if (string.IsNullOrEmpty(HFD_CodCliente.Value) || HFD_CodCliente.Value == "0")
            {
                mensaje = "Debe seleccionar un Cliente.";
                MostrarMensaje("Debe seleccionar un Cliente.", "warning");
                // return; // No usamos return, dejamos que el if/else de abajo maneje el mensaje
            }

            DataTable dtDetalle = Session["MantenimientoDetalleProforma"] as DataTable;
            if (mensaje == "" && (dtDetalle == null || dtDetalle.Rows.Count == 0))
            {
                mensaje = "Debe agregar al menos un producto al detalle.";
               MostrarMensaje("Debe agregar al menos un producto al detalle.", "warning");
                // return;
            }

            // ¡¡NUEVA VALIDACIÓN (Para evitar el crash)!!
            if (mensaje == "" && string.IsNullOrEmpty(TXT_FecEmision.Text))
            {
                mensaje = "La Fecha de Emisión no puede estar vacía.";
                MostrarMensaje("La Fecha de Emisión no puede estar vacía.", "warning");
                TXT_FecEmision.Focus();
                // return;
            }
            if (mensaje == "" && string.IsNullOrEmpty(TXT_FecCaducidad.Text))
            {
                mensaje = "La Fecha de Caducidad no puede estar vacía.";
                MostrarMensaje("La Fecha de Caducidad no puede estar vacía.", "warning");
                TXT_FecCaducidad.Focus();
                // return;
            }

            // --- 2. RECOLECTAR DATOS Y LLAMAR A LA LÓGICA (Solo si no hay errores) ---
            if (mensaje == "")
            {
                string nroProforma = TXT_NroProforma.Text;
                string codCliente = HFD_CodCliente.Value;
                string codEmpleado = HFD_CodEmpleado.Value;
                DateTime fecEmision = Convert.ToDateTime(TXT_FecEmision.Text);
                DateTime fecCaducidad = Convert.ToDateTime(TXT_FecCaducidad.Text);
                string estado = "Pendiente";

                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
                decimal total = decimal.Parse(LBL_Total.Text,
                               System.Globalization.NumberStyles.Currency,
                               culture.NumberFormat);

                // --- 3. LLAMAR A LA CAPA DE LÓGICA ---
                mensaje = ProformaLOGIC.GuardarProforma(nroProforma, codCliente, codEmpleado,
                              fecEmision, fecCaducidad, total,
                                estado, dtDetalle);
            }
        }
        catch (FormatException fex)
        {
            // Captura errores de conversión (ej. fecha vacía o total mal formateado)
            mensaje = "Error de Formato: " + fex.Message;
        }
        catch (Exception ex)
        {
            // Captura cualquier otro error (ej. SQL, etc.)
            mensaje = "Error fatal: " + ex.Message;
        }

        // --- 4. MOSTRAR RESULTADO ---
        // Esta parte AHORA SÍ se ejecutará, incluso si hay un error
        if (mensaje.Contains("correctamente")) // Éxito
        {
            
            MostrarMensaje(mensaje, "success");

            LimpiarFormulario();
            CargarProformas(); 
            Nuevo(); // Genera un nuevo código
        }
        else // Error
        {
            MostrarMensaje(mensaje, "error");
        }
    }

  

   
    protected void BTN_Buscar_Modal_Click(object sender, EventArgs e)
    {
        string buscar = this.txtBuscarNombre.Text.Trim();
        string mensajeBD = ""; // Variable para capturar el mensaje del SP

        // Limpiamos la grilla al inicio
        GV_Clientes_Modal.DataSource = null;
        GV_Clientes_Modal.DataBind();

        if (string.IsNullOrEmpty(buscar))
        {
            // Si no escribió nada, no hacemos nada o mostramos un aviso simple
            // return; 
        }
        else
        {
            try
            {
                // 1. Llamamos al método actualizado con el parámetro 'out'
                // Nota: Si este método está en otra clase (DAO/Logic), instánciala primero.
                // Ejemplo: ClienteDAO dao = new ClienteDAO();
                // DataTable dtClientes = dao.Listar_Clientes_Busqueda(buscar, out mensajeBD);

                DataTable dtClientes = Listar_Clientes_Busqueda(buscar, out mensajeBD);

                // 2. Evaluamos resultados
                if (dtClientes != null && dtClientes.Rows.Count > 0)
                {
                    // CASO A: Encontramos clientes activos -> Llenamos la grilla
                    GV_Clientes_Modal.DataSource = dtClientes;
                    GV_Clientes_Modal.DataBind();
                }
                else
                {
                    // CASO B: No hay filas. ¿Por qué?
                    if (!string.IsNullOrEmpty(mensajeBD))
                    {
                        // El SP nos dijo que el cliente existe pero está BLOQUEADO
                        MostrarMensaje(mensajeBD, "warning");
                    }
                    else
                    {
                        // El mensaje vino vacío, el cliente NO EXISTE
                        MostrarMensaje("No se encontraron clientes con esos datos.", "info");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores con SweetAlert en lugar de Response.Write
                MostrarMensaje("Error al buscar: " + ex.Message, "error");
            }
        }

        // 3. Mantenemos el modal abierto después del PostBack
        ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModal", "$('#modalBuscarCliente').modal('show');", true);
    }

    // ==========================================
    // MÉTODO 2: Ejecución del Procedimiento Almacenado
    // ==========================================
    public DataTable Listar_Clientes_Busqueda(string busqueda, out string mensajeRespuesta)
    {
        DataTable DT = new DataTable();
        mensajeRespuesta = ""; // Inicializamos vacío

        try
        {
            // 1. Abrir Conexión
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
            }

            SqlCommand CMD = new SqlCommand("USP_Listar_Clientes_Busqueda", Global.CN);
            CMD.CommandType = CommandType.StoredProcedure;

            // 2. Parámetro de Entrada (Lo que escribes en la caja de texto)
            CMD.Parameters.AddWithValue("@pBusqueda", busqueda);

            // 3. Parámetro de SALIDA (El mensaje de bloqueo/inactivo)
            SqlParameter ParamMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
            ParamMensaje.Direction = ParameterDirection.Output;
            CMD.Parameters.Add(ParamMensaje);

            // 4. Llenar el DataTable
            SqlDataAdapter DA = new SqlDataAdapter(CMD);
            DA.Fill(DT);

            // 5. RECUPERAR EL MENSAJE DE SALIDA
            // Esto se hace DESPUÉS del Execute o Fill
            if (ParamMensaje.Value != DBNull.Value)
            {
                mensajeRespuesta = ParamMensaje.Value.ToString();
            }

            DA.Dispose();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            // Aseguramos cerrar la conexión siempre
            if (Global.CN.State == ConnectionState.Open)
            {
                Global.CN.Close();
            }
        }

        return DT;
    }

    // ==========================================
    // MÉTODO 3: Manejo de Selección en el GridView
    // ==========================================
    protected void GV_Clientes_Modal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SeleccionarCliente")
        {

            string codigoClienteSeleccionado = e.CommandArgument.ToString();

            // 1. Encontrar la fila que disparó el comando
            GridViewRow selectedRow = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            // 2. Obtener el Nombre Completo de la celda de la fila (Ajusta el índice según tu GridView)
            // ASUME: La columna 1 (índice 1) contiene el nombre completo
            string nombreCliente = selectedRow.Cells[1].Text; // Esto puede fallar si es TemplateField, ver nota.

            // --- Si usas TemplateField para el nombre (como te recomendé): ---
            // Debes encontrar el Label dentro del TemplateField.
            Label lblNombreCompleto = (Label)selectedRow.FindControl("LBL_NombreCompleto");
            if (lblNombreCompleto != null)
            {
                nombreCliente = lblNombreCompleto.Text;
            }
            // -------------------------------------------------------------------

            // Paso 3: Asignar valores a los campos del formulario principal
            this.HFD_CodCliente.Value = codigoClienteSeleccionado; // Guarda el ID para grabar
            this.TXT_Cliente.Text = nombreCliente;                 // Muestra el nombre al usuario

            UP_Cliente.Update();

            // Cerrar el modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CerrarModal", "$('#modalBuscarCliente').modal('hide');", true);
        }
    }


    protected void GV_Proformas_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Atender")
        {
            string nroProforma = e.CommandArgument.ToString();
            Response.Redirect("Pedido.aspx?nroProforma=" + nroProforma);
        }

        // --- Lógica para el botón IMPRIMIR ---
        if (e.CommandName == "Imprimir")
        {
            // 1. Obtener el Nro de Proforma del botón
            string nroProforma = e.CommandArgument.ToString();

            if (string.IsNullOrEmpty(nroProforma))
            {
                // ¡Usamos tu SweetAlert!
                MostrarMensaje("Error: No se pudo obtener el NroProforma para imprimir.", "error");
                return;
            }

            try
            {
                // 2. Llamar a la lógica para obtener los datos
                DataSet ds = ProformaLOGIC.ObtenerDatosProforma(nroProforma);

                if (ds.Tables.Count >= 2 && ds.Tables[0].Rows.Count > 0)
                {
                    // 3. Guardar datos en Sesión para la página de impresión
                    Session["PrintCabecera"] = ds.Tables[0];
                    Session["PrintDetalle"] = ds.Tables[1];

                    // 4. Abrir la nueva pestaña de impresión
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenPrintWindow",
                        "window.open('ImprimirProforma.aspx', '_blank');", true);
                }
                else
                {
                    // ¡Usamos tu SweetAlert!
                    MostrarMensaje("Error: No se encontraron datos para la proforma " + nroProforma, "error");
                }
            }
            catch (Exception ex)
            {
                // ¡Usamos tu SweetAlert!
                MostrarMensaje("Error fatal al imprimir: " + ex.Message, "error");
            }
        }


        // --- Lógica para el botón ELIMINAR ---
        if (e.CommandName == "EliminarProforma")
        {
            string nroProforma = e.CommandArgument.ToString();
            // ... (Aquí va tu lógica existente para eliminar la proforma) ...
            // Ej: oBusquedaProforma.Eliminar(nroProforma);
            // CargarProformas(); // Recargar el grid
        }
    
 
}




    /// <summary>
    /// MODAL DE BUSCAR PRODUCTO
    /// </summary>
    private void ActualizarFiltrosYGrid()
    {
       

        // 2. Guardar la selección actual de cada filtro
        string suc_val_nombre = DDL_Sucursal.SelectedItem.Text;
        string mar_val = DDL_FiltroMarca.SelectedValue;
        string mod_val = DDL_FiltroModelo.SelectedValue;
        string tal_val = DDL_FiltroTalla.SelectedValue;
        string col_val = DDL_FiltroColor.SelectedValue;

        // 3. Llamar al SP que devuelve 5 tablas
        DataSet ds = ProformaLOGIC.BuscarStockYFiltros(suc_val_nombre, mar_val, mod_val, tal_val, col_val);

        // 4. Llenar el GridView (Tabla 0)
        GV_ProductosSucursal.DataSource = ds.Tables["GridViewProductos"];
        GV_ProductosSucursal.DataBind();

        // 5. Llenar TODOS los DDLs (Tablas 1-4)
        //    Usamos un método 'Helper' para no perder la selección
        LlenarDDLPreservando(DDL_FiltroMarca, ds.Tables["FiltroMarcas"], "Marca", mar_val);
        LlenarDDLPreservando(DDL_FiltroModelo, ds.Tables["FiltroModelos"], "Modelo", mod_val);
        LlenarDDLPreservando(DDL_FiltroTalla, ds.Tables["FiltroTallas"], "Talla", tal_val);
        LlenarDDLPreservando(DDL_FiltroColor, ds.Tables["FiltroColores"], "Color", col_val);

        // 6. Forzar la actualización del panel del modal
        UP_ModalBusqueda.Update();
    }


    private void LlenarDDLPreservando(DropDownList ddl, DataTable data, string nombreColumna, string valorSeleccionado)
    {
        // 1. Limpia el control
        ddl.Items.Clear();

        // 2. ¡¡AQUÍ ESTÁ LA CORRECCIÓN DE C#!!
        // Limpia la selección recordada (ViewState) ANTES de enlazar.
        // Esto evita el error "ArgumentOutOfRangeException" si 'data' está vacía.
        ddl.ClearSelection();
        ddl.SelectedValue = null;

        // 3. Asigna el origen de datos (de la BD)
        ddl.DataSource = data;
        ddl.DataTextField = nombreColumna;
        ddl.DataValueField = nombreColumna;

        // 4. ¡Haz el DataBind! (Ya no fallará aquí)
        ddl.DataBind();

        // 5. Inserta el item "<Todos>" al inicio
        ddl.Items.Insert(0, new ListItem("<Todos>", ""));

        // 6. Ahora es seguro re-seleccionar el valor
        if (ddl.Items.FindByValue(valorSeleccionado) != null)
        {
            ddl.SelectedValue = valorSeleccionado;
        }
        else
        {
            // Si el valor ya no existe, selecciona "<Todos>" por defecto
            ddl.SelectedIndex = 0;
        }
    }
    protected void DDL_FiltroMarca_Changed(object sender, EventArgs e)
    {
        ActualizarFiltrosYGrid();
    }

    /// <summary>
    /// FILTRO 2: Se ejecuta cuando el usuario selecciona un MODELO.
    /// </summary>
    protected void DDL_FiltroModelo_Changed(object sender, EventArgs e)
    {
        ActualizarFiltrosYGrid();
    }

    /// <summary>
    /// FILTRO 3: Se ejecuta cuando el usuario selecciona una TALLA.
    /// </summary>
    protected void DDL_FiltroTalla_Changed(object sender, EventArgs e)
    {
        ActualizarFiltrosYGrid();
    }

    /// <summary>
    /// FILTRO 4: Se ejecuta cuando el usuario selecciona un COLOR.
    /// </summary>
    protected void DDL_FiltroColor_Changed(object sender, EventArgs e)
    {
        ActualizarFiltrosYGrid();

    }


    protected void GV_ProductosSucursal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // 1. Verificamos que el comando sea el de "SeleccionarProducto"
        if (e.CommandName == "SeleccionarProducto")
        {
            // 2. Obtiene el índice de la fila que se presionó
            int index = Convert.ToInt32(e.CommandArgument);

            // 3. Obtiene los DataKeys de esa fila
            //    (Asegúrate que tu .aspx tenga todos estos DataKeyNames)
            DataKey key = GV_ProductosSucursal.DataKeys[index];

            string nroSerie = key.Values["NroSerie_Producto"].ToString();
            string codProducto = key.Values["CodProducto"].ToString();
            string nombreProducto = key.Values["ProductoDescripcion"].ToString();
            string stock = key.Values["Stock_Actual"].ToString();

            // Convertimos los precios de forma segura
            string precioMenor = Convert.ToDecimal(key.Values["Prec_Venta_Menor"]).ToString("N2");
            string precioMayor = Convert.ToDecimal(key.Values["Prec_Venta_Mayor"]).ToString("N2");

            // 4. Pasa los datos a los controles de la PÁGINA PRINCIPAL

            // Formateamos el texto para que el vendedor vea toda la info
            TXT_ProductoNombre.Text = $"{nombreProducto} ( Stock: {stock})";

            // Guarda los datos clave en los HiddenFields
            HFD_CodProducto.Value = codProducto;
            HFD_NroSerie.Value = nroSerie;
            HFD_StockDisponible.Value = stock;

            // Por defecto, usamos el PRECIO MENOR como el precio cotizado
            HFD_PrecioUnitario.Value = precioMenor;
            // También lo ponemos en el TextBox de Precio Cotizado
            TXT_PrecCotizado.Text = precioMenor;

            // Guardamos el precio mayor por si lo necesitas para descuentos
            HFD_PrecioMayor.Value = precioMayor;

            // También actualizamos los Labels de la página principal
            LBL_PrecMenor.Text = precioMenor;
            LBL_PrecMayor.Text = precioMayor;

            // 5. Pone 1 por defecto y enfoca la cantidad
            TXT_Cantidad.Text = "1";
            TXT_Cantidad.Focus(); // Pone el cursor en la cantidad

            UP_FormularioDetalle.Update();

            // 6. CIERRA EL MODAL (usando JavaScript de Bootstrap 5)
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "CerrarModalProductoScript",
                // Este script obtiene la instancia del modal y la oculta
                "var modalProducto = bootstrap.Modal.getInstance(document.getElementById('modalBuscarProducto')); modalProducto.hide();",
                true
            );
        }
    }

    void CargarProformas()
    {
        // Llama al nuevo método de la lógica que lista TODAS las proformas
        DataTable dt = ProformaLOGIC.ListarProformasTodos();

        GV_Proformas.DataSource = dt;
        GV_Proformas.DataBind();
    }

    // 2. EVENTO CLICK DEL BOTÓN BUSCAR
    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        
        // la lista completa de proformas.
        CargarProformas();
    }

    // 3. EVENTO DE PAGINACIÓN (MUY IMPORTANTE)
    // Este método es necesario para que la paginación del gridview funcione.
    protected void GV_Proformas_PageIndexChanging(object sender, GridViewPageEventArgs e){
        GV_Proformas.PageIndex = e.NewPageIndex;
        CargarProformas(); // Vuelve a cargar los datos para la nueva página
     }
    


}
