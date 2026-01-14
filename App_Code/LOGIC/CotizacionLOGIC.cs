using System;
using System.Data;


public class CotizacionLOGIC
{
    CotizacionDAO CotizacionDAO = new CotizacionDAO();

    public string Generar_NroCotizacion()
    {
       return  CotizacionDAO.Generar_NroCotizacion();
    }

    public DataTable Listar_Proveedor()
    {
        return CotizacionDAO.Listar_Proveedor();
    }

    public DataTable Listar_Productos_NroSerie()
    {
        return CotizacionDAO.Listar_Productos_NroSerie();
    }

    public string Guardar(string nro, string codEmpleado, string codProveedor, DateTime fec, decimal total, string estado, DataTable detalle)
    {
        if (string.IsNullOrWhiteSpace(nro)) return "ERROR: Nro de cotización es obligatorio";
        if (string.IsNullOrWhiteSpace(codEmpleado)) return "ERROR: Código empleado es obligatorio";
        if (string.IsNullOrWhiteSpace(codProveedor)) return "ERROR: Código proveedor es obligatorio";
        if (detalle == null || detalle.Rows.Count == 0) return "ERROR: Agregue detalles";

        return CotizacionDAO.Guardar(nro, codEmpleado, codProveedor, fec, total, estado, detalle);
    }

    public DataSet Buscar_Productos_Cotizacion(string sucursal, string marca, string modelo, string talla, string color)
    {
        // Simplemente pasamos los parámetros al DAO
        // La lógica "pesada" ya está en el SQL que devuelve las 5 tablas
        return CotizacionDAO.Buscar_Productos_Cotizacion(sucursal, marca, modelo, talla, color);
    }

    public DataTable Listar_Cotizaciones(string filtro) => CotizacionDAO.Listar_Cotizaciones(filtro);
}
