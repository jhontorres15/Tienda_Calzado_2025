using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de PedidoLOGIC
/// </summary>
public class PedidoLOGIC
{
    private PedidoDAO pedidoDAO = new PedidoDAO();

    public DataTable ObtenerTiposPedido()
    {
        // Podrías agregar lógica de negocio aquí (validaciones, etc.)
        // Por ahora, solo llama al DAO
        return pedidoDAO.ListarTiposPedido();
    }

    public string RegistrarPedido(string nroPedido, string codSucursal, string codTipoPedido,
                                  string codCliente, string codEmpleado, DateTime fecPedido,
                                  decimal total, string estado, DataTable dtDetalle)
    {
        // Aquí podrías poner validaciones extras si quisieras antes de llamar al DAO

        return pedidoDAO.RegistrarPedido(nroPedido, codSucursal, codTipoPedido,
                                      codCliente, codEmpleado, fecPedido,
                                      total, estado, dtDetalle);
    }

    public string GenerarNroPedido()
    {
        return pedidoDAO.GenerarNroPedido();
    }

    public bool GuardarRelacionProforma(string nroProforma, string nroPedido)
    {
        try
        {
            // Simplemente pasamos los datos al DAO
            return pedidoDAO.Registrar_Proforma_Pedido(nroProforma, nroPedido);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable ListarPedidos()
    {
        return pedidoDAO.ListarPedidos();
    }

    public DataSet CargarDetalle_DesdeProforma(string nroProforma, string codSucursal, out string mensajeAviso)
    {
        // Simplemente pasamos la llamada al DAO
        return pedidoDAO.CargarDetalle_DesdeProforma(nroProforma, codSucursal, out mensajeAviso);
    }
}