using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reporte_Productos : System.Web.UI.Page
{
    //Declarar una variable que haga referencia al archivo:Global.asax
    ASP.global_asax Global = new ASP.global_asax();

    //Crear el Método Público: Listar_Articulos
    public void Listar_Productos()
    {
        //Abrir Conexión con la Base de Datos
        Global.CN.Open();
        //Crear un Nuevo DataTable
        DataTable DT = new DataTable();
        //Crear un Nuevo Adaptador de Datos
        SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM V_Listar_Productos", Global.CN);
        //Cargar Datos del Adaptador de Datos a un DataTable
        DA.Fill(DT);
        //Cargar Datos del DataTable en el Control: GV_Articulos
        GV_Productos.DataSource = DT;
        GV_Productos.DataBind();

        //Liberar Recursos del Adaptador de Datos
        DA.Dispose();
        //Cerrar Conexión con la Base de Datos
        
         Global.CN.Close();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if(IsPostBack==false)
        {
            this.Listar_Productos();
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        string filtro = txtBuscar.Text.Trim();

        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM V_Listar_Productos WHERE Producto LIKE @filtro OR CodProducto LIKE @filtro", Global.CN);
        da.SelectCommand.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
        da.Fill(dt);

        GV_Productos.DataSource = dt;
        GV_Productos.DataBind();
    }

    protected void GV_Productos_RowCommand(object source, GridViewCommandEventArgs e)
    {
        //Evaluar si el nombre del boton es : BTN_Ver
        if (e.CommandName== "BTN_Ver")
        {
            //obtener el indice del comandos seleccionado
            int indice = Convert.ToInt32(e.CommandArgument);

            Session["MantenimientoProductos_CodProducto"] = this.GV_Productos.Rows[indice].Cells[1].Text;
            Session["MantenimientoProductos_Producto"] = this.GV_Productos.Rows[indice].Cells[2].Text;
            Session["MantenimientoProductos_Descripcion"] = Server.HtmlDecode(this.GV_Productos.Rows[indice].Cells[3].Text);
            Session["MantenimientoProductos_Categoria"] = this.GV_Productos.Rows[indice].Cells[4].Text;
            Session["MantenimientoProductos_Marca"] = this.GV_Productos.Rows[indice].Cells[5].Text;
            Session["MantenimientoProductos_Modelo"] = this.GV_Productos.Rows[indice].Cells[6].Text;
            Session["MantenimientoProductos_Talla"] = this.GV_Productos.Rows[indice].Cells[7].Text;
            Session["MantenimientoProductos_Color"] = this.GV_Productos.Rows[indice].Cells[8].Text;
            Session["MantenimientoProductos_Prec_Venta_Menor"] = this.GV_Productos.Rows[indice].Cells[9].Text;
            Session["MantenimientoProductos_Prec_Venta_Mayor"] = this.GV_Productos.Rows[indice].Cells[10].Text;
            Session["MantenimientoProductos_Stock"] = this.GV_Productos.Rows[indice].Cells[11].Text;
            Session["MantenimientoProductos_Estado"] = this.GV_Productos.Rows[indice].Cells[12].Text;

            Image img = (Image)GV_Productos.Rows[indice].FindControl("IMG_Item");
            if (img != null)
            {
                string base64 = img.ImageUrl.Split(',')[1]; // separar el Base64
                byte[] foto = Convert.FromBase64String(base64);
                Session["MantenimientoProductos_Foto"] = foto;
            }

            Session["MantenimientoProductos_Tipo_Transaccion"] = "ACTUALIZAR";


            //REEDIRECCIONAR AL FROMULARIO MANTENIMIENTO PRODUCTOS
            Response.Redirect("Mantenimiento_Productos.aspx");
        }
    }

 
  

}