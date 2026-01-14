using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de DocumentoIdentidadLOGIC
/// </summary>
public class DocumentoIdentidadLOGIC
{
    private readonly DocumentoIdentidadDAO dao;
    public DocumentoIdentidadLOGIC()
    {
       dao = new DocumentoIdentidadDAO();
    }

    public List<string> ObtenerTiposDocumento(string nacionalidad)
    {
        return dao.ListarTiposDocumento(nacionalidad);
    }

}