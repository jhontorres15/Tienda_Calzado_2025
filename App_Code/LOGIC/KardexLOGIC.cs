using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de KardexLOGIC
/// </summary>
public class KardexLOGIC
{

    KardexDAO KardexDAO = new KardexDAO();

    public DataTable ListarSucursales()
    {

        return KardexDAO.ListarSucursales();
    }

    public DataTable Listar_Tipo_Facturacion()
    {
        return KardexDAO.Listar_Tipo_Facturacion();
    }


    public DataTable Listar_Tipo_Operacion()
    {
        return KardexDAO.Listar_Tipo_Operacion();
    }

    public int Generar_NroKardex()
    {
        return KardexDAO.Generar_NroKardex();
    }

    public DataSet ObtenerDatosFactura(string nroSerie)
    {
        return KardexDAO.ObtenerDatosFactura(nroSerie);
    }

    public string Guardar_Kardex(string nroOperacion, string codEmpleado, string tipoOperacion, string codSucursal, DateTime fecha, string codTipoFact, string nroSerie, string observaciones, DataTable detalle)
    {
        if (string.IsNullOrWhiteSpace(nroOperacion)) return "ERROR: Nro de operación es obligatorio";
        if (string.IsNullOrWhiteSpace(codEmpleado)) return "ERROR: Código de empleado es obligatorio";
        if (string.IsNullOrWhiteSpace(tipoOperacion)) return "ERROR: Tipo de operación es obligatorio";
        if (string.IsNullOrWhiteSpace(codSucursal)) return "ERROR: Sucursal es obligatoria";
        if (detalle == null || detalle.Rows.Count == 0) return "ERROR: Detalle vacío";
        return KardexDAO.Guardar_Kardex(nroOperacion, codEmpleado, tipoOperacion, codSucursal, fecha, codTipoFact, nroSerie, observaciones, detalle);
    }
}

