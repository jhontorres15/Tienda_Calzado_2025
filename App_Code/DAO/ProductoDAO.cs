using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Descripción breve de ProductoDAO
/// </summary>
public class ProductoDAO
{

    private string CN = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;


    //----------------------------------------------------------
    // MÉTODOS PARA LISTAR CODProducto
    //----------------------------------------------------------

    public string GenerarCodigoProducto()
    {
        string codigo = "";

        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Generar_CodProducto", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parámetro OUTPUT
            SqlParameter param = new SqlParameter("@CodProducto", SqlDbType.Char, 7);
            param.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(param);

            cn.Open();
            cmd.ExecuteNonQuery();

            codigo = param.Value.ToString();
        }

        return codigo;
    }

    //----------------------------------------------------------
    // MÉTODOS PARA LISTAR cATEGORIA
    //----------------------------------------------------------

    public List<string> ListarCategorias()
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Categorias", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Cambia el índice si tu SP devuelve otra estructura
                lista.Add(dr.GetValue(1).ToString());
            }
        }

        return lista;
    }

    //----------------------------------------------------------
    // MÉTODOS PARA LISTAR MARCA
    //----------------------------------------------------------

    public List<string> ListarMarcas()
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Marcas", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que el índice 1 del SP devuelve el Nombre de la Marca
                lista.Add(dr.GetValue(1).ToString());
            }
        }

        return lista;
    }

    //----------------------------------------------------------
    // MÉTODOS PARA LISTAR MODELOS (DEPENDIENTE DE MARCA)
    //----------------------------------------------------------

    public List<string> ListarModelos(string Marca)
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Modelos", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parámetro de entrada: Nombre de la Marca
            cmd.Parameters.AddWithValue("@Marca", Marca);

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que el índice 0 del SP devuelve el Nombre del Modelo
                lista.Add(dr.GetValue(0).ToString());
            }
        }

        return lista;
    }

    //----------------------------------------------------------
    // MÉTODOS PARA LISTAR TALLAS
    //----------------------------------------------------------

    public List<string> ListarTallas()
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Tallas", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                // Asumimos que el índice 1 del SP devuelve el Nombre/Valor de la Talla
                lista.Add(dr.GetValue(1).ToString());
            }
        }

        return lista;
    }




    // --------------------------------------------------------------------------------
    // 3. MANTENIMIENTO (GUARDAR/ACTUALIZAR)
    // --------------------------------------------------------------------------------

    public string GuardarProducto(
                                     string CodProducto, 
                                     string Producto, 
                                     string Descripcion_Producto, 
                                     string Talla,
                                     string Modelo,
                                     string Color, 
                                     string Prec_Venta_Menor, 
                                     string Prec_Venta_Mayor,
                                     string Stock_General,
                                     string Categoria,
                                     byte[] FotoBytes, 
                                     string Estado_Producto,
                                     string Tipo_Transaccion)
    {
        string mensaje = "";

        // --- MANEJO DE VALORES NUMÉRICOS SEGUROS EN EL DAO ---
        // Esto previene el FormatException si los campos están vacíos o usan formato peruano (coma).
        decimal precioMenor;
        decimal precioMayor;
        int stock;

        // 1. Parseo seguro de Precios (Decimal)
        decimal.TryParse(Prec_Venta_Menor.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out precioMenor);
        decimal.TryParse(Prec_Venta_Mayor.Trim().Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out precioMayor);

        // 2. Parseo seguro de Stock (Int)
        int.TryParse(Stock_General.Trim(), out stock);


        using (SqlConnection cn = new SqlConnection(CN))
        {
            SqlCommand cmd = new SqlCommand("USP_Mantenimiento_Producto", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            // --- Parámetros de Entrada ---
            cmd.Parameters.AddWithValue("@CodProducto", CodProducto.ToUpper());
            cmd.Parameters.AddWithValue("@Producto", Producto.ToUpper());
            cmd.Parameters.AddWithValue("@Descripcion_Producto", Descripcion_Producto);
            cmd.Parameters.AddWithValue("@Talla", Talla);
            cmd.Parameters.AddWithValue("@Modelo", Modelo);
            cmd.Parameters.AddWithValue("@Color", Color);
            cmd.Parameters.AddWithValue("@Prec_Venta_Menor", precioMenor);
            cmd.Parameters.AddWithValue("@Prec_Venta_Mayor", precioMayor);
            cmd.Parameters.AddWithValue("@Stock_General", stock);
            cmd.Parameters.AddWithValue("@Categoria", Categoria);

            // Manejo de Foto (VarBinary) con DBNull
            SqlParameter paramFoto = new SqlParameter("@Foto", SqlDbType.VarBinary, -1);
            paramFoto.Value = FotoBytes != null ? (object)FotoBytes : DBNull.Value;
            cmd.Parameters.Add(paramFoto);

            cmd.Parameters.AddWithValue("@Estado_Producto", Estado_Producto);
            cmd.Parameters.AddWithValue("@Tipo_Transaccion", Tipo_Transaccion);

            // --- Parámetro de Salida ---
            SqlParameter paramMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 255);
            paramMensaje.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(paramMensaje);

            cn.Open();
            cmd.ExecuteNonQuery();

            mensaje = paramMensaje.Value.ToString();
        }

        return mensaje;
    }

}