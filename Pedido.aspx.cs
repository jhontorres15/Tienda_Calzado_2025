using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pedido : System.Web.UI.Page
{
    // Variables para almacenar datos de proforma verificada
    private string proformaVerificada = "";
    private string pedidoAsociado = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarClientes();
            CargarEmpleados();
            CargarProductos();
            CargarPedidos();
            TXT_FecPedido.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

            // Inicializar ViewState para detalles
            ViewState["DetallesPedido"] = new DataTable();
            CrearTablaDetalles();
        }
    }


    private void CargarClientes()
    {
        // Simulación de datos - En producción conectar a base de datos
        DDL_Cliente.Items.Clear();
        DDL_Cliente.Items.Add(new ListItem("-- Seleccionar Cliente --", "0"));
        DDL_Cliente.Items.Add(new ListItem("Juan Pérez - 12345678", "CLI00001"));
        DDL_Cliente.Items.Add(new ListItem("María García - 87654321", "CLI00002"));
        DDL_Cliente.Items.Add(new ListItem("Carlos López - 11223344", "CLI00003"));
        DDL_Cliente.Items.Add(new ListItem("Ana Martínez - 55667788", "CLI00004"));
        DDL_Cliente.Items.Add(new ListItem("Luis Rodríguez - 99887766", "CLI00005"));
    }

    private void CargarEmpleados()
    {
        // Simulación de datos - En producción conectar a base de datos
        DDL_Empleado.Items.Clear();
        DDL_Empleado.Items.Add(new ListItem("-- Seleccionar Empleado --", "0"));
        DDL_Empleado.Items.Add(new ListItem("Pedro Sánchez - Vendedor", "EMP01"));
        DDL_Empleado.Items.Add(new ListItem("Laura Torres - Supervisora", "EMP02"));
        DDL_Empleado.Items.Add(new ListItem("Miguel Flores - Vendedor", "EMP03"));
        DDL_Empleado.Items.Add(new ListItem("Carmen Ruiz - Gerente", "EMP04"));
        DDL_Empleado.Items.Add(new ListItem("Roberto Díaz - Vendedor", "EMP05"));
    }

    private void CargarProductos()
    {
        // Simulación de datos - En producción conectar a base de datos
        DDL_Producto.Items.Clear();
        DDL_Producto.Items.Add(new ListItem("-- Seleccionar Producto --", "0"));
        DDL_Producto.Items.Add(new ListItem("Zapato Formal Negro - Talla 42", "ZAP0001"));
        DDL_Producto.Items.Add(new ListItem("Sandalia Dama Café - Talla 38", "SAN0001"));
        DDL_Producto.Items.Add(new ListItem("Bota Trabajo Cuero - Talla 44", "BOT0001"));
        DDL_Producto.Items.Add(new ListItem("Tenis Deportivo Blanco - Talla 40", "TEN0001"));
        DDL_Producto.Items.Add(new ListItem("Zapato Casual Marrón - Talla 41", "ZAP0002"));
    }

    private void CargarPedidos()
    {
        // Simulación de datos - En producción conectar a base de datos
        DataTable dt = new DataTable();
        dt.Columns.Add("NroPedido");
        dt.Columns.Add("Cliente");
        dt.Columns.Add("Empleado");
        dt.Columns.Add("TipoPedido");
        dt.Columns.Add("Fec_Pedido", typeof(DateTime));
        dt.Columns.Add("Total", typeof(decimal));
        dt.Columns.Add("Estado_Pedido");

        dt.Rows.Add("PED-00000001", "Juan Pérez", "Pedro Sánchez", "NORMAL", DateTime.Now.AddDays(-5), 250.50m, "Completado");
        dt.Rows.Add("PED-00000002", "María García", "Laura Torres", "URGENT", DateTime.Now.AddDays(-3), 180.75m, "En Proceso");
        dt.Rows.Add("PED-00000003", "Carlos López", "Miguel Flores", "ESPECI", DateTime.Now.AddDays(-1), 320.00m, "Pendiente");
        dt.Rows.Add("PED-00000004", "Ana Martínez", "Carmen Ruiz", "MAYOREO", DateTime.Now, 450.25m, "Entregado");

        GV_Pedidos.DataSource = dt;
        GV_Pedidos.DataBind();
    }


    protected void BTN_VerificarProforma_Click(object sender, EventArgs e)
    {
        string nroProforma = TXT_NroProformaVerificar.Text.Trim();

        if (string.IsNullOrEmpty(nroProforma))
        {
            MostrarMensaje("Ingrese número de proforma", "alert-warning");
            return;
        }

        // Simulación de verificación en Tb_Proforma_Pedido
        // En producción: consultar base de datos
        bool proformaExiste = VerificarProformaEnBD(nroProforma);

        if (proformaExiste)
        {
            string nroPedidoAsociado = ObtenerPedidoAsociado(nroProforma);

            if (!string.IsNullOrEmpty(nroPedidoAsociado))
            {
                MostrarMensaje($"Proforma {nroProforma} ya tiene un pedido asociado: {nroPedidoAsociado}", "alert-warning");
                BTN_CargarProforma.Enabled = false;
            }
            else
            {
                MostrarMensaje($"Proforma {nroProforma} verificada correctamente. Puede cargar los datos.", "alert-success");
                BTN_CargarProforma.Enabled = true;
                proformaVerificada = nroProforma;
            }
        }
        else
        {
            MostrarMensaje($"La proforma {nroProforma} no existe en el sistema", "alert-danger");
            BTN_CargarProforma.Enabled = false;
        }
    }

    protected void BTN_CargarProforma_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(proformaVerificada))
        {
            MostrarMensaje("Primero debe verificar una proforma válida", "alert-warning");
            return;
        }

        // Cargar datos de la proforma
        CargarDatosProforma(proformaVerificada);
        MostrarMensaje($"Datos de la proforma {proformaVerificada} cargados exitosamente", "alert-success");
    }

    private bool VerificarProformaEnBD(string nroProforma)
    {
        // Simulación - En producción consultar Tb_Proforma
        string[] proformasExistentes = { "PRO-000001", "PRO-000002", "PRO-000003", "PRO-000004", "PRO-000005" };
        return proformasExistentes.Contains(nroProforma);
    }

    private string ObtenerPedidoAsociado(string nroProforma)
    {
        // Simulación - En producción consultar Tb_Proforma_Pedido
        // Simular que PRO-000001 ya tiene pedido asociado
        if (nroProforma == "PRO-000001")
            return "PED-00000001";

        return ""; // No tiene pedido asociado
    }

    private void CargarDatosProforma(string nroProforma)
    {
        // Simulación de carga de datos de proforma
        // En producción: consultar Tb_Proforma y Tb_Detalle_Proforma

        // Generar nuevo número de pedido
        TXT_NroPedido.Text = GenerarNumeroPedido();
        TXT_CodSucursal.Text = "SUC01";
        DDL_TipoPedido.SelectedValue = "NORMAL";
        DDL_Cliente.SelectedValue = "CLI00001";
        DDL_Empleado.SelectedValue = "EMP01";
        DDL_EstadoPedido.SelectedValue = "Pendiente";

        // Cargar detalles de la proforma
        CargarDetallesProforma(nroProforma);
    }

    private void CargarDetallesProforma(string nroProforma)
    {
        // Simulación de detalles de proforma
        DataTable dt = (DataTable)ViewState["DetallesPedido"];
        dt.Clear();

        // Simular productos de la proforma
        dt.Rows.Add("ZAP0001", "SUC01", 2, 125.00m, 0.00m, 0.00m, 250.00m);
        dt.Rows.Add("SAN0001", "SUC01", 1, 85.50m, 5.00m, 4.28m, 81.22m);
        dt.Rows.Add("BOT0001", "SUC01", 1, 180.00m, 10.00m, 18.00m, 162.00m);

        ViewState["DetallesPedido"] = dt;
        GV_DetallePedido.DataSource = dt;
        GV_DetallePedido.DataBind();

        CalcularTotal();
    }

    private string GenerarNumeroPedido()
    {
        // Simulación - En producción obtener de base de datos
        Random rand = new Random();
        int numero = rand.Next(100000, 999999);
        return $"PED-{numero:D8}";
    }

   

    private void CrearTablaDetalles()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("NroSerie_Producto");
        dt.Columns.Add("CodSucursal");
        dt.Columns.Add("Cantidad", typeof(int));
        dt.Columns.Add("Prec_Venta", typeof(decimal));
        dt.Columns.Add("Porcentaje_Dscto", typeof(decimal));
        dt.Columns.Add("Dscto", typeof(decimal));
        dt.Columns.Add("SubTotal", typeof(decimal));

        ViewState["DetallesPedido"] = dt;
    }

    protected void DDL_Producto_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_Producto.SelectedIndex > 0)
        {
            // Simulación de precio del producto
            decimal precio = 0;
            switch (DDL_Producto.SelectedValue)
            {
                case "ZAP0001": precio = 125.00m; break;
                case "SAN0001": precio = 85.50m; break;
                case "BOT0001": precio = 180.00m; break;
                case "TEN0001": precio = 95.75m; break;
                case "ZAP0002": precio = 110.25m; break;
            }
            TXT_PrecVenta.Text = precio.ToString("F2");
            TXT_CodSucursal.Text = "SUC01";
        }
    }

    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
        if (DDL_Producto.SelectedIndex <= 0) return;

        DataTable dt = (DataTable)ViewState["DetallesPedido"];

        string producto = DDL_Producto.SelectedValue;
        string sucursal = TXT_CodSucursal.Text;
        int cantidad = Convert.ToInt32(TXT_Cantidad.Text);
        decimal precio = Convert.ToDecimal(TXT_PrecVenta.Text);
        decimal porcentajeDscto = Convert.ToDecimal(TXT_PorcentajeDscto.Text);

        decimal importe = cantidad * precio;
        decimal descuento = importe * (porcentajeDscto / 100);
        decimal subtotal = importe - descuento;

        dt.Rows.Add(producto, sucursal, cantidad, precio, porcentajeDscto, descuento, subtotal);

        ViewState["DetallesPedido"] = dt;
        GV_DetallePedido.DataSource = dt;
        GV_DetallePedido.DataBind();

        LimpiarDetalle();
        CalcularTotal();
    }

    protected void GV_DetallePedido_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EliminarDetalle")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dt = (DataTable)ViewState["DetallesPedido"];
            dt.Rows.RemoveAt(index);

            ViewState["DetallesPedido"] = dt;
            GV_DetallePedido.DataSource = dt;
            GV_DetallePedido.DataBind();

            CalcularTotal();
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
        MostrarMensaje("Formulario limpio para nuevo pedido", "alert-info");
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        // Validaciones
        if (string.IsNullOrEmpty(TXT_NroPedido.Text.Trim()))
        {
            MostrarMensaje("Complete todos los campos obligatorios", "alert-warning");
            return;
        }

        DataTable dt = (DataTable)ViewState["DetallesPedido"];
        if (dt.Rows.Count == 0)
        {
            MostrarMensaje("Debe agregar al menos un producto al pedido", "alert-warning");
            return;
        }

        // Simulación de grabado
        // En producción: insertar en Tb_Orden_Pedido y Tb_Detalle_Pedido

        // Si viene de proforma, registrar en Tb_Proforma_Pedido
        if (!string.IsNullOrEmpty(proformaVerificada))
        {
            RegistrarProformaPedido(proformaVerificada, TXT_NroPedido.Text);
        }

        MostrarMensaje("Pedido grabado exitosamente", "alert-success");
        CargarPedidos();
        LimpiarFormulario();
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
            // Cargar datos del pedido para edición
            CargarPedidoParaEdicion(nroPedido);
            MostrarMensaje($"Pedido {nroPedido} cargado para edición", "alert-info");
        }
        else if (e.CommandName == "EliminarPedido")
        {
            // Simulación de eliminación
            MostrarMensaje($"Pedido {nroPedido} eliminado", "alert-success");
            CargarPedidos();
        }
    }


    // solo par aque fincione sin ningun probolema




    private void RegistrarProformaPedido(string nroProforma, string nroPedido)
    {
        // En producción: registrar en Tb_Proforma_Pedido
        // Por ahora solo simula
        System.Diagnostics.Debug.WriteLine($"Proforma {nroProforma} asociada al pedido {nroPedido}");
    }

    private void CargarPedidoParaEdicion(string nroPedido)
    {
        // En producción: cargar datos de Tb_Orden_Pedido y Tb_Detalle_Pedido
        MostrarMensaje($"Pedido {nroPedido} cargado correctamente (modo edición simulado)", "alert-info");
    }
    private void CalcularTotal()
    {
        DataTable dt = (DataTable)ViewState["DetallesPedido"];
        decimal total = 0;

        foreach (DataRow row in dt.Rows)
        {
            total += Convert.ToDecimal(row["SubTotal"]);
        }

        LBL_Importe.Text = total.ToString("F2");
    }

    private void LimpiarDetalle()
    {
        DDL_Producto.SelectedIndex = 0;
        TXT_Cantidad.Text = "";
        TXT_PrecVenta.Text = "";
        TXT_PorcentajeDscto.Text = "0";
    }
    private void MostrarMensaje(string mensaje, string tipoAlerta)
    {
        
    }
    void LimpiarFormulario()
    {

    }
   
}