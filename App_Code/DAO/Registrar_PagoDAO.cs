using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Registrar_Pago
/// </summary>
public class Registrar_PagoDAO
{

    // Obtener la cadena de conexión de tu Web.config
    private string cnx = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public int Generar_NroPago()
    {
        int nuevoCodigo = 0;

        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_NroPago", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Configurar el parámetro de SALIDA (OUTPUT)
                SqlParameter parametro = new SqlParameter();
                parametro.ParameterName = "@NuevoNroPago";
                parametro.SqlDbType = SqlDbType.Int;
                parametro.Direction = ParameterDirection.Output; // ¡Esto es clave!

                cmd.Parameters.Add(parametro);

                cn.Open();
                cmd.ExecuteNonQuery(); // Ejecutamos

                // Leemos el valor que SQL puso en el parámetro
                nuevoCodigo = Convert.ToInt32(cmd.Parameters["@NuevoNroPago"].Value);
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos 0 o lanzamos la excepción
                nuevoCodigo = 0;
                throw ex;
            }
        }
        return nuevoCodigo;
    }

    public DataTable Listar_Forma_Pago()
    {
        DataTable dt = new DataTable();
        // Usa tu cadena de conexión
        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Forma_Pago", cn);
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


    public string Registrar_Pago_Validado(int nroPago, string codEmpleado, string nroSerieFactura, string codFormaPago, decimal importe, DateTime fecPago)
    {
        string mensajeRespuesta = "";

        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Registrar_Pago_Con_Validacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // --- Parámetros de Entrada ---
                // 1. El número de pago que generaste en el formulario (int)
                cmd.Parameters.AddWithValue("@NroPago", nroPago);

                // 2. Datos del empleado y la factura
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                cmd.Parameters.AddWithValue("@NroSerie_Facturacion", nroSerieFactura);

                // 3. Datos del pago
                cmd.Parameters.AddWithValue("@CodForma_Pago", codFormaPago);
                cmd.Parameters.AddWithValue("@Importe_Pago", importe);

                // 4. La fecha seleccionada por el usuario
                cmd.Parameters.AddWithValue("@Fecha_Pago", fecPago);

                // --- Parámetro de Salida (OUTPUT) ---
                // Aquí recibiremos: "OK...", "ERROR...", etc.
                SqlParameter paramMensaje = new SqlParameter();
                paramMensaje.ParameterName = "@MensajeSalida";
                paramMensaje.SqlDbType = SqlDbType.VarChar;
                paramMensaje.Size = 100;
                paramMensaje.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramMensaje);

                cn.Open();
                cmd.ExecuteNonQuery();

                // Recuperamos el mensaje que nos envió SQL Server
                mensajeRespuesta = cmd.Parameters["@MensajeSalida"].Value.ToString();
            }
            catch (Exception ex)
            {
                mensajeRespuesta = "ERROR CRÍTICO EN DAO: " + ex.Message;
            }
        }
        return mensajeRespuesta;
    }

    public DataTable Listar_Pagos(string filtro)
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Pagos", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro);

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

