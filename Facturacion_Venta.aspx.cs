using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Facturacion_Venta : System.Web.UI.Page
{

    FacturacionLOGIC FacturacionLOGIC = new FacturacionLOGIC();


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            CargarListadoFacturas();
            ListarTiposFacturacion();
            TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

            GenerarNumeroSerie();

            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);


            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField (para guardar en BD)
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            if (Session["MantenimientoFacturacion_NroFacturacion"] != null &&
          Session["MantenimientoFacturacion_NroFacturacion"].ToString() != "")
            {
                Session["MantenimientoFacturacion_Tipo_Transaccion"] = "ACTUALIZAR";
                
            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);
            }

            // Verificar si venimos de la página de Pedidos con un código
            if (Request.QueryString["nroPedido"] != null)
            {
                string nroPedido = Request.QueryString["nroPedido"];
                TXT_Pedido.Text = nroPedido; // Asumiendo que tienes un TextBox para el pedido
                CargarDatosDelPedido(nroPedido);
            }
        }
    }


    private void CargarDatosDelPedido(string nroPedido)
    {
        try
        {
           

            // 1. Llamamos al método nuevo
            DataSet ds = FacturacionLOGIC.ObtenerDatosPedido_ParaFactura(nroPedido);

            // ------------------------------------------------------
            // TABLA 0: CABECERA (Llenar HiddenFields y Totales)
            // ------------------------------------------------------
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];

                // Datos ocultos para guardar después
                HF_CodCliente.Value = row["CodCliente"].ToString();
                HF_ClienteNombre.Value = row["ClienteNombre"].ToString();
                HF_ClienteDocNumero.Value = row["ClienteDocNumero"].ToString();
                HF_ClienteDireccion.Value = row["ClienteDireccion"].ToString();

                // Totales calculados desde SQL
                // Fíjate que usamos los nombres EXACTOS que pusiste en el SP con 'AS'
                TXT_Importe.Text = row["ImporteBase"].ToString();
                TXT_IGV.Text = row["IGVCalculado"].ToString();
                TXT_Total.Text = row["Total"].ToString();
            }

            // ------------------------------------------------------
            // TABLA 1: DETALLE (Llenar la Grilla)
            // ------------------------------------------------------
            if (ds.Tables[1].Rows.Count > 0)
            {
                GV_Detalle.DataSource = ds.Tables[1];
                GV_Detalle.DataBind();
            }
            else
            {
                GV_Detalle.DataSource = null;
                GV_Detalle.DataBind();
            }
        }
        catch (Exception ex)
        {
            // Mostrar error en pantalla si falla
            MostrarMensaje(ex.Message,"error"); 
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

    protected void BTN_Facturar_Click(object sender, EventArgs e)
    {
        try
        {
            // 1. VALIDACIÓN DE DATOS (Evita el error de referencia nula)

            // Validar Sesión de Empleado (El error más común)
            if (Session["MantenimientoUsuario_Empleado_CodEmpleado"] == null)
            {
                MostrarMensaje("La sesión del usuario ha expirado. Por favor, inicie sesión nuevamente.", "warning");
                return; // Detiene el proceso
            }

            // Validar Campos de Texto Vacíos
            if (string.IsNullOrEmpty(TXT_NroSerie.Text))
            {
                MostrarMensaje("Error: No se ha generado el Número de Serie.", "warning");
                return;
            }

            if (DDL_Tipo.SelectedValue == "0" || DDL_Tipo.SelectedValue == "")
            {
                MostrarMensaje("Por favor, seleccione el Tipo de Comprobante.", "warning");
                return;
            }

            if (string.IsNullOrEmpty(TXT_Pedido.Text))
            {
                MostrarMensaje("Error: No hay un pedido cargado para facturar.", "warning");
                return;
            }

            // Validar Montos (Evitar error de conversión si están vacíos)
            if (string.IsNullOrEmpty(TXT_Importe.Text) || string.IsNullOrEmpty(TXT_IGV.Text) || string.IsNullOrEmpty(TXT_Total.Text))
            {
                MostrarMensaje("Error: Los montos de la venta no se han calculado correctamente.", "warning");
                return;
            }


            // Recoger datos de los controles
            string serie = TXT_NroSerie.Text;
            string tipoDoc = DDL_Tipo.SelectedValue;
            string empleado = Session["MantenimientoUsuario_Empleado_CodEmpleado"].ToString();
            string pedido = TXT_Pedido.Text;

            decimal subTotal = Convert.ToDecimal(TXT_Importe.Text);
            decimal igv = Convert.ToDecimal(TXT_IGV.Text);
            decimal total = Convert.ToDecimal(TXT_Total.Text);

            // Llamar a la lógica
            bool exito = FacturacionLOGIC.Registrar_Facturacion(serie, tipoDoc, empleado, pedido, subTotal, igv, total);

            if (exito)
            {
                // CASO ÉXITO: Icono verde ("success")
                MostrarMensaje("Factura Registrada y Pedido Actualizado Correctamente", "success");

                // NOTA: Si quieres limpiar los campos después de guardar, hazlo aquí.
               
                TXT_Pedido.Text = "";
                GV_Detalle.DataSource = null;
                GV_Detalle.DataBind();
                CargarListadoFacturas();
            }
            else
            {
                //CASO ERROR DE LÓGICA: Icono rojo("error")
                  MostrarMensaje("No se pudo registrar la factura. Verifique los datos.", "error");
            }
        }
        catch (Exception ex)
        {
            // CASO ERROR TÉCNICO (CRASH): Icono rojo ("error")
            MostrarMensaje("Error crítico del sistema: " + ex.Message, "error");
        }
    }


    private void CargarListadoFacturas()
    {
        try
        {
            // 1. Llamamos al método que creaste en la Capa de Negocio
            DataTable dt = FacturacionLOGIC.Listar_Facturas_Venta();

            // 2. Verificamos si hay datos
            if (dt != null && dt.Rows.Count > 0)
            {
                // 3. Vinculamos los datos al GridView
                GV_Facturas.DataSource = dt;
                GV_Facturas.DataBind();

                // (Opcional) Mostrar cantidad de registros
                // LBL_TotalRegistros.Text = "Total de Facturas: " + dt.Rows.Count.ToString();
            }
            else
            {
                // Si no hay datos, limpiamos la grilla (mostrará el EmptyDataText)
                GV_Facturas.DataSource = null;
                GV_Facturas.DataBind();
            }
        }
        catch (Exception ex)
        {
            // Manejo de error visual
            Response.Write("<script>alert('Error al cargar el listado: " + ex.Message + "');</script>");
        }
    }

    private void ListarTiposFacturacion()
    {
        try
        {
            // Llamamos al método que devuelve el DataTable
            DataTable dt = FacturacionLOGIC.Listar_Tipo_Facturacion();

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

    private void GenerarNumeroSerie()
    {
        try
        {
            // Llamamos al método que genera el código (VENT000001)
            string serieGenerada = FacturacionLOGIC.Generar_NroSerie_Facturacion();

            // Asignamos al TextBox y lo bloqueamos para que no lo editen
            TXT_NroSerie.Text = serieGenerada;
            TXT_NroSerie.Enabled = false; // O ReadOnly = true
        }
        catch (Exception ex)
        {
          

            // Reemplazamos el LBL_Mensaje por la alerta moderna
            MostrarMensaje("Error al generar serie: " + ex.Message, "error");
        }
    }


    



    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
       
        LimpiarFormulario();
        GenerarNumeroSerie();

    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
        LBL_Mensaje.Text = string.Empty;
    }

   

    private bool Validar()
    {
        LBL_Mensaje.CssClass = "text-danger";
        if (string.IsNullOrWhiteSpace(TXT_NroSerie.Text)) { LBL_Mensaje.Text = "Ingrese Nro. Serie."; return false; }
        if (string.IsNullOrEmpty(DDL_Tipo.SelectedValue)) { LBL_Mensaje.Text = "Seleccione Tipo de Facturación."; return false; }
        if (string.IsNullOrEmpty(TXT_Pedido.Text)) { LBL_Mensaje.Text = "Seleccione Nro. Pedido."; return false; }
        
       
        return true;
    }

    private void GenerarPdfComprobante()
    {
        //// Intenta usar iTextSharp; si no está disponible, genera HTML como fallback
        //string appData = Server.MapPath("~/App_Data");
        //Directory.CreateDirectory(appData);
        //string nombreArchivo = $"{DDL_Tipo.SelectedItem.Text}_{TXT_NroSerie.Text}_{TXT_Pedido.Text}.pdf";
        //string rutaPdf = Path.Combine(appData, nombreArchivo);

        //try
        //{
        //    // Necesita paquete iTextSharp instalado en el proyecto
        //    // using iTextSharp.text; using iTextSharp.text.pdf;
        //    var dtDetalle = (DataTable)ViewState[VS_DETALLE];
        //    dynamic doc = Activator.CreateInstance(Type.GetType("iTextSharp.text.Document, itextsharp"));
        //    dynamic writer = Activator.CreateInstance(Type.GetType("iTextSharp.text.pdf.PdfWriter, itextsharp"));
        //    var fs = new FileStream(rutaPdf, FileMode.Create);
        //    writer = writer.GetInstance(doc, fs);
        //    doc.Open();

        //    var titulo = $"{DDL_Tipo.SelectedItem.Text} - Serie: {TXT_NroSerie.Text}";
        //    var pTitulo = Activator.CreateInstance(Type.GetType("iTextSharp.text.Paragraph, itextsharp"), titulo);
        //    doc.Add(pTitulo);
        //    var pInfo = Activator.CreateInstance(Type.GetType("iTextSharp.text.Paragraph, itextsharp"), $"Pedido: {TXT_Pedido.Text}    Fecha: {TXT_FecEmision.Text}");
        //    doc.Add(pInfo);

        //    // Tabla detalle
        //    dynamic pdfTable = Activator.CreateInstance(Type.GetType("iTextSharp.text.pdf.PdfPTable, itextsharp"), 5);
        //    pdfTable.AddCell("Código"); pdfTable.AddCell("Producto"); pdfTable.AddCell("Cant."); pdfTable.AddCell("Precio"); pdfTable.AddCell("Importe");
        //    foreach (DataRow r in dtDetalle.Rows)
        //    {
        //        pdfTable.AddCell(r["CodProducto"].ToString());
        //        pdfTable.AddCell(r["Producto"].ToString());
        //        pdfTable.AddCell(r["Cantidad"].ToString());
        //        pdfTable.AddCell(Convert.ToDecimal(r["Precio"]).ToString("N2"));
        //        pdfTable.AddCell(Convert.ToDecimal(r["Importe"]).ToString("N2"));
        //    }
        //    doc.Add(pdfTable);

        //    // Totales
        //    var pTot = Activator.CreateInstance(Type.GetType("iTextSharp.text.Paragraph, itextsharp"), $"SubTotal: {TXT_Importe.Text}   IGV: {TXT_IGV.Text}   Total: {TXT_Total.Text}");
        //    doc.Add(pTot);

        //    doc.Close(); fs.Close();
        //}
        //catch
        //{
        //    // Fallback: generar HTML imprimible
        //    string rutaHtml = Path.Combine(appData, $"{DDL_Tipo.SelectedItem.Text}_{TXT_NroSerie.Text}_{TXT_Pedido.Text}.html");
        //    var dtDetalle = (DataTable)ViewState[VS_DETALLE];
        //    using (var sw = new StreamWriter(rutaHtml))
        //    {
        //        sw.WriteLine("<html><head><meta charset='utf-8'><title>Comprobante</title>");
        //        sw.WriteLine("<style>body{font-family:Arial} table{border-collapse:collapse;width:100%} td,th{border:1px solid #ccc;padding:6px} .tot{margin-top:10px}</style></head><body>");
        //        sw.WriteLine($"<h2>{DDL_Tipo.SelectedItem.Text} - Serie: {TXT_NroSerie.Text}</h2>");
        //        sw.WriteLine($"<p>Pedido: {TXT_Pedido.Text} &nbsp;&nbsp; Fecha: {TXT_FecEmision.Text}</p>");
        //        sw.WriteLine("<table><thead><tr><th>Código</th><th>Producto</th><th>Cant.</th><th>Precio</th><th>Importe</th></tr></thead><tbody>");
        //        foreach (DataRow r in dtDetalle.Rows)
        //        {
        //            sw.WriteLine($"<tr><td>{r["CodProducto"]}</td><td>{r["Producto"]}</td><td>{r["Cantidad"]}</td><td>{Convert.ToDecimal(r["Precio"]).ToString("N2")}</td><td>{Convert.ToDecimal(r["Importe"]).ToString("N2")}</td></tr>");
        //        }
        //        sw.WriteLine("</tbody></table>");
        //        sw.WriteLine($"<div class='tot'><strong>SubTotal:</strong> {TXT_Importe.Text} &nbsp; <strong>IGV:</strong> {TXT_IGV.Text} &nbsp; <strong>Total:</strong> {TXT_Total.Text}</div>");
        //        sw.WriteLine("</body></html>");
        //    }
        //}
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {

    }

    protected void GV_Facturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //GV_Facturas.PageIndex = e.NewPageIndex;
        //var dt = (DataTable)ViewState[VS_FACTURAS];
        //GV_Facturas.DataSource = dt; GV_Facturas.DataBind();
    }

    protected void GV_Facturas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // Verificar que el comando sea el correcto
        if (e.CommandName == "Atender")
        {
            try
            {
                // 1. Obtener el argumento (NroPedido o NroSerie según configuraste)


                string nroSerieFactura = e.CommandArgument.ToString();

                // 2. Redirigir al formulario de Pago
                // Enviamos el código por URL para que la otra página sepa qué cobrar
                Response.Redirect("Registro_Pago.aspx?nroSerie=" + nroSerieFactura);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al intentar registrar pago: " + ex.Message, "error");
            }
        }

        // --- Lógica para el botón IMPRIMIR ---
        if (e.CommandName == "Imprimir")
        {
            string nroSerieFactura = e.CommandArgument.ToString();

            if (string.IsNullOrEmpty(nroSerieFactura))
            {
                MostrarMensaje("Error: No se pudo obtener el número de factura.", "error");
                return;
            }

            try
            {
                // 2. Abrir la ventana de impresión DIRECTAMENTE
                // Pasamos el ID por URL. La otra página se encargará de buscar los datos.
                string script = "window.open('ImprimirComprobante.aspx?serie=" + nroSerieFactura + "', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenPrintWindow", script, true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al abrir impresión: " + ex.Message, "error");
            }
        }
        }

    private void LimpiarFormulario()
    {
        TXT_NroSerie.Text = "";
        DDL_Tipo.SelectedIndex = 0;
       
        TXT_Pedido.Text = "";
        TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-dd");
      
 
      
        TXT_Importe.Text = "0.00"; TXT_IGV.Text = "0.00"; TXT_Total.Text = "0.00";
    }

   






}