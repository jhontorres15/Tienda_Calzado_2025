using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de FacturacionDAO
/// </summary>
public class FacturacionDAO
{

    private string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    // 1. METODO PARA LISTAR (Devuelve un DataTable)


    public DataSet ObtenerDatosFactura_ParaImpresion(string nroSerie)
    {
        DataSet ds = new DataSet();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("USP_Facturacion_ObtenerDatos_Impresion", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerie);

                    // El DataAdapter llena el DataSet con los 2 SELECT que tiene el SP
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Es buena práctica lanzar la excepción para que el LOGIC la maneje
                throw new Exception("Error DAO al obtener datos de impresión: " + ex.Message);
            }
        }

        return ds;
    }
    public DataTable Listar_Tipo_Facturacion()
    {
        DataTable dt = new DataTable();

        // Usamos "using" para asegurar que la conexión se cierre sola
        using (SqlConnection cn = new SqlConnection(CN))
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

    // 2. METODO PARA GENERAR SERIE (Devuelve string)
    public string Generar_NroSerie_Facturacion()
    {
        string codigoGenerado = "";

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_NroSerie_Facturacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de SALIDA (OUTPUT)
                SqlParameter parametro = new SqlParameter();
                parametro.ParameterName = "@NroSerie";
                parametro.SqlDbType = SqlDbType.Char;
                parametro.Size = 10;
                parametro.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(parametro);

                cn.Open();
                cmd.ExecuteNonQuery();

                // Leer el valor que devolvió SQL
                codigoGenerado = cmd.Parameters["@NroSerie"].Value.ToString();
            }
            catch (Exception ex)
            {
                codigoGenerado = "ERROR"; // Manejo simple de error
            }
        }
        return codigoGenerado;
    }


    public DataSet ObtenerDatosPedido_ParaFactura(string nroPedido)
    {
        DataSet ds = new DataSet();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Pedido_ObtenerDatos", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Pasamos el parámetro
                cmd.Parameters.AddWithValue("@NroPedido", nroPedido);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                cn.Open();
                // Esto llena:
                // ds.Tables[0] -> Cabecera (Cliente, ImporteBase, IGV, Total)
                // ds.Tables[1] -> Detalle (Productos para la grilla)
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                // Es buena práctica lanzar el error para verlo en la capa superior
                throw new Exception("Error en D_Facturacion: " + ex.Message);
            }
        }
        return ds;
    }

    public bool Registrar_Facturacion(string nroSerie, string codTipo, string codEmpleado, string nroPedido, decimal subTotal, decimal igv, decimal total)
    {
        bool respuesta = false;

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Registrar_Facturacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Agregamos los parámetros que pide tu Store Procedure
                cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerie);
                cmd.Parameters.AddWithValue("@CodTipo_Facturacion", codTipo);
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                cmd.Parameters.AddWithValue("@NroPedido", nroPedido);
                cmd.Parameters.AddWithValue("@SubTotal", subTotal);
                cmd.Parameters.AddWithValue("@IGV", igv);
                cmd.Parameters.AddWithValue("@Total", total);

                cn.Open();

                // Ejecutamos. Si devuelve filas afectadas (>0), fue exitoso.
                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas > 0)
                {
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos false (podrías guardar el log del error aquí)
                respuesta = false;
                throw ex;
            }
        }
        return respuesta;
    }

    public DataTable Listar_Facturas_Venta()
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Facturas_Venta", cn);
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
}


