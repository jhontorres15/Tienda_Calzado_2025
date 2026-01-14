using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Registro_Pago : System.Web.UI.Page
{

    Registro_PagoLOGIC Registro_PagoLOGIC = new Registro_PagoLOGIC();

    protected void Page_Load(object sender, EventArgs e)
    {

   
        if (!IsPostBack)
        {
            
            CargarFormasPago();
            TXT_Fecha.Text = DateTime.Now.ToString("yyyy-MM-dd");

            CargarListadoPagos();


            string codEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_CodEmpleado"]);
            string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);


            if (!string.IsNullOrEmpty(codEmpleadoSesion) && !string.IsNullOrEmpty(nombreEmpleadoSesion))
            {
                // Asignar el nombre al TextBox (visible)
                TXT_Empleado.Text = nombreEmpleadoSesion.Trim();

                // Asignar el ID al HiddenField (para guardar en BD)
                HFD_CodEmpleado.Value = codEmpleadoSesion.Trim();
            }

            if (Session["MantenimientoRegistroPago_NroPago"] != null &&
                Session["MantenimientoRegistroPago_NroPago"].ToString() != "")
            {
                Session["MantenimientoRegistro_Tipo_Transaccion"] = "ACTUALIZAR";

            }
            else
            {
                //Invocar al Evento: BTN_Nuevo_Click
                this.BTN_Nuevo_Click(null, null);

                GenerarNroPago();
            }

            // Verificar si venimos de la página de Pedidos con un código
            if (Request.QueryString["nroSerie"] != null)
            {
                string nroFactura = Request.QueryString["nroSerie"];
                TXT_NroSerie.Text = nroFactura; // Asumiendo que tienes un TextBox para el pedido
                GenerarNroPago();
            }

        }
    }


    private void GenerarNroPago()
    {
        try
        {
            // 1. Llamar a la capa de negocio
            int nuevoId = Registro_PagoLOGIC.Generar_NroPago();

            // 2. Mostrar en el TextBox
            // (ToString("D6") rellena con ceros: 000001, 000002...)
            TXT_NroPago.Text = nuevoId.ToString("D6");
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error al generar ID de Pago: " + ex.Message + "');</script>");
        }
    }

    private void CargarFormasPago()
    {
        try
        {
      
            DataTable dt = Registro_PagoLOGIC.Listar_Forma_Pago();

            // 2. Configurar el DropDownList
            DDL_FormaPago.DataSource = dt;
            DDL_FormaPago.DataTextField = "Forma_Pago";      // Lo que ve el usuario (Nombre)
            DDL_FormaPago.DataValueField = "CodForma_Pago";  // El código interno (EFE, YAP)
            DDL_FormaPago.DataBind();

            // 3. Agregar la opción por defecto al inicio
            DDL_FormaPago.Items.Insert(0, new ListItem("-- Seleccione --", "0"));
        }
        catch (Exception ex)
        {
            // Mostrar error si falla la carga
            LBL_Mensaje.CssClass = "alert alert-danger";
            LBL_Mensaje.Text = "Error al cargar formas de pago: " + ex.Message;
        }
    }

    protected void BTN_Verificar_Click(object sender, EventArgs e)
    {
        try
        {
            var tipo = DDL_FormaPago.SelectedValue;
            decimal importe = 0;
            decimal.TryParse(TXT_Importe.Text, out importe);
            var referencia = TXT_Referencia.Text;

            string mensaje;
            bool ok = PaymentVerificationService.Verify(tipo, importe, referencia, out mensaje);

            LBL_Mensaje.CssClass = ok ? "alert alert-success" : "alert alert-danger";
            LBL_Mensaje.Text = mensaje;
         
        }
        catch (Exception ex)
        {
            LBL_Mensaje.CssClass = "alert alert-danger";
            LBL_Mensaje.Text = "Error: " + ex.Message;
           
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

    protected void BTN_Registrar_Click(object sender, EventArgs e)
    {
        try
        {
            // 1. Validaciones Básicas
            if (string.IsNullOrEmpty(TXT_NroSerie.Text))
            {
                MostrarMensaje("No se ha especificado una Factura para pagar.", "warning");
                return;
            }
            if (DDL_FormaPago.SelectedValue == "0")
            {
                MostrarMensaje("Seleccione una Forma de Pago.", "warning");
                return;
            }

            // 2. Obtener Datos
            int nroPago = int.Parse(TXT_NroPago.Text); // El número generado
            string codEmpleado = HFD_CodEmpleado.Value;
            string nroSerieFactura = TXT_NroSerie.Text;
            string codFormaPago = DDL_FormaPago.SelectedValue;

            decimal importe = 0;
            if (!decimal.TryParse(TXT_Importe.Text, out importe) || importe <= 0)
            {
                MostrarMensaje("Ingrese un importe válido mayor a 0.", "warning");
                return;
            }

            // Obtener Fecha (Si falla, usa hoy)
            DateTime fechaPago;
            if (!DateTime.TryParse(TXT_Fecha.Text, out fechaPago))
            {
                fechaPago = DateTime.Now;
            }

            // 3. Llamar a la Lógica
            string respuesta = Registro_PagoLOGIC.Registrar_Pago_Validado(nroPago, codEmpleado, nroSerieFactura, codFormaPago, importe, fechaPago);

            // 4. Evaluar Respuesta
            if (respuesta.StartsWith("OK"))
            {
                // ÉXITO (Puede ser Pago Completo o Parcial)
                MostrarMensaje(respuesta, "success");

                // Bloqueamos para evitar doble click
                BTN_Registrar.Enabled = false;

                // Refrescar el listado
                CargarListadoPagos();

            }
            else if (respuesta.StartsWith("ERROR"))
            {
                // ERROR DE LÓGICA (Ej: Sobrepago, Factura no existe)
                MostrarMensaje(respuesta, "error");
            }
            else
            {
                MostrarMensaje(respuesta, "info");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error crítico: " + ex.Message, "error");
        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();



        GenerarNroPago();
    }

    private void LimpiarFormulario()
    {
      
       
        TXT_NroSerie.Text = "";
        TXT_Fecha.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        DDL_FormaPago.SelectedIndex = 0;
        TXT_Importe.Text = string.Empty;
  
        TXT_Referencia.Text = string.Empty;
    }

    private void CargarListadoPagos(string filtro = "")
    {
        try
        {
            DataTable dt = Registro_PagoLOGIC.Listar_Pagos(filtro);
            GV_Pagos.DataSource = dt;
            GV_Pagos.DataBind();
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al listar pagos: " + ex.Message, "error");
        }
    }

    // 3. Evento del Botón Buscar
    protected void BTN_BuscarPago_Click(object sender, EventArgs e)
    {
        string filtro = TXT_BuscarPago.Text.Trim();
        CargarListadoPagos(filtro);
    }

    // 4. Evento de Paginación (Para que funcione el PageSize="5")
    protected void GV_Pagos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Pagos.PageIndex = e.NewPageIndex;
        string filtro = TXT_BuscarPago.Text.Trim();
        CargarListadoPagos(filtro);
    }
}