using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Proforma
/// </summary>
public class ProformaDAO
{
  
        // Obtener la cadena de conexión de tu Web.config
        private string cnx = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

   
    /// [0]=GridView, [1]=Marcas, [2]=Modelos, [3]=Tallas, [4]=Colores
    /// </returns>
    public DataSet BuscarStockYFiltros(string sucursalNombre, string marca, string modelo, string talla, string color) // <-- CAMBIO 1
    {
        DataSet ds = new DataSet();

        using (SqlConnection con = new SqlConnection(cnx))
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("USP_Buscar_Stock_Proforma", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada

                // ¡CAMBIO 2! (Nombre del parámetro y variable)
                cmd.Parameters.AddWithValue("@SucursalNombre", sucursalNombre);

                // Asegurarnos de que los filtros vacíos se envíen como "" y no como null
                cmd.Parameters.AddWithValue("@Marca", string.IsNullOrEmpty(marca) ? "" : marca);
                cmd.Parameters.AddWithValue("@Modelo", string.IsNullOrEmpty(modelo) ? "" : modelo);
                cmd.Parameters.AddWithValue("@Talla", string.IsNullOrEmpty(talla) ? "" : talla);
                cmd.Parameters.AddWithValue("@Color", string.IsNullOrEmpty(color) ? "" : color);

                // Usamos SqlDataAdapter para llenar el DataSet con las 5 tablas
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }
        }

        // Asignar nombres a las tablas en el DataSet para claridad
        if (ds.Tables.Count == 5)
        {
            ds.Tables[0].TableName = "GridViewProductos";
            ds.Tables[1].TableName = "FiltroMarcas";
            ds.Tables[2].TableName = "FiltroModelos";
            ds.Tables[3].TableName = "FiltroTallas";
            ds.Tables[4].TableName = "FiltroColores";
        }

        return ds;
    }

    public string Guardar(string nroProforma, string codCliente, string codEmpleado,
                          DateTime fecEmision, DateTime fecCaducidad, decimal total,
                          string estado, DataTable dtDetalle) // Recibe la tabla de detalle
    {
        string mensaje = "";

        // Usamos 'using' para garantizar que todo se cierre correctamente
        using (SqlConnection con = new SqlConnection(cnx))
        {
            using (SqlCommand cmd = new SqlCommand("USP_Proforma_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // --- Parámetros de la Cabecera ---
                cmd.Parameters.AddWithValue("@NroProforma", nroProforma);
                cmd.Parameters.AddWithValue("@CodCliente", codCliente);
                cmd.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                cmd.Parameters.AddWithValue("@Fec_Emision", fecEmision);
                cmd.Parameters.AddWithValue("@Fec_Caducidad", fecCaducidad);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.Parameters.AddWithValue("@Estado_Proforma", estado);

                // --- Parámetro TVP (El Detalle) ---
                SqlParameter paramDetalle = new SqlParameter("@Detalle", SqlDbType.Structured);
                paramDetalle.Value = dtDetalle;
                paramDetalle.TypeName = "dbo.TipoDetalleProforma"; // El TYPE que creamos en SQL
                cmd.Parameters.Add(paramDetalle);

                // --- Parámetro de Salida (Mensaje) ---
                SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                paramMensaje.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramMensaje);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    // Obtenemos el mensaje de respuesta del SP
                    mensaje = paramMensaje.Value.ToString();
                }
                catch (Exception ex)
                {
                    // Si algo falla, capturamos el error
                    mensaje = "Error en DAO: " + ex.Message;
                }
            } // cmd.Dispose() se llama aquí
        } // con.Close() y con.Dispose() se llaman aquí

        return mensaje;
    }

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

    public DataTable ListarProformasTodos()
    {
        DataTable dt = new DataTable();
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                // Llama al nuevo SP (USP_Proforma_Listar_Todos)
                using (SqlCommand cmd = new SqlCommand("USP_Listar_Proforma", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    // No se necesitan parámetros

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Manejo de error
                Console.WriteLine("Error en ProformaDAO.ListarProformasTodos: " + ex.Message);
            }
        }
        return dt;
    }


    public DataSet ObtenerDatosProforma(string nroProforma)
    {
        DataSet ds = new DataSet(); // Usamos un DataSet para recibir múltiples tablas
        using (SqlConnection con = new SqlConnection(cnx))
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("USP_Proforma_ObtenerDatos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NroProforma", nroProforma);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    // El DataAdapter llenará el DataSet con AMBAS tablas
                    // ds.Tables[0] será la Cabecera
                    // ds.Tables[1] será el Detalle
                    da.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                // Manejo de error
                Console.WriteLine("Error en ProformaDAO.ObtenerDatosProforma: " + ex.Message);
            }
        }
        return ds;
    }
}