using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class CotizacionDAO
{
    private readonly string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string Generar_NroCotizacion()
    {
        string codigo = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_NroCotizacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                var p = new SqlParameter("@NroCotizacion", SqlDbType.Char, 10) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(p);
                cn.Open(); cmd.ExecuteNonQuery();
                codigo = Convert.ToString(p.Value);
            }
            catch
            {
                codigo = "COT0000001";
            }
        }
        return codigo;
    }

    public DataTable Listar_Proveedor()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Proveedores", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("CodProveedor"); dt.Columns.Add("nombre");
                dt.Rows.Add("PRV001", "Proveedor 1");
            }
        }
        return dt;
    }

    public DataTable Listar_Productos_NroSerie()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Productos_NroSerie", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("NroSerie_Producto");
            }
        }
        return dt;
    }

    public string Guardar(string nro, string codEmpleado, string codProveedor, DateTime fec, decimal total, string estado, DataTable detalle)
    {

        // 1. CREAR UNA COPIA para no afectar la tabla original que se muestra en la web
        DataTable dtParaSQL = detalle.Copy();

        // 2. ELIMINAR LA COLUMNA "Producto" (Nombre) que sobra y causa el error
        // La base de datos solo espera 8 columnas, y "Producto" es la novena.
        if (dtParaSQL.Columns.Contains("Producto"))
        {
            dtParaSQL.Columns.Remove("Producto");
        }

        string mensaje = "";
        using (SqlConnection cn = new SqlConnection(CN))
        using (SqlCommand cmd = new SqlCommand("USP_Cotizacion_Guardar", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NroCotizacion", nro);
            cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
            cmd.Parameters.AddWithValue("@CodProveedor", codProveedor);
            cmd.Parameters.AddWithValue("@Fec_Cotizacion", fec);
            cmd.Parameters.AddWithValue("@Total", total);
            cmd.Parameters.AddWithValue("@Estado_Cotizacion", estado);

            SqlParameter pDet = new SqlParameter("@Detalle", SqlDbType.Structured)
            {
                Value = dtParaSQL,
                TypeName = "dbo.TipoDetalleCotizacion"
            };
            cmd.Parameters.Add(pDet);

            SqlParameter pMsg = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                mensaje = Convert.ToString(pMsg.Value);
                if (string.IsNullOrWhiteSpace(mensaje)) mensaje = "OK";
            }
            catch (Exception ex)
            {
                mensaje = "ERROR SQL: " + ex.Message;
            }
        }
        return mensaje;
    }

    public DataTable Listar_Cotizaciones(string filtro)
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Cotizaciones", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? string.Empty);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("NroCotizacion"); dt.Columns.Add("CodProveedor"); dt.Columns.Add("CodEmpleado"); dt.Columns.Add("Fec_Cotizacion", typeof(DateTime)); dt.Columns.Add("Total", typeof(decimal)); dt.Columns.Add("Estado_Cotizacion");
            }
        }
        return dt;
    }


    public DataSet Buscar_Productos_Cotizacion(string sucursal, string marca, string modelo, string talla, string color)
    {
        DataSet ds = new DataSet();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Buscar_Producto_Cotizacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Enviamos los parámetros. Si alguno es null, enviamos cadena vacía para evitar errores en el SP.
                cmd.Parameters.AddWithValue("@SucursalNombre", sucursal ?? "");
                cmd.Parameters.AddWithValue("@Marca", marca ?? "");
                cmd.Parameters.AddWithValue("@Modelo", modelo ?? "");
                cmd.Parameters.AddWithValue("@Talla", talla ?? "");
                cmd.Parameters.AddWithValue("@Color", color ?? "");

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                // El DataAdapter llena el DataSet con las 5 tablas que devuelve el SP
                da.Fill(ds);
            }
            catch (Exception)
            {
                // En caso de error, retornamos un DataSet vacío para que no rompa la aplicación
                ds = new DataSet();
            }
        }
        return ds;
    }
}




