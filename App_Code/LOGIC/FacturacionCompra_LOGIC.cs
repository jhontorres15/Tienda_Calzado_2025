using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Facturacion_LOGIC
/// </summary>
public class FacturacionCompra_LOGIC
{
     FacturacionCompraDAO dao = new FacturacionCompraDAO();

    public string Generar_NroSerie() => dao.Generar_NroSerie_Facturacion();

    public DataTable Listar_Empresas() => dao.Listar_Empresas();

    public DataTable Listar_Tipo_Facturacion() => dao.Listar_Tipo_Facturacion();

   
    public DataSet ObtenerDatosCompra_ParaFactura(string nroCompra)
    {
        try
        {
            // Pequeña validación antes de ir a BD
            if (string.IsNullOrEmpty(nroCompra))
            {
                throw new Exception("El número de compra no puede estar vacío.");
            }

            return dao.ObtenerDatosCompra_ParaFactura(nroCompra);
        }
        catch (Exception ex)
        {
            // Re-lanzamos la excepción para mostrarla en el mensaje de alerta del formulario
            throw ex;
        }
    }

    public string Guardar(string nroSerie, string codEmpresa, string codTipo, string nroCompra,
                         DateTime fecEmision, // <--- Este debe estar aquí
                         decimal sub, decimal igv, decimal total, string estado)
    {
        // ... validaciones ...

        // Pasamos 'fecEmision' al DAO
        if (dao.Registrar_Facturacion(nroSerie, codEmpresa, codTipo, nroCompra, fecEmision, sub, igv, total, estado))
        {
            return "OK";
        }
        else
        {
            return "ERROR al registrar la factura en la base de datos.";
        }
    }

     public DataTable listar_facturas(string filtro) => dao.listar_facturas(filtro);



}