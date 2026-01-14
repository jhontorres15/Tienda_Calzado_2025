using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Menu_Web : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreEmpleadoSesion = Convert.ToString(Session["MantenimientoUsuario_Empleado_NombreEmpleado"]);
        this.TXT_Usuario.Text = nombreEmpleadoSesion;
    }

    private void MostrarMensaje(string mensaje, string tipo)
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "Mensaje",
         $"Swal.fire({{ icon: '{tipo}', title: 'Información', text: '{mensaje}', confirmButtonText: 'Aceptar' }});", true);
    }
}
