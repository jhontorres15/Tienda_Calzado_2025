using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public class CompraDAO
{
    private readonly string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string Generar_NroCompra()
    {
        string codigo = "";
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Generar_NroCompra", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                var p = new SqlParameter("@NroCompra", SqlDbType.Char, 10) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(p);
                cn.Open(); cmd.ExecuteNonQuery();
                codigo = Convert.ToString(p.Value);
            }
            catch
            {
                codigo = "COM0000001";
            }
        }
        return codigo;
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

    public string Guardar(string nro, string codEmpleado, string nroCot, DateTime fec, decimal total, string estado, DataTable detalle)
    {
        string mensaje = "";
        using (SqlConnection cn = new SqlConnection(CN))
        using (SqlCommand cmd = new SqlCommand("USP_Orden_Compra_Guardar", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NroCompra", nro);
            cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
            cmd.Parameters.AddWithValue("@NroCotizacion", nroCot);
            cmd.Parameters.AddWithValue("@Fec_Compra", fec);
            cmd.Parameters.AddWithValue("@Total", total);
            cmd.Parameters.AddWithValue("@Estado_Compra", estado);

            SqlParameter pDet = new SqlParameter("@Detalle", SqlDbType.Structured) { Value = detalle, TypeName = "dbo.TipoDetalleCompra" };
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

    public DataTable Listar_Compras(string filtro)
    {
        DataTable dt = new DataTable();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            try
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Orden_Compra", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? string.Empty);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch
            {
                dt.Columns.Add("NroCompra"); dt.Columns.Add("NroCotizacion"); dt.Columns.Add("CodEmpleado"); dt.Columns.Add("Fec_Compra", typeof(DateTime)); dt.Columns.Add("Total", typeof(decimal)); dt.Columns.Add("Estado_Compra");
            }
        }
        return dt;
    }



    public DataSet CargarDetalle_DesdeCotizacion(string nroCotizacion, string codSucursal, out string mensajeAviso)
    {
        DataSet ds = new DataSet();
        mensajeAviso = "";

        using (SqlConnection con = new SqlConnection(CN))
        {
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("USP_CargarDetalle_DesdeCotizacion_Compra", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // --- Parámetros de Entrada ---
                    cmd.Parameters.AddWithValue("@NroCotizacion", nroCotizacion);
                    cmd.Parameters.AddWithValue("@CodSucursal", codSucursal);

                    // --- Parámetro de Salida (OUTPUT) ---
                    // Es vital definir el tamaño (Size) igual que en el SP (VARCHAR(500))
                    SqlParameter paramMensaje = new SqlParameter("@MensajeAviso", SqlDbType.VarChar, 500);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    // --- Ejecución ---
                    // Usamos SqlDataAdapter para llenar el DataSet (soporta múltiples tablas)
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }

                    // Recuperamos el mensaje que devolvió SQL
                    mensajeAviso = cmd.Parameters["@MensajeAviso"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                // Si falla la conexión o SQL, devolvemos el error en el mensaje
                mensajeAviso = "ERROR DAO: " + ex.Message;
                ds = null; // Anulamos el dataset para evitar lecturas erróneas
            }
        }

        return ds;
    }


    public string Registrar_OrdenCompra(string nroCompra, string codEmpleado, string nroCotizacion,
                                        DateTime fecCompra, decimal total, string estado, DataTable dtDetalleOrigen)
    {
        string mensaje = "";

        using (SqlConnection con = new SqlConnection(CN))
        {
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("USP_Orden_Compra_Guardar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // --- Parámetros Escalares (Datos de cabecera) ---
                    cmd.Parameters.AddWithValue("@NroCompra", nroCompra);
                    cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                    cmd.Parameters.AddWithValue("@NroCotizacion", nroCotizacion);
                    cmd.Parameters.AddWithValue("@Fec_Compra", fecCompra);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@Estado_Compra", estado);

                    // --- PARÁMETRO TIPO TABLA (TVP) ---
                    // 1. Creamos una tabla limpia con la estructura EXACTA del TYPE SQL
                    DataTable dtSQL = new DataTable();
                    dtSQL.Columns.Add("CodSucursal", typeof(string));
                    dtSQL.Columns.Add("NroSerie_Producto", typeof(string));
                    dtSQL.Columns.Add("Prec_Compra", typeof(decimal));
                    dtSQL.Columns.Add("Cantidad", typeof(int));
                    dtSQL.Columns.Add("Importe", typeof(decimal));
                    dtSQL.Columns.Add("Porcentaje_Dscto", typeof(decimal));
                    dtSQL.Columns.Add("Dscto", typeof(decimal));
                    dtSQL.Columns.Add("SubTotal", typeof(decimal));

                    // 2. Llenamos esta tabla limpia con los datos de la sesión
                    foreach (DataRow row in dtDetalleOrigen.Rows)
                    {
                        dtSQL.Rows.Add(
                            row["CodSucursal"],
                            row["NroSerie_Producto"],
                            row["Prec_Compra"],
                            row["Cantidad"],
                            row["Importe"],
                            row["Porcentaje_Dscto"],
                            row["Dscto"],
                            row["SubTotal"]
                        );
                    }

                    // 3. Enviamos la tabla limpia como parámetro Structured
                    SqlParameter paramDetalle = new SqlParameter("@Detalle", SqlDbType.Structured);
                    paramDetalle.TypeName = "dbo.TipoDetalleCompra"; // Nombre exacto del TYPE en SQL
                    paramDetalle.Value = dtSQL;
                    cmd.Parameters.Add(paramDetalle);

                    // --- Parámetro de Salida ---
                    SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                    paramMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramMensaje);

                    // Ejecutar
                    cmd.ExecuteNonQuery();

                    // Obtener respuesta
                    mensaje = cmd.Parameters["@Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR DAO: " + ex.Message;
            }
        }
        return mensaje;
    }
}

