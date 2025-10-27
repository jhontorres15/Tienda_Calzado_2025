using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class Proforma : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarClientes();
            CargarEmpleados();
            CargarProductos();
            TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            TXT_FecCaducidad.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
            CargarProformas();
        }
    }

    private void CargarClientes()
    {
        // Aquí cargarías los clientes desde la base de datos
        DDL_Cliente.Items.Clear();
        DDL_Cliente.Items.Add(new ListItem("-- Seleccionar Cliente --", "0"));
        // Ejemplo de datos estáticos (reemplazar con consulta a BD)
        DDL_Cliente.Items.Add(new ListItem("Juan Pérez - 12345678", "12345678"));
        DDL_Cliente.Items.Add(new ListItem("María García - 87654321", "87654321"));
    }

    private void CargarEmpleados()
    {
        // Aquí cargarías los empleados desde la base de datos
        DDL_Empleado.Items.Clear();
        DDL_Empleado.Items.Add(new ListItem("-- Seleccionar Empleado --", "0"));
        // Ejemplo de datos estáticos (reemplazar con consulta a BD)
        DDL_Empleado.Items.Add(new ListItem("Carlos López - EMP01", "EMP01"));
        DDL_Empleado.Items.Add(new ListItem("Ana Martín - EMP02", "EMP02"));
    }

    private void CargarProductos()
    {
        // Aquí cargarías los productos desde la base de datos
        DDL_Producto.Items.Clear();
        DDL_Producto.Items.Add(new ListItem("-- Seleccionar Producto --", "0"));
        // Ejemplo de datos estáticos (reemplazar con consulta a BD)
        DDL_Producto.Items.Add(new ListItem("Zapato Deportivo Nike - PRO001", "PRO001"));
        DDL_Producto.Items.Add(new ListItem("Sandalia Adidas - PRO002", "PRO002"));
    }

    private void CargarProformas()
    {
        // Crear tabla de ejemplo para el GridView
        DataTable dt = new DataTable();
        dt.Columns.Add("NroProforma");
        dt.Columns.Add("Cliente");
        dt.Columns.Add("Empleado");
        dt.Columns.Add("Fec_Emision", typeof(DateTime));
        dt.Columns.Add("Fec_Caducidad", typeof(DateTime));
        dt.Columns.Add("Total", typeof(decimal));
        dt.Columns.Add("Estado_Proforma");

        // Datos de ejemplo
        dt.Rows.Add("PRO-000001", "Juan Pérez", "Carlos López", DateTime.Now.AddDays(-5), DateTime.Now.AddDays(25), 150.50, "Pendiente");
        dt.Rows.Add("PRO-000002", "María García", "Ana Martín", DateTime.Now.AddDays(-3), DateTime.Now.AddDays(27), 320.75, "Aprobada");

        GV_Proformas.DataSource = dt;
        GV_Proformas.DataBind();
    }

    protected void DDL_Producto_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_Producto.SelectedIndex > 0)
        {
            // Aquí cargarías los precios del producto seleccionado desde la BD
            LBL_PrecMenor.Text = "45.00";
            LBL_PrecMayor.Text = "55.00";
            TXT_PrecCotizado.Text = "50.00";
        }
        else
        {
            LBL_PrecMenor.Text = "0.00";
            LBL_PrecMayor.Text = "0.00";
            TXT_PrecCotizado.Text = "";
        }
    }

    protected void BTN_AgregarDetalle_Click(object sender, EventArgs e)
    {
        // Validar campos del detalle
        if (string.IsNullOrEmpty(TXT_CodSucursal.Text) ||
            DDL_Producto.SelectedIndex <= 0 ||
            string.IsNullOrEmpty(TXT_Cantidad.Text) ||
            string.IsNullOrEmpty(TXT_PrecCotizado.Text))
        {
            // Mostrar mensaje de error
            return;
        }

        // Crear o recuperar la tabla de detalles de la sesión
        DataTable dtDetalle = Session["DetalleProforma"] as DataTable;
        if (dtDetalle == null)
        {
            dtDetalle = new DataTable();
            dtDetalle.Columns.Add("NroSerie_Producto");
            dtDetalle.Columns.Add("Cantidad", typeof(int));
            dtDetalle.Columns.Add("Prec_Cotizado", typeof(decimal));
            dtDetalle.Columns.Add("Importe", typeof(decimal));
        }

        // Agregar nuevo detalle
        DataRow row = dtDetalle.NewRow();
        row["NroSerie_Producto"] = DDL_Producto.SelectedItem.Text;
        row["Cantidad"] = Convert.ToInt32(TXT_Cantidad.Text);
        row["Prec_Cotizado"] = Convert.ToDecimal(TXT_PrecCotizado.Text);
        row["Importe"] = Convert.ToInt32(TXT_Cantidad.Text) * Convert.ToDecimal(TXT_PrecCotizado.Text);

        dtDetalle.Rows.Add(row);
        Session["DetalleProforma"] = dtDetalle;

        // Actualizar GridView
        GV_DetalleProforma.DataSource = dtDetalle;
        GV_DetalleProforma.DataBind();

        // Calcular total
        CalcularTotal();

        // Limpiar campos
        LimpiarDetalle();
    }

    private void CalcularTotal()
    {
        DataTable dtDetalle = Session["DetalleProforma"] as DataTable;
        if (dtDetalle != null)
        {
            decimal total = 0;
            foreach (DataRow row in dtDetalle.Rows)
            {
                total += Convert.ToDecimal(row["Importe"]);
            }
            LBL_Total.Text = "S/. " + total.ToString("N2");
        }
    }

    private void LimpiarDetalle()
    {
        TXT_CodSucursal.Text = "";
        DDL_Producto.SelectedIndex = 0;
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
            DataTable dtDetalle = Session["DetalleProforma"] as DataTable;
            if (dtDetalle != null && index < dtDetalle.Rows.Count)
            {
                dtDetalle.Rows.RemoveAt(index);
                Session["DetalleProforma"] = dtDetalle;

                GV_DetalleProforma.DataSource = dtDetalle;
                GV_DetalleProforma.DataBind();

                CalcularTotal();
            }
        }
    }

    protected void GV_DetalleProforma_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_DetalleProforma.PageIndex = e.NewPageIndex;
        DataTable dtDetalle = Session["DetalleProforma"] as DataTable;
        if (dtDetalle != null)
        {
            GV_DetalleProforma.DataSource = dtDetalle;
            GV_DetalleProforma.DataBind();
        }
    }

    protected void BTN_Nuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    private void LimpiarFormulario()
    {
        TXT_NroProforma.Text = "";
        DDL_Cliente.SelectedIndex = 0;
        DDL_Empleado.SelectedIndex = 0;
        TXT_FecEmision.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        TXT_FecCaducidad.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        DDL_EstadoProforma.SelectedIndex = 0;
        LBL_Total.Text = "S/. 0.00";

        Session["DetalleProforma"] = null;
        GV_DetalleProforma.DataSource = null;
        GV_DetalleProforma.DataBind();

        LimpiarDetalle();
    }

    protected void BTN_Grabar_Click(object sender, EventArgs e)
    {
        // Aquí implementarías la lógica para guardar en la base de datos
        // Validar que hay detalles
        DataTable dtDetalle = Session["DetalleProforma"] as DataTable;
        if (dtDetalle == null || dtDetalle.Rows.Count == 0)
        {
            // Mostrar mensaje: "Debe agregar al menos un producto"
            return;
        }

        try
        {
            // Guardar proforma y detalles en la base de datos
            // ... código de inserción ...

            // Mostrar mensaje de éxito
            LimpiarFormulario();
            CargarProformas();
        }
        catch (Exception ex)
        {
            // Mostrar mensaje de error
        }
    }

    protected void BTN_Cancelar_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    protected void BTN_Imprimir_Click(object sender, EventArgs e)
    {
        // Implementar lógica de impresión/exportación
    }

    protected void BTN_Buscar_Click(object sender, EventArgs e)
    {
        // Implementar búsqueda de proformas
        CargarProformas(); // Por ahora recarga todas
    }

    protected void GV_Proformas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string nroProforma = e.CommandArgument.ToString();

        if (e.CommandName == "EditarProforma")
        {
            // Cargar datos de la proforma para edición
        }
        else if (e.CommandName == "EliminarProforma")
        {
            // Eliminar proforma
            CargarProformas();
        }
    }

    protected void GV_Proformas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV_Proformas.PageIndex = e.NewPageIndex;
        CargarProformas();
    }
}