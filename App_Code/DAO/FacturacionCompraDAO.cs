using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de FacturacionCompraDAO
/// </summary>
public class FacturacionCompraDAO
{
    private string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;


    public DataSet ObtenerDatosCompra_ParaFactura(string nroCompra)
    {
        DataSet ds = new DataSet();

        using (SqlConnection con = new SqlConnection(CN))
        {
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("USP_Compra_ObtenerDatos_ParaFactura", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    cmd.Parameters.AddWithValue("@NroCompra", nroCompra);

                    // Usamos DataAdapter para llenar el DataSet (Tablas 0 y 1)
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Lanzamos el error hacia arriba para que el LOGIC o el formulario lo manejen
                throw new Exception("Error DAO al obtener datos de compra: " + ex.Message);
            }
        }

        return ds;
    }

    // 1. METODO PARA LISTAR (Devuelve un DataTable)
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

        // Asumo que tu variable de conexión se llama 'cadena' o 'CN' según tu clase DAO
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                cn.Open();
                // 1. Nombre EXACTO del SP creado arriba
                SqlCommand cmd = new SqlCommand("USP_Generar_NroSerie_FactCompra", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // 2. Parámetro de SALIDA (OUTPUT)
                // El nombre debe ser EXACTO al del SP: @NroSerie_Facturacion
                SqlParameter parametro = new SqlParameter();
                parametro.ParameterName = "@NroSerie_Facturacion";
                parametro.SqlDbType = SqlDbType.Char;
                parametro.Size = 10;
                parametro.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(parametro);

                cmd.ExecuteNonQuery();

                // 3. Leer el valor devuelto
                if (cmd.Parameters["@NroSerie_Facturacion"].Value != DBNull.Value)
                {
                    codigoGenerado = cmd.Parameters["@NroSerie_Facturacion"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                // Retornamos vacío o un código de error controlado
                codigoGenerado = "";
                throw new Exception("Error al generar serie: " + ex.Message);
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

    public bool Registrar_Facturacion(string nroSerie, string codEmpresa, string codTipo,
                                      string nroCompra, DateTime fecEmision, decimal subTotal,
                                      decimal igv, decimal total, string estado)
    {
        bool respuesta = false;

        // Asumo que tu variable de cadena se llama 'cadena' o 'CN'
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                // 1. Nombre EXACTO del Procedimiento Almacenado
                SqlCommand cmd = new SqlCommand("USP_Facturacion_Compra_Guardar", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // 2. Parámetros de Entrada
                cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerie);
                cmd.Parameters.AddWithValue("@CodEmpresa", codEmpresa);
                cmd.Parameters.AddWithValue("@CodTipo_Facturacion", codTipo);
                cmd.Parameters.AddWithValue("@NroCompra", nroCompra);

                // ¡IMPORTANTE! Aquí agregamos la fecha que faltaba
                cmd.Parameters.AddWithValue("@Fec_Emision", fecEmision);

                cmd.Parameters.AddWithValue("@SubTotal", subTotal);
                cmd.Parameters.AddWithValue("@IGV", igv);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Estado_Facturacion", estado);

                // 3. Parámetro de Salida (Mensaje)
                // Necesario para capturar el "OK" o "Error" del SP
                SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                paramMensaje.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramMensaje);

                cn.Open();
                cmd.ExecuteNonQuery();

                // 4. Analizar la respuesta del SQL
                string mensajeRetorno = cmd.Parameters["@Mensaje"].Value.ToString();

                // Si el mensaje empieza con "OK", todo salió bien
                if (mensajeRetorno.StartsWith("OK"))
                {
                    respuesta = true;
                }
                else
                {
                    // Opcional: Podrías lanzar una excepción con el mensaje de error del SQL
                    // throw new Exception(mensajeRetorno); 
                    respuesta = false;
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                throw ex;
            }
        }

        return respuesta;
    }

    public DataTable listar_facturas(string filtro)
    {
        DataTable dt = new DataTable();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                // CORRECCIÓN: Cambiado de _Venta a _Compra
                SqlCommand cmd = new SqlCommand("USP_Listar_Facturas_Compra", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@filtro", filtro);

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