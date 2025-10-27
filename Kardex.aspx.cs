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

    // Método para limpiar los controles del formulario
    public void limpiar()
    {
        TXT_NroOperacion.Text = "";
        TXT_Cantidad.Text = "";
        //this.TXT_FechaOperacion.Text = DateTime.Now.ToString("yyyy-MM-dd");
        DDL_TipoOperacion.SelectedIndex = 0;
        //DDL_Producto.SelectedIndex = 0;
       // Lb_StockActual.Text = "";
        TXT_Observaciones.Text = "";
        TXT_Cantidad.Focus();
    }

    // Listar los productos disponibles
    public void Listar_Productos()
    {
        Global.CN.Open();
       // SqlDataReader lector;
        //SqlCommand cmd = new SqlCommand("USP_Listar_Productos", Global.CN);
        //cmd.CommandType = CommandType.StoredProcedure;

        //lector = cmd.ExecuteReader();

        //DDL_Producto.Items.Clear();
        //DDL_Producto.Items.Add("--Seleccione--");
        //while (lector.Read())
        {
            // Mostrar el nombre, pero guardar el código
          //  DDL_Producto.Items.Add(new ListItem(lector["Producto"].ToString(), lector["CodProducto"].ToString()));
        }

       // cmd.Dispose();
        Global.CN.Close();
    }

    // Método para generar nuevo número de operación
    //public void Nuevo_NumOperacion()
    //{
    //    Global.CN.Open();

    //    SqlParameter ParamNroOperacion = new SqlParameter();
    //    ParamNroOperacion.Direction = ParameterDirection.Output;

    //    SqlCommand CMDNuevo = new SqlCommand("USP_Generar_NroOperacion_Kardex", Global.CN);
    //    CMDNuevo.CommandType = CommandType.StoredProcedure;

    //    ParamNroOperacion.ParameterName = "@NroOperacion";
    //    ParamNroOperacion.SqlDbType = SqlDbType.Char;
    //    ParamNroOperacion.Size = 8;

    //    CMDNuevo.Parameters.Add(ParamNroOperacion);
    //    CMDNuevo.ExecuteNonQuery();

    //    TXT_NroOperacion.Text = ParamNroOperacion.Value.ToString();

    //    CMDNuevo.Dispose();
    //    Global.CN.Close();
    //}

    // Método para registrar operación en Kardex
    //public void Guardar(string Tipo_Transaccion)
    //{
    //    Global.CN.Open();

    //    SqlCommand CMDGuardar = new SqlCommand("USP_Mantenimiento_Kardex", Global.CN);
    //    CMDGuardar.CommandType = CommandType.StoredProcedure;

    //    CMDGuardar.Parameters.AddWithValue("@NroOperacion", TXT_NroOperacion.Text);
    //    //CMDGuardar.Parameters.AddWithValue("@CodProducto", DDL_Producto.SelectedValue);
    //    CMDGuardar.Parameters.AddWithValue("@TipoOperacion", DDL_TipoOperacion.SelectedValue);
    //    CMDGuardar.Parameters.AddWithValue("@Cantidad", Convert.ToInt32(TXT_Cantidad.Text));
    //    //CMDGuardar.Parameters.AddWithValue("@FechaOperacion", Convert.ToDateTime(TXT_FechaOperacion.Text));
    //    //CMDGuardar.Parameters.AddWithValue("@Observacion", TXT_Observacion.Text);
    //    CMDGuardar.Parameters.AddWithValue("@TipoTransaccion", Tipo_Transaccion);

    //    SqlParameter ParamMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 255);
    //    ParamMensaje.Direction = ParameterDirection.Output;
    //    CMDGuardar.Parameters.Add(ParamMensaje);

    //    CMDGuardar.ExecuteNonQuery();

    //    Response.Write("<script>alert('" + ParamMensaje.Value.ToString() + "');</script>");

    //    CMDGuardar.Dispose();
    //    Global.CN.Close();
    //}

    //// Cargar stock actual del producto seleccionado


    //// Listar Kardex general
    //public void Listar_Kardex()
    //{
    //    Global.CN.Open();

    //    SqlDataAdapter DA = new SqlDataAdapter("USP_Listar_Kardex", Global.CN);
    //    DA.SelectCommand.CommandType = CommandType.StoredProcedure;
    //    DataTable DT = new DataTable();
    //    DA.Fill(DT);

    //    GV_Kardex.DataSource = DT;
    //    GV_Kardex.DataBind();

    //    Global.CN.Close();
    //}

    //// EVENTOS
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!IsPostBack)
    //    {
    //        this.Form.Attributes.Add("autocomplete", "off");
    //        limpiar();
    //        Listar_Productos();
    //        //Nuevo_NumOperacion();
    //        Listar_Kardex();

    //        DDL_TipoOperacion.Items.Clear();
    //        DDL_TipoOperacion.Items.Add("--Seleccione--");
    //        DDL_TipoOperacion.Items.Add("ENTRADA");
    //        DDL_TipoOperacion.Items.Add("SALIDA");

    //        Session["Kardex_Tipo_Transaccion"] = "GUARDAR";
    //    }
    //}

    //protected void BTN_Nuevo_Click(object sender, EventArgs e)
    //{
    //    limpiar();
    //   // Nuevo_NumOperacion();
    //    Session["Kardex_Tipo_Transaccion"] = "GUARDAR";
    //}

    //protected void BTN_Guardar_Click(object sender, EventArgs e)
    //{
    //    Guardar(Session["Kardex_Tipo_Transaccion"].ToString());
    //    limpiar();
    //    Listar_Kardex();
    //    //Nuevo_NumOperacion();
    //}

    //protected void DDL_Producto_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    //Cargar_StockActual();
    //}

    //protected void BTN_Reporte_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("Reporte_Kardex.aspx");
    //}



}
