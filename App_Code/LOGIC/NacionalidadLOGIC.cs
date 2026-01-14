using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de NacionalidadLOGIC
/// </summary>
public class NacionalidadLOGIC
{
    
    private NacionalidadDAO dao = new NacionalidadDAO();

         public List<string> ObtenerNacionalidades()
        {
            return dao.ListarNacionalidades();
        }
    
}