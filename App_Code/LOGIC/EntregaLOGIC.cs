using System;
using System.Data;

public class EntregaLOGIC
{
    private readonly EntregaDAO dao = new EntregaDAO();

    public string Generar_NroEntrega()
    {
        return dao.Generar_NroEntrega();
    }

    public DataTable Listar_Tipo_Entrega()
    {
        return dao.Listar_Tipo_Entrega();
    }

    public string Registrar_Entrega(string nroEntrega, string codEmpleado, string codTipoEntrega, DateTime fecEntrega,
        string nroGuia, string cpostal, string direccion, string estado, string obs)
    {
        if (string.IsNullOrWhiteSpace(nroEntrega)) return "ERROR: Nro de Entrega es obligatorio";
        if (string.IsNullOrWhiteSpace(codEmpleado)) return "ERROR: Código de empleado es obligatorio";
        if (string.IsNullOrWhiteSpace(codTipoEntrega)) return "ERROR: Tipo de entrega es obligatorio";
        if (string.IsNullOrWhiteSpace(nroGuia)) return "ERROR: Nro Guía de Remisión es obligatorio";
        if (string.IsNullOrWhiteSpace(direccion)) return "ERROR: Dirección de entrega es obligatoria";
        if (direccion.Length > 100) return "ERROR: Dirección supera el máximo permitido";
        if (!string.IsNullOrWhiteSpace(cpostal) && cpostal.Length > 6) return "ERROR: Código postal inválido";
        if (!string.IsNullOrWhiteSpace(obs) && obs.Length > 200) return "ERROR: Observaciones exceden el máximo permitido";

        return dao.Registrar_Entrega(nroEntrega, codEmpleado, codTipoEntrega, fecEntrega, nroGuia, cpostal, direccion, estado, obs);
    }

    public DataTable Listar_Entregas(string filtro)
    {
        return dao.Listar_Entregas(filtro);
    }
}
