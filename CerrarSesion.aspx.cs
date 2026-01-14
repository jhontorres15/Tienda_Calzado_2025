using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CerrarSesion : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 1. Borrar todas las variables de la memoria (Usuario, Carrito, etc.)
        Session.Clear();

        // 2. Destruir la sesión actual completamente
        Session.Abandon();

        // 3. Borrar la cookie de sesión del navegador (Opcional, pero recomendado para seguridad extra)
        if (Request.Cookies["ASP.NET_SessionId"] != null)
        {
            Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
        }

        // 4. Redirigir inmediatamente al Login
        Response.Redirect("Login_Administrativo.aspx");
    }
}