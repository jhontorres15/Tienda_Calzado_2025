using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Registro_PagoLOGIC
/// </summary>
public class Registro_PagoLOGIC
{

    Registrar_PagoDAO Registrar_PagoDAO = new Registrar_PagoDAO();
    public int Generar_NroPago()
    {
        return Registrar_PagoDAO.Generar_NroPago();
    }

    public DataTable Listar_Forma_Pago()
    {
       
        return Registrar_PagoDAO.Listar_Forma_Pago();
    }

    // Método para Registrar el Pago con Validación
    public string Registrar_Pago_Validado(int nroPago, string codEmpleado, string nroSerieFactura, string codFormaPago, decimal importe, DateTime fecPago)
    {
        // Simplemente pasamos los parámetros al DAO
        return Registrar_PagoDAO.Registrar_Pago_Validado(nroPago, codEmpleado, nroSerieFactura, codFormaPago, importe, fecPago);
    }

    public DataTable Listar_Pagos(string filtro)
    {
        return Registrar_PagoDAO.Listar_Pagos(filtro);
    }
}