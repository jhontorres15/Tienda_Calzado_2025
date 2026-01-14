using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class NacionalidadDAO
{
    private string conexion = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;

    public List<string> ListarNacionalidades()
    {
        List<string> lista = new List<string>();

        using (SqlConnection cn = new SqlConnection(conexion))
        {
            SqlCommand cmd = new SqlCommand("USP_Listar_Nacionalidad", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(dr.GetString(0));
            }
        }

        return lista;
    }
}