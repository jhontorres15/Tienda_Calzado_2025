using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class EmpresaProveedorDAO
{
    private readonly string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string Generar_CodEmpresa()
    {
        string codigo = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_CodEmpresaProveedor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                var p = new SqlParameter("@CodEmpresa", SqlDbType.Char, 5) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(p);
                cn.Open(); cmd.ExecuteNonQuery();
                codigo = Convert.ToString(p.Value);
            }
            catch { codigo = "EMP01"; }
        }
        return codigo;
    }

    public DataTable Listar_Paises()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Paises", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("CodPais"); dt.Columns.Add("Nacionalidad");
            }
        }
        return dt;
    }

    public string Mantener(string codEmpresa, string razon, string ruc, string codPais, string direccion, string telefono, string email, string estado, string tipo)
    {
        string mensaje = "";
        using (SqlConnection cn = new SqlConnection(CN))
        using (SqlCommand cmd = new SqlCommand("USP_Mantenimiento_Empresa_Proveedor", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CodEmpresa", codEmpresa);
            cmd.Parameters.AddWithValue("@Razon_Social", razon);
            cmd.Parameters.AddWithValue("@Ruc", ruc);
            cmd.Parameters.AddWithValue("@CodPais", codPais);
            cmd.Parameters.AddWithValue("@Direccion", direccion);
            cmd.Parameters.AddWithValue("@Telefono", telefono);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Estado_Empresa", estado);
            cmd.Parameters.AddWithValue("@Tipo_Transaccion", tipo);
            SqlParameter pMsg = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            try
            {
                cn.Open(); cmd.ExecuteNonQuery();
                mensaje = Convert.ToString(pMsg.Value);
                if (string.IsNullOrWhiteSpace(mensaje)) mensaje = "OK";
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: " + ex.Message;
            }
        }
        return mensaje;
    }

    public DataTable Listar_Empresas(string filtro)
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Empresas_Proveedor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? string.Empty);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("CodEmpresa"); dt.Columns.Add("Razon_Social"); dt.Columns.Add("Ruc"); dt.Columns.Add("Nacionalidad"); dt.Columns.Add("Direccion"); dt.Columns.Add("Telefono"); dt.Columns.Add("Email"); dt.Columns.Add("Estado_Empresa");
            }
        }
        return dt;
    }
}

