using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ImprimirComprobante : System.Web.UI.Page
{
    // Instancia de tu lógica de Facturación
    FacturacionLOGIC logic = new FacturacionLOGIC();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Capturamos el ID de la URL
            string nroSerie = Request.QueryString["serie"];

            if (!string.IsNullOrEmpty(nroSerie))
            {
                CargarDatos(nroSerie);
            }
            else
            {
                Response.Write("Error: No se especificó el número de comprobante.");
            }
        }
    }

    private void CargarDatos(string nroSerie)
    {
        // Instancia tu lógica (Asegúrate que FacturacionVenta_LOGIC exista y tenga el método)
        FacturacionLOGIC logic = new FacturacionLOGIC();

        // 1. Obtener Datos (El SQL ya trae todo listo)
        DataSet ds = logic.ObtenerDatosFactura_ParaImpresion(nroSerie);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            DataRow cab = ds.Tables[0].Rows[0];

            // 2. Llenar Etiquetas con lo que trajo el SQL
            // El SQL ya decidió si es "FACTURA" o "BOLETA" en la columna 'TituloDocumento'
            LBL_TipoComprobante.Text = cab["TituloDocumento"].ToString();

            // El SQL ya decidió si es "RUC:" o "DNI:" en la columna 'ClienteDocTipo'
            LBL_TipoDocIdentidad.Text = cab["ClienteDocTipo"].ToString().Substring(0, 3) + ":";

            // Resto de datos
            LBL_SerieCorrelativo.Text = nroSerie;
            LBL_FechaEmision.Text = Convert.ToDateTime(cab["Fec_Emision"]).ToString("dd/MM/yyyy");

            LBL_ClienteNombre.Text = cab["ClienteNombre"].ToString();
            LBL_ClienteDoc.Text = cab["ClienteDocNumero"].ToString();
            LBL_ClienteDireccion.Text = cab["ClienteDireccion"].ToString();

            // 3. Totales
            decimal subTotal = Convert.ToDecimal(cab["SubTotal"]);
            decimal igv = Convert.ToDecimal(cab["IGV"]);
            decimal total = Convert.ToDecimal(cab["Total"]);

            LBL_OpGravada.Text = subTotal.ToString("N2");
            LBL_IGV.Text = igv.ToString("N2");
            LBL_Total.Text = total.ToString("N2");

            // 4. Letras
            LBL_TotalLetras.Text = NumeroALetras(total);

            // 5. Detalle
            RPT_Detalle.DataSource = ds.Tables[1];
            RPT_Detalle.DataBind();
        }
        else
        {
            Response.Write("<h3>Error: No se encontraron datos para el comprobante " + nroSerie + "</h3>");
        }
    }

    private string NumeroALetras(decimal numero)
    {
        // Aquí puedes poner tu algoritmo real de conversión.
        return "SON: " + numero.ToString("N2") + " SOLES";
    }
}
