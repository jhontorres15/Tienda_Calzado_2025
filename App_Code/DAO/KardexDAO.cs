using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de KardexDAO
/// </summary>
public class KardexDAO
{

    // Obtener la cadena de conexión de tu Web.config
    private string cnx = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;


    public DataTable ListarSucursales()
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("USP_Listar_Sucursal_con_codigo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Usamos un SqlDataAdapter para llenar el DataTable
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Manejar el error (puedes lanzar la excepción o registrarla)
                // Por ahora, solo la mostramos en la consola
                Console.WriteLine("Error en SucursalDAO.ListarSucursales: " + ex.Message);
                // O puedes crear un DataTable con un error
                dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(ex.Message);
            }
        }
        return dt;
    }

    public DataTable Listar_Tipo_Facturacion()
    {
        DataTable dt = new DataTable();

        // Usamos "using" para asegurar que la conexión se cierre sola
        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_TipoFacturacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // El SqlDataAdapter llena la tabla automáticamente
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex; // O manejar el error según prefieras
            }
        }
        return dt;
    }

    public DataTable Listar_Tipo_Operacion()
    {
        DataTable dt = new DataTable();

        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_TipoOperacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cn.Open();
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        return dt;
    }

    public int Generar_NroKardex()
    {
        int numero = 0;

        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("USP_Generar_NroKardex", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter p = new SqlParameter("@NroOperacion", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(p);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    // Convertir el parámetro OUTPUT a int
                    numero = Convert.ToInt32(p.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en KardexDAO.Generar_NroKardex(): " + ex.Message);
                numero = 1; // valor inicial por defecto si ocurre error
            }
        }

        return numero;
    }

    public DataSet ObtenerDatosFactura(string nroSerie)
    {
        DataSet ds = new DataSet();
        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Factura_ObtenerDatos", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerie);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ObtenerDatosFactura: " + ex.Message);
            }
        }
        return ds;
    }

    public string Guardar_Kardex(string nroOperacion, string codEmpleado, string tipoOperacion, string codSucursal, DateTime fecha, string codTipoFact, string nroSerie, string observaciones, DataTable detalle)
    {
        string mensaje = "";
        using (SqlConnection cn = new SqlConnection(cnx))
        using (SqlCommand cmd = new SqlCommand("USP_Kardex_Guardar", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NroOperacion", nroOperacion);
            cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
            cmd.Parameters.AddWithValue("@CodTipo_Operacion", tipoOperacion);
            cmd.Parameters.AddWithValue("@CodSucursal", codSucursal);
            cmd.Parameters.AddWithValue("@Fec_Operacion", fecha);
            cmd.Parameters.AddWithValue("@CodTipo_Facturacion", codTipoFact);
            cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerie);
            cmd.Parameters.AddWithValue("@Observaciones", observaciones);

            SqlParameter pDet = new SqlParameter("@Detalle", SqlDbType.Structured) { Value = detalle, TypeName = "dbo.TipoDetalleKardex" };
            cmd.Parameters.Add(pDet);

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
}