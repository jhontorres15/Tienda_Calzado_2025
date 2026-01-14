using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de FacturacionLOGIC
/// </summary>
public class FacturacionLOGIC
{

    FacturacionDAO objDato = new FacturacionDAO();  
    // Retorna la tabla directa
    public DataTable Listar_Tipo_Facturacion()
    {
        return objDato.Listar_Tipo_Facturacion();
    }

    // Retorna el código generado
    public string Generar_NroSerie_Facturacion()
    {
        return objDato.Generar_NroSerie_Facturacion();
    }

    public DataSet ObtenerDatosFactura_ParaImpresion(string nroSerie)
    {
        return objDato.ObtenerDatosFactura_ParaImpresion(nroSerie);
    }

    public DataSet ObtenerDatosPedido_ParaFactura(string nroPedido)
    {
        return objDato.ObtenerDatosPedido_ParaFactura(nroPedido);
    }


    public bool Registrar_Facturacion(string nroSerie, string codTipo, string codEmpleado, string nroPedido, decimal subTotal, decimal igv, decimal total)
    {
        // Llamamos al método de la capa de datos
        return objDato.Registrar_Facturacion(nroSerie, codTipo, codEmpleado, nroPedido, subTotal, igv, total);
    }

    public DataTable Listar_Facturas_Venta()
    {
        return objDato.Listar_Facturas_Venta();
    }
}