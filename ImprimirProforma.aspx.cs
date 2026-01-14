using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ImprimirProforma : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // ¡NUEVO! Bloque try-catch para capturar el error exacto
        try
        {
            if (!IsPostBack)
            {
                DataTable dtCabecera = Session["PrintCabecera"] as DataTable;
                DataTable dtDetalle = Session["PrintDetalle"] as DataTable;

                if (dtCabecera != null && dtDetalle != null && dtCabecera.Rows.Count > 0)
                {
                    DataRow cabecera = dtCabecera.Rows[0];

                    string nroProforma = cabecera["NroProforma"].ToString();
                    this.Title = "Proforma N° " + nroProforma;
                    LBL_NroProforma.Text = nroProforma;

                    LBL_FechaEmision.Text = CheckNull(cabecera["Fec_Emision"], "DateTime");
                    LBL_Estado.Text = CheckNull(cabecera["Estado_Proforma"]);
                    LBL_Vendedor.Text = CheckNull(cabecera["EmpleadoNombre"]);

                    if (cabecera["Fec_Emision"] != DBNull.Value && cabecera["Fec_Caducidad"] != DBNull.Value)
                    {
                        DateTime fecEmision = Convert.ToDateTime(cabecera["Fec_Emision"]);
                        DateTime fecCaducidad = Convert.ToDateTime(cabecera["Fec_Caducidad"]);
                        LBL_Validez.Text = string.Format("{0} días (Hasta {1})",
                                            (fecCaducidad - fecEmision).Days,
                                            fecCaducidad.ToString("dd/MM/yyyy"));
                    }
                    else { LBL_Validez.Text = "N/A"; }

                    // Datos del Cliente (¡Esta es una zona de error común!)
                    LBL_ClienteNombre.Text = CheckNull(cabecera["ClienteNombre"]);
                    LBL_ClienteDocTipo.Text = CheckNull(cabecera["ClienteDocTipo"]) + ":";
                    LBL_ClienteDocNumero.Text = CheckNull(cabecera["ClienteDocNumero"]);
                    LBL_ClienteDireccion.Text = CheckNull(cabecera["ClienteDireccion"]);
                    LBL_ClienteTelefono.Text = CheckNull(cabecera["ClienteTelefono"]);
                    LBL_ClienteEmail.Text = CheckNull(cabecera["ClienteEmail"]);

                    RPT_Detalle.DataSource = dtDetalle;
                    RPT_Detalle.DataBind();

                    if (cabecera["Total"] != DBNull.Value)
                    {
                        decimal total = Convert.ToDecimal(cabecera["Total"]);
                        decimal subtotal = total / 1.18m;
                        decimal igv = total - subtotal;
                        LBL_Subtotal.Text = string.Format("S/. {0:F2}", subtotal);
                        LBL_IGV.Text = string.Format("S/. {0:F2}", igv);
                        LBL_Total.Text = string.Format("S/. {0:F2}", total);
                    }
                    else
                    {
                        LBL_Subtotal.Text = "S/. 0.00";
                        LBL_IGV.Text = "S/. 0.00";
                        LBL_Total.Text = "S/. 0.00";
                    }

                    Session["PrintCabecera"] = null;
                    Session["PrintDetalle"] = null;
                }
                else
                {
                    Response.Write("<h1>Error: No se encontraron datos en la Sesión.</h1>");
                    Response.Write("<p>La página principal (Mantenimiento_Proforma) no pudo cargar los datos o la sesión se perdió. Por favor, cierre esta pestaña e intente imprimir de nuevo.</p>");
                }
            }
        }
        catch (Exception ex)
        {
            // ¡ESTO ES LO MÁS IMPORTANTE!
            // Si el código falla (ej. una columna no existe), veremos el error aquí.
            Response.Write("<h1>¡ERROR DE C# EN LA PÁGINA DE IMPRESIÓN!</h1>");
            Response.Write("<p>El Stored Procedure devolvió datos, pero el código C# falló al leerlos.</p>");
            Response.Write("<p><b>Mensaje de Error:</b> " + ex.Message + "</p>");
            Response.Write("<p><b>Detalle:</b> " + ex.StackTrace + "</p>");
            Response.Write("<hr>");
            Response.Write("<p><b>SOLUCIÓN PROBABLE:</b> Asegúrate de que el Stored Procedure 'USP_Proforma_ObtenerDatos' en tu base de datos esté actualizado con la última versión (la que tiene las columnas 'ClienteDocTipo', 'ClienteDocNumero', etc.).</p>");
        }
    }

    private string CheckNull(object valor, string formato = null)
    {
        if (valor == DBNull.Value || valor == null)
        {
            return "N/A";
        }
        if (formato == "DateTime")
        {
            return Convert.ToDateTime(valor).ToString("dd/MM/yyyy");
        }
        return valor.ToString();
    }
}