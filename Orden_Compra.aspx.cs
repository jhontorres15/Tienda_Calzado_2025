using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Orden_Compra : System.Web.UI.Page
{
    CompraLOGIC logic = new CompraLOGIC();
    CotizacionLOGIC CotizacionLOGIC = new CotizacionLOGIC();
    ProformaLOGIC ProformaLOGIC = new ProformaLOGIC();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TXT_FecCompra.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            GenerarNumero();
             Listar_Sucursal();

            string codEmpleado = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nomEmpleado = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);
            if (!string.IsNullOrEmpty(codEmpleado) && !string.IsNullOrEmpty(nomEmpleado))
            {
                TXT_Empleado.Text = nomEmpleado.Trim();
                HFD_CodEmpleado.Value = codEmpleado.Trim();
            }

            CargarListado();


            // 3. LÓGICA AUTOMÁTICA: Si venimos de "Atender Proforma"
            if (Request.QueryString["NroCotizacion"] != null)
            {
                string proformaURL = Request.QueryString["NroCotizacion"];
                TXT_NroCotizacionOrigen.Text = proformaURL;

                // Disparamos la carga automáticamente
                ProcesarCargaDeCotizacion(proformaURL);
            }
        }
    }

    private void ProcesarCargaDeCotizacion(string nroCotizacion)
    {
        try
        {
            // 1. Validar Sucursal (Necesario porque Tb_Detalle_Cotizacion usa CodSucursal)
            if (DDL_Sucursal.SelectedValue == "0" || DDL_Sucursal.SelectedValue == "")
            {
                MostrarMensaje("Seleccione la sucursal para cargar los ítems correspondientes.", "warning");
                return;
            }

            string codSucursal = DDL_Sucursal.SelectedValue;
            string mensajeRespuesta = "";

            // Instancia de tu capa lógica (Asegúrate de tener este método creado en CompraLOGIC)
            CompraLOGIC logic = new CompraLOGIC();

            // 2. LLAMADA A LA BASE DE DATOS
            // Llama al SP: USP_CargarDetalle_DesdeCotizacion_Compra
            DataSet ds = logic.CargarDetalle_DesdeCotizacion(nroCotizacion, codSucursal, out mensajeRespuesta);

            // 3. VALIDACIÓN DE ERROR (PROVEEDOR INACTIVO O COTIZACION INEXISTENTE)
            if (mensajeRespuesta.StartsWith("ERROR"))
            {
                //LBL_EstadoCotizacion.CssClass = "alert alert-danger";
                //LBL_EstadoCotizacion.Text = mensajeRespuesta;
                //LBL_EstadoCotizacion.Visible = true; // Asegúrate de quitar d-none si usas Bootstrap classes dinámicas

                MostrarMensaje(mensajeRespuesta, "error");

                // Limpiamos grilla
                GV_DetalleCompra.DataSource = null;
                GV_DetalleCompra.DataBind();
                return;
            }

            // 4. CARGAR GRILLA DE PRODUCTOS (Tabla 1 del DataSet)
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                // Vinculamos la grilla
                GV_DetalleCompra.DataSource = ds.Tables[1];
                GV_DetalleCompra.DataBind();

                // Guardar en Variable de Sesión (Importante para el botón Grabar)
                Session["DetalleCompra"] = ds.Tables[1];

                // 5. CALCULAR TOTALES VISUALES
                // Reutilizamos tu método existente CalcularTotales
                CalcularTotales(ds.Tables[1]);

                // Llenamos datos visuales extra si es necesario
                TXT_NroCotizacion.Text = nroCotizacion; // El campo destino

                // Mostrar mensaje de éxito LBL_EstadoCotizacion.CssClass = "alert alert-success";
               
               // LBL_EstadoCotizacion.Text = "Cotización cargada correctamente.";
                MostrarMensaje("Ítems de cotización cargados.", "success");
            }
            else
            {
                // Si la consulta no trajo filas (quizás la cotización es de otra sucursal)
               // LBL_EstadoCotizacion.CssClass = "alert alert-warning";
              //  LBL_EstadoCotizacion.Text = "La cotización no tiene ítems asignados a esta sucursal.";
                MostrarMensaje("No hay ítems en esta cotización para la sucursal seleccionada.", "warning");

                GV_DetalleCompra.DataSource = null;
                GV_DetalleCompra.DataBind();
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar Cotización: " + ex.Message, "error");
        }
    }


    private void MostrarMensaje(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalCotizacion", script, true);
    }


    protected void BTN_AbrirModalBuscar_Click(object sender, EventArgs e)
    {
       

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

        if (DDL_Sucursal.Items.Count > 0)
        {
            DDL_Sucursal.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// MODAL DE BUSCAR PRODUCTO
    /// </summary>
    private void ActualizarFiltrosYGrid()
    {


        try
        {
            // 1. Obtener valores
            string suc_val_nombre = DDL_Sucursal.SelectedItem.Text; // Nombre exacto de la sucursal
            string mar_val = DDL_FiltroMarca.SelectedValue;
            string mod_val = DDL_FiltroModelo.SelectedValue;
            string tal_val = DDL_FiltroTalla.SelectedValue;
            string col_val = DDL_FiltroColor.SelectedValue;

            // 2. Llamar al SP
            DataSet ds = CotizacionLOGIC.Buscar_Productos_Cotizacion(suc_val_nombre, mar_val, mod_val, tal_val, col_val);

            if (ds != null && ds.Tables.Count >= 5)
            {
                // 3. Llenar GridView (Tabla 0)
                // ¡CORREGIDO! Usamos índice [0] en vez de nombre string
                GV_Productos.DataSource = ds.Tables[0];
                GV_Productos.DataBind();

                // 4. Llenar DDLs (Tablas 1-4)
                // ¡CORREGIDO! Usamos índices [1], [2], [3], [4]
                LlenarDDLPreservando(DDL_FiltroMarca, ds.Tables[1], "Marca", mar_val);
                LlenarDDLPreservando(DDL_FiltroModelo, ds.Tables[2], "Modelo", mod_val);
                LlenarDDLPreservando(DDL_FiltroTalla, ds.Tables[3], "Talla", tal_val);
                LlenarDDLPreservando(DDL_FiltroColor, ds.Tables[4], "Color", col_val);

                // 5. Actualizar panel
                if (UP_ModalBusqueda != null) UP_ModalBusqueda.Update();
            }
        }
        catch (Exception ex)
        {
            // Muestra el error para saber qué pasa
            MostrarMensaje("Error al cargar productos: " + ex.Message, "error");
        }
    }


    private void LlenarDDLPreservando(DropDownList ddl, DataTable data, string nombreColumna, string valorSeleccionado)
    {
        // 1. Limpia el control
        ddl.Items.Clear();

        // 2. ¡¡AQUÍ ESTÁ LA CORRECCIÓN DE C#!!
        // Limpia la selección recordada (ViewState) ANTES de enlazar.
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
        // Verificamos que no sea nulo/vacío y que exista en la nueva lista
        if (!string.IsNullOrEmpty(valorSeleccionado) && ddl.Items.FindByValue(valorSeleccionado) != null)
        {
            ddl.SelectedValue = valorSeleccionado;
        }
        else
        {
            // Si el valor ya no existe (ej. el modelo no pertenece a la marca seleccionada), selecciona "<Todos>"
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


    protected void GV_Productos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // 1. Verificamos que el comando sea el de "SeleccionarProducto"
        if (e.CommandName == "SeleccionarProducto")
        {
            // 2. Obtiene el índice de la fila que se presionó
            int index = Convert.ToInt32(e.CommandArgument);

            // 3. Obtiene los DataKeys de esa fila
            // IMPORTANTE: Asegúrate de que en el .aspx tengas: 
            // DataKeyNames="CodProducto, NroSerie_Producto, NombreComercial, Stock_Actual, Prec_Venta_Mayor"
            DataKey key = GV_Productos.DataKeys[index];

            string nroSerie = Convert.ToString(key.Values["NroSerie_Producto"]);

            // Nota: En tu SP llamamos a la columna concatenada 'NombreComercial'
            string nombreProducto = Convert.ToString(key.Values["NombreComercial"]);
            string ProductoDescripcion = Convert.ToString(key.Values["ProductoDescripcion"]);
            string stock = Convert.ToString(key.Values["Stock_Actual"]);

            // Precios referenciales (útil para saber cuánto costaba venderlo)
            string precioMayor = "0.00";
            if (key.Values["Prec_Venta_Mayor"] != DBNull.Value)
            {
                precioMayor = Convert.ToDecimal(key.Values["Prec_Venta_Mayor"]).ToString("N2");
            }

            // 4. Pasa los datos a los controles de la PÁGINA PRINCIPAL

            // Mostramos Nombre + Stock actual (informativo para compras)
            TXT_ProductoNombre.Text = $"{nombreProducto} (Ref. Stock: {stock})";

            // Guardamos los datos clave en los HiddenFields

            HFD_NroSerie.Value = nroSerie;

            HDF_ProductoDescripcion.Value = ProductoDescripcion;
            // 5. Pone 1 por defecto y enfoca la cantidad
            TXT_Cantidad.Text = "1";

            // Si tuvieras un campo de precio costo, lo podrías limpiar o pre-llenar aquí
            // TXT_PrecioCosto.Text = ""; 

            TXT_Cantidad.Focus();

            // Actualizamos el panel del formulario principal para que se vean los cambios
            if (UP_FormularioDetalle != null) UP_FormularioDetalle.Update();

            // 6. CIERRA EL MODAL (usando JavaScript de Bootstrap 5)
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "CerrarModalProductoScript",
                "var modalElement = document.getElementById('modalBuscarProducto'); if(modalElement){ var modalProducto = bootstrap.Modal.getInstance(modalElement); if(modalProducto){modalProducto.hide();} else { new bootstrap.Modal(modalElement).hide(); } }",
                true
            );
        }
    }



    void GenerarNumero()
    {
        TXT_NroCompra.Text = logic.Generar_NroCompra();
        TXT_NroCompra.Enabled = false;
    }

 

    void Mostrar(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalCompra", script, true);
    }

    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
        try
        {
            // Validamos campos de texto
            if (string.IsNullOrWhiteSpace(TXT_ProductoNombre.Text) ||
                string.IsNullOrWhiteSpace(TXT_Cantidad.Text) ||
                string.IsNullOrWhiteSpace(TXT_PrecCompra.Text))
            {
                Mostrar("Faltan datos del producto, cantidad o precio.", "warning");
                return;
            }

            // 2. CONVERSIÓN SEGURA DE NÚMEROS (Evita errores de formato)
            int cantidad = 0;
            decimal precio = 0;
            decimal porcentaje = 0;

            // Intentamos convertir. Si falla (devuelve false), mostramos error.
            if (!int.TryParse(TXT_Cantidad.Text, out cantidad) || cantidad <= 0)
            {
                Mostrar("La cantidad debe ser un número entero mayor a 0.", "warning");
                return;
            }

            // Usamos NumberStyles para aceptar puntos o comas según configuración
            if (!decimal.TryParse(TXT_PrecCompra.Text, out precio))
            {
                Mostrar("El precio de compra no tiene un formato válido.", "warning");
                return;
            }

            if (!string.IsNullOrWhiteSpace(TXT_PorcentajeDscto.Text))
            {
                if (!decimal.TryParse(TXT_PorcentajeDscto.Text, out porcentaje))
                {
                    porcentaje = 0; // Si escribieron texto invalido, asumimos 0
                }
            }

            // 3. INICIALIZAR DATATABLE
            DataTable dt = Session["DetalleCompra"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("CodSucursal");
                dt.Columns.Add("Producto");
                dt.Columns.Add("NroSerie_Producto");
                dt.Columns.Add("Prec_Compra", typeof(decimal));
                dt.Columns.Add("Cantidad", typeof(int));
                dt.Columns.Add("Importe", typeof(decimal));
                dt.Columns.Add("Porcentaje_Dscto", typeof(decimal));
                dt.Columns.Add("Dscto", typeof(decimal));
                dt.Columns.Add("SubTotal", typeof(decimal));
            }

            // 4. CÁLCULOS MATEMÁTICOS
            decimal importe = cantidad * precio;
            decimal dscto = Math.Round(importe * (porcentaje / 100m), 2);
            decimal subtotal = importe - dscto;

            // 5. AGREGAR FILA
            DataRow row = dt.NewRow();
            // Usamos SelectedItem.Text para guardar el NOMBRE de la sucursal, no el código, para que se vea bien en la grilla
            row["CodSucursal"] = DDL_Sucursal.SelectedItem.Text;
            row["Producto"] = HDF_ProductoDescripcion.Value; // Asegúrate que el HiddenField tenga datos
            row["NroSerie_Producto"] = HFD_NroSerie.Value;
            row["Prec_Compra"] = precio;
            row["Cantidad"] = cantidad;
            row["Importe"] = importe;
            row["Porcentaje_Dscto"] = porcentaje;
            row["Dscto"] = dscto;
            row["SubTotal"] = subtotal;

            dt.Rows.Add(row);

            // 6. ACTUALIZAR SESIÓN Y GRILLA
            Session["DetalleCompra"] = dt;
            GV_DetalleCompra.DataSource = dt;
            GV_DetalleCompra.DataBind();

            // Recalcular totales generales
            CalcularTotales(dt);

            // 7. LIMPIEZA DE CAMPOS PARA EL SIGUIENTE ITEM
            // OJO: No resetees la Sucursal si el usuario va a seguir agregando productos a la misma sucursal.
            // DDL_Sucursal.SelectedIndex = 0; // <-- Comentado para mejor experiencia de usuario

            TXT_ProductoNombre.Text = "";
            TXT_Cantidad.Text = "";
            TXT_PrecCompra.Text = "";
            TXT_PorcentajeDscto.Text = "0";
            HDF_ProductoDescripcion.Value = "";
            HFD_NroSerie.Value = "";

            // Enfocar cantidad o botón buscar para agilizar
            BTN_AbrirModalBuscar.Focus();
        }
        catch (Exception ex)
        {
            // Esto te dirá exactamente qué error está ocurriendo
            Mostrar("Error al agregar detalle: " + ex.Message, "error");
        }
    }

    protected void GV_DetalleCompra_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EliminarDetalle")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtDetalle = Session["DetalleCompra"] as DataTable;
            if (dtDetalle != null && index < dtDetalle.Rows.Count)
            {
                dtDetalle.Rows.RemoveAt(index);
                Session["DetalleCompra"] = dtDetalle;

                GV_DetalleCompra.DataSource = dtDetalle;
                GV_DetalleCompra.DataBind();

                CalcularTotales(dtDetalle);
            }
        }
    }

    void CalcularTotales(DataTable dt)
    {
        decimal importe = 0m, dscto = 0m, subtotal = 0m;
        foreach (DataRow r in dt.Rows)
        {
            importe += Convert.ToDecimal(r["Importe"]);
            dscto += Convert.ToDecimal(r["Dscto"]);
            subtotal += Convert.ToDecimal(r["SubTotal"]);
        }
        //LBL_Importe.Text = importe.ToString("N2");
        //LBL_Descuento.Text = dscto.ToString("N2");
        //LBL_SubTotal.Text = subtotal.ToString("N2");
        LBL_TotalCompra.Text = "S/. " + subtotal.ToString("N2");
        
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        Limpiar();
        GenerarNumero();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        Limpiar();
    }

    void Limpiar()
    {
        TXT_NroCompra.Text = "";
        TXT_NroCotizacion.Text = "";
        TXT_FecCompra.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
       
        LBL_Importe.Text = "0.00";
        LBL_Descuento.Text = "0.00";
        LBL_SubTotal.Text = "0.00";
        LBL_TotalCompra.Text = "S/. 0.00";
        Session["DetalleCompra"] = null;
        GV_DetalleCompra.DataSource = null;
        GV_DetalleCompra.DataBind();
        DDL_Sucursal.SelectedIndex = 0;
        TXT_Cantidad.Text = "";
        TXT_PrecCompra.Text = "";
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)

    {
        // 1. Validar Sesión

        if (Session["MantenimientoUsuario_Empleado_CodEmpleado"] == null)

        {

            Mostrar("La sesión ha expirado. Por favor inicie sesión nuevamente.", "warning");
            return;

        }

        try

        {


            // 2. Obtener Detalle (DataTable) PRIMERO
            DataTable dt2 = Session["DetalleCompra"] as DataTable;
            if (dt2 == null || dt2.Rows.Count == 0)
            {
                Mostrar("Agregue al menos un detalle a la orden.", "warning");
                return;
            }

            // 3. CALCULAR EL TOTAL DESDE EL DATATABLE (CORRECCIÓN)
            // Recorremos las filas y sumamos la columna "SubTotal"
            decimal totalCalculado = 0;
            foreach (DataRow fila in dt2.Rows)
            {
                totalCalculado += Convert.ToDecimal(fila["SubTotal"]);
            }

            // 2. Obtener valores
            string nro = TXT_NroCompra.Text.Trim();
           string codEmpleado = HFD_CodEmpleado.Value.Trim(); // O Session[...] si prefieres

            string nroCot = TXT_NroCotizacion.Text.Trim();
            string estado = "Pendiente";


            // Validación de fecha segura

            if (!DateTime.TryParse(TXT_FecCompra.Text, out DateTime fec))

            {

                fec = DateTime.Now; // Fallback por seguridad

            }



        

            // 3. Obtener Detalle

            DataTable dt = Session["DetalleCompra"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)

            {

                Mostrar("Agregue al menos un detalle a la orden.", "warning");

                return;

            }



            // 4. Instanciar Lógica y Guardar

            CompraLOGIC logic = new CompraLOGIC(); // Asegúrate de instanciarlo

            string msg = logic.Guardar(nro, codEmpleado, nroCot, fec, totalCalculado, estado, dt);



            // 5. Analizar respuesta

            if (msg.StartsWith("OK"))

            {

                Mostrar("Orden de compra registrada exitosamente.", "success");



                // Opcional: Limpiar todo para una nueva orden

                Limpiar();

                GenerarNumero(); // Generar el siguiente correlativo

                CargarListado(); // Refrescar la grilla de abajo

            }

            else

            {

                // Mostrar error que viene de SQL o C#

                Mostrar(msg, "error");

            }

        }

        catch (Exception ex)

        {

            Mostrar("Error inesperado en el formulario: " + ex.Message, "error");

        }

    }


    protected void GV_Compras_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string NroCompra = e.CommandArgument.ToString();

        if (e.CommandName == "EditarPedido")
        {

            MostrarMensaje($"Compra {NroCompra} cargado para edición", "alert-info");
        }
        else if (e.CommandName == "Atender")
        {
            Response.Redirect($"Facturacion_Compra.aspx?NroCompra=" + NroCompra);

        }
    }

    void CargarListado()
    {
        var dt = logic.Listar_Compras(TXT_Buscar.Text.Trim());
        GV_Compras.DataSource = dt;
        GV_Compras.DataBind();
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        CargarListado();
    }

    protected void GV_DetalleCompra_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_DetalleCompra.PageIndex = e.NewPageIndex;
        var dt = Session["DetalleCompra"] as DataTable;
        GV_DetalleCompra.DataSource = dt;
        GV_DetalleCompra.DataBind();
    }

    protected void BTN_CargarCotizacion_Click(object sender, EventArgs e)
    {
        string nroCot = TXT_NroCotizacionOrigen.Text.Trim();


        if (string.IsNullOrEmpty(nroCot))
        {
            MostrarMensaje("Ingrese el número de cotización a cargar.", "warning");
            return;
        }

        ProcesarCargaDeCotizacion(nroCot);
    }
}
