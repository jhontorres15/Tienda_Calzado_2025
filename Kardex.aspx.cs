using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Kardex : System.Web.UI.Page
{
    // Referencia a Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    KardexLOGIC kardexLOGIC = new KardexLOGIC();    

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



    void Listar_Sucursal()
    {
        // 1. Llama al método de la capa lógica que SÍ devuelve un DataTable
        DataTable kardeXLOGIC = kardexLOGIC.ListarSucursales();

        // 2. Asigna el origen de datos
        DDL_Sucursal.DataSource = kardeXLOGIC;

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

    void Listar_TipoOperacion()
    {
        // 1. Llama al método de la capa lógica que SÍ devuelve un DataTable
        DataTable KARDEXLOGIC = kardexLOGIC.Listar_Tipo_Operacion();

        // 2. Asigna el origen de datos
        DDL_TipoOperacion.DataSource = KARDEXLOGIC;

        // 3. ¡ESTA ES LA PARTE IMPORTANTE!
        // Asignamos el CÓDIGO (ej: "S0001") al "Value" (valor interno)
        DDL_TipoOperacion.DataValueField = "CodTipo_Operacion";

        // Asignamos el NOMBRE (ej: "gygsports") al "Text" (lo que ve el usuario)
        DDL_TipoOperacion.DataTextField = "Tipo_Operacion";

        // 4. Carga los datos
        DDL_TipoOperacion.DataBind();

        // 5. (Opcional) Agrega un item por defecto
        DDL_TipoOperacion.Items.Insert(0, new ListItem("-- Seleccione --", ""));
    }

    // Método para limpiar los controles del formulario
    public void limpiar()
    {
        TXT_NroOperacion.Text = "";
      
        this.TXT_Fecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
        DDL_TipoOperacion.SelectedIndex = 0;
        DDL_Sucursal.SelectedIndex = 0;
        DDL_Tipo.SelectedIndex = 0;
       
        TXT_Observaciones.Text = "";
   
    }

    // Listar los productos disponibles
   


    public void listar_kardex()
    {
        try
        {
            // Verificar si la conexión está cerrada antes de abrirla para evitar error
            if (Global.CN.State == ConnectionState.Closed)
            {
                Global.CN.Open();
            }

            // Corrección de mayúsculas en Clases y Propiedades
            SqlDataAdapter da = new SqlDataAdapter("usp_listar_kardex", Global.CN);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable dt = new DataTable();
            da.Fill(dt);

            // Asignar al GridView
            GV_Kardex.DataSource = dt;
            GV_Kardex.DataBind();
        }
        catch (Exception ex)
        {
            // Manejo de errores (opcional)
            MostrarMensaje("Error de: " + ex.Message, "error");
        }
        finally
        {
            // El bloque finally se ejecuta SIEMPRE, haya error o no.
            // Aseguramos cerrar la conexión aquí.
            if (Global.CN.State == ConnectionState.Open)
            {
                Global.CN.Close();
            }
        }
    }
    public void Nuevo_NumOperacion()
    {
        KardexLOGIC logic = new KardexLOGIC();

        int codigo = logic.Generar_NroKardex();
        TXT_NroOperacion.Text = codigo.ToString();

       
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.Form.Attributes.Add("autocomplete", "off");
            limpiar();
          
            Nuevo_NumOperacion();
            listar_kardex();
            Listar_TipoOperacion();
            ListarTiposFacturacion();
            Listar_Sucursal();

           

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);

            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField 
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }


            Session["Kardex_Tipo_Transaccion"] = "GUARDAR";
        }
    }


    private void ListarTiposFacturacion()
    {
        try
        {
            // Llamamos al método que devuelve el DataTable
            DataTable dt = kardexLOGIC.Listar_Tipo_Facturacion();

            // Vinculamos los datos al DropDownList
            DDL_Tipo.DataSource = dt;
            DDL_Tipo.DataTextField = "Tipo_Facturacion";      // Lo que ve el usuario (Nombre)
            DDL_Tipo.DataValueField = "CodTipo_Facturacion";  // El código interno (T01, T02)
            DDL_Tipo.DataBind();
        }
        catch (Exception ex)
        {
            // Reemplazamos el LBL_Mensaje por la alerta moderna
            MostrarMensaje("Error al cargar tipos: " + ex.Message, "error");
        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        limpiar();
       Nuevo_NumOperacion();
        Session["Kardex_Tipo_Transaccion"] = "GUARDAR";
    }

  


    //protected void BTN_Reporte_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("Reporte_Kardex.aspx");
    //}


    protected void BTN_CargarFactura_Click(object sender, EventArgs e)
    {
 try
    {
        string nroSerie = TXT_NroSerie_Cargar.Text.Trim();
        if (string.IsNullOrEmpty(nroSerie)) 
        { 
            MostrarMensaje("Ingrese Nro de Serie.", "warning"); 
            return; 
        }

        // 1. Obtener los DataSet desde la base de datos (Usa el nuevo SP)
        var ds = kardexLOGIC.ObtenerDatosFactura(nroSerie);

        // Verificamos que vengan las 2 tablas (Cabecera y Detalle) y que la cabecera tenga datos
        if (ds.Tables.Count >= 2 && ds.Tables[0].Rows.Count > 0)
        {
            // --- PROCESAR CABECERA (Tabla 0) ---
            var cab = ds.Tables[0].Rows[0];

            // Cargar el Tipo de Facturación (Boleta, Factura, etc.)
            if (cab.Table.Columns.Contains("CodTipo_Facturacion"))
                DDL_Tipo.SelectedValue = Convert.ToString(cab["CodTipo_Facturacion"]);
            
            TXT_NroSerieFacturacion.Text = nroSerie;

            // OPCIONAL: Podrías leer la columna "Origen_Datos" si quieres saber si es VENTA o COMPRA
               string origen = cab["Origen_Datos"].ToString(); 

            // --- PROCESAR DETALLE (Tabla 1) ---
            DataTable det = ds.Tables[1];

            // NOTA: Ya no necesitamos el bucle 'foreach' para calcular costos.
            // El Stored Procedure ahora devuelve 'Costo_Unitario' y 'Costo_Total' directamente,
            // ya sea desde Ventas o desde Compras.

            // Guardamos en sesión para persistencia
            Session["DetalleKardex"] = det;

            // Enlazamos a la Grilla
            GV_DetalleKardex.DataSource = det;
            GV_DetalleKardex.DataBind();

            // Actualizamos los totales visuales del formulario
            CalcularTotalCosto(det);
            
            MostrarMensaje("Documento cargado correctamente.", "success");
        }
        else
        {
            MostrarMensaje("No se encontraron datos o el documento no está en estado 'Emitida'.", "info");
        }
    }
    catch (Exception ex)
    {
        MostrarMensaje("Error al cargar: " + ex.Message, "error");
    }
    }

    private void CalcularTotalCosto(DataTable det)
    {
        decimal total = 0m;
        foreach (DataRow r in det.Rows)
        {
            total += Convert.ToDecimal(r["Costo_Total"]);
        }
        TXT_TotalCosto.Text = total.ToString("N2");
    }

    protected void BTN_GuardarKardex_Click(object sender, EventArgs e)
    {
        try
        {
            string nroOperacion = TXT_NroOperacion.Text.Trim();
            string codEmpleado = HFD_CodEmpleado.Value.Trim();
            string tipoOperacion = DDL_TipoOperacion.SelectedValue;
            string codSucursal = DDL_Sucursal.SelectedValue;
            DateTime fecha = DateTime.Parse(TXT_Fecha.Text);
            string codTipoFact = DDL_Tipo.SelectedValue;
            string nroSerie = TXT_NroSerieFacturacion.Text.Trim();
            string obs = TXT_Observaciones.Text.Trim();

            DataTable det = Session["DetalleKardex"] as DataTable;
            if (det == null || det.Rows.Count == 0) { MostrarMensaje("No hay detalle para guardar.", "warning"); return; }

            DataTable tvp = new DataTable();
            tvp.Columns.Add("NroSerie_Producto");
            tvp.Columns.Add("Cantidad", typeof(int));
            tvp.Columns.Add("Costo_Unitario", typeof(decimal));
            tvp.Columns.Add("Costo_Total", typeof(decimal));
            foreach (DataRow r in det.Rows)
            {
                var row = tvp.NewRow();
                row["NroSerie_Producto"] = Convert.ToString(r["NroSerie_Producto"]);
                row["Cantidad"] = Convert.ToInt32(r["Cantidad"]);
                row["Costo_Unitario"] = Convert.ToDecimal(r["Costo_Unitario"]);
                row["Costo_Total"] = Convert.ToDecimal(r["Costo_Total"]);
                tvp.Rows.Add(row);
            }

            string msg = kardexLOGIC.Guardar_Kardex(nroOperacion, codEmpleado, tipoOperacion, codSucursal, fecha, codTipoFact, nroSerie, obs, tvp);
            if (msg.StartsWith("OK"))
            {
                MostrarMensaje("Kardex guardado.", "success");
                listar_kardex();
                
                limpiar();
                Nuevo_NumOperacion();
            }
            else
            {
                MostrarMensaje(msg, "error");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al guardar: " + ex.Message, "error");
        }
    }

}
