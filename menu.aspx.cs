using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class menu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CargarDatosGraficos();
    }

    private void CargarDatosGraficos()
    {
        // ---------------------------------------------------------
        // SIMULACIÓN DE DATOS (Aquí reemplazarías con tu SQL)
        // ---------------------------------------------------------

        // 1. Datos Empleados (Nombres y Montos)
        string labelsEmp = "['TUANAMA', 'GIMENEZ', 'LOPEZ', 'UGARTE', 'RUEDA']";
        string valuesEmp = "[4500, 3800, 3200, 2900, 2500]";

        // 2. Datos Diarios (Días y Montos)
        string labelsDia = "['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes']";
        string valuesDia = "[12000, 15000, 11000, 18000, 14000]";

        // 3. Datos Estados (Etiquetas y Cantidades)
        string labelsEst = "['Entregado', 'Cancelado', 'Pendiente', 'Aprobado']";
        string valuesEst = "[65, 5, 15, 15]";

        /* COMO HACERLO CON SQL REAL (Ejemplo):
           ------------------------------------
           DataTable dt = new DataTable();
           // Llenar dt con SQL...
           
           // Convertir columna a string JS:
           string labels = "['" + string.Join("','", dt.AsEnumerable().Select(r => r["NombreColumna"])) + "']";
           string values = "[" + string.Join(",", dt.AsEnumerable().Select(r => r["ValorColumna"])) + "]";
        */

        // ---------------------------------------------------------
        // INYECCIÓN DE SCRIPT
        // ---------------------------------------------------------
        // Construimos la llamada a la función JS que creamos en el ASPX
        StringBuilder script = new StringBuilder();
        script.Append("<script>");
        script.Append("document.addEventListener('DOMContentLoaded', function() {");

        script.Append("  var dataEmp = { labels: " + labelsEmp + ", values: " + valuesEmp + " };");
        script.Append("  var dataDia = { labels: " + labelsDia + ", values: " + valuesDia + " };");
        script.Append("  var dataEst = { labels: " + labelsEst + ", values: " + valuesEst + " };");

        // Llamar a la función renderizarGraficos definida en el HTML
        script.Append("  renderizarGraficos(dataEmp, dataDia, dataEst);");

        script.Append("});");
        script.Append("</script>");

        // Inyectar el script en la página
        ClientScript.RegisterStartupScript(this.GetType(), "CargarGraficos", script.ToString());
    }
}