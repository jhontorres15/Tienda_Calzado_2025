using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;

/// <summary>
/// Servicio para interactuar con la API Oficial de WhatsApp Business Platform (Meta).
/// </summary>
public class WhapiService
{
    // CONFIGURACIÓN: Creadenciales de la API de Meta
    // Nota: Este Token es temporal (24h). Para producción, usa uno permanente.

    // 1. URL base de la API de Meta (Versión v22.0)
    private readonly string _apiUrlBase = "https://graph.facebook.com/v22.0/";

    // 2. ID del número de teléfono (El número registrado en la API)
    private readonly string _phoneId = "834292676443452";

    // 3. Token de Acceso (El que obtuviste en el panel de Meta)
    private readonly string _accessToken = "EAAL7VU0rPHcBQHnZBkAyUEPyAHFZAOHde05g1y8yzuGZBL4vBhIF3JgZBCAv746NpUPlkZAzLbDaZCMuZBhGjv4pIefGJEnFyBCjWPhD4CQi2Pn9HdiiHtZAtMHL4BI6CWhdWrylIZB1ZADUG2ypHWVVGLGl2CvzZCpBVHOgUIiGZCwbTUy6aqf7TSyyTqBKOuZB5t3TewbJ7QgKy7PRzNV1Odk1FIU3DU1JEI7VJbBsiezhtt8p7BDlDSiVynxV13jMdrz8p4bdeSIUjMN38zgSgNVDAk0vf";

    public WhapiService()
    {
        // Constructor vacío
    }

    // Adaptamos el método para enviar un mensaje de TEXTO simple (o plantilla)
    // NOTA: Para Meta, el primer mensaje debe ser una plantilla pre-aprobada (como 'hello_world')
    public string SendText(string phoneNumber, string message)
    {
        try
        {
            using (var client = new HttpClient())
            {
                // URL del endpoint de Meta: {apiUrlBase}{phoneId}/messages
                var endpoint = $"{_apiUrlBase}{_phoneId}/messages";

                // 1. Autenticación con el Token de Meta
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                // 2. Cuerpo JSON para enviar el mensaje (usando la plantilla 'hello_world' por defecto)
                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = phoneNumber, // Número de teléfono del destinatario
                    type = "template", // Usamos 'template' para el primer mensaje
                    template = new
                    {
                        name = "hello_world", // Nombre de la plantilla aprobada
                        language = new { code = "en_US" }
                    }
                };

                var json = new JavaScriptSerializer().Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // El método es asíncrono, pero lo forzamos a ser síncrono para WebForms
                var response = client.PostAsync(endpoint, content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return $"OK - Respuesta: {responseString}";
                }
                else
                {
                    return $"Error: {response.StatusCode} - {responseString}";
                }
            }
        }
        catch (Exception ex)
        {
            return $"Excepción: {ex.Message}";
        }
    }

    // NOTA: La API de Meta requiere un proceso diferente para enviar archivos (media) 
    // y no acepta Base64 directamente en la misma solicitud. Es un proceso de 2 pasos.
    // El método SendMedia anterior ha sido deshabilitado o debe ser reescrito completamente.

    public string SendMedia(string phoneNumber, Stream fileStream, string fileName, string mediaType)
    {
        return "El envío de archivos a través de la API de Meta requiere un proceso de dos pasos (subir el archivo y luego enviar la URL) y debe ser reescrito. Usa SendText por ahora.";
    }


}