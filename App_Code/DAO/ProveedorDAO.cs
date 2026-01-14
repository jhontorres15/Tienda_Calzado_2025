using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class ProveedorDAO
{
    private readonly string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string Generar_CodProveedor()
    {
        string codigo = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_CodProveedor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                var p = new SqlParameter("@CodProveedor", SqlDbType.Char, 6) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(p);
                cn.Open(); cmd.ExecuteNonQuery();
                codigo = Convert.ToString(p.Value);
            }
            catch { codigo = "PRV001"; }
        }
        return codigo;
    }

    public DataTable Listar_Empresas()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Empresas_Proveedor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch { dt.Columns.Add("CodEmpresa"); dt.Columns.Add("Razon_Social"); }
        }
        return dt;
    }

    public string Mantener(string codProveedor, string apellidos, string nombres, string telefono, string email, string cpostal, string direccion, string codEmpresa, string estado, string tipo)
    {
        string mensaje = "";
        using (SqlConnection cn = new SqlConnection(CN))
        using (SqlCommand cmd = new SqlCommand("USP_Mantenimiento_Proveedor", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodProveedor", codProveedor);
            cmd.Parameters.AddWithValue("@Apellidos", apellidos);
            cmd.Parameters.AddWithValue("@Nombres", nombres);
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Distrito", cpostal);
            cmd.Parameters.AddWithValue("@Direccion", direccion);
            cmd.Parameters.AddWithValue("@CodEmpresa", codEmpresa);
            cmd.Parameters.AddWithValue("@Estado_Proveedor", estado);
            cmd.Parameters.AddWithValue("@Tipo_Transaccion", tipo);
            SqlParameter pMsg = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            try { cn.Open(); cmd.ExecuteNonQuery(); mensaje = Convert.ToString(pMsg.Value); if (string.IsNullOrWhiteSpace(mensaje)) mensaje = "OK"; }
            catch (Exception ex) { mensaje = "ERROR: " + ex.Message; }
        }
        return mensaje;
    }

    public DataTable Listar_Proveedores(string filtro)
    {
        DataTable dt = new DataTable();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Proveedores", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? string.Empty);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("CodProveedor");
                dt.Columns.Add("Apellidos");
                dt.Columns.Add("Nombres");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Email");

                dt.Columns.Add("Departamento");
                dt.Columns.Add("Provincia");
                dt.Columns.Add("Distrito");

                dt.Columns.Add("CPostal");
                dt.Columns.Add("Direccion");
                dt.Columns.Add("Razon_Social");
                dt.Columns.Add("Estado_Proveedor");
            }
        }

        return dt;
    }

}

