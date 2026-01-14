using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;



public class UsuarioEmpleadoLOGIC
{
    UsuarioEmpleadoDAO usuarioEmpleadodao = new UsuarioEmpleadoDAO();

    public DataTable ObtenerUsuariosParaGrid()
    {
        return usuarioEmpleadodao.ListarUsuariosVista();
    }


}