using System;
using System.Data;

public class CompraLOGIC
{
    private readonly CompraDAO dao = new CompraDAO();

    public string Generar_NroCompra() => dao.Generar_NroCompra();

    public DataTable Listar_Productos_NroSerie() => dao.Listar_Productos_NroSerie();

  
    public DataTable Listar_Compras(string filtro) => dao.Listar_Compras(filtro);

    public DataSet CargarDetalle_DesdeCotizacion(string nroCotizacion, string codSucursal, out string mensajeAviso)
    {
        try
        {
            // Validaciones previas de negocio (opcional)
            if (string.IsNullOrEmpty(nroCotizacion))
            {
                mensajeAviso = "ERROR: El número de cotización es obligatorio.";
                return null;
            }

            // Llamada al DAO
            return dao.CargarDetalle_DesdeCotizacion(nroCotizacion, codSucursal, out mensajeAviso);
        }
        catch (Exception ex)
        {
            mensajeAviso = "ERROR LOGIC: " + ex.Message;
            return null;
        }
    }

    public string Guardar(string nroCompra, string codEmpleado, string nroCotizacion,
                          DateTime fecCompra, decimal total, string estado, DataTable dtDetalle)
    {
        try
        {
            // Validaciones básicas de negocio antes de ir a BD
            if (dtDetalle == null || dtDetalle.Rows.Count == 0)
            {
                return "ERROR: El detalle de la compra no puede estar vacío.";
            }

            // Llamada al DAO
            return dao.Registrar_OrdenCompra(nroCompra, codEmpleado, nroCotizacion, fecCompra, total, estado, dtDetalle);
        }
        catch (Exception ex)
        {
            return "ERROR LOGIC: " + ex.Message;
        }
    }
}

