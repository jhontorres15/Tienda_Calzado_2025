using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Descripción breve de AlmacenDAO
/// </summary>
public class AlmacenDAO
{


    private string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public string GenerarNumeroDeSerie()
    {
        string nroSerie = "";

        using (SqlConnection cn = new SqlConnection(CN))
        {
            // Nombre del procedimiento almacenado para generar la serie
            SqlCommand cmd = new SqlCommand("USP_Generar_NroSerieProducto", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parámetro OUTPUT: @NroSerie CHAR(7)
            SqlParameter param = new SqlParameter("@NroSerie", SqlDbType.Char, 7);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);

            cn.Open();
            cmd.ExecuteNonQuery();

            // Capturar el valor devuelto
            nroSerie = param.Value.ToString();
        }

        return nroSerie;
    }

    public List<string> ListarSucursal()
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(CN))
        using (SqlCommand cmd = new SqlCommand("USP_Listar_Sucursal", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    lista.Add(dr.GetValue(0).ToString());
                }
            }
        }

        return lista;
    }

    public List<string> ListarMarcasFiltradas()
    {
        List<string> lista = new List<string>();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Marcas_Filtradas", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que la única columna devuelta es Marca
                lista.Add(dr["Marca"].ToString().Trim());
            }
        }
        return lista;
    }

    public List<string> ListarModelosFiltrados(string Marca)
    {
        List<string> lista = new List<string>();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Modelos_Filtrados", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Marca", Marca);
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que la única columna devuelta es Modelo
                lista.Add(dr["Modelo"].ToString().Trim());
            }
        }
        return lista;
    }

    public List<string> ListarTallasFiltradas(string Marca, string Modelo)
    {
        List<string> lista = new List<string>();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            // 1. 💡 Usar el nuevo SP que acepta parámetros
            SqlCommand cmd = new SqlCommand("USP_Listar_Tallas_Filtradas_Por_Modelo", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // 2. ✅ AGREGAR LOS PARÁMETROS para que el SP filtre
            cmd.Parameters.AddWithValue("@Marca", Marca);
            cmd.Parameters.AddWithValue("@Modelo", Modelo);

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(dr["Talla"].ToString().Trim());
            }
        }
        return lista;
    }
    public List<string> ListarColoresFiltrados(string Marca, string Modelo, string Talla)
    {
        List<string> lista = new List<string>();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Colores_Filtrados", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Marca", Marca);
            cmd.Parameters.AddWithValue("@Modelo", Modelo);
            cmd.Parameters.AddWithValue("@Talla", Talla);
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que la única columna devuelta es Color
                lista.Add(dr["Color"].ToString().Trim());
            }
        }
        return lista;
    }

    public List<ListItem> ListarProductosFinales(string Marca, string Modelo, string Talla, string Color)
    {
        List<ListItem> lista = new List<ListItem>();
        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Productos_Finales", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Marca", Marca);
            cmd.Parameters.AddWithValue("@Modelo", Modelo);
            cmd.Parameters.AddWithValue("@Talla", Talla);
            cmd.Parameters.AddWithValue("@Color", Color);
            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Valor: CodProducto, Texto: DescripcionFinal (CodProducto + Nombre/Detalle)
                string value = dr["CodProducto"].ToString();
                string text = dr["DescripcionFinal"].ToString();
                lista.Add(new ListItem(text, value));
            }
        }
        return lista;
    }


    // Método para manejar INSERT, UPDATE y DELETE y devolver solo el mensaje.
    public string MantenimientoAlmacen(
        string sucursal,
        string nroSerieProducto,
        string codProducto,
        int stockActual,
        int stockMinimo,
        int stockMaximo,
        string estadoProducto,
        string tipoTransaccion)
    {
        string mensajeSalida = "";

        // 1. Crear la conexión (Ajusta la referencia a tu clase de conexión real)
        using (SqlConnection cn = new SqlConnection(CN)) // Suponiendo una clase ConexionDB
        {
            cn.Open();

            // 2. Definir el comando
            using (SqlCommand cmd = new SqlCommand("USP_Almacen_Sucursal", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. Definir y Asignar Parámetros de ENTRADA
                cmd.Parameters.AddWithValue("@Sucursal", sucursal);
                cmd.Parameters.AddWithValue("@NroSerie_Producto", nroSerieProducto);

                // Nota: El SP espera el código del producto en el parámetro @Producto
                cmd.Parameters.AddWithValue("@Producto", codProducto);

                cmd.Parameters.AddWithValue("@Stock_Actual", stockActual);
                cmd.Parameters.AddWithValue("@Stock_Minimo", stockMinimo);
                cmd.Parameters.AddWithValue("@Stock_Maximo", stockMaximo);
                cmd.Parameters.AddWithValue("@Estado_Producto", estadoProducto);
                cmd.Parameters.AddWithValue("@Tipo_Transaccion", tipoTransaccion);

                // 4. Definir Parámetro de SALIDA (@Mensaje)
                SqlParameter parMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200);
                parMensaje.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(parMensaje);

                // 5. Ejecutar el comando
                cmd.ExecuteNonQuery();

                // 6. Obtener el valor del parámetro de salida
                mensajeSalida = parMensaje.Value.ToString();
            }
        }
        return mensajeSalida;
    }



    public DataTable ListarInventarioCompleto()
    {
        DataTable dt = new DataTable();

        using (SqlConnection cn = new SqlConnection(CN)) // Suponiendo una clase ConexionDB
        {
            cn.Open();

            // Consulta SQL que selecciona DIRECTAMENTE de la vista sin WHERE
            string consultaSQL = @"
                SELECT * FROM V_Listar_Productos_Almacen";

            using (SqlCommand cmd = new SqlCommand(consultaSQL, cn))
            {
                cmd.CommandType = CommandType.Text;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt;
    }


    public DataTable ObtenerFiltrosPorCodProducto(string codProducto)
    {
        DataTable dt = new DataTable("Filtros");
        using (SqlConnection cn = new SqlConnection(CN))
        {
            cn.Open();
            // ESTA ES LA CONSULTA SQL DEFINITIVA
            string sql = @"
                SELECT
                    MAR.Marca,       -- Nombre de la Marca (string)
                    MOD.Modelo,      -- Nombre del Modelo (string)
                    TALLA.Talla,     -- Nombre de la Talla (string)
                    PROD.Color       -- Nombre del Color (string)
                FROM
                    Tb_Producto AS PROD
                INNER JOIN
                    Tb_Modelo AS MOD ON PROD.CodModelo = MOD.CodModelo
                INNER JOIN
                    Tb_Marca AS MAR ON MOD.CodMarca = MAR.CodMarca
                INNER JOIN
                    Tb_Talla AS TALLA ON PROD.CodTalla = TALLA.CodTalla
                WHERE
                    PROD.CodProducto = @CodProducto";

            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@CodProducto", codProducto);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
        }
        return dt; // Devuelve la tabla con los 4 NOMBRES
    }
}