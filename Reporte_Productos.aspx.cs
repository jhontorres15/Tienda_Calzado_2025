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


}