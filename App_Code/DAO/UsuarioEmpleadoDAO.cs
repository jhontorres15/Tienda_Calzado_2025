using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de UsusarioEmpleadoDAO
/// </summary>
public class UsuarioEmpleadoDAO
{


    // Obtener la cadena de conexión de tu Web.config
    private string cnx = ConfigurationManager.ConnectionStrings["conexionBDproductos"].ConnectionString;
    public DataTable ListarUsuariosVista()
    {
        DataTable dt = new DataTable();

        using (SqlConnection cn = new SqlConnection(cnx))
        {
                // No necesitamos abrir la conexión manualmente con DataAdapter, él lo gestiona
                string query = "SELECT * FROM V_Listar_UsuarioEmpleado";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);

                // El método Fill abre la conexión, ejecuta el select y llena la tabla
                da.Fill(dt);
            
           
        }

        return dt;
    }
}