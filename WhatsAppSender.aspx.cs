using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WhatsAppSender : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Opcional: Verificar si el usuario es administrador o empleado
    }

    protected void BTN_Enviar_Click(object sender, EventArgs e)
    {
        string telefono = TXT_Telefono.Text.Trim();
        string mensaje = TXT_Mensaje.Text.Trim();

        if (string.IsNullOrEmpty(telefono))
        {
            ShowError("El teléfono es obligatorio.");
            return;
        }

        // Instanciar el servicio
        WhapiService whapi = new WhapiService();
        string resultado = "";

        // Enviar solo texto si no hay archivo
        if (!FU_Archivo.HasFile)
        {
            if (string.IsNullOrEmpty(mensaje))
            {
                ShowError("Debes escribir un mensaje o adjuntar un archivo.");
                return;
            }
            resultado = whapi.SendText(telefono, mensaje);
        }
        else
        {
            // Enviar archivo
            string extension = Path.GetExtension(FU_Archivo.FileName).ToLower();
            string mediaType = "document";
            if (extension == ".jpg" || extension == ".png" || extension == ".jpeg") mediaType = "image";
            else if (extension == ".mp4") mediaType = "video";

            // Si hay mensaje, se envía primero (opcional, o se maneja según la API)
            // La API send-media no parece tener caption en el código PHP revisado, así que enviamos texto por separado si existe.
            if (!string.IsNullOrEmpty(mensaje))
            {
                whapi.SendText(telefono, mensaje);
            }

            resultado = whapi.SendMedia(telefono, FU_Archivo.FileContent, FU_Archivo.FileName, mediaType);
        }

        if (resultado == "OK")
        {
            ShowSuccess("Mensaje enviado correctamente a " + telefono);
            TXT_Mensaje.Text = "";
            FU_Archivo.Attributes.Clear();
        }
        else
        {
            ShowError(resultado);
        }
    }

    private void ShowError(string msg)
    {
        PNL_Result.Visible = true;
        PNL_Result.CssClass = "alert alert-danger";
        LBL_Result.Text = msg;
    }

    private void ShowSuccess(string msg)
    {
        PNL_Result.Visible = true;
        PNL_Result.CssClass = "alert alert-success";
        LBL_Result.Text = msg;
    }
}
