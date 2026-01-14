using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class EntregaDAO
{
    private string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string Generar_NroEntrega()
    {
        string codigoGenerado = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_NroEntrega", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter parametro = new SqlParameter("@NroEntrega", SqlDbType.Char, 12);
                parametro.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(parametro);

                cn.Open();
                cmd.ExecuteNonQuery();
                codigoGenerado = Convert.ToString(cmd.Parameters["@NroEntrega"].Value);
            }
            catch
            {
                codigoGenerado = "ENT00000001"; // fallback
            }
        }
        return codigoGenerado;
    }

    public DataTable Listar_Tipo_Entrega()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Tipo_Entrega", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("CodTipo_Entrega");
                dt.Columns.Add("Tipo_Entrega");
                dt.Rows.Add("TEN01", "ENVÍO A DOMICILIO");
                dt.Rows.Add("TEN02", "RECOJO EN TIENDA");
            }
        }
        return dt;
    }

    public string Registrar_Entrega(string nroEntrega, string codEmpleado, string codTipoEntrega, DateTime fecEntrega,
        string nroGuia, string cpostal, string direccion, string estado, string obs)
    {
        string mensajeSalida = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Registrar_Entrega", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NroEntrega", nroEntrega);
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                cmd.Parameters.AddWithValue("@CodTipo_Entrega", codTipoEntrega);
                cmd.Parameters.AddWithValue("@Fec_Entrega", fecEntrega);
                cmd.Parameters.AddWithValue("@NroGuia_Remision", nroGuia);
                cmd.Parameters.AddWithValue("@CPostal_Entrega", cpostal);
                cmd.Parameters.AddWithValue("@Direccion_Entrega", direccion);
                cmd.Parameters.AddWithValue("@Estado_Entrega", estado);
                cmd.Parameters.AddWithValue("@Obs_Estado_Entrega", obs);

                SqlParameter paramMensaje = new SqlParameter();
                paramMensaje.ParameterName = "@MensajeSalida";
                paramMensaje.SqlDbType = SqlDbType.VarChar;
                paramMensaje.Size = 150;
                paramMensaje.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramMensaje);

                cn.Open();
                cmd.ExecuteNonQuery();
                mensajeSalida = Convert.ToString(cmd.Parameters["@MensajeSalida"].Value);
                if (string.IsNullOrWhiteSpace(mensajeSalida)) mensajeSalida = "OK";
            }
            catch (Exception ex)
            {
                mensajeSalida = "ERROR: " + ex.Message;
            }
        }
        return mensajeSalida;
    }

    public DataTable Listar_Entregas(string filtro)
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Entregas", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? string.Empty);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("NroEntrega");
                dt.Columns.Add("CodEmpleado");
                dt.Columns.Add("CodTipo_Entrega");
                dt.Columns.Add("Fec_Entrega", typeof(DateTime));
                dt.Columns.Add("NroGuia_Remision");
                dt.Columns.Add("CPostal_Entrega");
                dt.Columns.Add("Direccion_Entrega");
                dt.Columns.Add("Estado_Entrega");
                dt.Columns.Add("Obs_Estado_Entrega");
            }
        }
        return dt;
    }
}

