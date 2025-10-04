<%@ Application Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>


<script runat="server">

    //Crear una variable publica de tipo:SqlConnection

    public SqlConnection CN = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString);

    void session_Start(object sender, EventArgs e)
        {
        //codigo que se ejecuta al iniciarse una nueva sesion
        Session.Add("MantenimientoProductos_CodProducto","");
        Session.Add("MantenimientoProductos_Tipo_Transaccion","");


        }
</script>

