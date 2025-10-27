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
        // Inicializar las sesiones
        Session.Add("MantenimientoProductos_CodProducto", "");
        Session.Add("MantenimientoProductos_Producto", "");
        Session.Add("MantenimientoProductos_Descripcion", "");
        Session.Add("MantenimientoProductos_Categoria", "");
        Session.Add("MantenimientoProductos_Marca", "");
        Session.Add("MantenimientoProductos_Modelo", "");
        Session.Add("MantenimientoProductos_Talla", "");
        Session.Add("MantenimientoProductos_Color", "");
        Session.Add("MantenimientoProductos_Genero", "");
        Session.Add("MantenimientoProductos_Prec_Venta_Menor", "");
        Session.Add("MantenimientoProductos_Prec_Venta_Mayor", "");
        Session.Add("MantenimientoProductos_Stock", "");
        Session.Add("MantenimientoProductos_Estado", "");
        Session.Add("MantenimientoProductos_Foto", "");
        Session.Add("MantenimientoProductos_Tipo_Transaccion", "");


                // Inicializar las sesiones para mantenimiento de empleados
        Session.Add("MantenimientoEmpleado_CodEmpleado", "");
        Session.Add("MantenimientoEmpleado_Apellido", "");
        Session.Add("MantenimientoEmpleado_Nombre", "");
        Session.Add("MantenimientoEmpleado_Fec_Nac", "");
        Session.Add("MantenimientoEmpleado_Sexo", "");
        Session.Add("MantenimientoEmpleado_Estado_Civil", "");
        Session.Add("MantenimientoEmpleado_Nro_Hijos", "");
        Session.Add("MantenimientoEmpleado_CodPais", "");
        Session.Add("MantenimientoEmpleado_CodDoc_Identidad", "");
        Session.Add("MantenimientoEmpleado_NroDoc_Identidad", "");
        Session.Add("MantenimientoEmpleado_Fec_Contrato", "");
        Session.Add("MantenimientoEmpleado_Fec_Termino_Contrato", "");
        Session.Add("MantenimientoEmpleado_CodSucursal", "");
        Session.Add("MantenimientoEmpleado_CodArea", "");
        Session.Add("MantenimientoEmpleado_CodCargo", "");
        Session.Add("MantenimientoEmpleado_Sueldo", "");
        Session.Add("MantenimientoEmpleado_CPostal", "");
        Session.Add("MantenimientoEmpleado_Direccion", "");
        Session.Add("MantenimientoEmpleado_Telefono", "");
        Session.Add("MantenimientoEmpleado_Email", "");
        Session.Add("MantenimientoEmpleado_Foto", "");
        Session.Add("MantenimientoEmpleado_Estado_Empleado", "");
        Session.Add("MantenimientoEmpleado_Obs_Estado_Empleado", "");

        Session.Add("MantenimientoEmpleado_Tipo_Transaccion", "");

    }

    void Session_End(object sender, EventArgs e)
    {
        Session.Remove("MantenimientoProductos_CodProducto");
        Session.Remove("MantenimientoProductos_Producto");
        Session.Remove("MantenimientoProductos_Descripcion");
        Session.Remove("MantenimientoProductos_Categoria");
        Session.Remove("MantenimientoProductos_Marca");
        Session.Remove("MantenimientoProductos_Modelo");
        Session.Remove("MantenimientoProductos_Talla");
        Session.Remove("MantenimientoProductos_Color");
        Session.Remove("MantenimientoProductos_Prec_Venta_Menor");
        Session.Remove("MantenimientoProductos_Prec_Venta_Mayor");
        Session.Remove("MantenimientoProductos_Stock");
        Session.Remove("MantenimientoProductos_Estado");
        Session.Remove("MantenimientoProductos_Foto");
        Session.Remove("MantenimientoProductos_Tipo_Transaccion");

        // Eliminar las sesiones para mantenimiento de empleados
        Session.Remove("MantenimientoEmpleado_CodEmpleado");
        Session.Remove("MantenimientoEmpleado_Apellido");
        Session.Remove("MantenimientoEmpleado_Nombre");
        Session.Remove("MantenimientoEmpleado_Fec_Nac");
        Session.Remove("MantenimientoEmpleado_Sexo");
        Session.Remove("MantenimientoEmpleado_Estado_Civil");
        Session.Remove("MantenimientoEmpleado_Nro_Hijos");
        Session.Remove("MantenimientoEmpleado_CodPais");
        Session.Remove("MantenimientoEmpleado_CodDoc_Identidad");
        Session.Remove("MantenimientoEmpleado_NroDoc_Identidad");
        Session.Remove("MantenimientoEmpleado_Fec_Contrato");
        Session.Remove("MantenimientoEmpleado_Fec_Termino_Contrato");
        Session.Remove("MantenimientoEmpleado_CodSucursal");
        Session.Remove("MantenimientoEmpleado_CodArea");
        Session.Remove("MantenimientoEmpleado_CodCargo");
        Session.Remove("MantenimientoEmpleado_Sueldo");
        Session.Remove("MantenimientoEmpleado_CPostal");
        Session.Remove("MantenimientoEmpleado_Direccion");
        Session.Remove("MantenimientoEmpleado_Telefono");
        Session.Remove("MantenimientoEmpleado_Email");
        Session.Remove("MantenimientoEmpleado_Foto");
        Session.Remove("MantenimientoEmpleado_Estado_Empleado");
        Session.Remove("MantenimientoEmpleado_Obs_Estado_Empleado");
        Session.Remove("MantenimientoEmpleado_Tipo_Transaccion");

    }
</script>

