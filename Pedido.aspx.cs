using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pedido : System.Web.UI.Page
{

    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();


    ProformaLOGIC ProformaLOGIC = new ProformaLOGIC();
    PedidoLOGIC pedidoLOGIC = new PedidoLOGIC();

    // Variables para almacenar datos de proforma verificada
    private string proformaVerificada = "";
    private string pedidoAsociado = "";

    private void ListarTipoPedido()
    {
        try
        {
            // 1. Obtener los datos (usando tu método)
            DataTable dtTipos = pedidoLOGIC.ObtenerTiposPedido();

            // 2. Asignar el origen de datos al control
            DDL_TipoPedido.DataSource = dtTipos;

            // 3. Especificar qué columnas usar
            // (Basado en la imagen de Tb_Tipo_Pedido que me mostraste antes)
            DDL_TipoPedido.DataTextField = "Tipo_Pedido";     // La columna que el usuario VE
            DDL_TipoPedido.DataValueField = "CodTipo_Pedido"; // El valor que se GUARDA (ej: TPED01)

            // 4. Enlazar los datos (¡esto los carga!)
            DDL_TipoPedido.DataBind();

            // 5. Agregar el item de "Seleccione" al inicio
            //    Usamos "" como valor para que las validaciones (RequiredFieldValidator) funcionen
            DDL_TipoPedido.Items.Insert(0, new ListItem("-- Seleccione Tipo --", ""));
        }
        catch (Exception ex)
        {
            // Es buena idea tener un método para mostrar errores
            MostrarMensaje("Error al cargar tipos de pedido: " + ex.Message, "error");

          
        }
    }


    private void MostrarMensaje(string mensaje, string tipo)
    {
        // 1. LIMPIEZA AGRESIVA DE CARACTERES QUE ROMPEN JAVASCRIPT
        string mensajeLimpio = mensaje
            .Replace("'", "\\'")       // Escapar comillas simples
            .Replace("\"", "\\\"")     // Escapar comillas dobles
            .Replace("\r\n", " ")      // Quitar saltos de línea de Windows
            .Replace("\n", " ")        // Quitar saltos de línea de Linux/Mac
            .Replace("\r", " ");       // Quitar retornos de carro

        // 2. Generar el Script
        string script = $@"
    Swal.fire({{ 
        icon: '{tipo}', 
        title: 'Información', 
        text: '{mensajeLimpio}', 
        confirmButtonText: 'Aceptar' 
    }});";

        // 3. Inyectar
        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlertScript", script, true);
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

    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
  
        if (string.IsNullOrEmpty(HFD_NroSerie.Value))
        {
            MostrarMensaje("Busque y seleccione un producto.", "warning");
            return;
        }

        int cantidad = 0;
        if (!int.TryParse(TXT_Cantidad.Text, out cantidad) || cantidad <= 0)
        {
            MostrarMensaje("La cantidad debe ser un número mayor a 0.", "warning");
            return;
        }

        decimal precio = 0;
        if (!decimal.TryParse(TXT_PrecVenta.Text, out precio) || precio < 0)
        {
            MostrarMensaje("El precio cotizado no es válido.", "warning");
            return;
        }

        decimal porcentajeDscto = 0;
        if (!decimal.TryParse(TXT_PorcentajeDscto.Text, out porcentajeDscto) || porcentajeDscto < 0 || porcentajeDscto > 100)
        {
            MostrarMensaje("El porcentaje de descuento debe estar entre 0 y 100.", "warning");
            return;
        }

        int stockDisponible = 0;
        int.TryParse(HFD_StockDisponible.Value, out stockDisponible);

        if (cantidad > stockDisponible)
        {
            MostrarMensaje($"La cantidad ({cantidad}) no puede superar el stock disponible ({stockDisponible}).", "warning");
            return;
        }


   
        DataTable dt = Session["DetallePedido"] as DataTable;
        if (dt == null)
        {
            dt = new DataTable();
            dt.Columns.Add("NroSerie_Producto");
            dt.Columns.Add("ProductoNombre");
            dt.Columns.Add("CodSucursal");
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Prec_Venta", typeof(decimal));
            dt.Columns.Add("Porcentaje_Dscto", typeof(decimal));
            dt.Columns.Add("Dscto", typeof(decimal));
            dt.Columns.Add("SubTotal", typeof(decimal));
        }
       
    

        string nroSerie = HFD_NroSerie.Value;
        string sucursal = DDL_Sucursal.SelectedValue; // Asumiendo que el Value es el CodSucursal
        string ProductoNombre = TXT_ProductoNombre.Text; // El texto descriptivo


        string nroSerieParaAgregar = HFD_NroSerie.Value;
        // Recorremos la tabla en la Sesión
        foreach (DataRow drExistente in dt.Rows)
        {
            // Comparamos el NroSerie
            if (drExistente["NroSerie_Producto"].ToString().Equals(nroSerieParaAgregar))
            {
                // Si ya existe, mostramos un error y salimos
                MostrarMensaje($"El producto con serie {nroSerieParaAgregar} ya está en el detalle. No puede agregarlo dos veces.", "warning");
                return;
            }
        }

        // --- 4. Cálculos ---
        decimal importe = cantidad * precio;
        decimal descuento = importe * (porcentajeDscto / 100);
        decimal subtotal = importe - descuento;

        // --- 5. Agregar a la tabla ---
        // El orden debe coincidir con CrearTablaDetalles:
        // NroSerie_Producto, CodSucursal, Cantidad, Prec_Venta, Porcentaje_Dscto, Dscto, SubTotal, ProductoNombre
       // dt.Rows.Add(nroSerie, ProductoNombre, sucursal, cantidad, precio, porcentajeDscto, descuento, subtotal);


        DataRow row = dt.NewRow();
        row["NroSerie_Producto"] = nroSerie;
        row["ProductoNombre"] = ProductoNombre;
        row["CodSucursal"] = sucursal;
        row["Prec_Venta"] = Convert.ToDecimal(precio);
        row["Cantidad"] = cantidad;
        row["Porcentaje_Dscto"] = Convert.ToDecimal(porcentajeDscto);
        row["Dscto"] = Convert.ToDecimal(descuento);
        row["SubTotal"] = Convert.ToDecimal(subtotal);

        dt.Rows.Add(row);


        Session["DetallePedido"] = dt; // 


        // --- 6. Actualizar UI ---

        GV_DetallePedido.DataSource = dt;
        GV_DetallePedido.DataBind();

        LimpiarDetalle();
        CalcularTotal();
        UP_FormularioDetalle.Update();

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
            TXT_PrecVenta.Text = precioMenor;
            // Guarda los datos clave en los HiddenFields
            HFD_CodProducto.Value = codProducto;
            HFD_NroSerie.Value = nroSerie;
            HFD_StockDisponible.Value = stock;

            // Por defecto, usamos el PRECIO MENOR como el precio cotizado
            HFD_PrecioUnitario.Value = precioMenor;
            // También lo ponemos en el TextBox de Precio Cotizado
           // TXT_PrecCotizado.Text = precioMenor;

            // Guardamos el precio mayor por si lo necesitas para descuentos
            HFD_PrecioMayor.Value = precioMayor;

            // También actualizamos los Labels de la página principal
          //  LBL_PrecMenor.Text = precioMenor;
            //LBL_PrecMayor.Text = precioMayor;

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


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["DetallePedido"] = null;

            CargarPedidos();
            TXT_FecPedido.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            Listar_Sucursal();
            ListarTipoPedido();
            Nuevo();
            LimpiarFormulario();

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);


            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField (para guardar en BD)
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            // 3. LÓGICA AUTOMÁTICA: Si venimos de "Atender Proforma"
            if (Request.QueryString["nroProforma"] != null)
            {
                string proformaURL = Request.QueryString["nroProforma"];
                TXT_NroProformaVerificar.Text = proformaURL;

                // Disparamos la carga automáticamente
                ProcesarCargaDeProforma(proformaURL);
            }
        }
    }




    private void CargarPedidos()
    {
        try
        {
            DataTable dt = pedidoLOGIC.ListarPedidos();
            GV_Pedidos.DataSource = dt;
            GV_Pedidos.DataBind();
        }
        catch (Exception ex)
        {
            // Puedes usar tu método MostrarMensaje si lo tienes en esta página
             MostrarMensaje("Error al cargar lista: " + ex.Message, "error");
            
        }
    }





    // Método auxiliar para sumarizar los totales de la DataTable
    private void CalcularTotalesDesdeTabla(DataTable dt)
    {
        decimal totalImporte = 0;
        decimal totalDescuento = 0;
        decimal totalGeneral = 0;

        // Recorremos las filas que trajimos de la Proforma
        foreach (DataRow row in dt.Rows)
        {
            // Obtenemos cantidad y precio para el importe bruto
            decimal cantidad = Convert.ToDecimal(row["Cantidad"]);
            decimal precio = Convert.ToDecimal(row["Prec_Venta"]);

            // Sumamos al acumulador
            totalImporte += (cantidad * precio);

            // Sumamos descuentos (En tu SP actual esto viene en 0, pero lo dejamos listo)
            totalDescuento += Convert.ToDecimal(row["Dscto"]);

            // Sumamos el SubTotal final de la fila
            totalGeneral += Convert.ToDecimal(row["SubTotal"]);
        }

        // Asignar a las etiquetas (Labels) de tu diseño HTML
        // Usamos "N2" para que siempre muestre 2 decimales (ej: 150.00)
        LBL_Importe.Text = totalImporte.ToString("N2");
        LBL_Descuento.Text = totalDescuento.ToString("N2");

        // En este caso el SubTotal y Total Pedido son iguales porque no estamos desglosando IGV aquí,
        // sino en la facturación.
        LBL_SubTotal.Text = totalGeneral.ToString("N2");
        LBL_TotalPedido.Text = "S/ " + totalGeneral.ToString("N2");
    }


    private void ProcesarCargaDeProforma(string nroProforma)
    {
        try
        {
            // Validar Sucursal (Es vital para el stock)
            if (DDL_Sucursal.SelectedValue == "0" || DDL_Sucursal.SelectedValue == "")
            {
                MostrarMensaje("Seleccione la sucursal para verificar el stock disponible.", "warning");
                return;
            }

            string codSucursal = DDL_Sucursal.SelectedValue;
            string mensajeRespuesta = "";

            // 1. LLAMADA A LA BASE DE DATOS
            // Retorna un DataSet con 2 tablas y un mensaje de salida
            DataSet ds = pedidoLOGIC.CargarDetalle_DesdeProforma(nroProforma, codSucursal, out mensajeRespuesta);

            // 2. VALIDACIÓN DE SEGURIDAD (CLIENTE BLOQUEADO)
            if (mensajeRespuesta.StartsWith("ERROR"))
            {
                MostrarMensaje(mensajeRespuesta, "error");
                // Limpiamos para evitar procesar datos inválidos
                TXT_Cliente.Text = "";
                GV_DetallePedido.DataSource = null;
                GV_DetallePedido.DataBind();
                return;
            }

            // 3. CARGAR DATOS DEL CLIENTE (Tabla 0)
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow rowCliente = ds.Tables[0].Rows[0];
                TXT_Cliente.Text = rowCliente["NombreCliente"].ToString();
                HFD_CodCliente.Value = rowCliente["CodCliente"].ToString();
            }

            // 4. CARGAR GRILLA DE PRODUCTOS (Tabla 1)
            if (ds.Tables[1].Rows.Count > 0)
            {
                GV_DetallePedido.DataSource = ds.Tables[1];
                GV_DetallePedido.DataBind();

                // Guardar en ViewState para persistencia
                Session["DetallePedido"] = ds.Tables[1];

                // Calcular totales visuales
                CalcularTotalesDesdeTabla(ds.Tables[1]);

                // 5. MOSTRAR RESULTADO (Advertencias de Stock vs Éxito)
                if (mensajeRespuesta == "OK")
                {
                    MostrarMensaje("Datos cargados correctamente. Stock completo.", "success");
                }
                else
                {
                    // Si no es OK ni ERROR, es una ADVERTENCIA de stock incompleto
                    MostrarMensaje(mensajeRespuesta, "warning");
                }
            }
            else
            {
                MostrarMensaje("La proforma no tiene items con stock disponible en esta sucursal.", "error");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al procesar proforma: " + ex.Message, "error");
        }
    }





    

    protected void GV_DetallePedido_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EliminarDetalle")
        {
            //int index = Convert.ToInt32(e.CommandArgument);
            //DataTable dt = (DataTable)ViewState["DetallesPedido"];
            //dt.Rows.RemoveAt(index);

            //ViewState["DetallePedido"] = dt;
            //GV_DetallePedido.DataSource = dt;
            //GV_DetallePedido.DataBind();

            //CalcularTotal();
        }
    }

    protected void GV_DetallePedido_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_DetallePedido.PageIndex = e.NewPageIndex;
        GV_DetallePedido.DataSource = (DataTable)ViewState["DetallesPedido"];
        GV_DetallePedido.DataBind();
    }

  

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        Nuevo();
        CargarPedidos();
        MostrarMensaje("Formulario limpio para nuevo pedido", "alert-info");
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e) 
    {
        string mensaje = "";

        try
        {
            // --- 1. VALIDACIONES DEL FORMULARIO ---
            if (string.IsNullOrEmpty(HFD_CodCliente.Value) || HFD_CodCliente.Value == "0")
            {
                mensaje = "Debe seleccionar un Cliente.";
                MostrarMensaje(mensaje, "warning");
            }

            // Validar que haya productos en sesión
            DataTable dtDetalle = Session["DetallePedido"] as DataTable;
            if (mensaje == "" && (dtDetalle == null || dtDetalle.Rows.Count == 0))
            {
                mensaje = "Debe agregar al menos un producto al pedido.";
                MostrarMensaje(mensaje, "warning");
                return;
            }

            // Validar Fecha
            if (mensaje == "" && string.IsNullOrEmpty(TXT_FecPedido.Text))
            {
                mensaje = "La Fecha del Pedido no puede estar vacía.";
                MostrarMensaje(mensaje, "warning");
                TXT_FecPedido.Focus();
                return;
            }

            if (DDL_TipoPedido.SelectedValue == "0")
            {
                mensaje = "Debe seleccionar un tipo de pedido.";
            }


            // --- 2. RECOLECTAR DATOS (Solo si no hay errores) ---
            if (mensaje == "")
            {
                // Recolectar datos variables
                string nroPedido = TXT_NroPedido.Text;
                string codCliente = HFD_CodCliente.Value;
                string codEmpleado = HFD_CodEmpleado.Value;
                string codSucursal = DDL_Sucursal.SelectedValue;
                string codTipoPedido = DDL_TipoPedido.SelectedValue;

                DateTime fecPedido = Convert.ToDateTime(TXT_FecPedido.Text);
                string estado = "Pendiente";

                // Recuperar el DataTable de la sesión
                DataTable dt = Session["DetallePedido"] as DataTable;

                // Parsear Total
                decimal total = decimal.Parse(LBL_TotalPedido.Text.Replace("S/", "").Trim());

                // --- 3. GUARDAR EL PEDIDO PRINCIPAL ---
                PedidoLOGIC objLogic = new PedidoLOGIC();

                mensaje = objLogic.RegistrarPedido(nroPedido, codSucursal, codTipoPedido,
                                                   codCliente, codEmpleado, fecPedido,
                                                   total, estado, dt);

                // --- 4. SI EL PEDIDO SE GUARDÓ BIEN, ACTUALIZAMOS LA PROFORMA ---
                if (mensaje.Contains("correctamente"))
                {
                    // Verificamos si este pedido vino de una Proforma (si la caja de texto no está vacía)
                    if (!string.IsNullOrEmpty(TXT_NroProformaVerificar.Text))
                    {
                        // Llamamos al método nuevo que creamos anteriormente
                        bool proformaActualizada = objLogic.GuardarRelacionProforma(TXT_NroProformaVerificar.Text, nroPedido);

                        if (proformaActualizada)
                        {
                            mensaje += " Y Proforma actualizada a 'Aprobada'.";
                        }
                    }
                }
            }
        }
        catch (FormatException fex)
        {
            mensaje = "Error de formato numérico o fecha: " + fex.Message;
        }
        catch (Exception ex)
        {
            mensaje = "Error inesperado: " + ex.Message;
        }

        // --- 5. MOSTRAR RESULTADO FINAL ---
        if (mensaje.Contains("correctamente") || mensaje.Contains("registrado"))
        {
            MostrarMensaje(mensaje, "success");

            LimpiarFormulario();   // Borra cajas de texto y Session["DetallePedido"]
            Nuevo();               // Genera el siguiente código PED...
        }
        else
        {
            if (!string.IsNullOrEmpty(mensaje))
                MostrarMensaje(mensaje, "error");
        }

    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        MostrarMensaje("Operación cancelada", "alert-info");
    }

    protected void BTN_Imprimir_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TXT_NroPedido.Text.Trim()))
        {
            MostrarMensaje("Seleccione un pedido para imprimir", "alert-warning");
            return;
        }

        // Simulación de impresión
        string script = "alert('Función de impresión - Pedido: " + TXT_NroPedido.Text + "');";
        ClientScript.RegisterStartupScript(this.GetType(), "Imprimir", script, true);
    }



    void CalcularTotal()
    {
        decimal totalAcumulado = 0;

        // 1. Verificamos si existe la sesión y si tiene datos
        if (Session["DetallePedido"] != null)
        {
            DataTable dt = (DataTable)Session["DetallePedido"];

            // 2. Recorremos cada fila para sumar
            foreach (DataRow fila in dt.Rows)
            {
                // NOTA: Asegúrate que en tu DataTable la columna se llame "Total", "Importe" o "SubTotal"
                // Si usas otro nombre, cámbialo aquí abajo ↓
                totalAcumulado += Convert.ToDecimal(fila["SubTotal"]);
            }
        }

        // 3. Actualizamos la etiqueta visualmente
        LBL_TotalPedido.Text = "S/ " + totalAcumulado.ToString("N2");
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        // Simulación de búsqueda
        CargarPedidos(); // En producción filtrar por TXT_Buscar.Text
        MostrarMensaje("Búsqueda realizada", "alert-info");
    }

    protected void GV_Pedidos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string nroPedido = e.CommandArgument.ToString();

        if (e.CommandName == "EditarPedido")
        {
           
            MostrarMensaje($"Pedido {nroPedido} cargado para edición", "alert-info");
        }
        else if (e.CommandName == "Atender")
        {
            Response.Redirect($"Facturacion_Venta.aspx?nroPedido=" + nroPedido);
          
        }
    }
    public string ObtenerClaseEstado(string estado)
    {
        switch (estado)
        {
            case "Pendiente":
                return "estado-pendiente";
            case "En Proceso":
                return "estado-proceso";
            case "Completado":
                return "estado-completado";
            case "Cancelado":
                return "estado-cancelado";
            case "Entregado":
                return "estado-entregado";
            default:
                return "";
        }
    }


    // solo par aque fincione sin ningun probolema





    // Botón manual "Cargar Datos"
    protected void BTN_CargarProforma_Click(object sender, EventArgs e)
    {
        string nro = TXT_NroProformaVerificar.Text.Trim();
        if (!string.IsNullOrEmpty(nro))
        {
            ProcesarCargaDeProforma(nro);
        }
        else
        {
            MostrarMensaje("Ingrese un número de proforma.", "warning");
        }
    }


    private void LimpiarFormulario()
    {
        // 1. Limpiar Controles de Cabecera
        HFD_CodCliente.Value = "0";
        TXT_Cliente.Text = "";

        // Opcional: Si quieres resetear la fecha a "Ahora"
        TXT_FecPedido.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

        if (DDL_TipoPedido.Items.Count > 0) DDL_TipoPedido.SelectedIndex = 0;
        if (DDL_Sucursal.Items.Count > 0) DDL_Sucursal.SelectedIndex = 0;

        // 2. Limpiar Controles de Detalle (Cajitas de agregar producto)
        LimpiarDetalle();

        // 3. Limpiar Totales Visuales
        LBL_Importe.Text = "0.00";
        // Si tienes un label de Total en cabecera:
        // LBL_TotalPedido.Text = "0.00"; 

        // 4. ¡IMPORTANTE! Matar la Sesión del detalle
        Session["DetallePedido"] = null;

        // 5. Limpiar el GridView visualmente
        GV_DetallePedido.DataSource = null;
        GV_DetallePedido.DataBind();
        TXT_NroProformaVerificar.Text = "";
        // 6. Actualizar los UpdatePanels (Si usas AJAX)
        UP_Cliente.Update();
        UP_FormularioDetalle.Update();

        // 7. Generar nuevo código (Llamamos a la lógica para obtener el siguiente PED-XXX)
        Nuevo();
    }

    // Método auxiliar para limpiar solo las cajitas de producto (ya lo tenías, pero aseguramos)
    private void LimpiarDetalle()
    {
        HFD_CodProducto.Value = "";
        HFD_NroSerie.Value = "";
        HFD_StockDisponible.Value = "";
  
        TXT_ProductoNombre.Text = "";
        TXT_Cantidad.Text = "1";
        TXT_PrecVenta.Text = "";
        TXT_PorcentajeDscto.Text = "0";

        TXT_ProductoNombre.Focus();
    }


    private void Nuevo()
    {
        try
        {
            // Llamamos a tu capa lógica que conecta con el DAO
            TXT_NroPedido.Text = pedidoLOGIC.GenerarNroPedido();
        }
        catch (Exception ex)
        {
          
            MostrarMensaje("Error al generar serie: " + ex.Message, "error");
        }
    }

}