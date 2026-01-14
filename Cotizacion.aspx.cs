using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Cotizacion : System.Web.UI.Page
{

    ProformaLOGIC ProformaLOGIC = new ProformaLOGIC();
    CotizacionLOGIC logic = new CotizacionLOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TXT_FecCotizacion.Text = DateTime.Now.ToString("yyyy-MM-dd");
           
             CargarProveedores();
            //CargarProductos();

            CargarListado();
            // Inicializar sesión de detalle vacía
            Session["DetalleCotizacion"] = null;
            Listar_Sucursal();

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);

            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            //Evaluar si la Variable de tipo Session "MantenimientoEmpleado_CodEmpleado" es diferente de vacio
            if (Session["MantenimientoCotizacion_NroCotizacion"].ToString() != "")
            {
                
                //Invocar al Método: Cargar_Datos()
                // this.CargarEmpleado(this.TXT_CodEmpleado.Text);
            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }

            // CargarListado();
        }
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
            DataSet ds = logic.Buscar_Productos_Cotizacion(suc_val_nombre, mar_val, mod_val, tal_val, col_val);

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
            string ProductoDescripcion = Convert.ToString(key.Values["ProductoDescripcion"]);
            // Nota: En tu SP llamamos a la columna concatenada 'NombreComercial'
            string nombreProducto = Convert.ToString(key.Values["NombreComercial"]);
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


    private void GenerarNumero()
    {
        try
        {
            TXT_NroCotizacion.Text = logic.Generar_NroCotizacion();
            TXT_NroCotizacion.Enabled = false;
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al generar número: " + ex.Message, "error");
        }
    }

    private void CargarProveedores()
    {
        var dt = logic.Listar_Proveedor();
        DDL_Proveedor.DataSource = dt;
        DDL_Proveedor.DataTextField = dt.Columns.Contains("Nombre") ? "Nombre" : dt.Columns[1].ColumnName;
        DDL_Proveedor.DataValueField = dt.Columns.Contains("CodProveedor") ? "CodProveedor" : dt.Columns[0].ColumnName;
        DDL_Proveedor.DataBind();
    }

    protected void GV_DetalleCotizacion_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EliminarDetalle")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtDetalle = Session["DetalleCotizacion"] as DataTable;
            if (dtDetalle != null && index < dtDetalle.Rows.Count)
            {
                dtDetalle.Rows.RemoveAt(index);
                Session["DetalleCotizacion"] = dtDetalle;

                GV_Detalle.DataSource = dtDetalle;
                GV_Detalle.DataBind();

                CalcularTotales(dtDetalle);
            }
        }
    }


    private void MostrarMensaje(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalCotizacion", script, true);
    }

    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
        if (DDL_Sucursal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(TXT_ProductoNombre.Text) || string.IsNullOrWhiteSpace(TXT_Cantidad.Text) || string.IsNullOrWhiteSpace(TXT_PrecCompra.Text))
        {
            MostrarMensaje("Complete los campos del detalle.", "warning");
            return;
        }

        DataTable dt = Session["DetalleCotizacion"] as DataTable;
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

        int cantidad = Convert.ToInt32(TXT_Cantidad.Text);
        decimal precio = Convert.ToDecimal(TXT_PrecCompra.Text);
        decimal porcentaje = 0;
        decimal.TryParse(TXT_PorcentajeDscto.Text, out porcentaje);

        decimal importe = cantidad * precio;
        decimal dscto = Math.Round(importe * (porcentaje / 100m), 2);
        decimal subtotal = importe - dscto;

        DataRow row = dt.NewRow();
        row["CodSucursal"] = DDL_Sucursal.Text.Trim();
        row["Producto"] = HDF_ProductoDescripcion.Value;
        row["NroSerie_Producto"] = HFD_NroSerie.Value;
        row["Prec_Compra"] = precio;
        row["Cantidad"] = cantidad;
        row["Importe"] = importe;
        row["Porcentaje_Dscto"] = porcentaje;
        row["Dscto"] = dscto;
        row["SubTotal"] = subtotal;
        dt.Rows.Add(row);

        Session["DetalleCotizacion"] = dt;
        GV_Detalle.DataSource = dt;
        GV_Detalle.DataBind();
        CalcularTotales(dt);
        
        DDL_Sucursal.SelectedIndex = 0;
        TXT_Cantidad.Text = "";
        TXT_PrecCompra.Text = "";
        

    }



    private void CalcularTotales(DataTable dt)
    {
        decimal importe = 0m, dscto = 0m, subtotal = 0m;
        foreach (DataRow r in dt.Rows)
        {
            importe += Convert.ToDecimal(r["Importe"]);
            dscto += Convert.ToDecimal(r["Dscto"]);
            subtotal += Convert.ToDecimal(r["SubTotal"]);
        }
        TXT_Importe.Text = importe.ToString("N2");
        TXT_Dscto.Text = dscto.ToString("N2");
        TXT_SubTotal.Text = subtotal.ToString("N2");
        LBL_Total.Text = subtotal.ToString("N2");
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        GenerarNumero();
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        TXT_NroCotizacion.Text = "";
        DDL_Proveedor.SelectedIndex = 0;
        DDL_Estado.SelectedIndex = 0;
        TXT_FecCotizacion.Text = DateTime.Now.ToString("yyyy-MM-dd");
        LBL_Total.Text = "0.00";
        TXT_Importe.Text = "0.00";
        TXT_Dscto.Text = "0.00";
        TXT_SubTotal.Text = "0.00";

        Session["DetalleCotizacion"] = null;
        GV_Detalle.DataSource = null;
        GV_Detalle.DataBind();
       
        TXT_Cantidad.Text = "";
        TXT_PrecCompra.Text = "";
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        try
        {
            // 1. Validar Usuario
            if (Session["MantenimientoUsuario_Empleado_CodEmpleado"] == null)
            {
                MostrarMensaje("La sesión ha expirado.", "warning");
                return;
            }

            // 2. Validar Detalle
            DataTable dt = Session["DetalleCotizacion"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarMensaje("Agregue al menos un producto al detalle.", "warning");
                return;
            }

            // 3. Recopilar Datos
            string nro = TXT_NroCotizacion.Text.Trim();
            string codEmpleado = HFD_CodEmpleado.Value.Trim();
            string codProveedor = DDL_Proveedor.SelectedValue;

            // TryParse es más seguro para fechas por si el campo viene vacío
            DateTime fec;
            if (!DateTime.TryParse(TXT_FecCotizacion.Text, out fec)) fec = DateTime.Now;

            // --- CORRECCIÓN IMPORTANTE AQUÍ ---
            // Quitamos el "S/." y espacios antes de convertir
            string totalLimpio = LBL_Total.Text.Replace("S/.", "").Replace("S/", "").Trim();
            decimal total = 0;
            decimal.TryParse(totalLimpio, out total);
            // ----------------------------------

            string estado = DDL_Estado.SelectedValue;

            // 4. Guardar
            string mensaje = logic.Guardar(nro, codEmpleado, codProveedor, fec, total, estado, dt);

            if (mensaje.StartsWith("OK"))
            {
                MostrarMensaje("Cotización registrada correctamente.", "success");

                // Reiniciar formulario
                LimpiarFormulario();
                GenerarNumero();
                CargarListado();
                
            }
            else
            {
                MostrarMensaje(mensaje, "error");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error crítico al grabar: " + ex.Message, "error");
        }
    }

    private void CargarListado()
    {
        var dt = logic.Listar_Cotizaciones(TXT_Buscar.Text.Trim());
        GV_Cotizaciones.DataSource = dt;
        GV_Cotizaciones.DataBind();
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        CargarListado();
    }

    protected void GV_Cotizaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Cotizaciones.PageIndex = e.NewPageIndex;
        CargarListado();
    }


    protected void GV_Cotizacion_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Atender")
        {
            string NroCotizacion = e.CommandArgument.ToString();
            Response.Redirect("Orden_Compra.aspx?NroCotizacion=" + NroCotizacion);
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

}

