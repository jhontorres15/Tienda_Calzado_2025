using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de PedidoDAO
/// </summary>
public class PedidoDAO
{
    // Obtener la cadena de conexión de tu Web.config
    private string cnx = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public DataTable ListarTiposPedido()
    {
        DataTable dt = new DataTable();
        // Usamos 'using' para asegurar que la conexión se cierre
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("USP_Listar_TiposPedido", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                // Manejar el error (ej. lanzarlo para que la capa LOGIC lo vea)
                throw ex;
            }
        } // 'using' cierra la conexión aquí, incluso si hay error
        return dt;
    }

    // 1. GENERAR EL NÚMERO DE PEDIDO
    public string GenerarNroPedido()
    {
        string nro = "";
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("USP_Generar_NroPedido", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de Salida
                SqlParameter pNro = new SqlParameter("@NroPedido", SqlDbType.Char, 12);
                pNro.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pNro);

                cmd.ExecuteNonQuery();

                nro = pNro.Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        return nro;
    }

    public string RegistrarPedido(string nroPedido, string codSucursal, string codTipoPedido,
                                  string codCliente, string codEmpleado, DateTime fecPedido,
                                  decimal total, string estado, DataTable dtDetalle)
    {
        string mensaje = "";

        // =========================================================================
        // PASO 1: CREAR UNA TABLA "LIMPIA" SOLO PARA SQL (7 Columnas)
        // =========================================================================
        DataTable dtParaSQL = new DataTable();

        // Definimos las columnas EXACTAS que pide tu tipo de dato en SQL (dbo.TipoDetallePedido)
        dtParaSQL.Columns.Add("NroSerie_Producto", typeof(string));
     
        dtParaSQL.Columns.Add("Prec_Venta", typeof(decimal));
        dtParaSQL.Columns.Add("Cantidad", typeof(int));
        dtParaSQL.Columns.Add("Porcentaje_Dscto", typeof(decimal));
        dtParaSQL.Columns.Add("Dscto", typeof(decimal));
        dtParaSQL.Columns.Add("SubTotal", typeof(decimal));

        // =========================================================================
        // PASO 2: COPIAR DATOS (Ignorando 'ProductoNombre')
        // =========================================================================
        foreach (DataRow filaOriginal in dtDetalle.Rows)
        {
            DataRow filaNueva = dtParaSQL.NewRow();

            // Copiamos dato por dato. NO COPIAMOS 'ProductoNombre'
            filaNueva["NroSerie_Producto"] = filaOriginal["NroSerie_Producto"];
          
            filaNueva["Prec_Venta"] = filaOriginal["Prec_Venta"];
            filaNueva["Cantidad"] = filaOriginal["Cantidad"];
            filaNueva["Porcentaje_Dscto"] = filaOriginal["Porcentaje_Dscto"];
            filaNueva["Dscto"] = filaOriginal["Dscto"];
            filaNueva["SubTotal"] = filaOriginal["SubTotal"];

            dtParaSQL.Rows.Add(filaNueva);
        }
        // =========================================================================

        using (SqlConnection con = new SqlConnection(cnx))
        {
            using (SqlCommand cmd = new SqlCommand("USP_Registrar_Pedido", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // --- Parámetros de la Cabecera ---
                cmd.Parameters.AddWithValue("@NroPedido", nroPedido);
                cmd.Parameters.AddWithValue("@CodSucursal", codSucursal);
                cmd.Parameters.AddWithValue("@CodTipo_Pedido", codTipoPedido);
                cmd.Parameters.AddWithValue("@CodCliente", codCliente);
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                cmd.Parameters.AddWithValue("@Fec_Pedido", fecPedido);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Estado_Pedido", estado);

                // --- Parámetro TVP (El Detalle) ---
                SqlParameter paramDetalle = new SqlParameter("@Detalle", SqlDbType.Structured);

                // ¡AQUÍ ESTÁ EL CAMBIO IMPORTANTE!
                // Enviamos dtParaSQL (la de 7 columnas) en lugar de dtDetalleOriginal (la de 8 columnas)
                paramDetalle.Value = dtParaSQL;

                paramDetalle.TypeName = "dbo.TipoDetallePedido";

                cmd.Parameters.Add(paramDetalle);

                // --- Parámetro de Salida (Mensaje) ---
                SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                paramMensaje.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramMensaje);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = paramMensaje.Value.ToString();
                }
                catch (Exception ex)
                {
                    mensaje = "Error en DAO Pedido: " + ex.Message;
                }
            }
        }
        return mensaje;
    }

    public DataTable ListarPedidos()
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(cnx))
        {
            using (SqlCommand cmd = new SqlCommand("USP_Listar_Pedidos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Opcional: Manejar error o lanzarlo
                    throw new Exception("Error en DAO al listar pedidos: " + ex.Message);
                }
            }
        }
        return dt;
    }

    // Método que retorna un DataSet (Tablas) y un mensaje de texto (out string)
    public DataSet CargarDetalle_DesdeProforma(string nroProforma, string codSucursal, out string mensajeAviso)
    {
        DataSet ds = new DataSet();
        mensajeAviso = ""; // Inicializamos la variable de salida

        using (SqlConnection cn = new SqlConnection(cnx))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_CargarDetalle_DesdeProforma", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // --- Parámetros de Entrada ---
                cmd.Parameters.AddWithValue("@NroProforma", nroProforma);
                cmd.Parameters.AddWithValue("@CodSucursal", codSucursal);

                // --- Parámetro de Salida (OUTPUT) ---
                // Aquí recibiremos: "OK", "ADVERTENCIA DE STOCK..." o "ERROR: Cliente Suspendido"
                SqlParameter paramMensaje = new SqlParameter();
                paramMensaje.ParameterName = "@MensajeAviso";
                paramMensaje.SqlDbType = SqlDbType.VarChar;
                paramMensaje.Size = 500;
                paramMensaje.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramMensaje);

                // --- Ejecución ---
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cn.Open();

                // El Fill llenará:
                // ds.Tables[0] -> Datos del Cliente (Cabecera)
                // ds.Tables[1] -> Listado de Productos (Detalle con stock ajustado)
                da.Fill(ds);

                // Recuperamos el mensaje que nos envió el SQL
                if (cmd.Parameters["@MensajeAviso"].Value != DBNull.Value)
                {
                    mensajeAviso = cmd.Parameters["@MensajeAviso"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                // Si falla la conexión o el SQL, devolvemos null y el error en el mensaje
                mensajeAviso = "ERROR CRÍTICO EN DAO: " + ex.Message;
                return null;
            }
        }
        return ds;
    }

    // Método para vincular el Pedido con la Proforma y actualizar estado
    public bool Registrar_Proforma_Pedido(string nroProforma, string nroPedido)
    {
        bool respuesta = false;

        // Usamos 'using' para asegurar que la conexión se cierre automáticamente
        // Asumo que tu cadena de conexión se llama "Cnx" en el Web.Config, ajustalo si es diferente
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                con.Open();

                // Llamamos al Stored Procedure que creamos en el paso anterior
                using (SqlCommand cmd = new SqlCommand("usp_RegistrarPedido_ActualizarProforma", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parámetros
                    cmd.Parameters.AddWithValue("@NroProforma", nroProforma);
                    cmd.Parameters.AddWithValue("@NroPedido", nroPedido);

                    // Ejecutamos la consulta
                    int filas = cmd.ExecuteNonQuery();

                    // Si filas > 0, significa que se hizo el insert y el update correctamente
                    if (filas > 0)
                    {
                        respuesta = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Es buena práctica lanzar el error hacia la capa lógica para saber qué pasó
                throw new Exception("Error en DAO al registrar relación Proforma-Pedido: " + ex.Message);
            }
        }

        return respuesta;
    }
}