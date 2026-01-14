using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Facturacion_Compra : System.Web.UI.Page
{
    FacturacionCompra_LOGIC logic = new FacturacionCompra_LOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TXT_FecEmision.Enabled= false;
            GenerarNumero();
            
            CargarTipos();
            CargarListado();




         
            if (Session["MantenimientoFacturacionCompra_NroFacturacion"] != null &&
          Session["MantenimientoFacturacionCompra_NroFacturacion"].ToString() != "")
            {
                //Session["MantenimientoFacturacion_Tipo_Transaccion"] = "ACTUALIZAR";

            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }

            // Verificar si venimos de la página de Pedidos con un código
            if (Request.QueryString["NroCompra"] != null)
            {
                string NroCompra = Request.QueryString["NroCompra"];
                TXT_NroCompra.Text = NroCompra; // Asumiendo que tienes un TextBox para el pedido
                CargarDatosDeCompra(NroCompra);
            }

        }
    }

    private void CargarDatosDeCompra(string nroCompra)
    {
        try
        {
            // 1. Llamamos a la capa lógica (Asumiendo que tienes CompraLOGIC o FacturacionLOGIC)
            // Debes crear este método en tu LOGIC que llame al SP 'USP_Compra_ObtenerDatos_ParaFactura'
         
            DataSet ds = logic.ObtenerDatosCompra_ParaFactura(nroCompra);

            // ------------------------------------------------------
            // TABLA 0: CABECERA (Datos del Proveedor y Totales)
            // ------------------------------------------------------
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                // Llenamos los HiddenFields para guardar después en Tb_Facturacion_Compra
                HF_CodEmpresa.Value = row["CodEmpresa"].ToString(); // Vital para el INSERT final

                // Mostramos datos en cajas de texto (Visual para el usuario)
                TXT_ProveedorNombre.Text = row["ProveedorNombre"].ToString();
                TXT_RUC.Text = row["ProveedorRuc"].ToString();
               // TXT_Direccion.Text = row["ProveedorDireccion"].ToString();

                // Totales calculados (Base, IGV, Total)
                TXT_SubTotal.Text = row["ImporteBase"].ToString();   // Numeric(10,2)
                TXT_IGV.Text = row["IGVCalculado"].ToString();       // Numeric(10,2)
                TXT_Total.Text = row["Total"].ToString();            // Numeric(10,2)
            }
            else
            {
                MostrarMensaje("No se encontraron datos para la Orden de Compra Nro " + nroCompra, "warning");
                return;
            }

            // ------------------------------------------------------
            // TABLA 1: DETALLE (Llenar la Grilla de productos)
            // ------------------------------------------------------
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                GV_Detalle.DataSource = ds.Tables[1];
                GV_Detalle.DataBind();

                // Opcional: Guardar en sesión si necesitas manipular el detalle antes de grabar la factura
                Session["DetalleFacturaCompra"] = ds.Tables[1];
            }
            else
            {
                GV_Detalle.DataSource = null;
                GV_Detalle.DataBind();
            }
        }
        catch (Exception ex)
        {
            // Mostrar error en pantalla si falla la conexión o lógica
            MostrarMensaje("Error al cargar datos de compra: " + ex.Message, "error");
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


    private void GenerarNumero()
    {
        TXT_NroSerie.Text = logic.Generar_NroSerie();
        TXT_NroSerie.Enabled = false;
    }

 
    private void CargarTipos()
    {
        var dt = logic.Listar_Tipo_Facturacion();
        DDL_TipoFact.DataSource = dt;
        DDL_TipoFact.DataTextField = dt.Columns.Contains("Tipo_Facturacion") ? "Tipo_Facturacion" : dt.Columns[1].ColumnName;
        DDL_TipoFact.DataValueField = dt.Columns.Contains("CodTipo_Facturacion") ? "CodTipo_Facturacion" : dt.Columns[0].ColumnName;
        DDL_TipoFact.DataBind();
    }

    private void Mostrar(string mensaje, string tipo)
    {
        string s = (mensaje ?? string.Empty).Replace("'", "\\'").Replace("\"", "\\\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        string script = $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{s}', confirmButtonText: 'Aceptar' }});";
        ScriptManager.RegisterStartupScript(this, GetType(), "SwalFactCompra", script, true);
    }

    protected void BTN_CargarCompra_Click(object sender, EventArgs e)
    {
        string nro = TXT_NroCompra.Text.Trim();
        if (string.IsNullOrEmpty(nro)) { Mostrar("Ingrese Nro de Compra.", "warning"); return; }
        var ds = logic.ObtenerDatosCompra_ParaFactura(nro);
        if (ds.Tables.Count >= 2 && ds.Tables[1].Rows.Count > 0)
        {
            DataTable det = ds.Tables[1];
            GV_Detalle.DataSource = det;
            GV_Detalle.DataBind();
            CalcularTotales(det);
            Mostrar("Orden de compra cargada.", "success");
        }
        else
        {
            Mostrar("No se encontraron datos para la compra.", "info");
        }
    }

    private void CalcularTotales(DataTable det)
    {
        decimal sub = 0m;
        foreach (DataRow r in det.Rows)
        {
            sub += Convert.ToDecimal(r["SubTotal"]);
        }
        decimal igv = Math.Round(sub * 0.18m, 2);
        decimal total = Math.Round(sub + igv, 2);
        TXT_SubTotal.Text = sub.ToString("N2");
        TXT_IGV.Text = igv.ToString("N2");
        TXT_Total.Text = total.ToString("N2");
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

    private void Limpiar()
    {
       
        DDL_TipoFact.SelectedIndex = 0;
        TXT_NroCompra.Text = "";
        TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-dd");
        TXT_SubTotal.Text = "0.00";
        TXT_IGV.Text = "0.00";
        TXT_Total.Text = "0.00";
        GV_Detalle.DataSource = null;
        GV_Detalle.DataBind();
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        try
        {
            string nroSerie = TXT_NroSerie.Text.Trim();
            string codEmpresa = HF_CodEmpresa.Value;
            string codTipo = DDL_TipoFact.SelectedValue;
            string nroCompra = TXT_NroCompra.Text.Trim();
            DateTime fec = DateTime.Parse(TXT_FecEmision.Text);
            decimal sub = Convert.ToDecimal(TXT_SubTotal.Text);
            decimal igv = Convert.ToDecimal(TXT_IGV.Text);
            decimal total = Convert.ToDecimal(TXT_Total.Text);
            string estado = "Pagada";

            string msg = logic.Guardar(nroSerie, codEmpresa, codTipo, nroCompra,fec, sub, igv, total, estado);
            if (msg.StartsWith("OK"))
            {
                Mostrar("Factura de compra registrada.", "success");
                Limpiar();
                CargarListado();
            }
            else
            {
                Mostrar(msg, "error");
            }
        }
        catch (Exception ex)
        {
            Mostrar("Error crítico: " + ex.Message, "error");
        }
    }

    private void CargarListado()
    {


        var dt = logic.listar_facturas(TXT_Buscar.Text.Trim());
        GV_Facturas.DataSource = dt;
        GV_Facturas.DataBind();
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        CargarListado();
    }

    protected void GV_Facturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Facturas.PageIndex = e.NewPageIndex;
        CargarListado();
    }
}

