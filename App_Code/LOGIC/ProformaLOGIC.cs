using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static System.Collections.Specialized.BitVector32;


public class ProformaLOGIC
{
    ProformaDAO ProformaDAO = new ProformaDAO();

    public DataSet BuscarStockYFiltros(string sucursalNombre, string marca, string modelo, string talla, string color)
    {
        // Pasa el 'sucursalNombre'
        return ProformaDAO.BuscarStockYFiltros(sucursalNombre, marca, modelo, talla, color);
    }

    public string GuardarProforma(string nroProforma, string codCliente, string codEmpleado,
                                  DateTime fecEmision, DateTime fecCaducidad, decimal total,
                                  string estado, DataTable dtDetalle)
    {
        // --- Lógica de Negocio ---

        // 1. Validar que el detalle no sea nulo
        if (dtDetalle == null || dtDetalle.Rows.Count == 0)
        {
            return "Error: La proforma no tiene detalles.";
        }

        // 2. Preparar el DataTable para SQL:
        DataTable dtParaSQL = dtDetalle.Copy();

        if (dtParaSQL.Columns.Contains("ProductoDescripcion"))
        {
            dtParaSQL.Columns.Remove("ProductoDescripcion");
        }



  try
        {
            // --- Verificación de CodSucursal (char(5)) ---
            string colSucursal = "CodSucursal";
            int tamSucursal = 5; // Tamaño de tu SQL: char(5)
            if (dtParaSQL.Columns.Contains(colSucursal))
            {
                foreach (DataRow row in dtParaSQL.Rows)
                {
                    if (row[colSucursal] != DBNull.Value)
                    {
                        string valor = row[colSucursal].ToString();
                        if (valor.Length > tamSucursal)
                        {
                            row[colSucursal] = valor.Substring(0, tamSucursal);
                        }
                    }
                }
            }

            // --- Verificación de NroSerie_Producto (char(7)) ---
            string colSerie = "NroSerie_Producto";
            int tamSerie = 7; // Tamaño de tu SQL: char(7)
            if (dtParaSQL.Columns.Contains(colSerie))
                  {
                foreach (DataRow row in dtParaSQL.Rows)
                {
                    if (row[colSerie] != DBNull.Value)
                    {
                        string valor = row[colSerie].ToString();
                        if (valor.Length > tamSerie)
                        {
                            // Cortamos el string para que quepa
                            row[colSerie] = valor.Substring(0, tamSerie);
                        }
                    }
                }
            }

            dtParaSQL.AcceptChanges(); // Aplicamos los cortes
        }
        catch (Exception ex)
        {
            // Si falla la preparación, notifícalo.
            return "Error al preparar el detalle (LOGIC): " + ex.Message;
        }
        // --- FIN DE LA NUEVA SECCIÓN ---


        // 3. Llamar a la Capa de Datos (DAO)
        // (oBusqueda es tu instancia de ProformaDAO)
        return ProformaDAO.Guardar(nroProforma, codCliente, codEmpleado, fecEmision,
                   fecCaducidad, total, estado, dtParaSQL);
    }


    public DataTable ListarSucursales()
    {
 
        return ProformaDAO.ListarSucursales();
    }

    public DataTable ListarProformasTodos()
    {
        // Llama al DAO
        return ProformaDAO.ListarProformasTodos();
    }


    public DataSet ObtenerDatosProforma(string nroProforma)
    {
        // Llama directamente al método del DAO
        return ProformaDAO.ObtenerDatosProforma(nroProforma);
    }
}