using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de DocumentoIdentidadDAO
/// </summary>
public class DocumentoIdentidadDAO
{
    private readonly string conexion;

    public DocumentoIdentidadDAO()
    {
        conexion = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;
    }

    public List<string> ListarTiposDocumento(string nacionalidad)
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(conexion))
        using (SqlCommand cmd = new SqlCommand("USP_Listar_Documento_Identidad", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Nacionalidad", SqlDbType.VarChar, 60).Value = nacionalidad;

            cn.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    // La columna 1 es donde estás leyendo el tipo de documento
                    lista.Add(dr.GetValue(1).ToString());
                }
            }
        }

        return lista;
    }
}